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
    // Giá & số phòng
    public decimal? MinPricePerNight { get; set; }
    public int? AvailableRooms { get; set; }
    // Trạng thái kiểm duyệt
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";

    public bool IsWishlist { get; set; } = false;
    // mở rộng:
    public List<AmenityDTO> Amenities { get; set; } = new();
    // public List<PolicyDTO> Policies { get; set; } = new();
    // public List<RoomTypeDTO> RoomTypes { get; set; } = new();

}
