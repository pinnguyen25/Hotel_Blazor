public class HotelOwnerSummaryVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    // Quy mô
    public int RoomTypeCount { get; set; }     // THÊM
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }

    // Hiệu suất
    public double AvgRating { get; set; }
    public int ReviewCount { get; set; }      

    // Kinh doanh
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }

    // Trạng thái
    public bool IsVerified { get; set; }   
    public bool IsActive { get; set; } = true; 
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
