// DTO hiển thị danh sách đơn hàng cần xếp phòng (Pending/Confirmed)
public class OwnerBookingItemDTO
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } // PendingPayment, Confirmed...
    
    // Danh sách chi tiết các phòng trong đơn này
    public List<OwnerBookingRoomDetailDTO>? Rooms { get; set; }
}

public class OwnerBookingRoomDetailDTO
{
    public int BookingRoomId { get; set; } // ID của dòng trong bảng BookingRooms (PK)
    public string? RoomTypeName { get; set; }
    public string? BedTypeName { get; set; } // Khách yêu cầu giường gì?
    
    public int? AssignedRoomId { get; set; } // Đã gán phòng nào chưa? (Nếu null -> Hiện nút "Chọn phòng")
    public string? AssignedRoomNumber { get; set; } // Số phòng (VD: 101)
    
    public string? GuestName { get; set; } // Tên khách ở phòng này
}