namespace LW3.Models;

// Результати бізнес-операцій на рівні сервісу (доменні об'єкти, з яких AutoMapper формує response DTO)

public class ReceptionResult
{
    public RepairOrder Order { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    public Mechanic Mechanic { get; set; } = null!;
    public DateTime InspectionStart { get; set; }
    public DateTime InspectionEnd { get; set; }
}

public class CompletionResult
{
    public RepairOrder Order { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
