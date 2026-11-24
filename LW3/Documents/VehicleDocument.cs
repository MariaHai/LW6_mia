using LW3.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LW3.Documents;

public class VehicleDocument : IMongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("vin")]
    public string Vin { get; set; } = string.Empty;

    [BsonElement("license_plate")]
    public string LicensePlate { get; set; } = string.Empty;

    [BsonElement("make")]
    public string Make { get; set; } = string.Empty;

    [BsonElement("model")]
    public string Model { get; set; } = string.Empty;

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("mileage")]
    public int Mileage { get; set; }

    [BsonElement("owner_phone")]
    public string OwnerPhone { get; set; } = string.Empty;

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public VehicleStatus Status { get; set; }
}
