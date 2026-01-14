public class CustomerVM
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    
    // Thống kê nhanh (Tính toán từ bảng Bookings)
    public int TotalBookings { get; set; } 
    public decimal TotalSpent { get; set; } // Tổng tiền đã chi tiêu

    public bool IsActive { get; set; } // Trạng thái hoạt động
    public DateTime CreatedAt { get; set; }
}

public class OwnerVM
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? TaxCode { get; set; } // Lấy từ Users.TaxCode
    
    // Dữ liệu tổng hợp (Aggregated Data)
    public int TotalHotels { get; set; } // Count từ bảng Hotels
    public decimal WalletBalance { get; set; } // Lấy từ bảng OwnerWallets
    
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class UserDetailVM
{
    // === Thông tin chung (Common) ===
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    // === Dành riêng cho Owner (Nullable) ===
    public string? TaxCode { get; set; } // Mã số thuế
    public decimal? WalletBalance { get; set; } // Số dư ví
    public int? OwnedHotelsCount { get; set; } // Số lượng khách sạn

    // === Dành riêng cho Customer (Nullable) ===
    public int? TotalBookings { get; set; } // Tổng đơn đã đặt
    public decimal? TotalSpent { get; set; } // Tổng tiền đã chi
}