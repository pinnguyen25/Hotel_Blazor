public class HotelDetailForOwnerDTO : HotelDetailDTO
{
    // Thông tin quản lý
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    // Thống kê tổng quan
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalBookings { get; set; }
    public decimal? TotalRevenue { get; set; }

    // Liên hệ quản lý (nếu cần)
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }

    // Có thể thêm các chính sách nếu Owner được phép quản lý
    // public List<PolicyDTO> Policies { get; set; } = new();
}