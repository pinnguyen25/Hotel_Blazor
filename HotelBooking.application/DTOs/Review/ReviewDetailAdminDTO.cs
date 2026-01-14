// DTO dành riêng cho Admin hiển thị danh sách
public class ReviewDetailAdminDTO
{
    public int Id { get; set; }
    
    // --- KHÁC BIỆT 1: Thông tin Khách sạn ---
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty; 

    // --- KHÁC BIỆT 2: Định danh User kỹ hơn ---
    public int CustomerId { get; set; } // Để click vào xem profile user
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty; // Quan trọng để Admin check spam
    public string? CustomerAvatar { get; set; }

    // --- Nội dung Review (Giống DTO cũ) ---
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? Reply { get; set; } // Admin cũng cần xem Owner đã trả lời chưa
    
    // --- KHÁC BIỆT 3: Trạng thái hệ thống ---
    public bool IsDeleted { get; set; } // Admin cần thấy cả review đã ẩn
    public DateTime CreatedAt { get; set; }
}