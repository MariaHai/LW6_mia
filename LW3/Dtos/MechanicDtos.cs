using LW3.Models;

namespace LW3.Dtos;

public abstract class MechanicDetailsDto
{
    public string FullName { get; set; } = "";
    public Specialization Specialization { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; } = true;
}

// POST /api/Mechanics
public class CreateMechanicDto : MechanicDetailsDto
{
}

// PUT /api/Mechanics/{id}
public class UpdateMechanicDto : MechanicDetailsDto
{
}

public class ScheduleSlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string RepairOrderId { get; set; } = "";
}

public class MechanicResponseDto
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public Specialization Specialization { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public List<ScheduleSlotDto> Schedule { get; set; } = new();
}
