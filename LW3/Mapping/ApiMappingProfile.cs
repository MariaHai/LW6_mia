using AutoMapper;
using LW3.Dtos;
using LW3.Models;

namespace LW3.Mapping;

// DTO (контракт API) <-> Domain Model. Використовується у сервісах.
public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // ---------- User ----------
        CreateMap<User, UserResponseDto>();

        // ---------- Vehicle ----------
        CreateMap<CreateVehicleDto, Vehicle>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore());      // початковий статус виставляє модель (Registered)

        CreateMap<UpdateVehicleDto, Vehicle>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Vin, o => o.Ignore());         // VIN не змінюється

        CreateMap<PatchVehicleDto, Vehicle>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null)); // копіюємо лише передані поля

        CreateMap<Vehicle, VehicleResponseDto>();

        // ---------- Mechanic ----------
        CreateMap<CreateMechanicDto, Mechanic>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Schedule, o => o.Ignore());

        CreateMap<UpdateMechanicDto, Mechanic>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Schedule, o => o.Ignore());    // графік веде сервіс прийому авто

        CreateMap<PatchMechanicDto, Mechanic>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<ScheduleSlot, ScheduleSlotDto>();
        CreateMap<Mechanic, MechanicResponseDto>();

        // ---------- Part ----------
        CreateMap<CreatePartDto, Part>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<UpdatePartDto, Part>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<Part, PartResponseDto>();

        // ---------- RepairOrder (CRUD) ----------
        CreateMap<CreateRepairOrderDto, RepairOrder>()
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<UpdateRepairOrderDto, RepairOrder>()
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<PatchRepairOrderDto, RepairOrder>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<OrderPart, OrderPartDto>();
        CreateMap<Warranty, WarrantyDto>();

        CreateMap<RepairOrder, RepairOrderResponseDto>()
            .ForMember(d => d.AmountDue, o => o.MapFrom(s => s.TotalCost - s.Advance));

        // ---------- Бізнес-задача 1: прийом автомобіля ----------
        CreateMap<ReceiveVehicleDto, Vehicle>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore());

        CreateMap<ReceptionResult, ReceiveVehicleResponseDto>()
            .ForMember(d => d.OrderId, o => o.MapFrom(s => s.Order.Id))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Order.Status))
            .ForMember(d => d.Complaints, o => o.MapFrom(s => s.Order.Complaints))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.Order.CreatedAt))
            .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.Vehicle.Id))
            .ForMember(d => d.Vin, o => o.MapFrom(s => s.Vehicle.Vin))
            .ForMember(d => d.LicensePlate, o => o.MapFrom(s => s.Vehicle.LicensePlate))
            .ForMember(d => d.MechanicId, o => o.MapFrom(s => s.Mechanic.Id))
            .ForMember(d => d.MechanicName, o => o.MapFrom(s => s.Mechanic.FullName));

        // ---------- Бізнес-задача 2: кошторис і запчастини ----------
        CreateMap<RepairOrder, EstimateResponseDto>()
            .ForMember(d => d.OrderId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.AmountDue, o => o.MapFrom(s => s.TotalCost - s.Advance));

        // ---------- Бізнес-задача 3: видача авто і гарантія ----------
        CreateMap<CompletionResult, CompleteOrderResponseDto>()
            .ForMember(d => d.OrderId, o => o.MapFrom(s => s.Order.Id))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Order.Status))
            .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.Vehicle.Id))
            .ForMember(d => d.VehicleStatus, o => o.MapFrom(s => s.Vehicle.Status))
            .ForMember(d => d.TotalCost, o => o.MapFrom(s => s.Order.TotalCost))
            .ForMember(d => d.Advance, o => o.MapFrom(s => s.Order.Advance))
            .ForMember(d => d.PaidAmount, o => o.MapFrom(s => s.Order.PaidAmount))
            .ForMember(d => d.CompletedAt, o => o.MapFrom(s => s.Order.CompletedAt))
            .ForMember(d => d.QualityControlMechanicId, o => o.MapFrom(s => s.Order.QualityControlMechanicId))
            .ForMember(d => d.Warranty, o => o.MapFrom(s => s.Order.Warranty));
    }
}
