public class RoomTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int? AdultCapacity { get; set; }
    public int? ChildCapacity { get; set; }
    public int? MaxGuests => AdultCapacity + ChildCapacity; // Tính tự động
    public decimal Area { get; set; }
    public string? BedType { get; set; }
    public string? ViewType { get; set; }
    public int TotalRooms { get; set; }      // Tổng số phòng thuộc loại này
    public int AvailableRooms { get; set; }  // Số phòng còn trống
    public string? DefaultImageUrl { get; set; } // Hình ảnh đại diện
    public List<string> RoomImages { get; set; } = new();
    public List<AmenityDTO> Amenities { get; set; } = new();
    public List<RoomTypeServiceDTO> RoomTypeServices { get; set; } = new();
    public List<RoomBedTypeDTO> Beds { get; set; } = new();
    public List<RoomViewTypeDTO> Views { get; set; } = new();

    public bool IsFreeCancellation { get; set; }
    public bool IsBreakfastIncluded { get; set; }

}

