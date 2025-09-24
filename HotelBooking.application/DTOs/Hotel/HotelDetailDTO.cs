public class HotelDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();

    public double AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;

    // public List<AmenityDTO> Amenities { get; set; } = new(); sau này thêm DTO
    // public List<PolicyDTO> Policies { get; set; } = new();
    // public List<RoomTypeDTO> RoomTypes { get; set; } = new();

    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";

    public bool IsWishlisted { get; set; } = false;
}
