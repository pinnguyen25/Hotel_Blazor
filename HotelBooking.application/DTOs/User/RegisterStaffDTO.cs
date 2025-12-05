public class RegisterStaffDTO
{
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int HotelId { get; set; }           // Hotel mà Staff làm việc
    public string? Position { get; set; }       // Receptionist, Manager, Cleaner...
}