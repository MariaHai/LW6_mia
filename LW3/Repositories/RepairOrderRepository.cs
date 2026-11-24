using AutoMapper;
using LW3.Documents;
using LW3.Models;
using LW3.Settings;
using MongoDB.Driver;

namespace LW3.Repositories;

public class RepairOrderRepository : MongoRepositoryBase<RepairOrder, RepairOrderDocument>, IRepairOrderRepository
{
    public RepairOrderRepository(IMongoDatabase database, IMapper mapper)
        : base(database, mapper, CollectionNames.RepairOrders)
    {
    }

    public async Task<IReadOnlyList<RepairOrder>> GetByVehicleIdAsync(string vehicleId)
    {
        var documents = await Collection.Find(x => x.VehicleId == vehicleId).ToListAsync();
        return documents.Select(ToModel).ToList();
    }

    public async Task<long> CountCompletedByVehicleAsync(string vehicleId)
    {
        return await Collection.CountDocumentsAsync(
            x => x.VehicleId == vehicleId && x.Status == OrderStatus.Completed);
    }
}
