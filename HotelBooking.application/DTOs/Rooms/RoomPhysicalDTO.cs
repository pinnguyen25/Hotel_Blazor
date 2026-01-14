public class RoomPhysicalDTO
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string? RoomTypeName { get; set; } // Để hiển thị
    public string? RoomNumber { get; set; } // Tên phòng: 101, 102
    public string? Status { get; set; } // Available, Maintenance, Dirty
    public string? Floor { get; set; }
    public bool IsActive { get; set; } // Có thể tạm ẩn
    // Thông tin booking hiện tại
    public DateTime? CurrentCheckIn { get; set; }
    public DateTime? CurrentCheckOut { get; set; }

    // BỔ SUNG: Thông tin khách đang ở (Realtime)
    public string? CurrentGuestName { get; set; } 
    public int? CurrentBookingId { get; set; }// Master ID (Dùng cho Invoice, Add Service)
    public int? CurrentBookingRoomId { get; set; }// Detail ID (Dùng cho Update Guest Name)
    public DateTime? NextCheckIn { get; set; } // Khách sắp đến (để lễ tân biết đường dọn)
    // Thêm thông tin về housekeeping task hiện tại (nếu có)
    public string? HousekeeperName { get; set; } // Tên người dọn
    public string? MaintenanceReason { get; set; }
}

public class CreateRoomRequestDTO 
{
    public string? RoomNumber { get; set; }
}

public class UpdateRoomPhysicalDTO
{
    public string? RoomNumber { get; set; }
    public string? Status { get; set; } // Nếu Owner chuyển sang Maintenance, hệ thống phải chặn booking
}
// Thêm hành động Gán phòng
public class AssignRoomRequestDTO
{
    public int BookingRoomId { get; set; } // ID dòng trong bảng BookingRooms (đang null RoomId)
    public int PhysicalRoomId { get; set; } // ID phòng vật lý muốn gán vào
    public string? GuestName { get; set; } // Tên khách ở (nếu có)
}
public class UpdateGuestNameDTO
{
    public int BookingRoomId { get; set; }
    public string? GuestName { get; set; }
}