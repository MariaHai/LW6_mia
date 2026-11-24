using LW3.Models;

namespace LW3.Dtos;

public class PatchVehicleDto
{
    public int? Mileage { get; set; }
    public string? OwnerPhone { get; set; }
    public VehicleStatus? Status { get; set; }
}
