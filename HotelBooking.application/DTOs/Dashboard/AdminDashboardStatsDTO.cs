public class AdminDashboardStatsDTO
{
    // 1. Thống kê phê duyệt (Quan trọng nhất với Admin)
    public int PendingHotels { get; set; }  // Cần duyệt gấp
    public int PendingUsers { get; set; }   // User xin lên Owner (nếu có)

    // 2. Thống kê quy mô hệ thống
    public int ActiveHotels { get; set; }
    public int TotalUsers { get; set; }
    public int TotalHotels { get; set; }
    public int TotalBookings { get; set; } // Tổng booking toàn sàn (để xem độ sôi động)

    // 3. Thống kê tiêu cực (để xử lý sự cố)
    public int RejectedHotels { get; set; }
    public decimal TotalSystemRevenue { get; set; } // Tổng tiền booking toàn hệ thống (GMV)
    public decimal RevenueThisMonth { get; set; }

    public decimal PlatformTotalCommission { get; set; } // Tổng hoa hồng sàn đã thu (từ WalletTransactions)
    // Quản lý dòng tiền nợ (Rút tiền)
    public int PendingWithdrawals { get; set; }         // Số lượng yêu cầu rút tiền đang chờ duyệt
    public decimal CurrentWithdrawalDebt { get; set; }   // Tổng số tiền đang chờ thanh toán cho Owner
    // Quản lý thay đổi thông tin
    public int PendingUpdateRequests { get; set; }      // Số lượng yêu cầu sửa thông tin khách sạn
    // Đối tác
    public int TotalOwners { get; set; }                // Số lượng người dùng là chủ khách sạn
    // Chỉ số chất lượng toàn sàn
    public decimal SystemAvgRating { get; set; }        // Điểm đánh giá trung bình toàn hệ thống
    public int TotalReviews { get; set; }
}