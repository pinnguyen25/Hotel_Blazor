using System.ComponentModel.DataAnnotations;

public class RoomTypeCreateOrUpdateDTO
{
    // public int Id {get;set;}
    [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá mỗi đêm là bắt buộc")]
    [Range(10000, 100000000, ErrorMessage = "Giá phải từ 10.000 đến 100.000.000 VNĐ")]
    public decimal? PricePerNight { get; set; }

    [Required(ErrorMessage = "Sức chứa người lớn là bắt buộc")]
    [Range(1, 20, ErrorMessage = "Sức chứa người lớn từ 1 - 20")]
    public int? AdultCapacity { get; set; } = 2;

    [Range(0, 10, ErrorMessage = "Sức chứa trẻ em từ 0 - 10")]
    public int? ChildCapacity { get; set; } = 0;

    [Required(ErrorMessage = "Số lượng phòng là bắt buộc")]
    [Range(1, 1000, ErrorMessage = "Số lượng phòng từ 1 - 1000")]
    public int? Quantity { get; set; } = 1;
    public decimal? Area { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn loại giường")]
    public List<RoomBedRequestDTO>? Beds { get; set; } = new();   
    public List<int>? ViewIds { get; set; } = new();

    public bool? IsFreeCancellation { get; set; } // Dùng bool? để check null khi update
    public bool? IsBreakfastIncluded { get; set; }

    // 3. Tiện nghi: Chỉ cần ID -> Dùng List<int> (Bổ sung cái này)
    public List<int>? AmenityIds { get; set; } = new();
    public List<RoomTypeServiceRequestDTO>? RoomTypeServices { get; set; }
}
public class CreateRoomTypeResponseDTO
{
    public int RoomTypeId { get; set; }
    public string Status { get; set; } = "Draft";
}

public class UploadRRoomTypeResponseDTO
{
    public int RoomTypeId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class DeleteRoomTypeResponseDTO
{
    public int RoomTypeId { get; set; }
    public string Message { get; set; } = "Xóa thành công.";
}


public class RoomBedRequestDTO
{
    [Required]
    public int BedTypeId { get; set; }

    [Range(1, 10)]
    public int Quantity { get; set; } = 1;
    // public bool IsPrimary {get; set;} = false; // cho phép đánh dấu giường chính
}
