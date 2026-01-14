using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

public class RoomTypeCreateOrUpdateVM
{
    public int? Id { get; set; } // null khi create, dùng khi update
    [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giá mỗi đêm là bắt buộc")]
    [Range(10000, double.MaxValue, ErrorMessage = "Giá phòng không hợp lệ")]
    public decimal PricePerNight { get; set; }

    [Required(ErrorMessage = "Sức chứa người lớn là bắt buộc")]
    [Range(1, 20, ErrorMessage = "Sức chứa không hợp lệ")]
    public int AdultCapacity { get; set; } = 1;

    [Range(0, 10, ErrorMessage = "Sức chứa không hợp lệ")]
    public int ChildCapacity { get; set; } = 0;

    [Required(ErrorMessage = "Số lượng phòng là bắt buộc")]
    [Range(1, 1000, ErrorMessage = "Số lượng phòng phải lớn hơn 0")]
    public int Quantity { get; set; } = 1;
    public decimal? Area { get; set; } = 0; // m²
    public string? Description { get; set; }
    // --- Liên kết (Dạng ID hoặc Object đơn giản) ---
    public List<RoomBedRequestVM> Beds { get; set; } = new();
    public List<int> ViewIds { get; set; } = new();
    
    public List<int> AmenityIds { get; set; } = new();

    // --- DÙNG CHO HIỂN THỊ + XÓA ---
    public List<string> ImageUrls { get; set; } = new(); // Từ backend
    public List<string> ImagesToDelete { get; set; } = new();
    public List<RoomTypeServiceRequestVM>? RoomTypeServices { get; set; }

    public bool? IsFreeCancellation { get; set; } // Dùng bool? để check null khi update
    public bool? IsBreakfastIncluded { get; set; }
}

public class CreateRoomTypeResponseVM
{
    public int RoomTypeId { get; set; }
    public string Status { get; set; } = "Draft";
}

public class UploadRRoomTypeResponseVM
{
    public int RoomTypeId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class DeleteRoomTypeResponseVM
{
    public int RoomTypeId { get; set; }
    public string Message { get; set; } = "Xóa thành công.";
}

public class RoomBedRequestVM
{
    public int BedTypeId { get; set; }
    public int Quantity { get; set; }
}
