
using System.ComponentModel.DataAnnotations;

public class BannerUpdateDTO
{
    [Required]
    public string Page { get; set; }
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}