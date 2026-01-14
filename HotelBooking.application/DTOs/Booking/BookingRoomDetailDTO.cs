// 2. Chi tiết từng phòng trong đơn (Dùng chung cho cả Owner và User xem)
public class BookingRoomDetailDTO
{
    public int Id { get; set; } // BookingRoomId (Primary Key)

    // Thông tin loại phòng (Khách mua cái này)
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
    public string? BedTypeName { get; set; }
    public List<string> IncludedServices { get; set; } = new();
}
