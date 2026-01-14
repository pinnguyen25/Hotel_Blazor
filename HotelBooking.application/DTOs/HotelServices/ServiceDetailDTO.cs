public class ServiceDetailDTO : ServicesDTO 
{
    // --- Các trường thống kê (Mapping từ SQL Aggregate) ---
    public int UsageCount { get; set; }       // Tổng số khách sạn đang sử dụng
    public decimal AveragePrice { get; set; } // Giá trung bình thị trường

    // --- Danh sách chi tiết (Map từ Result Set 2) ---
    public List<HotelServiceUsageDTO> UsedByHotels { get; set; } = new();
}