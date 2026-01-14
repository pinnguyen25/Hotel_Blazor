public class InvoiceVM
{
    public string GuestName { get; set; }
    public string PhoneNumber { get; set; } = "";
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int TotalNights { get; set; }
    public decimal RoomTotal { get; set; }
    public decimal ServiceTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public List<BookingServiceVM> Services { get; set; } = new();
    public List<BookingRoomDetailVM> RoomDetails { get; set; } = new();
}