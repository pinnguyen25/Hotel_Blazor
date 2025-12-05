public class HotelResponseDTO : HotelCreateOrUpdateDTO
{
    public int HotelId { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime UpdatedAt { get; set; }
    public string Message { get; set; } = string.Empty;
    // public string Message { get; set; } = "Xóa thành công.";
}