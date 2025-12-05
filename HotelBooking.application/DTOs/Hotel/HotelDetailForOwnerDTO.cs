public class HotelDetailForOwnerDTO
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsVerified { get; set; }
    public string Status { get; set; } = "Active";
    public string CityName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string? ChainName { get; set; }
    public string? AccommodationTypeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<HotelImageDTO> Images { get; set; } = new();
    public List<AmenityDTO> Amenities { get; set; } = new();
    public List<PolicyDTO> Policies { get; set; } = new();
    public List<RoomTypeForOwnerDTO> RoomTypes { get; set; } = new();
}


public class HotelImageDTO
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
}
