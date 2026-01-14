// DTO đại diện cho 1 nhóm phòng (VD: Phòng Deluxe)
public class RoomGroupDTO
{
    public string BaseName { get; set; } // Tên chung
    public List<string> Images { get; set; }
    public List<AmenityDTO> Amenities { get; set; }
    public decimal Area { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildCapacity { get; set; }
    public List<RoomBedTypeDTO> Beds { get; set; }
    public List<RoomOptionDTO> Options { get; set; } // Danh sách các gói giá
}

// DTO đại diện cho 1 lựa chọn giá (VD: Gói bao ăn sáng, Gói không hoàn hủy...)
public class RoomOptionDTO
{
    public int Id { get; set; } // RoomTypeId thực tế để đặt
    public string Name { get; set; } 
    public decimal Price { get; set; }
    public bool IsFreeCancellation { get; set; }
    public bool IsBreakfastIncluded { get; set; }
    public int AvailableRooms { get; set; }
    public List<string> Services { get; set; } // Các dịch vụ đi kèm
}