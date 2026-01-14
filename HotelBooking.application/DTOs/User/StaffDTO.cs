public class StaffDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Position { get; set; }
    public bool IsActive { get; set; }
    public decimal? Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

// DTO để tạo nhân viên mới
public class CreateStaffRequestDTO
{
    public int HotelId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // Vị trí cực kỳ quan trọng để phân quyền sau này
    // Giá trị gợi ý: "Receptionist", "Housekeeper", "Manager"
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
}

public class UpdateStaffRequestDTO
{
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
    public bool IsActive { get; set; } // Khóa/Mở khóa
}