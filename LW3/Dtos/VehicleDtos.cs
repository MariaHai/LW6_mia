using LW3.Models;

namespace LW3.Dtos;

// Спільні поля для створення та редагування автомобіля
public abstract class VehicleDetailsDto
{
    public string LicensePlate { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string OwnerPhone { get; set; } = "";
}

// POST /api/Vehicles
public class CreateVehicleDto : VehicleDetailsDto
{
    public string Vin { get; set; } = "";
}

// PUT /api/Vehicles/{id}  (VIN не змінюється)
public class UpdateVehicleDto : VehicleDetailsDto
{
    public VehicleStatus Status { get; set; }
}

// Що API повертає клієнту
public class VehicleResponseDto
{
    public string Id { get; set; } = "";
    public string Vin { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string OwnerPhone { get; set; } = "";
    public VehicleStatus Status { get; set; }
}
