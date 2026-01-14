public class SchedulerBookingDTO
{
    public int Id { get; set; }           // BookingRoomId (hoặc BookingId tùy logic click)
    public int RoomId { get; set; }       // Để Radzen biết booking này nằm ở dòng phòng nào
    public int BookingId { get; set; }
    public string GuestName { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string RoomName { get; set; } = ""; // Tên phòng (để hiển thị tooltip nếu cần)
}