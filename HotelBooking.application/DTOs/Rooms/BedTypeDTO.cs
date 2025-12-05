public class BedTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string IconColor { get; set; } = "#54a9ffff";
}

public class BedTypeAdditional
{
    public string IconClass { get; set; } = null!;
    public string? IconColor { get; set; } = "#54a9ffff";
    public string? Description { get; set; }
}