namespace HotelBooking.webapp.ViewModels.Hotel;

public class HotelDetailVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new List<string>();
    public double AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public decimal? MinPricePerNight { get; set; }
    public decimal? MaxPricePerNight { get; set; }
    public decimal? AvgPricePerNight { get; set; }
    public int? AvailableRooms { get; set; }
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";
    public bool IsWishlist { get; set; } = false;
    public List<AmenityVM> Amenities { get; set; } = new();
    // public List<PolicyVM> Policies { get; set; } = new();
    // public List<RoomTypeVM> RoomTypes { get; set; } = new();
}