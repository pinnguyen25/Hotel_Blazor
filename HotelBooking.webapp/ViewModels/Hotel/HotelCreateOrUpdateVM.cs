using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;

public class HotelCreateOrUpdateVM
{
    public int HotelId { get; set; }

    [Required(ErrorMessage = "Tên khách sạn là bắt buộc")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Thành phố là bắt buộc")]
    public int CityId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public int? AccommodationTypeId { get; set; }
    public int? ChainId { get; set; }

    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    [EmailAddress] public string? ContactEmail { get; set; }

    // Dữ liệu hiển thị (dropdown, checkbox)
    public record SelectItem(int Id, string Name, bool IsSelected = false);
    public List<CityVM> Cities { get; set; } = new();
    public List<AccommodationTypeVM> AccommodationTypes { get; set; } = new();
    public List<SelectItem> Chains { get; set; } = new();

}

public class HotelImagesVM
{
    public string? CoverImageUrl { get; set; }
    public List<string> GalleryImageUrls { get; set; } = new();
    // 2. Ảnh đang chờ upload (người dùng mới chọn, chưa submit)
    [JsonIgnore]
    public List<IBrowserFile> PendingGalleryFiles { get; set; } = new();

    // Helper
    public int TotalImages => (CoverImageUrl != null ? 1 : 0) + GalleryImageUrls.Count;
    public bool IsComplete => CoverImageUrl != null && GalleryImageUrls.Count >= 4;
}