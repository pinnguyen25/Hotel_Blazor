public class RoomBedTypesVM
{
    public int BedTypeId { get; set; }
    public string BedTypeName { get; set; } = string.Empty;
    public string? BedTypeIconClass { get; set; }
    public string? BedTypeIconColor { get; set; }
    public int Quantity { get; set; }
    public bool IsPrimary { get; set; }
}