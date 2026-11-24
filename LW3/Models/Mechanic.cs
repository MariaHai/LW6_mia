namespace LW3.Models;

public class Mechanic
{
    public string? Id { get; set; }
    public string FullName { get; set; } = "";
    public Specialization Specialization { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; } = true;

    // Робочий графік: зарезервовані інтервали (керується сервісом при прийомі авто)
    public List<ScheduleSlot> Schedule { get; set; } = new();
}
