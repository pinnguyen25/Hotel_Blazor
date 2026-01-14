using System.Text.Json.Serialization;

public class RoomTypeDetailVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildCapacity { get; set; }
    public int Quantity { get; set; }
    public int? AvailableRooms { get; set; }
    public decimal? Area { get; set; }
    public bool? IsActive { get; set; } = true;
    public bool IsFreeCancellation { get; set; }
    public bool IsBreakfastIncluded { get; set; }
    public int? SortOrder { get; set; }
    // --- Danh sách chi tiết để hiển thị ---
    
    public string? DefaultImageUrl { get; set; }
    public List<string>? RoomImages { get; set; } = new();
    public List<AmenityVM> Amenities { get; set; } = new();
    public List<RoomBedTypesVM>? Beds { get; set; } = new();
    public List<RoomViewTypesVM>? Views { get; set; } = new();
    
    public List<RoomTypeServiceVM> RoomTypeServices { get; set; } = new();

    // --- Danh sách Id để lưu trữ ---
    public HashSet<int> SelectedAmenityIds { get; set; } = new();
    public HashSet<int> SelectedViewIds { get; set; } = new();
}

public class RoomBedTypesVM
{
    public int BedTypeId { get; set; }
    public string BedTypeName { get; set; } = string.Empty;
    public string? BedTypeIconClass { get; set; }
    public string? BedTypeIconColor { get; set; }
    public int Quantity { get; set; }
    public bool IsPrimary { get; set; }
}

public class RoomViewTypesVM
{
    public int ViewTypeId { get; set; }
    public string ViewTypeName { get; set; } = string.Empty;
    public string? ViewTypeIconClass { get; set; }
    public string? ViewTypeIconColor { get; set; }
    public bool IsPrimary { get; set; }
}