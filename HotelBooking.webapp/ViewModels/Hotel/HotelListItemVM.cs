namespace HotelBooking.webapp.ViewModels.Hotel;
using System.ComponentModel.DataAnnotations;
public class HotelListItemVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string? ShortDescription { get; set; }

    // Hình ảnh
    public string? CoverImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();

    // Rating & Review
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }

    // Giá
    public decimal? MinPricePerNight { get; set; }
    public decimal? MaxPricePerNight { get; set; }
    public decimal? AvgPricePerNight { get; set; }

    // Phòng & Sức chứa
    public int? AvailableRooms { get; set; }
    public int? MaxAdultCapacity { get; set; }
    public int? MaxChildCapacity { get; set; }

    // Tiện nghi nổi bật
    public List<AmenityVM> HighlightAmenities { get; set; } = new();

    // Wishlist
    public bool IsWishlist { get; set; }
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";
}

public class AmenityVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Parse từ Additional JSON

    [Required(ErrorMessage = "Icon class is required!")]
    public string? IconClass { get; set; }
    public string IconColor { get; set; } = "color: #4ea6feff";
}