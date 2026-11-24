using AutoMapper;
using LW3.Documents;
using LW3.Models;

namespace LW3.Mapping;

// Persistence Model (Document у MongoDB) <-> Domain Model.
// Використовується у репозиторіях: сервіси про *Document нічого не знають.
public class PersistenceMappingProfile : Profile
{
    public PersistenceMappingProfile()
    {
        CreateMap<Vehicle, VehicleDocument>().ReverseMap();
        CreateMap<Part, PartDocument>().ReverseMap();

        CreateMap<ScheduleSlot, ScheduleSlotDocument>().ReverseMap();
        CreateMap<Mechanic, MechanicDocument>().ReverseMap();

        CreateMap<OrderPart, OrderPartDocument>().ReverseMap();
        CreateMap<Warranty, WarrantyDocument>().ReverseMap();
        CreateMap<RepairOrder, RepairOrderDocument>().ReverseMap();

        CreateMap<User, UserDocument>().ReverseMap();
    }
}
