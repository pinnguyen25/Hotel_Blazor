using System.Globalization;
using HotelBooking.infrastructure.Models;

public class HotelDetailDTO
{
    public int HotelId { get; set; }
    public string HotelName  { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // ảnh
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    // đánh giá
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; } = 0;
    public List<ReviewDTO> Reviews { get; set; } = new();
    // Giá & số phòng
    public decimal? MinPricePerNight { get; set; }

    public int AvailableRooms { get; set; } = 0;
    public bool IsWishlist { get; set; } = false;

    // Trạng thái và xác thực
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";
    // mở rộng:
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<AmenityDTO> Amenities { get; set; } = new();
    public List<PolicyDTO> Policies { get; set; } = new();
    public List<RoomTypeDTO> RoomTypes { get; set; } = new();

}
