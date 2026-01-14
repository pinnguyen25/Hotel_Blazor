
public class SchedulerBookingVM
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int BookingId { get; set; }
    public string GuestName { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string RoomName { get; set; } = "";
}

public class MoveBookingRequestVM
{
    public int BookingId { get; set; }
    public int NewRoomId { get; set; }
    public DateTime NewCheckIn { get; set; }
    public DateTime NewCheckOut { get; set; }
}

public class WalkInBookingRequestVM
{
    public int PhysicalRoomId { get; set; }
    public string GuestName { get; set; } = "";
    public string? Phone { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal Price { get; set; }
    public string Note { get; set; } = "";
}