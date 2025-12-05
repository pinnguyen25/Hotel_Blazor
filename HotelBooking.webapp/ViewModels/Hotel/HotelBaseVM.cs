public class HotelBaseVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Status { get; set; } = "PendingVerification";
    public bool IsVerified { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}