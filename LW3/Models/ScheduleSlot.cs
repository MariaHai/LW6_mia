namespace LW3.Models;

// Зарезервований інтервал у графіку майстра (первинний огляд)
public class ScheduleSlot
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string RepairOrderId { get; set; } = "";
}
