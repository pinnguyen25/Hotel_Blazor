using System.ComponentModel.DataAnnotations;

public class AccommodationCreateOrUpdateDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters")]
    public string? Slug { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}
