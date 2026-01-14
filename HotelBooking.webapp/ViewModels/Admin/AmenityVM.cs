public class AmenityVM
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string IconColor { get; set; } = "#54a9ffff";
    public string? Additional { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsSelected { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? CreatedByName { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AmenityCreateOrUpdateVM
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? IconColor { get; set; }
}