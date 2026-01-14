// HotelBooking.webapp.ViewModels.Hotel

public class RoomGroupVM
{
    public string BaseName { get; set; }
    public string Description { get; set; }
    public List<string> Images { get; set; } = new();
    public double Area { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildCapacity { get; set; }
    public List<AmenityVM> Amenities { get; set; } = new();
    public List<RoomBedTypesVM> Beds { get; set; } = new();
    
    // Danh sách các gói giá
    public List<RoomOptionVM> Options { get; set; } = new();
}

public class RoomOptionVM
{
    public int Id { get; set; } // RoomTypeId thực tế
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsFreeCancellation { get; set; }
    public bool IsBreakfastIncluded { get; set; }
    public int AvailableRooms { get; set; }
    public List<string> Services { get; set; } = new();
    
    // Thuộc tính hỗ trợ UI (không map từ DB)
    public int BookingQuantity { get; set; } = 0; 
}