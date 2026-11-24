using AutoMapper;
using LW3.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LW3.Repositories;

// Спільна реалізація CRUD для всіх сутностей.
// Знає про MongoDB і *Document, назовні віддає лише доменні моделі.
// Перетворення Document <-> Model виконує AutoMapper (PersistenceMappingProfile).
public abstract class MongoRepositoryBase<TModel, TDocument> : IRepository<TModel>
    where TDocument : class, IMongoDocument
{
    protected readonly IMongoCollection<TDocument> Collection;
    protected readonly IMapper Mapper;

    protected MongoRepositoryBase(IMongoDatabase database, IMapper mapper, string collectionName)
    {
        Collection = database.GetCollection<TDocument>(collectionName);
        Mapper = mapper;
    }

    protected TModel ToModel(TDocument document) => Mapper.Map<TModel>(document);

    protected TDocument ToDocument(TModel model) => Mapper.Map<TDocument>(model);

    // Id у документі зберігається як ObjectId, тому фільтруємо за "_id" саме по ObjectId
    private static FilterDefinition<TDocument> IdFilter(ObjectId id) =>
        Builders<TDocument>.Filter.Eq("_id", id);

    public async Task<IReadOnlyList<TModel>> GetAllAsync()
    {
        var documents = await Collection.Find(_ => true).ToListAsync();
        return documents.Select(ToModel).ToList();
    }

    public async Task<TModel?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return default;

        var document = await Collection.Find(IdFilter(objectId)).FirstOrDefaultAsync();
        return document is null ? default : ToModel(document);
    }

    public async Task<TModel> CreateAsync(TModel model)
    {
        var document = ToDocument(model);
        document.Id = string.Empty; // драйвер сам згенерує ObjectId

        await Collection.InsertOneAsync(document);
        return ToModel(document);
    }

    public async Task<bool> UpdateAsync(string id, TModel model)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;

        var document = ToDocument(model);
        document.Id = id;

        var result = await Collection.ReplaceOneAsync(IdFilter(objectId), document);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;

        var result = await Collection.DeleteOneAsync(IdFilter(objectId));
        return result.DeletedCount > 0;
    }
}
