public class OwnerHotelServiceDTO
{
    public int Id { get; set; } // Id của bảng trung gian HotelService
    public int HotelId {get;set;}
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; } // Mô tả gốc của Admin
    public decimal Price { get; set; }       // Giá do Owner set
    public string? Unit { get; set; }        // Đơn vị (lượt, kg, giờ...)
    public bool IsActive { get; set; }       // Owner có đang bật không?
    public int CreatedBy {get;set;}
    public DateTime? CreatedAt {get;set;}
    public int UpdateBy {get;set;}
}