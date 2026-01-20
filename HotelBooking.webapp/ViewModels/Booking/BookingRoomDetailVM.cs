
public class BookingRoomDetailVM
{
    public int Id { get; set; } // BookingRoomId (Primary Key)

    // Thông tin loại phòng (Khách mua cái này)
    public int BookingId { get; set; }
    public int RoomTypeId { get; set; }
    public string? RoomTypeName { get; set; }
    public decimal PricePerNight { get; set; }

    // Thông tin phòng vật lý (Quan trọng cho Lễ tân)
    public int? RoomId { get; set; } // Có thể null nếu chưa xếp phòng
    public string? RoomNumber { get; set; } // VD: "101", "VIP-02" (Lấy từ bảng Rooms)

    // Thông tin khách ở (Quan trọng cho Công an/Lễ tân)
    public string? GuestName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal Price { get; set; }
    // Trạng thái phụ (Optional)
    // public bool IsCheckedIn { get; set; } // Logic: RoomId != null && BookingStatus == CheckedIn
    public int Quantity { get; set; }
}
