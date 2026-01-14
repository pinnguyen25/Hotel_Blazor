//dùng để trả về hiển thị (trong RoomTypeForOwnerDTO)
using System.Text.Json.Serialization;

public class RoomTypeServiceVM
{
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; } // Cần tên để hiển thị
    public int Quantity { get; set; } = 1;
    public string? Note { get; set; }
}

// dùng để hứng dữ liệu từ Owner nhập vào (trong RoomTypeCreateOrUpdateDTO)
public class RoomTypeServiceRequestVM
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Note { get; set; } // Ví dụ: "Chỉ đưa đón 1 chiều"
}