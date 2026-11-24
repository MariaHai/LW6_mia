namespace LW3.Models;

// Електронна гарантія: 6 місяців або 10 000 км пробігу (що настане раніше)
public class Warranty
{
    public DateTime StartDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public int StartMileage { get; set; }
    public int ValidUntilMileage { get; set; }
    public List<string> CoveredWorks { get; set; } = new();
    public List<string> CoveredParts { get; set; } = new();
}
