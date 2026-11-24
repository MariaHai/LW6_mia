namespace LW3.Dtos;

// Бізнес-задача 2: затвердження кошторису та резервування запчастин
public class ApproveEstimateDto
{
    public double NormHours { get; set; }

    // електронне підтвердження бюджету клієнтом
    public bool ClientConfirmed { get; set; }

    // аванс, якщо вносився
    public decimal AdvanceAmount { get; set; }

    public List<PartRequestDto> Parts { get; set; } = new();
}

public class PartRequestDto
{
    public string PartId { get; set; } = "";
    public int Quantity { get; set; }
}
