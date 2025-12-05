using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

public class RoomTypeCreateOrUpdateVM
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giá mỗi đêm là bắt buộc")]
    [Range(10000, 100000000, ErrorMessage = "Giá phải từ 10.000 đến 100.000.000")]
    public decimal PricePerNight { get; set; }

    [Required(ErrorMessage = "Sức chứa người lớn là bắt buộc")]
    [Range(1, 20, ErrorMessage = "Sức chứa từ 1-20 người")]
    public int AdultCapacity { get; set; } = 1;

    [Range(0, 10, ErrorMessage = "Sức chứa trẻ em từ 0-10")]
    public int ChildCapacity { get; set; } = 0;

    [Required(ErrorMessage = "Số lượng phòng là bắt buộc")]
    [Range(1, 1000, ErrorMessage = "Số lượng phòng từ 1-1000")]
    public int TotalRooms { get; set; } = 1;
    public decimal? Area { get; set; } = 0; // m²
    public string? Description { get; set; }
    public List<RoomBedRequestVM> Beds { get; set; } = new();
    public List<RoomViewRequestVM> Views { get; set; } = new();
    
    public List<int> SelectedAmenityIds { get; set; } = new();

    // --- DÙNG CHO HIỂN THỊ + XÓA ---
    public List<string> ImageUrls { get; set; } = new(); // Từ backend
    public List<string> ImagesToDelete { get; set; } = new();

    public List<AmenityVM> AvailableAmenities { get; set; } = new();

}

public class RoomBedRequestVM
{
    public int BedTypeId { get; set; }
    public int Quantity { get; set; }
}

public class RoomViewRequestVM
{
    public int ViewTypeId { get; set; }
}
