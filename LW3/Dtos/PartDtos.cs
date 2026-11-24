namespace LW3.Dtos;

public abstract class PartDetailsDto
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// POST /api/Parts
public class CreatePartDto : PartDetailsDto
{
}

// PUT /api/Parts/{id}
public class UpdatePartDto : PartDetailsDto
{
}

public class PartResponseDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
