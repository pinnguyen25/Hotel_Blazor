public class RoomViewTypesVM
{
    public int ViewTypeId { get; set; }
    public string ViewTypeName { get; set; } = string.Empty;
    public string? ViewTypeIconClass { get; set; }
    public string? ViewTypeIconColor { get; set; }
    public bool IsPrimary { get; set; }
}