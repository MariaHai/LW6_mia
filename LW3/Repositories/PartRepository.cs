using AutoMapper;
using LW3.Documents;
using LW3.Models;
using LW3.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LW3.Repositories;

public class PartRepository : MongoRepositoryBase<Part, PartDocument>, IPartRepository
{
    public PartRepository(IMongoDatabase database, IMapper mapper)
        : base(database, mapper, CollectionNames.Parts)
    {
    }

    public async Task<bool> TryReserveAsync(string id, int quantity)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;

        // умова Quantity >= quantity і зменшення виконуються однією атомарною операцією
        var filter = Builders<PartDocument>.Filter.And(
            Builders<PartDocument>.Filter.Eq("_id", objectId),
            Builders<PartDocument>.Filter.Gte(x => x.Quantity, quantity));
        var update = Builders<PartDocument>.Update.Inc(x => x.Quantity, -quantity);

        var result = await Collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task ReleaseAsync(string id, int quantity)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return;

        var filter = Builders<PartDocument>.Filter.Eq("_id", objectId);
        var update = Builders<PartDocument>.Update.Inc(x => x.Quantity, quantity);

        await Collection.UpdateOneAsync(filter, update);
    }
}
