// 2. DTO để Owner thêm dịch vụ vào khách sạn
public class OwnerAddServiceDTO
{
    public int HotelId { get; set; }
    public int ServiceId { get; set; } // Chọn từ danh sách Admin tạo
    public decimal Price { get; set; }
    public string? Unit { get; set; }
}

// 3. DTO để Owner cập nhật giá/trạng thái
public class OwnerUpdateServiceDTO
{
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public bool IsActive { get; set; }
}

// 4. DTO hiển thị các dịch vụ "Chưa đăng ký" (để Owner chọn thêm)
public class AvailableServiceDTO
{
    public int Id { get; set; } // ServiceId
    public string? Name { get; set; }
    public string? Description { get; set; }
}

//Owner thêm dịch vụ vào đơn đặt phòng
public class AddServiceRequestDTO
{
    public int BookingId { get; set; }
    public List<ServiceItemRequestDTO> Services { get; set; } = new();
}

// Owner cập nhật số lượng dịch vụ trong đơn đặt phòng
public class UpdateServiceQuantityDTO
{
    public int BookingServiceId { get; set; } // ID dòng dịch vụ đã thêm
    public int NewQuantity { get; set; }      // Số lượng mới
}

public class ServiceItemRequestDTO
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
}