using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface IWishlistService
{
    public Task<bool> ToggleWishlistAsync(int userId, int hotelId);
    public Task<bool> IsInWishlistAsync(int userId, int hotelId);
    public Task<List<HotelCardDTO>> GetUserWishlistAsync(int userId); // lấy danh sách hotel đã lưu
}

public class WishlistService : IWishlistService
{
    private readonly HotelBookingContext _context;
    public WishlistService(HotelBookingContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleWishlistAsync(int userId, int hotelId)
    {
        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.HotelId == hotelId);
        if (wishlist != null)
        {
            _context.Wishlists.Remove(wishlist);
        }
        else
        {
            _context.Wishlists.Add(new Wishlist { UserId = userId, HotelId = hotelId, CreatedAt = DateTime.Now });
        }

        await _context.SaveChangesAsync();
        return wishlist == null; // True nếu thêm wishlist mới , False nếu xóa
    }

    public async Task<bool> IsInWishlistAsync(int userId, int hotelId)
    {
        return await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.HotelId == hotelId);
    }

    public async Task<List<HotelCardDTO>> GetUserWishlistAsync(int userId)
    {
        return await _context.Wishlists.Where(w => w.UserId == userId).Select(w => new HotelCardDTO
        {
            HotelId = w.Hotel.Id,
            HotelName = w.Hotel.Name,
            Address = w.Hotel.Address,
            Description = !string.IsNullOrEmpty(w.Hotel.Description)
                ? w.Hotel.Description.Substring(0, Math.Min(150, w.Hotel.Description.Length))
                : string.Empty,
            CoverImageUrl = w.Hotel.CoverImageUrl ?? string.Empty,

            ImageUrls = w.Hotel.HotelImages
                .OrderBy(i => i.Id)
                .Take(4)
                .Select(i => i.ImageUrl)
                .ToList(),

            MinPricePerNight = w.Hotel.RoomTypes.Any()
                ? w.Hotel.RoomTypes.Min(r => r.PricePerNight)
                : null,

            City = w.Hotel.City != null ? w.Hotel.City.Name : string.Empty,
            Country = w.Hotel.City != null && w.Hotel.City.Country != null
                ? w.Hotel.City.Country.Name
                : string.Empty,

            Amenities = w.Hotel.HotelAmenities
                .Select(a => a.Amenity.Name)
                .ToList(),

            RoomTypes = w.Hotel.RoomTypes
                .Select(r => r.Name)
                .ToList(),

            // Có ít nhất 1 phòng còn "Available"
            IsAvailable = w.Hotel.RoomTypes
                .SelectMany(rt => rt.Rooms)
                .Any(r => r.Status == "Available"),

            AverageRating = w.Hotel.Reviews.Any(r => r.Rating.HasValue)
                ? Math.Round(w.Hotel.Reviews.Average(r => r.Rating ?? 0), 1)
                : 0,

            ReviewCount = w.Hotel.Reviews.Count(r => r.Rating.HasValue),

            IsVerified = w.Hotel.IsVerified ?? false,
            Status = w.Hotel.Status ?? "PendingVerification",
            IsWishlist = true
        })
.ToListAsync();

    }
}