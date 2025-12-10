using System.Text.Json;
using System.ComponentModel.DataAnnotations; // Để dùng ValidationException nếu muốn
namespace HotelBooking.infrastructure.Models;

public partial class Amenity : ISoftDelete
{
    public static Amenity Create(string name, string? des, string iconClass, string? iconColor)
    {
        // 1. Validate (Bảo vệ tính đúng đắn)
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên tiện nghi không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(iconClass))
        {
            throw new ArgumentException("Tên icon không được để trống.", nameof(iconClass));
        }

        var amenity = new Amenity();

        amenity.IsDeleted = false;
        amenity.Name = name;

        var additional = new
        {
            Description = string.IsNullOrWhiteSpace(des) ? null : des.Trim(),
            IconClass = iconClass,
            // Logic default value nằm ở đây, không nằm ở Service
            IconColor = string.IsNullOrWhiteSpace(iconColor) ? "blue" : iconColor
        };

        amenity.Additional = JsonSerializer.Serialize(additional);

        return amenity;

    }

    public void Update(string name, string? des, string iconClass, string? iconColor)
    {
        // 1. Validate (Bảo vệ tính đúng đắn)
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên tiện nghi không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(iconClass))
        {
            throw new ArgumentException("Tên icon không được để trống.", nameof(iconClass));
        }

        // 2. Set dữ liệu
        this.Name = name;

        var additional = new
        {
            Description = string.IsNullOrWhiteSpace(des) ? null : des.Trim(),
            IconClass = iconClass,
            // Logic default value nằm ở đây, không nằm ở Service
            IconColor = string.IsNullOrWhiteSpace(iconColor) ? "blue" : iconColor
        };

        this.Additional = Additional = JsonSerializer.Serialize(additional);
    }

    public void Delete()
    {
        if (this.IsDeleted == true) return;

        // Logic xóa nằm gọn ở đây
        this.IsDeleted = true;

        // Nếu sau này muốn thêm logic: Set ngày xóa, đổi status...
        // Chỉ cần sửa ở đây, BaseManage không bị ảnh hưởng.
        // this.DeletedAt = DateTime.UtcNow;
    }
}