public class HotelCardDTO
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty; // Có thể lấy 150 ký tự đầu
    public string CoverImageUrl { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = new List<string>(); // Luôn chứa tối đa 4 ảnh
    public decimal? MinPricePerNight { get; set; }  // Giá phòng thấp nhất
    public string City { get; set; } = string.Empty; // lấy từ cities.Name
    public string Country { get; set; } = string.Empty; // countries.Name
    public List<string> Amenities { get; set; } = new();
    public List<string> RoomTypes { get; set; } = new();
    public bool IsAvailable { get; set; } = true;
    public double AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;

    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";

    public bool IsWishlist { get; set; } = false; // Cho user hiện tại
}
