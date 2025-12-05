public class BedTypeCreateOrUpdateDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string IconColor { get; set; } = "#54a9ffff";
}