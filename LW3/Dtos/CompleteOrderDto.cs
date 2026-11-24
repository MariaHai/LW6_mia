namespace LW3.Dtos;

// Бізнес-задача 3: контроль якості, видача авто та відкриття гарантії
public class CompleteOrderDto
{
    // майстер зміни, який проводив контроль якості
    public string ShiftMechanicId { get; set; } = "";

    // результат контролю якості
    public bool QualityControlPassed { get; set; }

    // сума, яку клієнт сплатив зараз (кошторис мінус аванс)
    public decimal PaidAmount { get; set; }

    // пробіг на момент видачі (необов'язково; не менший за поточний)
    public int? CurrentMileage { get; set; }
}
