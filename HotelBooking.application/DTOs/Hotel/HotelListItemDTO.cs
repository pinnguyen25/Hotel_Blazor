public class HotelListItemDTO : HotelBaseDTO
{
    // Mô tả ngắn 
    public string ShortDescription { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    // 2–3 tiện ích nổi bật
    public List<AmenityDTO> HighlightAmenities { get; set; } = new();
    // Wishlist cho user hiện tại
    public bool IsWishlist { get; set; } = false;
    // Giá & đánh giá
    public decimal? MinPricePerNight { get; set; }
    public decimal? MaxPricePerNight { get; set; }  
    public decimal? AvgPricePerNight { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; } = 0;
    // Phòng trống
    public int? AvailableRooms { get; set; }
    // Khả năng đặt phòng
    public bool IsBookable { get; set; }
    // Bổ sung cho search
    public int? MaxAdultCapacity { get; set; }
    public int? MaxChildCapacity { get; set; }
    public string? AccommodationType { get; set; }  // ← từ act.Name
    public string? Service {get;set;}
    public string? ChainName { get; set; }
}
