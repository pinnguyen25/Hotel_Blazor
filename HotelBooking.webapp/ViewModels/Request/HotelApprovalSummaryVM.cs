public class HotelApprovalSummaryVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty; // Cần hiện tên chủ để biết ai gửi
    public string CoverImageUrl { get; set; } = string.Empty; // Hiện ảnh nhỏ cho đẹp
    public string Address { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    
    // Thuộc tính tiện ích để hiển thị full trên giao diện (Optional)
    public string FullAddress => $"{Address}, {CityName}";
    public string Status { get; set; } = "PendingVerification";
    public DateTime SubmittedAt { get; set; }
}