using LW3.Dtos;

namespace LW3.Services;

public interface IRepairOrderService
{
    // ---- CRUD ----
    Task<IReadOnlyList<RepairOrderResponseDto>> GetAllAsync();
    Task<RepairOrderResponseDto> GetByIdAsync(string id);
    Task<RepairOrderResponseDto> CreateAsync(CreateRepairOrderDto dto);
    Task UpdateAsync(string id, UpdateRepairOrderDto dto);
    Task PatchAsync(string id, PatchRepairOrderDto dto);
    Task DeleteAsync(string id);

    // ---- бізнес-задачі варіанта 6 ----

    // 1. Прийом автомобіля та формування наряду-замовлення
    Task<ReceiveVehicleResponseDto> ReceiveVehicleAsync(ReceiveVehicleDto dto);

    // 2. Затвердження кошторису та резервування автозапчастин
    Task<EstimateResponseDto> ApproveEstimateAsync(string orderId, ApproveEstimateDto dto);

    // 3. Контроль якості, видача авто та відкриття гарантії
    Task<CompleteOrderResponseDto> CompleteOrderAsync(string orderId, CompleteOrderDto dto);
}
