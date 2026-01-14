public class OwnerDashboardStatsDTO
{
    // === NHÓM 1: TÀI CHÍNH (Quan trọng nhất với chủ) ===
    public decimal TotalRevenue { get; set; }       // Tổng doanh thu từ trước đến nay
    public decimal RevenueToday {get;set;}
    public decimal RevenueThisMonth { get; set; }   // Doanh thu tháng này (để so sánh hiệu quả)

    // === NHÓM 2: VẬN HÀNH HÔM NAY (Để lễ tân/chủ biết việc cần làm) ===
    public int NewBookingsToday { get; set; }
    public int CheckInsToday { get; set; }          // Số khách sẽ đến hôm nay
    public int CheckOutsToday { get; set; }         // Số khách sẽ đi hôm nay
    // public int PendingBookings { get; set; }        // Số đơn mới đang chờ xác nhận

    // === NHÓM 3: HIỆU SUẤT ===
    public int TotalRooms { get; set; }             // Tổng số phòng vật lý
    public int AvailableRooms { get; set; }         // Số phòng trống hiện tại (Realtime)
    public int OccupiedRooms { get; set; }

    // === 4. CHẤT LƯỢNG DỊCH VỤ  ===
    public double AverageRating { get; set; }       // Điểm trung bình (lấy từ bảng Reviews)
    public int TotalReviews { get; set; }           // Tổng số đánh giá
    
    // (Optional) Tỉ lệ lấp đầy
    // public double OccupancyRate => TotalRooms > 0 ? Math.Round((double)(TotalRooms - AvailableRooms) / TotalRooms * 100, 1) : 0;
    public List<BookingShortDTO> RecentBookings { get; set; } = new();
    public List<BookingShortDTO> TodayCheckIns { get; set; } = new();  // Đến hôm nay
    public List<BookingShortDTO> TodayCheckOuts { get; set; } = new();
}

public class BookingShortDTO
{
    public int Id { get; set; }
    public string GuestName { get; set; } // Tên khách (Xử lý ưu tiên ContactName)
    public string HotelName { get; set; } // Nếu cần
    public string HotelImage { get; set; } // Cần cái này để hiển thị ảnh
    public int TotalDays { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RoomTypeNames { get; set; } // Ví dụ: "Deluxe, Standard"
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}