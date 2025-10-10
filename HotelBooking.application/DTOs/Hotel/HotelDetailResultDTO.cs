using System.ComponentModel.DataAnnotations.Schema;

public class HotelDetailResultDTO
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string ImageUrls { get; set; } = "[]"; // JSON string
    public string Amenities { get; set; } = "[]"; // JSON string
    public string RoomTypes { get; set; } = "[]"; // JSON string
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinPricePerNight { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string Reviews { get; set; } = "[]"; // JSON string
    public int AvailableRooms { get; set; }
    // public string Policies { get; set; } = "[]"; // JSON string
    public bool IsWishlist { get; set; }
}