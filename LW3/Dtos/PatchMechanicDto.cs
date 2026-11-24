using LW3.Models;

namespace LW3.Dtos;

public class PatchMechanicDto
{
    public decimal? HourlyRate { get; set; }
    public bool? IsActive { get; set; }
    public Specialization? Specialization { get; set; }
}
