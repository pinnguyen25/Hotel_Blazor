using System.Globalization;

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
    public double AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public List<ReviewDTO> Reviews { get; set; } = new();
    // Giá & số phòng
    public decimal? MinPricePerNight { get; set; }
    public string FormattedMinPricePerNight =>
        MinPricePerNight.HasValue
            ? MinPricePerNight.Value.ToString("N0", new CultureInfo("vi-VN")) + "đ / đêm"
            : "N/A";
    public int AvailableRooms { get; set; } = 0;
    public bool IsWishlist { get; set; } = false;

    // Trạng thái và xác thực
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";
    // mở rộng:
    public List<AmenityDTO> Amenities { get; set; } = new();
    // public List<PolicyDTO> Policies { get; set; } = new();
    public List<RoomTypeDTO> RoomTypes { get; set; } = new();

}
