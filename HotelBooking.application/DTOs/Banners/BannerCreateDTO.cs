
using System.ComponentModel.DataAnnotations;

public class BannerCreateDTO
{
    [Required]
    public string Page { get; set; } = "Home"; // Mặc định là trang chủ
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    
    [Required(ErrorMessage = "Vui lòng chọn ít nhất 1 ảnh")]
    public List<IFormFile> Images { get; set; } = new();
}
