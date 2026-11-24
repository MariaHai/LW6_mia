using LW3.Models;

namespace LW3.Dtos;

// Бізнес-задача 1: прийом автомобіля та формування наряду-замовлення (request)
public class ReceiveVehicleDto
{
    public string Vin { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }

    // поточний зафіксований пробіг (не менший за попереднє звернення)
    public int Mileage { get; set; }
    public string OwnerPhone { get; set; } = "";

    // первинні скарги автовласника
    public List<string> Complaints { get; set; } = new();

    // майстер та потрібна спеціалізація (двигун, ходова, електрика)
    public string MechanicId { get; set; } = "";
    public Specialization Specialization { get; set; }

    // початок первинного огляду (за замовчуванням - зараз), тривалість 1 година
    public DateTime? InspectionStart { get; set; }
}
