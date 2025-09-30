using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HotelBooking.application.Helpers;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
public interface IHotelService
{
    // Lấy hotel theo id
    public Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null);
    // Lấy hotel rate cao
    public Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null);
    public Task<string> GetOwnerDashBoard(int ownerId);
    // search hotel theo name
    public Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string? destination, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, int? userId);
    public Task<List<CityDTO>> GetCityNameAsync();
    Task<List<AutocompleteDTO>> GetAutocompleteAsync(string keyword);
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
        // Load wishlistId nếu có userId
        var wishlistId = userId.HasValue
            ? await _context.Wishlists.Where(w => w.UserId == userId).Select(w => w.HotelId).ToListAsync()
            : new List<int>();

        var topHotels = await _context.Hotels
            .AsNoTracking()
            .Where(h => h.Reviews.Any() && h.Reviews.Average(r => r.Rating ?? 0) >= 6.5)
            .OrderByDescending(h => h.Reviews.Average(r => r.Rating ?? 0))
            .Take(10)
            .Select(h => new HotelListItemDTO
            {
                HotelId = h.Id,
                HotelName = h.Name,
                Address = h.Address,
                City = h.City.Name,
                Country = h.City.Country.Name,
                CoverImageUrl = h.CoverImageUrl,
                ImageUrls = h.HotelImages
                    .OrderBy(i => i.Id)
                    .Take(4)
                    .Select(i => i.ImageUrl)
                    .ToList(),
                HighlightAmenities = h.HotelAmenities
                    .Take(3)
                    .Select(a => new AmenityDTO
                    {
                        Id = a.Amenity.Id,
                        Name = a.Amenity.Name
                    })
                    .ToList(),
                MinPricePerNight = h.RoomTypes.Min(r => r.PricePerNight),
                MaxPricePerNight = h.RoomTypes.Max(r => r.PricePerNight),
                AvgPricePerNight = Math.Round(h.RoomTypes.Average(r => r.PricePerNight), 0),
                AvailableRooms = h.RoomTypes
                    .SelectMany(rt => rt.Rooms)
                    .Count(r => r.Status == "Available"),
                AverageRating = Math.Round(h.Reviews.Average(r => r.Rating ?? 0), 1),
                ReviewCount = h.Reviews.Count(r => r.Rating.HasValue),
                MaxAdultCapacity = h.RoomTypes.Max(rt => rt.AdultCapacity),
                MaxChildCapacity = h.RoomTypes.Max(rt => rt.ChildCapacity),
                IsWishlist = wishlistId.Contains(h.Id)
            })
            .ToListAsync();
        return topHotels;
    }

    public async Task<List<CityDTO>> GetCityNameAsync()
    {
        var countryCode = await _context.Countries
            .Where(co => co.Code == "VN")
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        var cities = await _context.Cities
            .Where(c => c.CountryId == countryCode)
            .ToListAsync(); 

        var result = cities.Select(c =>
        {
            var slug = c.Name.Slugify();
            return new CityDTO
            {
                Id = c.Id,
                Name = c.Name,
                Slug = slug,
                CountryId = c.CountryId,
                CoverImageUrl = $"/images/cities/{slug}.jpg"
            };
        }).ToList();

        return result;
    }

    public async Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null)
    {

        bool isWishlist = userId.HasValue && await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.HotelId == hotelId);

        // Map sang HotelDetailDTO
        var hotel = await _context.Hotels.Where(h => h.Id == hotelId).Select(h => new HotelDetailDTO
        {
            HotelId = h.Id,
            HotelName = h.Name,
            Address = h.Address,
            Description = h.Description,
            CoverImageUrl = h.CoverImageUrl ?? string.Empty,
            ImageUrls = h.HotelImages
            .Select(i => i.ImageUrl)
            .ToList(),

            AverageRating = h.Reviews.Any()
            ? Math.Round(h.Reviews.Average(r => r.Rating ?? 0), 1) : 0,

            ReviewCount = h.Reviews.Count,
            MinPricePerNight = h.RoomTypes.Any()
            ? h.RoomTypes.Min(r => r.PricePerNight)
            : null,
            AvailableRooms = h.RoomTypes
            .SelectMany(rt => rt.Rooms)
            .Count(r => r.Status == "Available"),

            IsVerified = h.IsVerified ?? false, // nếu trong DB giá trị IsVerified = null, thì DTO sẽ nhận false  
            Status = h.Status,
            IsWishlist = isWishlist,
            Amenities = h.HotelAmenities
            .Select(a => new AmenityDTO
            {
                Id = a.Amenity.Id,
                Name = a.Amenity.Name
                // IconCode = a.Amenity.IconCode ?? string.Empty
            }).ToList(),
            // Policies = ... (implement nếu có)
            // RoomTypes = ... (implement nếu có)
        }).FirstOrDefaultAsync();
        return hotel;
    }

    public async Task<string> GetOwnerDashBoard(int ownerId)
    {
        return await Task.FromResult($"Owner Dashboard for Owner ID: {ownerId}");
    }

    public async Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string? destination, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, int? userId = null)
    {
        // set giá trị mặc định nếu null
        checkIn ??= DateTime.Today;
        checkOut ??= checkIn.Value.AddDays(1);
        adults ??= 1;
        children ??= 0;
        rooms ??= 1;

        if (!string.IsNullOrEmpty(destination))
        {
            destination = destination.Trim(); // loại bỏ khoảng trắng đầu/cuối
            destination = System.Text.RegularExpressions.Regex.Replace(destination, @"\s+", " "); // chuẩn hóa khoảng trắng giữa các từ
            destination = destination.ToLowerInvariant(); // chuyển sang chữ thường
        }
        // load wishlistIds của user trước
        var wishlistIds = userId.HasValue
            ? await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.HotelId)
                .ToListAsync()
            : new List<int>();

        // lấy danh sách từ SP 
        var hotelRes = await _context.Database
        .SqlQueryRaw<SearchHotelResultDTO>(
            "EXEC sp_SearchHotels @destination={0}, @CheckIn={1}, @CheckOut={2}, @Adults={3}, @Children={4}, @Rooms={5}",
        destination, checkIn, checkOut, adults, children, rooms)
        .ToListAsync();

        // map sang DTO
        return hotelRes.Select(h => new HotelListItemDTO
        {
            HotelId = h.HotelId,
            HotelName = h.HotelName,
            Address = h.Address,
            City = h.CityName,
            Country = h.CountryName,
            CoverImageUrl = h.CoverImageUrl,
            ImageUrls = string.IsNullOrEmpty(h.Images)
            ? new List<string>()
            : JsonConvert.DeserializeObject<List<string>>(h.Images).Take(4).ToList(),
            // HighlightAmenities = string.IsNullOrEmpty(h.AmenityNames)
            // ? new List<AmenityDTO>()
            // : JsonConvert.DeserializeObject<List<string>>(h.AmenityNames)
            //     .Take(3)
            //     .Select(a => new AmenityDTO { Name = a })
            //     .ToList(),
            MinPricePerNight = h.MinPrice,
            MaxPricePerNight = h.MaxPrice,
            AvgPricePerNight = h.AvgPrice,
            AvailableRooms = h.AvailableRooms,
            AverageRating = h.AvgRating,
            ReviewCount = h.ReviewCount,
            IsWishlist = wishlistIds.Contains(h.HotelId)
            // nếu SP có cột ShortDescription, IsVerified, Status thì map thêm
            // ShortDescription = h.ShortDescription ?? string.Empty,
            // IsVerified = h.IsVerified,
            // Status = h.Status
        }).ToList();
    }

    public async Task<List<AutocompleteDTO>> GetAutocompleteAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return new List<AutocompleteDTO>();

        return await _context.Database
            .SqlQueryRaw<AutocompleteDTO>("EXEC sp_AutocompleteSearch @Keyword={0}", keyword)
            .ToListAsync();
    }
}