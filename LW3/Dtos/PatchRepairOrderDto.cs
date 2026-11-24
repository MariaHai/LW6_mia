using LW3.Models;

namespace LW3.Dtos;

public class PatchRepairOrderDto
{
    public OrderStatus? Status { get; set; }
    public double? NormHours { get; set; }
    public decimal? PartsCost { get; set; }
}
