using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;
public interface IAuthorizationService
{
    Task<bool> IsAdminAsync(int userId);
    Task<bool> IsOwnerAsync(int userId);
    Task<bool> IsAdminOrOwnerAsync(int userId, int? requiredOwnerId = null);
    Task<bool> CanManageHotelAsync(int userId, int hotelId);
    Task<bool> CanManageAccommodationAsync(int userId, int accommodationId);
}
public class AuthorizationService : IAuthorizationService
{
    private readonly HotelBookingContext _context;

    public AuthorizationService(HotelBookingContext context)
    {
        _context = context;
    }

    public async Task<bool> IsAdminAsync(int userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin");
    }

    public async Task<bool> IsOwnerAsync(int userId)
    {
        return await _context.UserRoles
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

        return await _context.Hotels
            .AnyAsync(h => h.Id == hotelId && h.OwnerId == userId && h.IsDeleted == false);
    }

    public async Task<bool> CanManageAccommodationAsync(int userId, int accommodationId)
    {
        if (await IsAdminAsync(userId)) return true;

        return await _context.AccommodationTypes
            .AnyAsync(a => a.Id == accommodationId && a.CreatedBy == userId && a.IsDeleted == false);
    }
}