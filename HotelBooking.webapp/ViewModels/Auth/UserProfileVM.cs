using System.ComponentModel.DataAnnotations;

public class UserProfileVM
{
    public int Id { get; set; } // Cần ID để biết update ai

    public string Username { get; set; } // Thường là ReadOnly (không cho sửa)

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(150)]
    public string FullName { get; set; }

    [Phone]
    public string PhoneNumber { get; set; } // Lấy từ RegisterVM sang

    public string AvatarUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    
    public string Address { get; set; } 
    public string Nationality { get; set; }
}