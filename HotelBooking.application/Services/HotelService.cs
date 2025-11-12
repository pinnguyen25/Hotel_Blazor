using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HotelBooking.application.Helpers;
using HotelBooking.application.Services;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
public interface IHotelService
{
    // Admin
    public Task<ApiResponse<List<AmenityDTO>>> GetAllAmenitiesAsync();
    public Task<ApiResponse<AmenityDTO>> CreateAmenityAsync(AmenityCreateOrUpdateDTO newAmenity, int? userId);
    public Task<ApiResponse<AmenityDTO>> UpdateAmenityAsync(int id, AmenityCreateOrUpdateDTO amenity, int? userId);
    public Task<ApiResponse<bool>> DeleteAmenityAsync(int id, int? userId);

    // accommodation
    public Task<ApiResponse<List<AccommodationTypeDTO>>> GetAllAccommodationTypesAsync();
    public Task<ApiResponse<List<AccommodationTypeDTO>>> GetAccommodationsByUserAsync(int userId);
    public Task<ApiResponse<AccommodationTypeDTO>> CreateAccommodationAsync(AccommodationCreateOrUpdateDTO accommodationType, int userId);
    public Task<ApiResponse<AccommodationTypeDTO>> UploadImageAsync(int id, IFormFile image, int userId);
    public Task<ApiResponse<AccommodationTypeDTO>> UpdateAccommodationAsync(int id, AccommodationCreateOrUpdateDTO accommodationType, int userId);
    public Task<ApiResponse<AccommodationTypeDTO>> UpdateAccWithImageAsync(int id, IFormFile image, int userId);
    public Task<ApiResponse<bool>> DeleteAsync(int id, int userId);

    // // event
    // public Task<ApiResponse<List<EventTypeDTO>>> GetAllEventAsync();
    // public Task<ApiResponse<List<EventTypeDTO>>> GetByAccommodationTypeAsync(int accommodationTypeId);
    // // accommodation event
    // public Task<ApiResponse<List<AccommodationEventDTO>>> GetAllAccommodationEventAsync();
    // // booking event
    // public Task<ApiResponse<List<EventBookingDTO>>> GetAllEventBookingsAsync();
    // public Task<ApiResponse<EventBookingDTO>> GetEventBookingByCustomerIdAsync(int id);

    // hotel
    public Task<List<HotelListItemDTO>> GetAllHotelsAsync(int? userId = null);
    // Lấy hotel theo id
    public Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null);
    // Lấy hotel theo loại lưu trú
    public Task<ApiResponse<IEnumerable<HotelListItemDTO>>> GetHotelsByAccommodationTypeAsync(int accTypeId, int? userId = null);
    // Lấy hotel rate cao
    public Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null);
    public Task<string> GetOwnerDashBoard(int ownerId);
    // search hotel theo name
    public Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string? destination, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, int? userId);
    // kiểm tra phòng trống
    public Task<int> CheckAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int adults, int children);
    public Task<List<CityDTO>> GetCityNameAsync();
    Task<List<AutocompleteDTO>> GetAutocompleteAsync(string keyword);

}


public class HotelService : IHotelService
{
    private readonly HotelBookingContext _context;
    private readonly IHotelRepository _hotelRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccommodationRepository _accommodationRepository;
    private readonly IWishlistService _wishlistService;
    private readonly IMemoryCache _cache;
    private readonly JwtAuthService _jwtAuthService;
    private readonly IFileUploadService _uploadService;
    public IUnitOfWork _dbu;
    public IWebHostEnvironment _env;

