public class AdminRevenueVM
{
    public decimal TotalLifetimeRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public decimal TotalRefundLoss { get; set; }
    public List<RevenueChartVM> MonthlyData { get; set; } = new(); // Dùng lại class DTO biểu đồ cũ
    public List<WalletTransactionVM> RecentTransactions { get; set; }
}