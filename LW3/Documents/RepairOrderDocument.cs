using LW3.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LW3.Documents;

public class RepairOrderDocument : IMongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("vehicle_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string VehicleId { get; set; } = string.Empty;

    [BsonElement("mechanic_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string MechanicId { get; set; } = string.Empty;

    [BsonElement("complaints")]
    public List<string> Complaints { get; set; } = new();

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status { get; set; }

    [BsonElement("norm_hours")]
    public double NormHours { get; set; }

    [BsonElement("parts_cost")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal PartsCost { get; set; }

    [BsonElement("created_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("parts")]
    public List<OrderPartDocument> Parts { get; set; } = new();

    [BsonElement("labor_cost")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal LaborCost { get; set; }

    [BsonElement("discount_percent")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal DiscountPercent { get; set; }

    [BsonElement("discount_amount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal DiscountAmount { get; set; }

    [BsonElement("total_cost")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalCost { get; set; }

    [BsonElement("advance")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Advance { get; set; }

    [BsonElement("paid_amount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal PaidAmount { get; set; }

    [BsonElement("client_confirmed_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ClientConfirmedAt { get; set; }

    [BsonElement("completed_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CompletedAt { get; set; }

    [BsonElement("quality_control_mechanic_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? QualityControlMechanicId { get; set; }

    [BsonElement("warranty")]
    public WarrantyDocument? Warranty { get; set; }
}

public class OrderPartDocument
{
    [BsonElement("part_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PartId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unit_price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }
}

public class WarrantyDocument
{
    [BsonElement("start_date")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartDate { get; set; }

    [BsonElement("valid_until")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime ValidUntil { get; set; }

    [BsonElement("start_mileage")]
    public int StartMileage { get; set; }

    [BsonElement("valid_until_mileage")]
    public int ValidUntilMileage { get; set; }

    [BsonElement("covered_works")]
    public List<string> CoveredWorks { get; set; } = new();

    [BsonElement("covered_parts")]
    public List<string> CoveredParts { get; set; } = new();
}
