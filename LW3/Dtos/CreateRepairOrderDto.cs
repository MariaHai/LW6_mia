using LW3.Models;

namespace LW3.Dtos;

// Спільні поля для створення та редагування наряду (CRUD)
public abstract class RepairOrderDetailsDto
{
    public string VehicleId { get; set; } = "";
    public string MechanicId { get; set; } = "";
    public List<string> Complaints { get; set; } = new();
}

// POST /api/RepairOrders  (статус Diagnostics та дата створення виставляє сервіс)
public class CreateRepairOrderDto : RepairOrderDetailsDto
{
}

// PUT /api/RepairOrders/{id}
public class UpdateRepairOrderDto : RepairOrderDetailsDto
{
    public OrderStatus Status { get; set; }
    public double NormHours { get; set; }
    public decimal PartsCost { get; set; }
}

public class OrderPartDto
{
    public string PartId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class WarrantyDto
{
    public DateTime StartDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public int StartMileage { get; set; }
    public int ValidUntilMileage { get; set; }
    public List<string> CoveredWorks { get; set; } = new();
    public List<string> CoveredParts { get; set; } = new();
}

public class RepairOrderResponseDto
{
    public string Id { get; set; } = "";
    public string VehicleId { get; set; } = "";
    public string MechanicId { get; set; } = "";
    public List<string> Complaints { get; set; } = new();
    public OrderStatus Status { get; set; }
    public double NormHours { get; set; }
    public decimal PartsCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderPartDto> Parts { get; set; } = new();
    public decimal LaborCost { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Advance { get; set; }

    // сума до сплати при видачі: кошторис мінус аванс
    public decimal AmountDue { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime? ClientConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? QualityControlMechanicId { get; set; }
    public WarrantyDto? Warranty { get; set; }
}
