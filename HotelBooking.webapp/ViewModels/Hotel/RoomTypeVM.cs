using HotelBooking.webapp.ViewModels.Hotel;

public class RoomTypeVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? PricePerNight { get; set; }
    public int? AdultCapacity { get; set; }
    public int? ChildCapacity { get; set; }
    public List<string> RoomImages { get; set; } = new();
    public int AvailableRooms { get; set; }
    public List<AmenityVM> Amenities { get; set; } = new(); 

}
