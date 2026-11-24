namespace LW3.Models;

// Доменна модель: без атрибутів MongoDB і без атрибутів валідації.
// Правила вхідних даних описані у валідаторах DTO (папка Validators).
public class Vehicle
{
    public string? Id { get; set; }
    public string Vin { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string OwnerPhone { get; set; } = "";
    public VehicleStatus Status { get; set; } = VehicleStatus.Registered;
}
