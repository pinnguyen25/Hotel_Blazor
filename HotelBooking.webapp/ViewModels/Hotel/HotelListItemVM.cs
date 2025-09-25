namespace HotelBooking.webapp.ViewModels.Hotel;

public class HotelCardVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new List<string>();
    public List<AmenityVM> HighlightAmenities { get; set; } = new();
    public bool IsWishlist { get; set; } = false;
    public decimal? MinPricePerNight { get; set; }
    public double AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public int? AvailableRooms { get; set; }
    public int? MaxAdultCapacity { get; set; }
    public int? MaxChildCapacity { get; set; }
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";
}

public class AmenityVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconCode { get; set; } = string.Empty;
}