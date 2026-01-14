public class ServiceAddOnVM
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
}

//Owner thêm dịch vụ vào đơn đặt phòng
public class AddServiceRequestVM
{
    public int BookingId { get; set; }
    public List<ServiceItemRequestVM> Services { get; set; } = new();
}

public class ServiceItemRequestVM
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
}