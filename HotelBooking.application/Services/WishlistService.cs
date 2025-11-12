using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface IWishlistService
{
    public Task<bool> ToggleWishlistAsync(int userId, int hotelId);
    public Task<bool> IsInWishlistAsync(int userId, int hotelId);
    public Task<List<HotelListItemDTO>> GetUserWishlistAsync(int userId); // lấy danh sách hotel đã lưu
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

    public async Task<List<HotelListItemDTO>> GetUserWishlistAsync(int userId)
    {
        return await _context.Wishlists.Where(w => w.UserId == userId).Select(w => new HotelListItemDTO
        {
            HotelId = w.Hotel.Id,
            HotelName = w.Hotel.Name,
            Address = w.Hotel.Address,
            City = w.Hotel.City != null ? w.Hotel.City.Name : string.Empty,
            Country = w.Hotel.City != null && w.Hotel.City.Country != null ? w.Hotel.City.Country.Name : string.Empty,
            ShortDescription = !string.IsNullOrEmpty(w.Hotel.Description)
            ? w.Hotel.Description.Substring(0, Math.Min(150, w.Hotel.Description.Length)) + "..."
            : string.Empty,
            CoverImageUrl = w.Hotel.CoverImageUrl ?? string.Empty,
            ImageUrls = w.Hotel.HotelImages
                .OrderBy(i => i.Id)
                .Take(4)
                .Select(i => i.ImageUrl)
                .ToList(),
            HighlightAmenities = w.Hotel.HotelAmenities
                .Take(3)
                .Select(a => new AmenityDTO
                {
                    Id = a.Amenity.Id,
                    Name = a.Amenity.Name,
                    // IconCode = a.Amenity.IconCode ?? string.Empty
                })
                .ToList(),
            MinPricePerNight = w.Hotel.RoomTypes.Any()
                ? w.Hotel.RoomTypes.Min(r => r.PricePerNight)
                : null,

            AvailableRooms = w.Hotel.RoomTypes
                .SelectMany(rt => rt.Rooms)
                .Count(r => r.Status == "Available"),

            AverageRating = w.Hotel.Reviews.Any(r => r.Rating.HasValue)
                ? Math.Round((decimal)w.Hotel.Reviews.Average(r => r.Rating ?? 0), 1)
                : 0,

            ReviewCount = w.Hotel.Reviews.Count(r => r.Rating.HasValue),
            MaxAdultCapacity = w.Hotel.RoomTypes.Any() ? w.Hotel.RoomTypes.Max(rt => rt.AdultCapacity) : null, 
            MaxChildCapacity = w.Hotel.RoomTypes.Any() ? w.Hotel.RoomTypes.Max(rt => rt.ChildCapacity) : null,
            IsWishlist = true
        })
        .ToListAsync();
    }
}