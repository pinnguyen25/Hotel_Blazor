public class SearchResultVM
{
    public string Destination { get; set; } = "";
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int Adults { get; set; } = 1;
    public int Children { get; set; } = 0;
    public int Rooms { get; set; } = 1;
    public string? HotelName { get; set; }
    public string? AccommodationType { get; set; } // Khách sạn, Homestay...
    
}