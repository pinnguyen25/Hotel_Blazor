namespace HotelBooking.webapp.ViewModels.Hotel;

public class HotelDetailVM
{
    // === CƠ BẢN (Public + Owner) ===
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();

    // === ĐÁNH GIÁ & GIÁ ===
    public decimal AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public decimal? MinPricePerNight { get; set; }

    // === TRẠNG THÁI ===
    public bool IsVerified { get; set; } = false;
    public string Status { get; set; } = "PendingVerification";
    public bool IsWishlist { get; set; } = false;

    // === NỘI DUNG CHI TIẾT ===
    public List<AmenityVM> Amenities { get; set; } = new();
    public List<PolicyVM> Policies { get; set; } = new();
    public List<RoomTypeDetailVM> RoomTypes { get; set; } = new();
    public List<RoomGroupVM> RoomGroups { get; set; }
    public List<ReviewVM> Reviews { get; set; } = new();

    // === CHỈ OWNER THẤY (THÊM MỚI) ===
    public int OwnerId { get; set; } // Dùng để kiểm tra chính chủ
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ChainName { get; set; }
    public string? AccommodationTypeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // === ẢNH CHI TIẾT (OWNER) ===
    public List<HotelImageVM> GalleryImages { get; set; } = new();
}


public class HotelImageVM
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
}
