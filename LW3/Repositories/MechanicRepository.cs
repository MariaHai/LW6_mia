using AutoMapper;
using LW3.Documents;
using LW3.Models;
using LW3.Settings;
using MongoDB.Driver;

namespace LW3.Repositories;

public class MechanicRepository : MongoRepositoryBase<Mechanic, MechanicDocument>, IMechanicRepository
{
    public MechanicRepository(IMongoDatabase database, IMapper mapper)
        : base(database, mapper, CollectionNames.Mechanics)
    {
    }
}
