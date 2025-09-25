using System.Linq;
using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface IHotelService
{
    public Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null);
    public Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null);
    public Task<string> GetOwnerDashBoard(int ownerId);
    public Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string cityName, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, int? userId);
}

public class HotelService : IHotelService
{
    private readonly HotelBookingContext _context; // check wishlist
    private readonly IHotelRepository _hotelRepository;
    public IUnitOfWork _dbu;

    public HotelService(HotelBookingContext context, IHotelRepository hotelRepository, IUnitOfWork dbu)
    {
        _context = context;
        _hotelRepository = hotelRepository;
        _dbu = dbu;
    }

    public async Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null)
    {
        // lấy hotel có rate >= 6.5 
        var query = _context.Hotels
        .Include(h => h.City).ThenInclude(c => c.Country)
        .Include(h => h.HotelImages)
        .Include(h => h.HotelAmenities).ThenInclude(ha => ha.Amenity)
        .Include(h => h.RoomTypes).ThenInclude(rt => rt.Rooms)
        .Include(h => h.Reviews)
        .Where(h => h.Reviews.Any() && h.Reviews.Average(r => r.Rating ?? 0) >= 6.5)
        .OrderByDescending(h => h.Reviews.Average(r => r.Rating ?? 0))
        .Take(10);  // Limit để tối ưu

        var hotels = await query.ToListAsync();

        // Load wishlistId nếu có userId
        var wishlistId = userId.HasValue
            ? await _context.Wishlists.Where(w => w.UserId == userId).Select(w => w.HotelId).ToListAsync()
            : new List<int>();

        var res = hotels.Select(h => new HotelListItemDTO
        {
            HotelId = h.Id,
            Name = h.Name,
            Address = h.Address,
            City = h.City?.Name ?? string.Empty,
            Country = h.City?.Country?.Name ?? string.Empty,
            ShortDescription = !string.IsNullOrEmpty(h.Description)
            ? h.Description.Substring(0, Math.Min(150, h.Description.Length)) + "..."
            : string.Empty,
            CoverImageUrl = h.CoverImageUrl ?? string.Empty,
            ImageUrls = h.HotelImages.OrderBy(i => i.Id).Take(4).Select(i => i.ImageUrl).ToList(),
            HighlightAmenities = h.HotelAmenities.Take(3)
            .Select(a => new AmenityDTO
            {
                Id = a.Amenity.Id,
                Name = a.Amenity.Name
                // IconCode = a.Amenity.IconCode ?? string.Empty;
            }).ToList(),

            MinPricePerNight = h.RoomTypes.Any()
            ? h.RoomTypes.Min(r => r.PricePerNight)
            : null,

            AvailableRooms = h.RoomTypes
            .SelectMany(rt => rt.Rooms)
            .Count(r => r.Status == "Available"),

            AverageRating = h.Reviews.Any(r => r.Rating.HasValue)
            ? Math.Round(h.Reviews.Average(r => r.Rating ?? 0), 1)
            : 0,

            ReviewCount = h.Reviews.Count(r => r.Rating.HasValue),
            MaxAdultCapacity = h.RoomTypes.Any() ? h.RoomTypes.Max(rt => rt.AdultCapacity) : null,
            MaxChildCapacity = h.RoomTypes.Any() ? h.RoomTypes.Max(rt => rt.ChildCapacity) : null,
            IsWishlist = wishlistId.Contains(h.Id)
        }).ToList();

        return res;
    }

    public async Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null)
    {
        var hotel = await _context.Hotels
        .Include(h => h.City).ThenInclude(c => c.Country)
        .Include(h => h.HotelImages)
        .Include(h => h.HotelAmenities).ThenInclude(ha => ha.Amenity)
        .Include(h => h.RoomTypes).ThenInclude(rt => rt.Rooms)
        .Include(h => h.Reviews)
        // .Include(h => h.Policies) // bổ sung khi có chính sách
        .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotel == null) return null;

        bool isWishlist = userId.HasValue && await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.HotelId == hotelId);

        // Map sang HotelDetailDTO
        return new HotelDetailDTO
        {
            HotelId = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            Description = hotel.Description,
            CoverImageUrl = hotel.CoverImageUrl ?? string.Empty,
            ImageUrls = hotel.HotelImages
            .Select(i => i.ImageUrl)
            .ToList(),

            AverageRating = hotel.Reviews.Any()
            ? Math.Round(hotel.Reviews.Average(r => r.Rating ?? 0), 1) : 0,

            ReviewCount = hotel.Reviews.Count,
            MinPricePerNight = hotel.RoomTypes.Any()
            ? hotel.RoomTypes.Min(r => r.PricePerNight) 
            : null,
            AvailableRooms = hotel.RoomTypes
            .SelectMany(rt => rt.Rooms)
            .Count(r => r.Status == "Available"),

            IsVerified = hotel.IsVerified ?? false, // nếu trong DB giá trị IsVerified = null, thì DTO sẽ nhận false  
            Status = hotel.Status,
            IsWishlist = isWishlist,
            Amenities = hotel.HotelAmenities
            .Select(a => new AmenityDTO
            {
                Id = a.Amenity.Id,
                Name = a.Amenity.Name
                // IconCode = a.Amenity.IconCode ?? string.Empty
            }).ToList(),
            // Policies = ... (implement nếu có)
            // RoomTypes = ... (implement nếu có)
        };
    }

    public async Task<string> GetOwnerDashBoard(int ownerId)
    {
        return await Task.FromResult($"Owner Dashboard for Owner ID: {ownerId}");
    }

    public async Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string cityName, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, int? userId = null)
    {
        // lấy danh sách HotelId từ SP
        var hotelId = await _context.Database
        .SqlQueryRaw<int>(
            "EXEC sp_SearchHotels @CityName={0}, @CheckIn={1}, @CheckOut={2}, @Adults={3}, @Children={4}, @Rooms={5}",
            cityName, checkIn, checkOut, adults, children, rooms)
        .ToListAsync();
        if (!hotelId.Any())
            return new List<HotelListItemDTO>();

        // load wishlistIds của user trước
        var wishlistIds = userId.HasValue
            ? await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.HotelId)
                .ToListAsync()
            : new List<int>();

        // lấy dữ liệu chi tiết ( gồm: ảnh, amenities, review,...)
        var hotels = await _context.Hotels
        .Include(h => h.City).ThenInclude(c => c.Country)
        .Include(h => h.HotelImages)
        .Include(h => h.HotelAmenities).ThenInclude(ha => ha.Amenity)
        .Include(h => h.RoomTypes).ThenInclude(rt => rt.Rooms)
        .Include(h => h.Reviews)
        .Where(h => hotelId.Contains(h.Id))
        .ToListAsync();
        // map sang DTO
        var res = hotels.Select(h => new HotelListItemDTO
        {
            HotelId = h.Id,
            Name = h.Name,
            Address = h.Address,
            City = h.City?.Name ?? string.Empty,
            Country = h.City?.Country?.Name ?? string.Empty,
            ShortDescription = !string.IsNullOrEmpty(h.Description)
            ? h.Description.Substring(0, Math.Min(150, h.Description.Length)) + "..."
            : string.Empty,

            CoverImageUrl = h.CoverImageUrl ?? string.Empty,
            ImageUrls = h.HotelImages.OrderBy(i => i.Id).Take(4).Select(i => i.ImageUrl).ToList(),

            HighlightAmenities = h.HotelAmenities.Take(3)
            .Select(a => new AmenityDTO
            {
                Id = a.Amenity.Id,
                Name = a.Amenity.Name
                // IconCode = a.Amenity.IconCode ?? string.Empty
            }).ToList(),

            MinPricePerNight = h.RoomTypes.Any()
            ? h.RoomTypes.Min(r => r.PricePerNight)
            : null,

            AvailableRooms = h.RoomTypes
            .SelectMany(rt => rt.Rooms)
            .Count(r => r.Status == "Available"),

            AverageRating = h.Reviews.Any(r => r.Rating.HasValue)
            ? Math.Round(h.Reviews.Average(r => r.Rating ?? 0), 1)
            : 0,

            ReviewCount = h.Reviews.Count(r => r.Rating.HasValue),

            MaxAdultCapacity = h.RoomTypes.Any() ? h.RoomTypes.Max(rt => rt.AdultCapacity) : null,
            MaxChildCapacity = h.RoomTypes.Any() ? h.RoomTypes.Max(rt => rt.ChildCapacity) : null,

            IsWishlist = wishlistIds.Contains(h.Id)
        }).ToList();

        return res;
    }
}