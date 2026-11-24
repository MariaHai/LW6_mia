using AutoMapper;
using LW3.Documents;
using LW3.Models;
using LW3.Settings;
using MongoDB.Driver;

namespace LW3.Repositories;

public class VehicleRepository : MongoRepositoryBase<Vehicle, VehicleDocument>, IVehicleRepository
{
    public VehicleRepository(IMongoDatabase database, IMapper mapper)
        : base(database, mapper, CollectionNames.Vehicles)
    {
    }

    public async Task<Vehicle?> GetByVinAsync(string vin)
    {
        var document = await Collection.Find(x => x.Vin == vin).FirstOrDefaultAsync();
        return document is null ? null : ToModel(document);
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        var document = await Collection.Find(x => x.LicensePlate == licensePlate).FirstOrDefaultAsync();
        return document is null ? null : ToModel(document);
    }
}
