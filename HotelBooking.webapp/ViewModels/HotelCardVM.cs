public class HotelCardVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new List<string>();
    public decimal? MinPricePerNight { get; set; } // Giá thấp nhất mỗi đêm
    public string City { get; set; } = string.Empty; // Thành phố
    public string Country { get; set; } = string.Empty; // Quốc gia
    public double AverageRating { get; set; } = 0; // Đánh giá trung bình
    public int ReviewCount { get; set; } = 0; // Số lượng đánh giá
    public bool IsWishlist { get; set; } = false;// Đánh dấu yêu thích
    public List<string> Amenities { get; set; } = new(); // Tiện nghi khách sạn
    public List<string> RoomTypes { get; set; } = new(); // Loại phòng có sẵn
    public bool IsAvailable { get; set; } = true; // Tình trạng phòng trống
    public bool IsVerified { get; set; } = false; // Xác thực khách sạn
    public string Status { get; set; } = "PendingVerification";
}