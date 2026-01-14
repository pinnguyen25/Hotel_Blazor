public class StaffVM
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Position { get; set; }  // Receptionist, Housekeeper, Manager
    public bool IsActive { get; set; }
    public decimal? Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStaffRequestVM
{
    public int HotelId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
}

public class UpdateStaffRequestVM
{
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
    public bool IsActive { get; set; } // Khóa/Mở khóa
}