public class HotelUpdateRequestAdminDTO
{
    public int RequestId { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string OwnerName { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; }
    // Dữ liệu hiện tại đang chạy
    public HotelCreateOrUpdateDTO CurrentData { get; set; }
    // Dữ liệu mới chủ khách sạn muốn đổi
    public HotelCreateOrUpdateDTO NewData { get; set; }
}

public class HotelVerificationRequestDTO
{
    public bool IsApproved { get; set; }
    public string? Reason { get; set; } // Optional: Lý do từ chối
}