using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;
public interface IAuthorizationService
{
    Task<bool> IsAdminAsync(int userId);
    Task<bool> IsOwnerAsync(int userId);
    Task<bool> IsAdminOrOwnerAsync(int userId, int? requiredOwnerId = null);
    Task<bool> CanManageHotelAsync(int userId, int hotelId);
    Task<bool> CanManageAccommodationAsync(int userId, int accommodationId);
    Task<bool> IsStaffAsync(int userId);
    Task<bool> IsStaffOfHotelAsync(int userId, int hotelId);
    Task<bool> CanOperateHotelAsync(int userId, int hotelId);
}
public class AuthorizationService : IAuthorizationService
{
    private readonly HotelBookingContext _context;

    public AuthorizationService(HotelBookingContext context)
    {
        _context = context;
    }
    private readonly string[] _operationalPositions = new[]
    {
        "Manager",
        "Receptionist" 
        // "Housekeeper" và "Security" không được vào đây
    };

    public async Task<bool> IsAdminAsync(int userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
    }

    public async Task<bool> IsOwnerAsync(int userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Owner");
    }

    public async Task<bool> IsAdminOrOwnerAsync(int userId, int? requiredOwnerId = null)
    {
        if (await IsAdminAsync(userId)) return true;
        if (requiredOwnerId.HasValue) return userId == requiredOwnerId.Value;
        return await IsOwnerAsync(userId);
    }

    public async Task<bool> CanManageHotelAsync(int userId, int hotelId)
    {
        if (await IsAdminAsync(userId)) return true;

        // 2. Check Owner sở hữu khách sạn này
        var isOwner = await _context.Hotels
            .AsNoTracking()
            .AnyAsync(h => h.Id == hotelId && h.OwnerId == userId && h.IsDeleted == false);
        if (isOwner) return true;

        // var position = await GetStaffPositionAsync(userId, hotelId);
        // return position == "General Manager";
        return false;
    }

    public async Task<bool> CanManageAccommodationAsync(int userId, int accommodationId)
    {
        if (await IsAdminAsync(userId)) return true;

        return await _context.AccommodationTypes
            .AnyAsync(a => a.Id == accommodationId && a.CreatedBy == userId && a.IsDeleted == false);
    }

    //staff
    private async Task<string?> GetStaffPositionAsync(int userId, int hotelId)
    {
        return await _context.Staffs
            .AsNoTracking()
            .Where(s => s.UserId == userId &&
                        s.HotelId == hotelId &&
                        s.IsActive &&
                        s.IsDeleted == false)
            .Select(s => s.Position)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsStaffAsync(int userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Staff");
    }

    public async Task<bool> IsStaffOfHotelAsync(int userId, int hotelId)
    {
        // Check trong bảng Staffs: User đó phải thuộc Hotel đó và KHÔNG bị khóa/xóa
        return await _context.Staffs
            .AsNoTracking()
            .AnyAsync(s => s.UserId == userId &&
                           s.HotelId == hotelId &&
                           s.IsActive == true &&
                           s.IsDeleted == false);
    }

    // Quyền VẬN HÀNH (Tác nghiệp: Check-in, Check-out, Thanh toán, Gán phòng)
    public async Task<bool> CanOperateHotelAsync(int userId, int hotelId)
    {
        // 1. Nếu là Admin hoặc Owner -> OK
        if (await CanManageHotelAsync(userId, hotelId)) return true;

        // 2. Nếu là Staff -> Check Position
        var position = await GetStaffPositionAsync(userId, hotelId);
        if (string.IsNullOrEmpty(position)) return false;

        // Check xem Position có nằm trong list cho phép không (bỏ qua hoa thường)
        return _operationalPositions.Contains(position, StringComparer.OrdinalIgnoreCase);
    }
}