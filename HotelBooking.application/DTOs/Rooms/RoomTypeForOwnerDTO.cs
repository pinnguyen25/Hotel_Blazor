using System.Text.Json.Serialization;
using HotelBooking.infrastructure.Models;

public class RoomTypeForOwnerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int? AdultCapacity { get; set; }
    public int? ChildCapacity { get; set; }
    public int? MaxGuests => AdultCapacity + ChildCapacity; // Tính tự động
    public int? Quantity { get; set; }
    public int? AvailableRooms { get; set; }  // Số phòng còn trống
    public decimal? Area { get; set; }
    public bool? IsActive { get; set; }
    public int? SortOrder { get; set; }
    // hình ảnh
    public List<string>? RoomImages { get; set; } = new();
    public string? DefaultImageUrl { get; set; }
    // tiện ích
    public List<AmenityDTO> Amenities { get; set; } = new();
    // giường và view
    public List<RoomBedTypeDTO>? Beds { get; set; }
    public List<RoomViewTypeDTO>? Views { get; set; }
    // [JsonPropertyName("Services")]
    public List<RoomTypeServiceDTO>? RoomTypeServices { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsFreeCancellation { get; set; }
    public bool IsBreakfastIncluded { get; set; }
}

public class RoomImageDTO
{
    public string? ImageUrl { get; set; } = string.Empty;
    public bool? IsDefault { get; set; }
    public int? SortOrder { get; set; }
}
