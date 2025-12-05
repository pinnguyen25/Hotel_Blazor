using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

// Chỉ dùng để tạo draft
public class HotelCreateOrUpdateDTO
{
    [Required(ErrorMessage = "Tên khách sạn là bắt buộc")]

    [StringLength(200, ErrorMessage = "Tên không được vượt quá 200 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]

    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Thành phố là bắt buộc")]
    public int CityId { get; set; }
    [StringLength(1000)]
    public string? Description { get; set; }
    public int? AccommodationTypeId { get; set; }
    public int? ChainId { get; set; }

    // LIÊN HỆ
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    [EmailAddress] public string? ContactEmail { get; set; }
}

// Dùng để thêm nhiều amenity/policy
public class HotelAmenitiesDTO
{
    public List<int> AmenityIds { get; set; } = new();
}

public class HotelPoliciesDTO
{
    public List<int> PolicyIds { get; set; } = new();
    public List<OwnerCustomPolicyDTO>? OwnerCustomPolicies { get; set; }
}

