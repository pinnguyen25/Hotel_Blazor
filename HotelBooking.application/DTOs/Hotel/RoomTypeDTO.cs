using System.ComponentModel.DataAnnotations.Schema;

public class RoomTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? PricePerNight { get; set; }
    public int? AdultCapacity { get; set; }
    public int? ChildCapacity { get; set; }
    public List<string> RoomImages { get; set; } = new List<string>();
    public int AvailableRooms { get; set; }  // Cần cho phần chọn phòng
    public List<AmenityDTO> Amenities { get; set; } = new();
}
