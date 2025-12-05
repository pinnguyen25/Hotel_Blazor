using HotelBooking.webapp.ViewModels.Hotel;

public class RoomTypeVM
{
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; } = 0;
    public int AdultCapacity { get; set; } = 1;
    public int ChildCapacity { get; set; } = 0;
    // ===  field cho Owner Wizard ===
    public int TotalRooms { get; set; } = 1;
    public decimal Area { get; set; } = 0;
    public string? BedType { get; set; }
    public string? ViewType { get; set; }
    public List<string> Images { get; set; } = new();
    public int AvailableRooms { get; set; }

    public List<AmenityVM> Amenities { get; set; } = new();

}
