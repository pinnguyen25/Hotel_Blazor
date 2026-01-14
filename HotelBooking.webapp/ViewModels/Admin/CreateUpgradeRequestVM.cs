using System.ComponentModel.DataAnnotations;

public class CreateUpgradeRequestVM
{
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    public string Address { get; set; } = null!;
    [Required(ErrorMessage = "Thiếu thông tin")]
    public string TaxCode { get; set; } = null!;
}