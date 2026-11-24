using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LW3.Documents;

public class PartDocument : IMongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unit_price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }
}
