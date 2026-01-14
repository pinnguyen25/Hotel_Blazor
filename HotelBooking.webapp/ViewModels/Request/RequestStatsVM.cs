public class RequestStatsVM
{
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    // Thống kê Hotel (Mới thêm)
    public int PendingHotels { get; set; }
    public int TotalActiveHotels { get; set; }
}
