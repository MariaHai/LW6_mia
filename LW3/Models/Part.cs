namespace LW3.Models;

// Запчастина на складі СТО
public class Part
{
    public string? Id { get; set; }
    public string Name { get; set; } = "";

    // фактичний залишок на складі
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
