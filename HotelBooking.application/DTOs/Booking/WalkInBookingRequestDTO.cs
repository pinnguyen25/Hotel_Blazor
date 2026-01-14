public class WalkInBookingRequestDTO
{
    public int PhysicalRoomId { get; set; } // ID phòng vật lý muốn đặt
    public string GuestName { get; set; } = "Khách vãng lai";
    public string? Phone { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal Price { get; set; }      // Giá tiền chốt với khách
    public string Note { get; set; } = "";
}