using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Components.Forms;

public class AccommodationTypeVM
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters")]
    public string? Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    public string? ImagePath { get; set; }  // hiển thị hình
    [NotMapped]
    public IBrowserFile? ImageFile { get; set; } // upload file từ Blazor
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? CreatedByName { get; set; }
}

public class AccommodationCreateOrUpdateVM
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters")]
    public string? Slug { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}