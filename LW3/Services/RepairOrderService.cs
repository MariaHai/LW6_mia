using AutoMapper;
using LW3.Dtos;
using LW3.Exceptions;
using LW3.Models;
using LW3.Repositories;

namespace LW3.Services;

public class RepairOrderService : IRepairOrderService
{
    private const int InspectionDurationMinutes = 60;
    private const int LoyaltyMinCompletedOrders = 3;
    private const decimal LoyaltyDiscountPercent = 10m;
    private const int WarrantyMonths = 6;
    private const int WarrantyMileageKm = 10000;

    private readonly IRepairOrderRepository _orders;
    private readonly IVehicleRepository _vehicles;
    private readonly IMechanicRepository _mechanics;
    private readonly IPartRepository _parts;
    private readonly IMapper _mapper;

    public RepairOrderService(
        IRepairOrderRepository orders,
        IVehicleRepository vehicles,
        IMechanicRepository mechanics,
        IPartRepository parts,
        IMapper mapper)
    {
        _orders = orders;
        _vehicles = vehicles;
        _mechanics = mechanics;
        _parts = parts;
        _mapper = mapper;
    }

    // ===================== CRUD =====================
    public async Task<decimal> GetAverageTotalCostAsync()
    {
        var orders = await _orders.GetAllAsync();
        return orders.Any() ? orders.Average(o => o.TotalCost) : 0m;
    }

