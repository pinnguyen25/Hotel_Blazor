
public class BookingCreateDTO
{
    public int HotelId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int TotalAdults { get; set; }
    public int TotalChildren { get; set; }
    
    // Thông tin liên hệ
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string? Note { get; set; }
    public string? VoucherCode { get; set; }
    public string PaymentMethod { get; set; } = "Direct";
    public List<BookingRoomDetailRequestDTO> SelectedRooms { get; set; } = new();
    public List<BookingServiceRequestDTO> Services { get; set; } = new();
}

public class BookingRoomDetailRequestDTO
{
    public int RoomTypeId { get; set; }       // Khách chọn loại phòng nào (Deluxe, Standard...)
    public int Quantity { get; set; } = 1;    // Số lượng phòng muốn đặt (VD: 2 phòng)
    public int? SelectedBedTypeId { get; set; } // Khách thích giường gì? (King/Twin) - Có thể null
}
public class BookingServiceRequestDTO
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
}
// 3. DTO Trả về sau khi đặt thành công
public class BookingResponseDTO
{
    public int BookingId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
    public string? PaymentUrl { get; set; } 
}
