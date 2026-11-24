namespace LW3.Models;

public class RepairOrder
{
    public string? Id { get; set; }

    // Id автомобіля та майстра (ObjectId-рядки)
    public string VehicleId { get; set; } = "";
    public string MechanicId { get; set; } = "";

    public List<string> Complaints { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.Diagnostics;
    public double NormHours { get; set; }
    public decimal PartsCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ---- поля нижче заповнюються бізнес-методами сервісу ----

    public List<OrderPart> Parts { get; set; } = new();

    // вартість робіт = нормогодини * ставка майстра (до знижки)
    public decimal LaborCost { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }

    // підсумок кошторису: LaborCost - DiscountAmount + PartsCost
    public decimal TotalCost { get; set; }
    public decimal Advance { get; set; }
    public decimal PaidAmount { get; set; }

    public DateTime? ClientConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // майстер зміни, який провів контроль якості
    public string? QualityControlMechanicId { get; set; }

    public Warranty? Warranty { get; set; }
}
