
using System.ComponentModel.DataAnnotations;

public class BookingCreateVM
{
    public int HotelId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int TotalAdults { get; set; }
    public int TotalChildren { get; set; }
    
    // Thông tin liên hệ
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    public string? ContactName { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    public string? ContactPhone { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? ContactEmail { get; set; }
    public string? Note { get; set; }
    public string? VoucherCode { get; set; }
    public string PaymentMethod { get; set; } = "Direct";
    public List<BookingRoomDetailRequestVM> SelectedRooms { get; set; } = new();
    public List<ServiceAddOnVM> Services { get; set; } = new();
}

public class BookingRoomDetailRequestVM
{
    public int RoomTypeId { get; set; }       // Khách chọn loại phòng nào (Deluxe, Standard...)
    public int Quantity { get; set; } = 1;    // Số lượng phòng muốn đặt (VD: 2 phòng)
    public int? SelectedBedTypeId { get; set; } // Khách thích giường gì? (King/Twin) - Có thể null
    public string RoomTypeName { get; set; } = "";
    public decimal PricePerNight { get; set; }
    public List<string> IncludedServiceNames {get;set;} = new(); // dịch vụ đi kèm
}

// 3. DTO Trả về sau khi đặt thành công
public class BookingResponseVM
{
    public int BookingId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Message { get; set; }
    // Có thể thêm PaymentUrl nếu tích hợp VNPAY/Momo
    public decimal DepositAmount { get; set; }
    public string? Status { get; set; }
    public string? PaymentUrl { get; set; } 
}
