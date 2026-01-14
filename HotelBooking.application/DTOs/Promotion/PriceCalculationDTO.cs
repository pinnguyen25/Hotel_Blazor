// 1. Request tính giá (Frontend gửi lên khi chọn phòng hoặc nhập mã)
public class PriceCalculationDTO
{
    public int HotelId { get; set; }
    public int UserId { get; set; }
    public List<BookingRoomDetailRequestDTO> SelectedRooms { get; set; } = new();
    public List<BookingServiceRequestDTO> SelectedServices { get; set; } = new();
    public string? VoucherCode { get; set; }

    // Thêm ngày để tính số đêm (Quan trọng để tính tiền phòng chính xác)
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

// 2. Kết quả tính giá trả về
public class PriceResultDTO
{
    public decimal OriginalTotal { get; set; }    // Tổng gốc (Tiền phòng + Tiền dịch vụ)
    public decimal DiscountAmount { get; set; }   // Tiền được giảm
    public decimal FinalTotal { get; set; }       // Khách phải trả

    public int? AppliedPromotionId { get; set; }
    public string? PromotionName { get; set; }    // VD: "Giảm giá 10%"
    public string? PromotionCode { get; set; }    // VD: "WELCOME"

    public bool IsSuccess { get; set; }           // True: Áp dụng OK, False: Lỗi hoặc ko có mã
    public string? Message { get; set; }          // VD: "Mã hết hạn"
}