    // Загальна вартість завершених замовлень за місяць (за CompletedAt)
    public async Task<decimal> GetMonthlyTotalCostAsync(int year, int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), month, "Month must be 1-12.");

        var orders = await _orders.GetAllAsync();
        return orders
            .Where(o => o.CompletedAt is { } d && d.Year == year && d.Month == month)
            .Sum(o => o.TotalCost);
    }

    // Найдорожче замовлення (виняток, якщо замовлень немає)
    public async Task<RepairOrder> GetMostExpensiveOrderAsync()
    {
        var orders = (await _orders.GetAllAsync()).ToList();
        if (orders.Count == 0)
            throw new InvalidOperationException("No repair orders found.");

        return orders.MaxBy(o => o.TotalCost)!;
    }

    // Кількість замовлень з певним статусом
    public async Task<int> CountByStatusAsync(OrderStatus status)
    {
        var orders = await _orders.GetAllAsync();
        return orders.Count(o => o.Status == status);
    }

    // Отримання замовлення: валідація id + відсутність сутності
    public async Task<RepairOrder> GetOrderOrThrowAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id is required.", nameof(id));

        var order = await _orders.GetByIdAsync(id);
        return order ?? throw new KeyNotFoundException($"Repair order '{id}' not found.");
    }

    // Кількість завершених ремонтів автомобіля
    public async Task<long> GetCompletedCountForVehicleAsync(string vehicleId)
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
            throw new ArgumentException("Vehicle id is required.", nameof(vehicleId));

        return await _orders.CountCompletedByVehicleAsync(vehicleId);
    }

    // Скільки клієнт витратив на завершені ремонти автомобіля
    public async Task<decimal> GetVehicleTotalSpentAsync(string vehicleId)
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
            throw new ArgumentException("Vehicle id is required.", nameof(vehicleId));

        var orders = await _orders.GetByVehicleIdAsync(vehicleId);
        return orders
            .Where(o => o.Status == OrderStatus.Completed)
            .Sum(o => o.TotalCost);
    }

    public async Task<IReadOnlyList<RepairOrderResponseDto>> GetAllAsync()
    {
        var orders = await _orders.GetAllAsync();
        return _mapper.Map<List<RepairOrderResponseDto>>(orders);
    }

    public async Task<RepairOrderResponseDto> GetByIdAsync(string id) =>
        _mapper.Map<RepairOrderResponseDto>(await GetEntityAsync(id));

    public async Task<RepairOrderResponseDto> CreateAsync(CreateRepairOrderDto dto)
    {
        await EnsureVehicleAndMechanicExistAsync(dto.VehicleId, dto.MechanicId);

        var order = _mapper.Map<RepairOrder>(dto);
        order.Status = OrderStatus.Diagnostics;
        order.CreatedAt = DateTime.UtcNow;

        var created = await _orders.CreateAsync(order);
        return _mapper.Map<RepairOrderResponseDto>(created);
    }

    public async Task UpdateAsync(string id, UpdateRepairOrderDto dto)
    {
        var order = await GetEntityAsync(id);
        await EnsureVehicleAndMechanicExistAsync(dto.VehicleId, dto.MechanicId);

        // копіюються лише поля DTO; кошторис, запчастини і гарантія (керуються бізнес-методами) лишаються
        _mapper.Map(dto, order);
        await _orders.UpdateAsync(id, order);
    }

    public async Task PatchAsync(string id, PatchRepairOrderDto dto)
    {
        var order = await GetEntityAsync(id);
        _mapper.Map(dto, order);
        await _orders.UpdateAsync(id, order);
    }

    public async Task DeleteAsync(string id)
    {
        if (!await _orders.DeleteAsync(id))
            throw new NotFoundException($"Наряд з ID={id} не знайдено.");
    }

    // ============ 1. Прийом автомобіля та формування наряду ============

    public async Task<ReceiveVehicleResponseDto> ReceiveVehicleAsync(ReceiveVehicleDto dto)
    {
        // 1) завантаження сутностей
        var mechanic = await _mechanics.GetByIdAsync(dto.MechanicId)
            ?? throw new NotFoundException($"Майстра з ID={dto.MechanicId} не знайдено.");
        var vehicle = await _vehicles.GetByVinAsync(dto.Vin);
        var vehicleWithPlate = await _vehicles.GetByLicensePlateAsync(dto.LicensePlate);

        // 2) перевірки бізнес-правил
        if (vehicleWithPlate is not null && (vehicle is null || vehicleWithPlate.Id != vehicle.Id))
            throw new ConflictException(
                $"Держномер {dto.LicensePlate} вже закріплений за іншим автомобілем.");

        if (vehicle is not null && dto.Mileage < vehicle.Mileage)
            throw new ConflictException(
                $"Пробіг {dto.Mileage} км менший за попередньо зафіксований ({vehicle.Mileage} км).");

        if (!mechanic.IsActive)
            throw new ConflictException($"Майстер {mechanic.FullName} неактивний і не може бути призначений.");

        if (mechanic.Specialization != dto.Specialization)
            throw new ConflictException(
                $"Спеціалізація майстра ({mechanic.Specialization}) не відповідає потрібній ({dto.Specialization}).");

        var start = ToUtc(dto.InspectionStart ?? DateTime.UtcNow);
        if (start < DateTime.UtcNow.AddMinutes(-5))
            throw new BadRequestException("Час первинного огляду не може бути в минулому.");

        var end = start.AddMinutes(InspectionDurationMinutes);
        if (mechanic.Schedule.Any(s => start < s.End && s.Start < end))
            throw new ConflictException(
                $"Майстер {mechanic.FullName} зайнятий у вибраний час ({start:u} - {end:u}).");

        // 3) зміна статусів і пов'язаних даних
        if (vehicle is null)
        {
            vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.Status = VehicleStatus.InService;
            vehicle = await _vehicles.CreateAsync(vehicle);
        }
        else
        {
            _mapper.Map(dto, vehicle); // оновлює дані автомобіля з картки прийому
            vehicle.Status = VehicleStatus.InService;
            await _vehicles.UpdateAsync(vehicle.Id!, vehicle);
        }

        // 4) збереження: наряд у статусі Diagnostics + резерв графіка майстра
        var order = await _orders.CreateAsync(new RepairOrder
        {
            VehicleId = vehicle.Id!,
            MechanicId = mechanic.Id!,
            Complaints = dto.Complaints,
            Status = OrderStatus.Diagnostics,
            CreatedAt = DateTime.UtcNow
        });

        mechanic.Schedule.Add(new ScheduleSlot
        {
            Start = start,
            End = end,
            RepairOrderId = order.Id!
        });
        await _mechanics.UpdateAsync(mechanic.Id!, mechanic);

        return _mapper.Map<ReceiveVehicleResponseDto>(new ReceptionResult
        {
            Order = order,
            Vehicle = vehicle,
            Mechanic = mechanic,
            InspectionStart = start,
            InspectionEnd = end
        });
    }

    // ============ 2. Затвердження кошторису та резервування запчастин ============

    public async Task<EstimateResponseDto> ApproveEstimateAsync(string orderId, ApproveEstimateDto dto)
    {
        // 1) завантаження сутностей
        var order = await GetEntityAsync(orderId);
        var mechanic = await _mechanics.GetByIdAsync(order.MechanicId)
            ?? throw new NotFoundException($"Майстра з ID={order.MechanicId} не знайдено.");

        // 2) перевірки
        if (order.Status != OrderStatus.Diagnostics)
            throw new ConflictException(
                $"Кошторис можна погодити лише для наряду у статусі Diagnostics, поточний статус: {order.Status}.");

        if (!dto.ClientConfirmed)
            throw new ConflictException(
                "Наряд не можна перевести у статус InProgress без електронного підтвердження бюджету клієнтом.");

        var lines = new List<OrderPart>();
        foreach (var item in dto.Parts)
        {
            var part = await _parts.GetByIdAsync(item.PartId)
                ?? throw new NotFoundException($"Запчастину з ID={item.PartId} не знайдено.");

            if (part.Quantity < item.Quantity)
                throw new ConflictException(
                    $"Недостатньо запчастини '{part.Name}' на складі: потрібно {item.Quantity}, є {part.Quantity}.");

            lines.Add(new OrderPart
            {
                PartId = part.Id!,
                Name = part.Name,
                Quantity = item.Quantity,
                UnitPrice = part.UnitPrice
            });
        }

        // 3) розрахунки: (нормогодини * ставка) + запчастини, знижка 10% на роботи для постійних клієнтів
        var completedOrders = await _orders.CountCompletedByVehicleAsync(order.VehicleId);
        var laborCost = Math.Round((decimal)dto.NormHours * mechanic.HourlyRate, 2);
        var discountPercent = completedOrders >= LoyaltyMinCompletedOrders ? LoyaltyDiscountPercent : 0m;
        var discountAmount = Math.Round(laborCost * discountPercent / 100m, 2);
        var partsCost = lines.Sum(l => l.UnitPrice * l.Quantity);
        var totalCost = laborCost - discountAmount + partsCost;

        if (dto.AdvanceAmount > totalCost)
            throw new BadRequestException(
                $"Аванс ({dto.AdvanceAmount}) не може перевищувати суму кошторису ({totalCost}).");

        // 4) резервування запчастин зі списанням залишку (з відкатом при помилці) і перехід у InProgress
        var reserved = new List<OrderPart>();
        try
        {
            foreach (var line in lines)
            {
                if (!await _parts.TryReserveAsync(line.PartId, line.Quantity))
                    throw new ConflictException(
                        $"Запчастини '{line.Name}' не вистачає на складі: залишок щойно змінився.");
                reserved.Add(line);
            }

            order.Parts = lines;
            order.NormHours = dto.NormHours;
            order.PartsCost = partsCost;
            order.LaborCost = laborCost;
            order.DiscountPercent = discountPercent;
            order.DiscountAmount = discountAmount;
            order.TotalCost = totalCost;
            order.Advance = dto.AdvanceAmount;
            order.ClientConfirmedAt = DateTime.UtcNow;
            order.Status = OrderStatus.InProgress;

            await _orders.UpdateAsync(order.Id!, order);
        }
        catch
        {
            foreach (var item in reserved)
                await _parts.ReleaseAsync(item.PartId, item.Quantity);
            throw;
        }

        return _mapper.Map<EstimateResponseDto>(order);
    }

    // ============ 3. Контроль якості, видача авто та відкриття гарантії ============

    public async Task<CompleteOrderResponseDto> CompleteOrderAsync(string orderId, CompleteOrderDto dto)
    {
        // 1) завантаження сутностей
        var order = await GetEntityAsync(orderId);
        var shiftMechanic = await _mechanics.GetByIdAsync(dto.ShiftMechanicId)
            ?? throw new NotFoundException($"Майстра зміни з ID={dto.ShiftMechanicId} не знайдено.");
        var vehicle = await _vehicles.GetByIdAsync(order.VehicleId)
            ?? throw new NotFoundException($"Автомобіль з ID={order.VehicleId} не знайдено.");

        // 2) перевірки
        if (order.Status != OrderStatus.ReadyForPickup)
            throw new ConflictException(
                $"Закрити наряд можна лише зі статусу ReadyForPickup, поточний статус: {order.Status}.");

        if (!shiftMechanic.IsActive)
            throw new ConflictException(
                $"Майстер зміни {shiftMechanic.FullName} неактивний і не може проводити контроль якості.");

        if (!dto.QualityControlPassed)
            throw new ConflictException("Наряд не можна закрити: контроль якості майстра зміни не пройдено.");

        if (dto.CurrentMileage.HasValue && dto.CurrentMileage.Value < vehicle.Mileage)
            throw new ConflictException(
                $"Пробіг при видачі ({dto.CurrentMileage} км) менший за зафіксований ({vehicle.Mileage} км).");

        // 3) розрахунок суми до сплати: кошторис мінус аванс
        var amountDue = order.TotalCost - order.Advance;
        if (dto.PaidAmount != amountDue)
            throw new ConflictException(
                $"Сума оплати ({dto.PaidAmount}) не відповідає сумі до сплати ({amountDue} = кошторис {order.TotalCost} - аванс {order.Advance}).");

        // 4) зміна статусів і гарантія
        var now = DateTime.UtcNow;

        if (dto.CurrentMileage.HasValue)
            vehicle.Mileage = dto.CurrentMileage.Value;
        vehicle.Status = VehicleStatus.Released;

        order.Status = OrderStatus.Completed;
        order.PaidAmount = dto.PaidAmount;
        order.CompletedAt = now;
        order.QualityControlMechanicId = shiftMechanic.Id;
        order.Warranty = new Warranty
        {
            StartDate = now,
            ValidUntil = now.AddMonths(WarrantyMonths),
            StartMileage = vehicle.Mileage,
            ValidUntilMileage = vehicle.Mileage + WarrantyMileageKm,
            CoveredWorks = order.Complaints.ToList(),
            CoveredParts = order.Parts.Select(p => p.Name).ToList()
        };

        // 5) збереження
        await _orders.UpdateAsync(order.Id!, order);
        await _vehicles.UpdateAsync(vehicle.Id!, vehicle);

        return _mapper.Map<CompleteOrderResponseDto>(new CompletionResult
        {
            Order = order,
            Vehicle = vehicle
        });
    }

    // ===================== допоміжні =====================

    private async Task<RepairOrder> GetEntityAsync(string id) =>
        await _orders.GetByIdAsync(id)
        ?? throw new NotFoundException($"Наряд з ID={id} не знайдено.");

    private async Task EnsureVehicleAndMechanicExistAsync(string vehicleId, string mechanicId)
    {
        if (await _vehicles.GetByIdAsync(vehicleId) is null)
            throw new NotFoundException($"Автомобіль з ID={vehicleId} не знайдено.");

        if (await _mechanics.GetByIdAsync(mechanicId) is null)
            throw new NotFoundException($"Майстра з ID={mechanicId} не знайдено.");
    }

    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
