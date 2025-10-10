using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HotelBooking.application.Helpers;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
public interface IHotelService
{
    // Admin
    public Task<ApiResponse<List<AmenityDTO>>> GetAllAmenitiesAsync();
    public Task<ApiResponse<AmenityDTO>> CreateAmenityAsync(AmenityCreateOrUpdateDTO newAmenity);
    public Task<ApiResponse<AmenityDTO>> UpdateAmenityAsync(int id, AmenityCreateOrUpdateDTO amenity);
    public Task<ApiResponse<bool>> DeleteAmenityAsync(int id);

    // 
    public Task<List<HotelListItemDTO>> GetAllHotelsAsync(int? userId = null);
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
    private readonly IAmenityRepository _amenityRepository;
    private readonly IWishlistService _wishlistService;
    private readonly IMemoryCache _cache;
    public IUnitOfWork _dbu;

    public HotelService(HotelBookingContext context, IHotelRepository hotelRepository, IAmenityRepository amenityRepository, IWishlistService wishlistService, IMemoryCache cache, IUnitOfWork dbu)
    {
        _context = context;
        _hotelRepository = hotelRepository;
        _amenityRepository = amenityRepository;
        _wishlistService = wishlistService;
        _cache = cache;
        _dbu = dbu;
    }


    public async Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null)
    {
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
            })
            .ToListAsync();

        if (userId.HasValue)
        {
            foreach (var hotel in topHotels)
            {
                hotel.IsWishlist = await _wishlistService.IsInWishlistAsync(userId.Value, hotel.HotelId);
            }
        }
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
        if (hotelId < 0)
        {
            throw new ArgumentException("Invalid hotel ID");
        }

        var hotel = await _context.Hotels
        .AsNoTracking()
        .Where(h => h.Id == hotelId && h.IsDeleted == false)
        .Select(h => new HotelDetailDTO
        {
            HotelId = h.Id,
            HotelName = h.Name,
            Address = h.Address,
            Description = h.Description,
            CoverImageUrl = h.CoverImageUrl ?? string.Empty,
            ImageUrls = h.HotelImages
                .Where(hi => hi.IsDeleted == false)
                .OrderBy(hi => hi.Id)
                .Select(hi => hi.ImageUrl).ToList(),
            Amenities = h.HotelAmenities
                .Where(ha => ha.Amenity.IsDeleted == false)
                .Select(ha => new AmenityDTO
                {
                    Id = ha.Amenity.Id,
                    Name = ha.Amenity.Name,
                    Additional = ha.Amenity.Additional
                }).ToList(),
            RoomTypes = h.RoomTypes.Where(rt => rt.IsDeleted == false).Select(rt => new RoomTypeDTO
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description,
                PricePerNight = rt.PricePerNight,
                AdultCapacity = rt.AdultCapacity,
                ChildCapacity = rt.ChildCapacity,
                RoomImages = rt.RoomImages
                        .Where(ri => ri.IsDeleted == false)
                        .Select(ri => ri.ImageUrl)
                        .ToList(),
                AvailableRooms = rt.Rooms.Count(r => r.IsDeleted == false && r.Status == "Available")
            }).ToList(),
            MinPricePerNight = h.RoomTypes
                    .Where(rt => rt.IsDeleted == false)
                    .Min(rt => rt.PricePerNight),
            AverageRating = h.Reviews.Any(r => r.IsDeleted == false && r.Rating.HasValue)
                    ? Math.Round(h.Reviews.Where(r => r.IsDeleted == false).Average(r => r.Rating ?? 0), 1)
                    : 0,
            ReviewCount = h.Reviews.Count(r => r.IsDeleted == false && r.Rating.HasValue),
            Reviews = h.Reviews
                .Where(r => r.IsDeleted == false)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    UserName = r.Customer.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToList(),
            AvailableRooms = h.RoomTypes
                .SelectMany(rt => rt.Rooms)
                .Count(r => r.IsDeleted == false && r.Status == "Available"),
            IsWishlist = false,
            IsVerified = false, // Adjust based on your logic
            Status = "Active" // Adjust based on your logic
        })
        .FirstOrDefaultAsync();

        if (hotel != null)
        {
            // map amenities
            hotel.Amenities = hotel.Amenities.Select(ha =>
            {
                var additional = new Dictionary<string, string>();
                try
                {
                    additional = JsonSerializer.Deserialize<Dictionary<string, string>>(ha.Additional ?? "{}") ?? new Dictionary<string, string>();
                }
                catch (JsonException)
                {
                    //
                }
                return new AmenityDTO
                {
                    Id = ha.Id,
                    Name = ha.Name,
                    IconClass = additional.GetValueOrDefault("IconClass") ?? "",
                    IconColor = additional.GetValueOrDefault("IconColor") ?? "blue"
                };
            }).ToList();

            if (userId.HasValue)
            {
                string cacheKey = $"Wishlist_{userId.Value}_{hotelId}";
                try
                {
                    if (!_cache.TryGetValue(cacheKey, out bool isWishlist))
                    {
                        isWishlist = await _wishlistService.IsInWishlistAsync(userId.Value, hotelId);
                        _cache.Set(cacheKey, isWishlist, TimeSpan.FromMinutes(10)); // Cache trong 10 phút
                    }
                    hotel.IsWishlist = isWishlist;
                }
                catch (Exception)
                {
                    hotel.IsWishlist = await _wishlistService.IsInWishlistAsync(userId.Value, hotelId);
                }
            }
        }
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
        destination!, checkIn.Value, checkOut.Value, adults, children, rooms)
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
            : JsonSerializer.Deserialize<List<string>>(h.Images)!.Take(4).ToList(),
            HighlightAmenities = string.IsNullOrEmpty(h.AmenityNames)
            ? new List<AmenityDTO>()
            : JsonSerializer.Deserialize<List<AmenityDTO>>(h.AmenityNames)!
                .Take(3).ToList(),
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

    public async Task<List<HotelListItemDTO>> GetAllHotelsAsync(int? userId = null)
    {
        return await GetSearchOptionsAsync(null, null, null, null, null, null, userId);
    }

    // Admin
    // =============== ĐỌC, THÊM, SỬA, XÓA TIỆN ÍCH CHO KHÁCH SẠN ================
    #region MANAGE AMENITIES
    public async Task<ApiResponse<List<AmenityDTO>>> GetAllAmenitiesAsync()
    {
        try
        {
            List<AmenityDTO> result = new List<AmenityDTO>();

            var amenities = await _amenityRepository.WhereAsync(a => a.IsDeleted == false);

            if (amenities == null || amenities.Count() == 0)
            {
                return new ApiResponse<List<AmenityDTO>>
                {
                    StatusCode = StatusCodeResponse.NotFound,
                    Message = MessageResponse.EMPTY_LIST,
                    Content = null
                };
            }

            foreach (var amenity in amenities)
            {
                var additional = JsonSerializer.Deserialize<Dictionary<string, string>>(amenity.Additional ?? "{}")
                ?? new Dictionary<string, string>();

                result.Add(new AmenityDTO
                {
                    Id = amenity.Id,
                    Name = amenity.Name,
                    Description = additional.GetValueOrDefault("Description", null),
                    IconClass = additional.GetValueOrDefault("IconClass", ""),
                    IconColor = additional.GetValueOrDefault("IconColor", "blue"),
                });
            }

            return new ApiResponse<List<AmenityDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = result
            };
        }
        catch (Exception)
        {
            return new ApiResponse<List<AmenityDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }

    }
    public async Task<ApiResponse<AmenityDTO>> CreateAmenityAsync(AmenityCreateOrUpdateDTO newAmenity)
    {
        try
        {
            // Người dùng mà tạo trùng tên tiện ích thì không cho phép
            var exists = await _amenityRepository.AnyAsync(a => a.Name.ToLower() == newAmenity.Name.ToLower() && a.IsDeleted == false);
            if (exists) return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            var additional = JsonSerializer.Serialize(new
            {
                Description = newAmenity.Description,
                IconClass = newAmenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newAmenity.IconColor) ? "blue" : newAmenity.IconColor
            });

            var amenity = new Amenity
            {
                Name = newAmenity.Name,
                IsDeleted = false,
                Additional = additional
            };

            // Map entity sang DTO để trả về cho FE
            var resultDTO = new AmenityDTO
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Description = newAmenity.Description,
                IconClass = newAmenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newAmenity.IconColor) ? "blue" : newAmenity.IconColor
            };

            await _amenityRepository.AddAsync(amenity);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.CREATE_SUCCESSFULLY,
                Content = resultDTO
            };
        }
        catch (Exception)
        {
            return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }
    }

    public async Task<ApiResponse<AmenityDTO>> UpdateAmenityAsync(int id, AmenityCreateOrUpdateDTO amenity)
    {
        try
        {
            var existingAmenity = await _amenityRepository.GetByIdAsync(id);
            if (existingAmenity == null)
                return new ApiResponse<AmenityDTO>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = MessageResponse.UPDATE_FAILED,
                    Content = null
                };

            // Người dùng mà đổi tên trùng với tên của 1 tiện ích khác thì cũng không cho phép
            var nameExists = await _amenityRepository.AnyAsync(a => a.Id != id && a.Name.ToLower() == amenity.Name.ToLower() && a.IsDeleted == false);
            if (nameExists) return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            existingAmenity.Name = amenity.Name;
            existingAmenity.Additional = JsonSerializer.Serialize(new
            {
                Description = string.IsNullOrWhiteSpace(amenity.Description) ? null : amenity.Description,
                IconClass = amenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(amenity.IconColor) ? "blue" : amenity.IconColor
            });

            var resultDTO = new AmenityDTO
            {
                Id = existingAmenity.Id,
                Name = existingAmenity.Name,
                Description = amenity.Description,
                IconClass = amenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(amenity.IconColor) ? "blue" : amenity.IconColor
            };

            await _amenityRepository.UpdateAsync(existingAmenity);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.UPDATE_SUCCESSFULLY,
                Content = resultDTO
            }; ;
        }
        catch (Exception)
        {
            return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            }; ;
        }
    }

    public async Task<ApiResponse<bool>> DeleteAmenityAsync(int id)
    {
        var amenity = await _amenityRepository.GetByIdAsync(id);

        if (amenity == null) return new ApiResponse<bool>
        {
            StatusCode = StatusCodeResponse.NotFound,
            Message = MessageResponse.NOT_FOUND,
            Content = false
        };

        try
        {
            amenity.IsDeleted = true;
            await _amenityRepository.UpdateAsync(amenity);
            await _dbu.SaveChangesAsync(); // EF Core tự track thay đổi
            return new ApiResponse<bool>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.DELETE_SUCCESSFULLY,
                Content = true
            };
        }
        catch (Exception)
        {
            return new ApiResponse<bool>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = false
            };
        }
    }
    #endregion

}