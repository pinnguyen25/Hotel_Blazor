public class HotelServiceUsageDTO
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty; // Tên chủ khách sạn (Admin cần biết ai quản lý)
    public decimal Price { get; set; }    // Giá mà khách sạn này set
    public string? Unit { get; set; }     // Đơn vị tính (kg, chiếc, lần...)
    public bool IsActive { get; set; } = true;   // Khách sạn có đang bật dịch vụ này không?
}