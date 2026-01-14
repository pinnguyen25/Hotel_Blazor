public class ReviewCreateVM
{
    public int BookingId { get; set; } // Chỉ cần gửi BookingId
    public decimal Rating { get; set; } // 1 - 10
    public string Comment { get; set; } = string.Empty;
}

public class ReviewVM
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAvatar { get; set; } // URL ảnh đại diện
    
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? Reply { get; set; } // Chủ khách sạn trả lời
    
    public DateTime CreatedAt { get; set; }
    public string RoomTypeName { get; set; } = string.Empty; // "Phòng Deluxe Double"
    public int StayNights { get; set; }
}

public class ReplyReviewVM
{
    public int ReviewId { get; set; }
    public string? Reply { get; set; }
}
