using System.ComponentModel.DataAnnotations;
public class AmenityDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    public string? IconClass { get; set; }
    public string? Description { get; set; }
    public string? Additional { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    public string? CreatedByName { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? IconColor { get; set; } = "#54a9ffff";
}

public class AmenityAdditional
{
    public string IconClass { get; set; } = null!;
    public string? IconColor { get; set; } = "#54a9ffff";
    public string? Description { get; set; }
}