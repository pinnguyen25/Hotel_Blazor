public class RoomTypeForOwnerVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildCapacity { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public decimal? Area { get; set; }
    public string? BedType { get; set; }
    public string? ViewType { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public string? DefaultImageUrl { get; set; }
    public List<string> RoomImages { get; set; } = new();
    public List<AmenityVM> Amenities { get; set; } = new();
    public List<RoomBedTypesVM> Beds { get; set; } = new();
    public List<RoomViewTypesVM> Views { get; set; } = new();
}