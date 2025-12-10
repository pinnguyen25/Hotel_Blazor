using System.Text.Json;
using System.ComponentModel.DataAnnotations; // Để dùng ValidationException nếu muốn
namespace HotelBooking.infrastructure.Models;

public partial class Policy : ISoftDelete
{
    public static Policy Create(string name, string? des, int poType)
    {
        // 1. Validate (Bảo vệ tính đúng đắn)
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên chính sách không được để trống.");
        }

        if (poType == null || poType == 0)
        {
            throw new ArgumentException("Loại chính sách không được để trống.", nameof(poType));
        }

        var policy = new Policy();

        policy.IsDeleted = false;
        policy.Name = name;
        policy.Description = des;
        policy.PolicyTypeId = poType;

        return policy;
    }

    public void Update(string name, string? des)
    {
        // 1. Validate (Bảo vệ tính đúng đắn)
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên tiện nghi không được để trống.");
        }
        

        // 2. Set dữ liệu
        this.Name = name;
        this.Description = des;
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