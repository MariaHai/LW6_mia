using LW3.Models;

namespace LW3.Dtos;

// Бізнес-задача 1 (response)
public class ReceiveVehicleResponseDto
{
    public string OrderId { get; set; } = "";
    public OrderStatus Status { get; set; }
    public List<string> Complaints { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public string VehicleId { get; set; } = "";
    public string Vin { get; set; } = "";
    public string LicensePlate { get; set; } = "";

    public string MechanicId { get; set; } = "";
    public string MechanicName { get; set; } = "";

    // зарезервований час первинного огляду
    public DateTime InspectionStart { get; set; }
    public DateTime InspectionEnd { get; set; }
}

// Бізнес-задача 2 (response)
public class EstimateResponseDto
{
    public string OrderId { get; set; } = "";
    public OrderStatus Status { get; set; }
    public double NormHours { get; set; }
    public decimal LaborCost { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PartsCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Advance { get; set; }
    public decimal AmountDue { get; set; }
    public List<OrderPartDto> Parts { get; set; } = new();
    public DateTime? ClientConfirmedAt { get; set; }
}

// Бізнес-задача 3 (response)
public class CompleteOrderResponseDto
{
    public string OrderId { get; set; } = "";
    public OrderStatus Status { get; set; }
    public string VehicleId { get; set; } = "";
    public VehicleStatus VehicleStatus { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Advance { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? QualityControlMechanicId { get; set; }
    public WarrantyDto? Warranty { get; set; }
}
