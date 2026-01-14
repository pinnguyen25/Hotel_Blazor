public class InvoiceDTO
{
    // Thông tin chung
    public int BookingId { get; set; }
    public string GuestName { get; set; } = ""; // Tên người đặt chính
    public string PhoneNumber { get; set; } = "";
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int TotalNights { get; set; }

    // Phần tiền phòng
    public decimal RoomTotal { get; set; }
    public List<BookingRoomDetailDTO> RoomDetails { get; set; } = new();
    public List<BookingServiceDTO> Services { get; set; } = new();
    public decimal ServiceTotal { get; set; }

    // Tổng cộng
    public decimal GrandTotal { get; set; }      // Tổng tất cả
    public decimal PaidAmount { get; set; }      // Đã trả trước (cọc)
    public decimal RemainingAmount { get; set; } // Cần thanh toán nốt
}