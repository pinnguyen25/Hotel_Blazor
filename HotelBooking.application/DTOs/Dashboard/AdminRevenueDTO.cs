public class AdminRevenueDTO
{
    public decimal TotalLifetimeRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public List<RevenueChartDTO> MonthlyData { get; set; } // Dùng lại class DTO biểu đồ cũ
    public List<WalletTransactionDTO> RecentTransactions { get; set; }
}

public class HotelRevenueRankDTO
{
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string OwnerName { get; set; }
    public decimal TotalRevenue { get; set; } // Tổng GMV (Tổng tiền bán phòng)
    public decimal CommissionPaid { get; set; } // Tổng phí sàn đã đóng cho Admin
    public int TotalBookings { get; set; }
}