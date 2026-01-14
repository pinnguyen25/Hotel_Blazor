public class BookingCartVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = "";
    public string? HotelImage {get;set;}
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public List<CartItemVM>? SelectedRooms { get; set; }
    public DateTime Timestamp { get; set; }
}
public class CartItemVM 
{
    public int RoomTypeId { get; set; }
    public string RoomName { get; set; }
    public string ImageUrl { get; set; }
    public decimal PricePerNight { get; set; }
    public int Quantity { get; set; }
    public List<string> IncludedServices { get; set; } = new();
}