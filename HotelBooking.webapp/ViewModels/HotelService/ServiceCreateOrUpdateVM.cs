using System.ComponentModel.DataAnnotations;

public class ServiceCreateOrUpdateVM
{
    [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
    [MaxLength(100, ErrorMessage = "Tên dịch vụ không được quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
    public string? Description { get; set; }
}