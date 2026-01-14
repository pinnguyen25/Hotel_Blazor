
using System.ComponentModel.DataAnnotations;

public class OwnerAddServiceVM
{
    public int HotelId { get; set; } // Lấy từ URL

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn một dịch vụ")]
    public int ServiceId { get; set; } // Dropdown chọn

    [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm")]
    public decimal Price { get; set; }

    public string? Unit { get; set; } = "Lượt"; // Giá trị mặc định
}


// 4. Dùng cho Dropdown chọn dịch vụ (Mapping từ API get-available-services)
public class AvailableServiceVM
{
    public int Id { get; set; } // ServiceId
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}
