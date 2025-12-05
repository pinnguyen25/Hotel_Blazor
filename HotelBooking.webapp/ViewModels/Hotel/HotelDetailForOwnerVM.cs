// public class HotelDetailForOwnerVM
// {
//     public int HotelId { get; set; }
//     public string HotelName { get; set; } = string.Empty;
//     public string Address { get; set; } = string.Empty;
//     public string? Description { get; set; }
//     public string? CoverImageUrl { get; set; }
//     public string? ContactName { get; set; }
//     public string? ContactPhone { get; set; }
//     public string? ContactEmail { get; set; }
//     public bool IsVerified { get; set; }
//     public bool IsActive { get; set; } = true;
//     public string Status { get; set; } = "Active";
//     public string CityName { get; set; } = string.Empty;
//     public string CountryName { get; set; } = string.Empty;
//     public string? ChainName { get; set; }
//     public string? AccommodationTypeName { get; set; }
//     public DateTime CreatedAt { get; set; }
//     public DateTime? UpdatedAt { get; set; }

//     public List<HotelImageVM> GalleryImages { get; set; } = new();
//     public List<AmenityVM> Amenities { get; set; } = new();
//     public List<PolicyVM> Policies { get; set; } = new();
//     public List<RoomTypeForOwnerVM> RoomTypes { get; set; } = new();
//     public HotelStatsVM Stats { get; set; } = new();
// }

// public class HotelImageVM
// {
//     public string ImageUrl { get; set; } = string.Empty;
//     public bool IsCover { get; set; }
//     public int SortOrder { get; set; }
// }

// public class HotelStatsVM
// {
//     public int TotalBookings { get; set; }
//     public decimal TotalRevenue { get; set; }
//     public double AvgRating { get; set; }
//     public int ReviewCount { get; set; }
//     public int TotalRooms { get; set; }
//     public int AvailableRooms { get; set; }
// }