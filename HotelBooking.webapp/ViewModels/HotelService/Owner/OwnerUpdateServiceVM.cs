using System.ComponentModel.DataAnnotations;

public class OwnerUpdateServiceVM
{
    [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm")]
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public bool IsActive { get; set; }
}
public class UpdateServiceQuantityVM
{
    public int BookingServiceId { get; set; } // ID dòng dịch vụ đã thêm
    public int NewQuantity { get; set; }      // Số lượng mới
}