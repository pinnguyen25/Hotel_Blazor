public class WithdrawalRequestAdminVM
{
    public int RequestId { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; }      // Quan trọng: Admin cần biết ai rút
    public string OwnerEmail { get; set; }     // Để liên hệ nếu cần
    
    public decimal Amount { get; set; }
    public decimal CurrentWalletBalance { get; set; } // Để Admin check xem ví còn đủ tiền không (nếu cần check tay)

    // Thông tin ngân hàng
    public string BankName { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankAccountName { get; set; }

    public string Status { get; set; }         // Pending, Approved, Rejected
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; } // Ngày xử lý
    public string? AdminNote { get; set; }     // Ghi chú của Admin (Mã giao dịch ngân hàng hoặc lý do từ chối)
}

public class AdminProcessWithdrawalVM
{
    public int RequestId { get; set; }
    public bool IsApproved { get; set; }       // true = Duyệt, false = Từ chối
    public string? AdminNote { get; set; }     // Nếu Duyệt: Nhập mã giao dịch NH. Nếu Từ chối: Nhập lý do.
}

public class HotelRevenueRankVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string OwnerName { get; set; }
    public decimal TotalRevenue { get; set; } // Tổng GMV (Tổng tiền bán phòng)
    public decimal CommissionPaid { get; set; } // Tổng phí sàn đã đóng cho Admin
    public int TotalBookings { get; set; }
}