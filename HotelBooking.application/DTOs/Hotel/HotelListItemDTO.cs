public class HotelListItemDTO
{
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    // Mô tả ngắn 
    public string ShortDescription { get; set; } = string.Empty;
    // Ảnh 
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    // 2–3 tiện ích nổi bật
    public List<AmenityDTO> HighlightAmenities { get; set; } = new();
    // Wishlist cho user hiện tại
    public bool IsWishlist { get; set; } = false; 
    // Giá & đánh giá
    public decimal? MinPricePerNight { get; set; } 
    public double AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    // Phòng trống
    public int? AvailableRooms { get; set; }
    // Bổ sung cho search
    public int? MaxAdultCapacity { get; set; }
    public int? MaxChildCapacity { get; set; }
}
