// 2. DTO dùng để trả về hiển thị (trong RoomTypeForOwnerDTO)
public class RoomTypeServiceDTO
{
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; } // Cần tên để hiển thị
    public int Quantity { get; set; } = 1;
    public string? Note { get; set; }
}

// 1. DTO dùng để hứng dữ liệu từ Owner nhập vào (trong RoomTypeCreateOrUpdateDTO)
public class RoomTypeServiceRequestDTO
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Note { get; set; } // Ví dụ: "Chỉ đưa đón 1 chiều"
}