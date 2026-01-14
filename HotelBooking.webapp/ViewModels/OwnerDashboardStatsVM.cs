public class OwnerDashboardStatsVM
{
    // === TÀI CHÍNH ===
    public decimal TotalRevenue { get; set; }
    public decimal RevenueToday { get; set; }
    public decimal RevenueThisMonth { get; set; }

    // === VẬN HÀNH ===
    public int NewBookingsToday { get; set; }
    public int CheckInsToday { get; set; }
    public int CheckOutsToday { get; set; }

    // === HIỆU SUẤT ===
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }

    // === CHẤT LƯỢNG ===
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }

    // // Helper tính toán tỉ lệ lấp đầy để vẽ biểu đồ
    // public double OccupancyRate => TotalRooms > 0
    //     ? Math.Round((double)OccupiedRooms / TotalRooms * 100, 1)
    //     : 0;

    public List<BookingShortVM> RecentBookings { get; set; } = new();
    public List<BookingShortVM> TodayCheckIns { get; set; } = new();
    public List<BookingShortVM> TodayCheckOuts { get; set; } = new();
}

public class BookingShortVM
{
    public int Id { get; set; }
    public string GuestName { get; set; } // Tên khách (Đã xử lý vãng lai ở BE)
    public string HotelName { get; set; }
    public string HotelImage { get; set; } // <--- Bổ sung
    public int TotalDays { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RoomTypeNames { get; set; } // Tên các phòng
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}