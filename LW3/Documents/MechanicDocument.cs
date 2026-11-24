using LW3.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LW3.Documents;

public class MechanicDocument : IMongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("full_name")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("specialization")]
    [BsonRepresentation(BsonType.String)]
    public Specialization Specialization { get; set; }

    [BsonElement("hourly_rate")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal HourlyRate { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; }

    [BsonElement("schedule")]
    public List<ScheduleSlotDocument> Schedule { get; set; } = new();
}

public class ScheduleSlotDocument
{
    [BsonElement("start")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Start { get; set; }

    [BsonElement("end")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime End { get; set; }

    [BsonElement("repair_order_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RepairOrderId { get; set; } = string.Empty;
}
