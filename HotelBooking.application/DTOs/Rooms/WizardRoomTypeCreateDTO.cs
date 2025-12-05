using System.ComponentModel.DataAnnotations;

public class WizardRoomTypeCreateDTO
{
    [Required] public string Name { get; set; } = string.Empty;

    [Required]
    [Range(10000, 100000000)]
    public decimal PricePerNight { get; set; }

    [Required]
    [Range(1, 20)]
    public int AdultCapacity { get; set; } = 2;

    [Range(0, 10)]
    public int ChildCapacity { get; set; } = 0;

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; } = 1; // Giới hạn nhỏ hơn cho wizard

    public decimal? Area { get; set; }

    // Chỉ bắt buộc 1 loại giường chính (đơn giản hóa UX)
    [Required(ErrorMessage = "Vui lòng chọn ít nhất 1 loại giường")]
    public List<WizardBedConfigDTO> Beds { get; set; } = new();

    // View không bắt buộc ở wizard đầu tiên
    public List<int>? ViewTypeIds { get; set; }
}

public class WizardBedConfigDTO
{
    [Required] public int BedTypeId { get; set; }
    [Range(1, 4)] public int Quantity { get; set; } = 1;
}

public class WizardRoomTypeResponseDTO
{
    public int RoomTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Quantity { get; set; }
    public string Message { get; set; } = "Tạo loại phòng thành công!";
}