    public HotelService
    (
        HotelBookingContext context,
        IHotelRepository hotelRepository,
        IAmenityRepository amenityRepository,
        IUserRepository userRepository,
        IWishlistService wishlistService,
        IAccommodationRepository accommodationRepository,
        IMemoryCache cache,
        IUnitOfWork dbu,
        IWebHostEnvironment env,
        JwtAuthService jwtAuth,
        IFileUploadService uploadService
    )
    {
        _context = context;
        _hotelRepository = hotelRepository;
        _amenityRepository = amenityRepository;
        _wishlistService = wishlistService;
        _accommodationRepository = accommodationRepository;
        _cache = cache;
        _dbu = dbu;
        _env = env;
        _jwtAuthService = jwtAuth;
        _uploadService = uploadService;
        _userRepository = userRepository;
    }

    private async Task<bool> UserIsAdmin(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;
        var roles = await _context.UserRoles.Include(ur => ur.Role).Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name)
        .ToListAsync();
        return roles.Contains("Admin");
    }

    public async Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null)
    {
        // lấy wishlistIds của user trước
        var wishlistIds = userId.HasValue
            ? await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.HotelId)
                .ToListAsync()
            : new List<int>();

        var topHotels = await _context.Database
            .SqlQueryRaw<SearchHotelResultDTO>("EXEC sp_GetTopHotels @Top={0}", 10)
            .ToListAsync();

        return topHotels.Select(t => new HotelListItemDTO
        {
            HotelId = t.HotelId,
            HotelName = t.HotelName,
            Address = t.Address,
            City = t.CityName,
            Country = t.CountryName,
            CoverImageUrl = t.CoverImageUrl,
            ImageUrls = string.IsNullOrEmpty(t.Images)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(t.Images)!.Take(4).ToList(),
            HighlightAmenities = string.IsNullOrEmpty(t.AmenityNames)
            ? new List<AmenityDTO>()
            : JsonSerializer.Deserialize<List<AmenityDTO>>(t.AmenityNames)!
                .Take(3).ToList(),
            MinPricePerNight = t.MinPrice,
            MaxPricePerNight = t.MaxPrice,
            AvgPricePerNight = t.AvgPrice,
            AvailableRooms = t.AvailableRooms,
            AverageRating = (decimal)t.AvgRating,
            ReviewCount = t.ReviewCount,
            IsWishlist = wishlistIds.Contains(t.HotelId)
        }).ToList();
    }

    public async Task<List<CityDTO>> GetCityNameAsync()
    {
        if (_cache.TryGetValue(CacheKey.CitiesKey, out List<CityDTO>? cachedCities) && cachedCities != null)
        {
            return cachedCities;
        }

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
        // cache trong 1 giờ
        _cache.Set(CacheKey.CitiesKey, result, TimeSpan.FromHours(1));
        return result;
    }

    public async Task<ApiResponse<IEnumerable<HotelListItemDTO>>> GetHotelsByAccommodationTypeAsync(int accTypeId, int? userId = null)
    {
        try
        {
            // Lấy wishlistIds nếu có userId
            var wishlistIds = userId.HasValue
                ? await _context.Wishlists
                    .Where(w => w.UserId == userId)
                    .Select(w => w.HotelId)
                    .ToListAsync()
                : new List<int>();

            var typeHotel = await _context.Database
            .SqlQueryRaw<SearchHotelResultDTO>("EXEC sp_GetHotelsByAccommodationType @AccommodationTypeId = {0}", accTypeId)
            .ToListAsync();

            var result = typeHotel.Select(h => new HotelListItemDTO
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                Address = h.Address,
                City = h.CityName,
                Country = h.CountryName,
                CoverImageUrl = h.CoverImageUrl,
                ImageUrls = string.IsNullOrEmpty(h.Images)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<Dictionary<string, string>>>(h.Images)!
                    .Select(imgObj => imgObj["ImageUrl"])
                    .Take(4)
                    .ToList(),
                HighlightAmenities = string.IsNullOrEmpty(h.AmenityNames)
                ? new List<AmenityDTO>()
                : JsonSerializer.Deserialize<List<AmenityDTO>>(h.AmenityNames)!
                    .Take(3)
                    .ToList(),
                MinPricePerNight = h.MinPrice,
                MaxPricePerNight = h.MaxPrice,
                AvgPricePerNight = h.AvgPrice,
                AvailableRooms = h.AvailableRooms,
                IsBookable = h.AvailableRooms > 0,
                AverageRating = (decimal)h.AvgRating,
                ReviewCount = h.ReviewCount,
                IsWishlist = wishlistIds.Contains(h.HotelId)
            }).ToList();

            return new ApiResponse<IEnumerable<HotelListItemDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.SUCCESS,
                Content = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<HotelListItemDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER
            };
        }
    }

    public async Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null)
    {
        if (hotelId < 0)
        {
            throw new ArgumentException("Invalid hotel ID");
        }

        var cacheKey = $"HotelDetail_{hotelId}_{userId ?? 0}";
        if (_cache.TryGetValue(cacheKey, out HotelDetailDTO? cachedHotel) && cachedHotel != null)
        {
            return cachedHotel;
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
            Policies = h.HotelPolicies
                .Where(hp => hp.Policy.IsDeleted == false).Select(hp => new PolicyDTO
                {
                    Id = hp.Policy.Id,
                    Name = hp.Policy.Name,
                    Description = hp.Policy.Description
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
                AvailableRooms = rt.Rooms.Count(r => r.IsDeleted == false && r.Status == "Available"),
                Amenities = rt.RoomAmenities
                        .Where(rta => rta.Amenity.IsDeleted == false)
                        .Select(rta => new AmenityDTO
                        {
                            Id = rta.Amenity.Id,
                            Name = rta.Amenity.Name,
                            Additional = rta.Amenity.Additional
                        }).ToList()
            }).ToList(),
            MinPricePerNight = h.RoomTypes
                    .Where(rt => rt.IsDeleted == false)
                    .Min(rt => (decimal?)rt.PricePerNight),
            AverageRating = h.Reviews.Any(r => r.IsDeleted == false && r.Rating.HasValue)
                    ? Math.Round((decimal)h.Reviews.Where(r => r.IsDeleted == false).Average(r => r.Rating ?? 0), 1)
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
                    Rating = (decimal)r.Rating,
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
                    //Log error
                }
                return new AmenityDTO
                {
                    Id = ha.Id,
                    Name = ha.Name,
                    IconClass = additional.GetValueOrDefault("IconClass") ?? "",
                    IconColor = additional.GetValueOrDefault("IconColor") ?? "blue"
                };
            }).ToList();
            // map room amenities
            foreach (var roomType in hotel.RoomTypes)
            {
                roomType.Amenities = roomType.Amenities.Select(ra =>
                {
                    var additional = new Dictionary<string, string>();
                    try
                    {
                        additional = JsonSerializer.Deserialize<Dictionary<string, string>>(ra.Additional ?? "{}") ?? new Dictionary<string, string>();
                    }
                    catch (JsonException)
                    {
                        // Log

                    }
                    return new AmenityDTO
                    {
                        Id = ra.Id,
                        Name = ra.Name,
                        IconClass = additional.GetValueOrDefault("IconClass") ?? "",
                        IconColor = additional.GetValueOrDefault("IconColor") ?? "blue"
                    };
                }).ToList();
            }
            // check wishlist
            if (userId.HasValue)
            {
                string wishlistCacheKey = $"Wishlist_{userId.Value}_{hotelId}";
                try
                {
                    if (!_cache.TryGetValue(wishlistCacheKey, out bool isWishlist))
                    {
                        isWishlist = await _wishlistService.IsInWishlistAsync(userId.Value, hotelId);
                        _cache.Set(wishlistCacheKey, isWishlist, TimeSpan.FromMinutes(10)); // Cache trong 10 phút
                    }
                    hotel.IsWishlist = isWishlist;
                }
                catch (Exception)
                {
                    hotel.IsWishlist = await _wishlistService.IsInWishlistAsync(userId.Value, hotelId);
                }
            }
            _cache.Set(cacheKey, hotel, TimeSpan.FromMinutes(10));
        }
        return hotel;
    }

    // kiểm tra phòng trống
    public async Task<int> CheckAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int adults, int children)
    {
        var availableRooms = await _context.Database
            .SqlQueryRaw<int>(
                "EXEC sp_CheckAvailableRooms @HotelId={0}, @CheckIn={1}, @CheckOut={2}, @Adults={3}, @Children={4}",
                hotelId, checkIn, checkOut, adults, children)
            .ToListAsync();

        return availableRooms.FirstOrDefault();
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
            : JsonSerializer.Deserialize<List<Dictionary<string, string>>>(h.Images)!
                .Select(imgObj => imgObj["ImageUrl"])
                .Take(4)
                .ToList(),
            HighlightAmenities = string.IsNullOrEmpty(h.AmenityNames)
            ? new List<AmenityDTO>()
            : JsonSerializer.Deserialize<List<AmenityDTO>>(h.AmenityNames)!
                .Take(3).ToList(),
            MinPricePerNight = h.MinPrice,
            MaxPricePerNight = h.MaxPrice,
            AvgPricePerNight = h.AvgPrice,
            AvailableRooms = h.AvailableRooms,
            IsBookable = h.AvailableRooms >= rooms.Value,
            AverageRating = (decimal)h.AvgRating,
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
            // kiểm tra cache trước
            if (_cache.TryGetValue(CacheKey.AmenitiesKey, out List<AmenityDTO>? cachedAmenities) && cachedAmenities != null)
            {
                return new ApiResponse<List<AmenityDTO>>
                {
                    StatusCode = StatusCodeResponse.Success,
                    Content = cachedAmenities
                };
            }

            var amenities = await _context.Amenities
            .AsNoTracking()
            .Where(a => a.IsDeleted == false)
            .ToListAsync();

            var result = new List<AmenityDTO>();
            foreach (var a in amenities)
            {
                AmenityAdditional? add = null;
                if (!string.IsNullOrWhiteSpace(a.Additional))
                {
                    add = JsonSerializer.Deserialize<AmenityAdditional>(a.Additional);
                }

                string? createdByName = null;
                if (a.CreatedBy.HasValue)
                {
                    var user = await _userRepository.GetByIdAsync(a.CreatedBy.Value);
                    createdByName = user?.FullName;
                }

                string? updatedByName = null;
                if (a.UpdatedBy.HasValue)
                {
                    var user = await _userRepository.GetByIdAsync(a.UpdatedBy.Value);
                    updatedByName = user?.FullName;
                }

                result.Add(new AmenityDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    IconClass = add?.IconClass ?? "default-icon",
                    IconColor = add?.IconColor ?? "#54a9ffff",
                    Description = add?.Description,
                    CreatedByName = createdByName,
                    UpdatedByName = updatedByName,
                    CreatedAt = a.CreatedAt ?? DateTime.MinValue,
                    UpdatedAt = a.UpdatedAt,
                    IsDeleted = a.IsDeleted ?? false
                });
            }

            // lưu vào cache
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            _cache.Set(CacheKey.AmenitiesKey, result, cacheOptions);

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

    public async Task<ApiResponse<AmenityDTO>> CreateAmenityAsync(AmenityCreateOrUpdateDTO newAmenity, int? userId)
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

            var additional = JsonSerializer.Serialize(new AmenityAdditional
            {
                Description = newAmenity.Description,
                IconClass = newAmenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newAmenity.IconColor) ? "blue" : newAmenity.IconColor
            });

            var amenity = new Amenity
            {
                Name = newAmenity.Name,
                IsDeleted = false,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                Additional = additional
            };
            await _amenityRepository.AddAsync(amenity);
            await _dbu.SaveChangesAsync();
            _cache.Remove(CacheKey.AmenitiesKey);

            string? createdByName = null;
            if (userId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                createdByName = user?.FullName;
            }
            // Map entity sang DTO để trả về cho FE
            var resultDTO = new AmenityDTO
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Description = newAmenity.Description,
                IconClass = newAmenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newAmenity.IconColor) ? "blue" : newAmenity.IconColor,
                CreatedByName = createdByName,
                CreatedAt = amenity.CreatedAt.Value
            };


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

    public async Task<ApiResponse<AmenityDTO>> UpdateAmenityAsync(int id, AmenityCreateOrUpdateDTO amenity, int? userId)
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
            existingAmenity.UpdatedBy = userId;
            existingAmenity.UpdatedAt = DateTime.Now;
            existingAmenity.Additional = JsonSerializer.Serialize(new AmenityAdditional
            {
                Description = string.IsNullOrWhiteSpace(amenity.Description) ? null : amenity.Description,
                IconClass = amenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(amenity.IconColor) ? "blue" : amenity.IconColor
            });

            string? createdByName = null;
            string? updatedByName = null;
            if (existingAmenity.CreatedBy.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(existingAmenity.CreatedBy.Value);
                createdByName = user?.FullName;
            }
            if (existingAmenity.UpdatedBy.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(existingAmenity.UpdatedBy.Value);
                updatedByName = user?.FullName;
            }

            var resultDTO = new AmenityDTO
            {
                Id = existingAmenity.Id,
                Name = existingAmenity.Name,
                Description = amenity.Description,
                IconClass = amenity.IconClass,
                IconColor = string.IsNullOrWhiteSpace(amenity.IconColor) ? "blue" : amenity.IconColor,
                CreatedByName = createdByName,
                UpdatedByName = updatedByName,
                CreatedAt = existingAmenity.CreatedAt!.Value,
                UpdatedAt = existingAmenity.UpdatedAt
            };

            await _amenityRepository.UpdateAsync(existingAmenity);
            await _dbu.SaveChangesAsync();
            _cache.Remove(CacheKey.AmenitiesKey);

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

    public async Task<ApiResponse<bool>> DeleteAmenityAsync(int id, int? userId)
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
            amenity.UpdatedBy = userId;
            amenity.UpdatedAt = DateTime.Now;

            await _amenityRepository.UpdateAsync(amenity);
            await _dbu.SaveChangesAsync(); // EF Core tự track thay đổi
            _cache.Remove(CacheKey.AmenitiesKey);

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

    // =============== ĐỌC, THÊM, SỬA, XÓA LƯU TRÚ CHO KHÁCH SẠN ================
    #region MANAGE ACCOMMODATION
    // Lấy tất cả loại lưu trú
    public async Task<ApiResponse<List<AccommodationTypeDTO>>> GetAllAccommodationTypesAsync()
    {
        try
        {
            var result = await _context.Database.SqlQueryRaw<AccommodationTypeDTO>("EXEC sp_GetAllAccommodationTypes").ToListAsync();
            return new ApiResponse<List<AccommodationTypeDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.SUCCESS,
                Content = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<AccommodationTypeDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER
            };
        }
    }

    // get by owner
    public async Task<ApiResponse<List<AccommodationTypeDTO>>> GetAccommodationsByUserAsync(int userId)
    {
        var accType = await _accommodationRepository.WhereAsync(acc => acc.IsDeleted == false && acc.CreatedBy == userId);
        if (!accType.Any())
        {
            return new ApiResponse<List<AccommodationTypeDTO>>
            {
                StatusCode = StatusCodeResponse.NotFound,
                Message = MessageResponse.EMPTY_LIST,
                Content = null
            };
        }

        var result = accType.Select(acc => new AccommodationTypeDTO
        {
            Id = acc.Id,
            Name = acc.Name,
            Slug = acc.Slug,
            Description = acc.Description,
            ImagePath = acc.ImagePath,
            CreatedBy = acc.CreatedBy,
            CreatedAt = acc.CreatedAt,
            UpdatedAt = acc.UpdatedAt
        }).ToList();

        return new ApiResponse<List<AccommodationTypeDTO>>
        {
            StatusCode = StatusCodeResponse.Success,
            Content = result
        };
    }

    // Thêm loại lưu trú
    public async Task<ApiResponse<AccommodationTypeDTO>> CreateAccommodationAsync(AccommodationCreateOrUpdateDTO accommodationType, int userId)
    {
        try
        {
            // kiểm tra trùng tên
            var exists = await _accommodationRepository.AnyAsync(acc => acc.Name.ToLower() == accommodationType.Name.ToLower() && acc.IsDeleted == false);
            if (exists)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.Conflict,
                    Message = MessageResponse.NAME_ALREADY_EXISTS,
                    Content = null
                };
            }

            // xử lý slug
            var slug = string.IsNullOrWhiteSpace(accommodationType.Slug)
                ? accommodationType.Name.Slugify()
                : accommodationType.Slug.Slugify();

            // Kiểm tra trùng slug
            var existsSlug = await _accommodationRepository.AnyAsync(acc =>
                acc.Slug.ToLower() == slug.ToLower() && acc.IsDeleted == false);
            if (existsSlug)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.Conflict,
                    Message = "Slug đã tồn tại, vui lòng nhập tên khác!",
                    Content = null
                };
            }
            // map DTO sang entity
            var accommodation = new AccommodationType
            {
                Name = accommodationType.Name,
                Slug = slug,
                Description = accommodationType.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };

            // lưu vào DB
            await _accommodationRepository.AddAsync(accommodation);
            await _dbu.SaveChangesAsync();


            return new ApiResponse<AccommodationTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.CREATE_SUCCESSFULLY,
                Content = new AccommodationTypeDTO
                {
                    Id = accommodation.Id,
                    Name = accommodation.Name,
                    Slug = accommodation.Slug,
                    Description = accommodation.Description,
                    CreatedBy = accommodation.CreatedBy,
                    CreatedAt = accommodation.CreatedAt,
                    UpdatedAt = accommodation.UpdatedAt
                }
            };
        }
        catch (Exception)
        {
            return new ApiResponse<AccommodationTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
            };
        }
    }
    // Thêm loại lưu trú có ảnh
    public async Task<ApiResponse<AccommodationTypeDTO>> UploadImageAsync(int id, IFormFile image, int userId)
    {
        try
        {
            var uploadAccImg = await _accommodationRepository.GetByIdAsync(id);
            if (uploadAccImg == null)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.NotFound,
                    Message = MessageResponse.NOT_FOUND
                };
            }
            // check quyền owner
            if (!await UserIsAdmin(userId) && uploadAccImg.CreatedBy != userId)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.Forbidden,
                    Message = "You can only modify your own accommodations."
                };
            }

            if (image == null || image.Length == 0)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = "No image provided."
                };
            }

            // Xóa image cũ nếu có
            if (!string.IsNullOrEmpty(uploadAccImg.ImagePath))
            {
                _uploadService.DeleteImage(uploadAccImg.ImagePath);
            }

            uploadAccImg.ImagePath = await _uploadService.SaveImageAsync(image, "accommodation");
            uploadAccImg.UpdatedAt = DateTime.Now;

            await _accommodationRepository.UpdateAsync(uploadAccImg);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<AccommodationTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = "Image uploaded successfully.",
                Content = new AccommodationTypeDTO
                {
                    Id = uploadAccImg.Id,
                    Name = uploadAccImg.Name,
                    Slug = uploadAccImg.Slug,
                    Description = uploadAccImg.Description,
                    ImagePath = uploadAccImg.ImagePath,
                    CreatedBy = uploadAccImg.CreatedBy,
                    CreatedAt = uploadAccImg.CreatedAt,
                    UpdatedAt = uploadAccImg.UpdatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<AccommodationTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
            };
        }
    }
    // Cập nhật loại lưu trú
    public async Task<ApiResponse<AccommodationTypeDTO>> UpdateAccommodationAsync(int id, AccommodationCreateOrUpdateDTO accommodationType, int userId)
    {
        try
        {
            var updateAcc = await _accommodationRepository.GetByIdAsync(id);
            if (updateAcc == null)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = MessageResponse.UPDATE_FAILED,
                    Content = null
                };
            }

            // Check quyền Owner
            if (!await UserIsAdmin(userId) && updateAcc.CreatedBy != userId)
            {
                return new ApiResponse<AccommodationTypeDTO>
                {
                    StatusCode = StatusCodeResponse.Forbidden,
                    Message = "You can only modify your own accommodations."
                };
            }
            // kiểm tra trùng tên nếu đổi tên
            if (!string.Equals(updateAcc.Name, accommodationType.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nameExists = await _accommodationRepository.AnyAsync(a => a.Name.ToLower() == accommodationType.Name.ToLower() && a.IsDeleted == false && a.Id != id);
                if (nameExists)
                {
                    return new ApiResponse<AccommodationTypeDTO>
                    {
                        StatusCode = StatusCodeResponse.Conflict,
                        Message = MessageResponse.NAME_ALREADY_EXISTS
                    };
                }
            }

            // cập nhật dữ liệu
            updateAcc.Name = accommodationType.Name;
            updateAcc.Slug = string.IsNullOrWhiteSpace(accommodationType.Slug)
                ? accommodationType.Name.Slugify()
                : accommodationType.Slug.Slugify();
            updateAcc.Description = accommodationType.Description;
            updateAcc.UpdatedAt = DateTime.Now;


            await _accommodationRepository.UpdateAsync(updateAcc);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<AccommodationTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.UPDATE_SUCCESSFULLY,
                Content = new AccommodationTypeDTO
                {
                    Id = updateAcc.Id,
                    Name = updateAcc.Name,
                    Slug = updateAcc.Slug,
                    Description = updateAcc.Description,
                    ImagePath = updateAcc.ImagePath,
                    CreatedBy = updateAcc.CreatedBy,
                    CreatedAt = updateAcc.CreatedAt,
                    UpdatedAt = updateAcc.UpdatedAt
                }
            };
        }
        catch (Exception)
        {
            return new ApiResponse<AccommodationTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
            };
        }
    }
    // Cập nhật ảnh loại lưu trú
    public async Task<ApiResponse<AccommodationTypeDTO>> UpdateAccWithImageAsync(int id, IFormFile image, int userId)
    {
        return await UploadImageAsync(id, image, userId);
    }
    // Xóa loại lưu trú
    public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
    {
        try
        {
            var delAcc = await _accommodationRepository.GetByIdAsync(id);
            if (delAcc == null)
            {
                return new ApiResponse<bool>
                {
                    StatusCode = StatusCodeResponse.NotFound,
                    Message = MessageResponse.NOT_FOUND,
                    Content = false
                };
            }
            // Check quyền Owner
            if (!await UserIsAdmin(userId) && delAcc.CreatedBy != userId)
            {
                return new ApiResponse<bool>
                {
                    StatusCode = StatusCodeResponse.Forbidden,
                    Message = "You can only delete your own accommodations.",
                    Content = false
                };
            }
            // xóa image nếu có
            if (!string.IsNullOrEmpty(delAcc.ImagePath))
            {
                _uploadService.DeleteImage(delAcc.ImagePath);
            }

            delAcc.IsDeleted = true;
            delAcc.UpdatedAt = DateTime.Now;
            await _accommodationRepository.UpdateAsync(delAcc);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.DELETE_SUCCESSFULLY,
                Content = true
            };
        }
        catch
        {
            return new ApiResponse<bool> { StatusCode = StatusCodeResponse.Error, Message = MessageResponse.ERROR_IN_SERVER };
        }
    }

    #endregion
}