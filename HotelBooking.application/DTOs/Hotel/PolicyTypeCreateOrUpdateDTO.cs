using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class PolicyTypeCreateOrUpdateDTO
{
    [Required(ErrorMessage = "Tên loại chính sách là bắt buộc")]
    public string Code { get; set; } = string.Empty;
    [Required(ErrorMessage = "Tên loại chính sách là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên loại chính sách không được vượt quá 100 ký tự")]
    [JsonPropertyName("name")]
    public string TypeName { get; set; } = string.Empty;
}