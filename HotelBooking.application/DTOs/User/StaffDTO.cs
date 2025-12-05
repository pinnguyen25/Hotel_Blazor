public class StaffDTO
{
    public int StaffId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Position { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}