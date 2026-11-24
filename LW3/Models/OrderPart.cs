namespace LW3.Models;

// Запчастина, погоджена та зарезервована в наряді
public class OrderPart
{
    public string PartId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
