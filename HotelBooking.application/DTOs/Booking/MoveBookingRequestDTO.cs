public class MoveBookingRequestDTO
{
    public int BookingId { get; set; }      // ID của Booking (Đơn hàng)
    public int NewRoomId { get; set; }      // ID phòng vật lý mới (Target)
    public DateTime NewCheckIn { get; set; }
    public DateTime NewCheckOut { get; set; }
}