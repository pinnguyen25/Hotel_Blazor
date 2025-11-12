public class EventBookingDTO
{
    public int Id { get; set; }
    public string HotelName { get; set; }
    public string EventName { get; set; }
    public string CustomerName { get; set; }
    public DateTime EventDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}
