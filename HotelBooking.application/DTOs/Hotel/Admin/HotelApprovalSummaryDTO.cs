public class HotelApprovalSummaryDTO
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty; // Admin cần biết ai gửi
    public string CoverImageUrl { get; set; } = string.Empty; // Hiện 1 ảnh nhỏ
    public string FullAddress => $"{Address}, {CityName}";
    public string Address { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; } // Ngày cập nhật cuối cùng
}
