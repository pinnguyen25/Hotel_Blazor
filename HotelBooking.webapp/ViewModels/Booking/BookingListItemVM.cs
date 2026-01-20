// trang chi tiết đơn đặt phòng cho khách xem
public class BookingDetailVM
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string HotelAddress { get; set; }
    public string HotelImage { get; set; } // Cover Image

    // Info khách
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string Note { get; set; }

    // Info thời gian & tiền
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int TotalNights { get; set; }
    public decimal TotalPrice { get; set; }
    
    public decimal DepositRequired { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    
    public string Status { get; set; } // PendingPayment, Confirmed...
    public DateTime CreatedAt { get; set; }

    // List phòng & Dịch vụ
    public List<BookingRoomDetailVM> Rooms { get; set; } = new();
    public List<BookingServiceVM> Services { get; set; } = new();
}

public class BookingListItemVM
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    // Thêm 2 trường này
    public string? HotelName { get; set; } 
    public string? HotelImage { get; set; }

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Status { get; set; }
    public int TotalRooms { get; set; }
    public bool IsPaid { get; set; }
    public bool HasReviewed { get; set; }
}
