using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using HotelBooking.application.Helpers;
using HotelBooking.application.Services;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
public interface IHotelService
{
    // Admin Manage
    // amenity
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

    // bedtype
    public Task<ApiResponse<List<BedTypeDTO>>> GetAllBedTypesAsync();
    public Task<ApiResponse<BedTypeDTO>> CreateBedTypeAsync(BedTypeCreateOrUpdateDTO newBedType, int adminId);
    public Task<ApiResponse<BedTypeDTO>> UpdateBedTypeAsync(int id, BedTypeCreateOrUpdateDTO bedType, int adminId);
    public Task<ApiResponse<bool>> DeleteBedTypeAsync(int id, int adminId);
    // viewtype
    public Task<ApiResponse<List<ViewTypeDTO>>> GetAllViewTypesAsync();
    public Task<ApiResponse<ViewTypeDTO>> CreateViewTypeAsync(ViewTypeCreateOrUpdateDTO newViewType, int adminId);
    public Task<ApiResponse<ViewTypeDTO>> UpdateViewTypeAsync(int id, ViewTypeCreateOrUpdateDTO viewType, int adminId);
    public Task<ApiResponse<bool>> DeleteViewTypeAsync(int id, int adminId);

    // Owner Manage
    public Task<ApiResponse<List<HotelOwnerSummaryDTO>>> GetOwnerHotelsSummaryAsync(int ownerId);
    public Task<ApiResponse<HotelDetailForOwnerDTO>> GetHotelDetailForOwnerAsync(int hotelId, int ownerId);
    public Task<ApiResponse<HotelDraftFullDTO>> GetHotelDraftAsync(int hotelId, int ownerId);
    // CRUD hotel
    public Task<ApiResponse<HotelResponseDTO>> CreateHotelAsync(HotelCreateOrUpdateDTO ownerHotel, int ownerId);
    public Task<ApiResponse<HotelResponseDTO>> UpdateHotelAsync(int hotelId, HotelCreateOrUpdateDTO ownerHotel, int ownerId);
    public Task<ApiResponse<HotelResponseDTO>> DeleteHotelAsync(int hotelId, int ownerId);
    public Task<ApiResponse<HotelImagesResponseDTO>> GetHotelImagesAsync(int hotelId, int ownerId);
    public Task<ApiResponse<string>> UploadHotelCoverImageAsync(int hotelId, IFormFile image, int ownerId);
    public Task<ApiResponse<string>> UploadHotelGalleryImageAsync(int hotelId, IFormFile image, int ownerId);
    public Task<ApiResponse<bool>> DeleteHotelImageAsync(int hotelId, string imageUrl, int ownerId);

    // Room Type
    public Task<ApiResponse<RoomTypeForOwnerDTO>> GetRoomTypeDetailAsync(int roomTypeId, int ownerId);
    public Task<ApiResponse<List<RoomTypeForOwnerDTO>>> GetRoomTypesAsync(int hotelId, int ownerId);
    public Task<ApiResponse<WizardRoomTypeResponseDTO>> WizardCreateFirstRoomTypeAsync(int hotelId, WizardRoomTypeCreateDTO dto, int ownerId);
    public Task<ApiResponse<WizardSubmitValidationDTO>> ValidateHotelForSubmitAsync(int hotelId, int ownerId);
    public Task<ApiResponse<CreateRoomTypeResponseDTO>> CreateRoomTypeAsync(int hotelId, RoomTypeCreateOrUpdateDTO roomType, int ownerId);
    public Task<ApiResponse<RoomTypeForOwnerDTO>> UpdateRoomTypeAsync(int roomTypeId, RoomTypeCreateOrUpdateDTO roomType, int ownerId);
    public Task<ApiResponse<DeleteRoomTypeResponseDTO>> DeleteRoomTypeAsync(int roomTypeId, int ownerId);
    public Task<ApiResponse<bool>> SelectAmenityToRoomTypeAsync(int roomTypeId, List<int> amenityIds, int ownerId);
    public Task<ApiResponse<string>> UploadRoomImageAsync(int roomTypeId, IFormFile image, int ownerId);
    public Task<ApiResponse<bool>> DeleteRoomImageAsync(int roomTypeId, string imageUrl, int ownerId);
    public Task<ApiResponse<RoomOptionsDTO>> GetRoomOptionsAsync();

    // Policy
    public Task<ApiResponse<List<PolicyTypeDTO>>> GetAllPolicyAsync();
    public Task<ApiResponse<PolicyDTO>> CreatePolicyAsync(PolicyCreateOrUpdateDTO newPolicy, int userId);
    public Task<ApiResponse<PolicyDTO>> UpdatePolicyAsync(int id, PolicyCreateOrUpdateDTO policy, int? userId);
    public Task<ApiResponse<bool>> DeletePolicyAsync(int id, int? userId);

    // policy type
    public Task<ApiResponse<PolicyTypeDTO>> CreatePolicyTypeAsync(PolicyTypeCreateOrUpdateDTO policyType, int adminId);
    public Task<ApiResponse<PolicyTypeDTO>> UpdatePolicyTypeAsync(int id, PolicyTypeCreateOrUpdateDTO pt, int? adminId);
    public Task<ApiResponse<bool>> DeletePolicyTypeAsync(int id, int? adminId);
    public Task<ApiResponse<PolicyTypeDTO>> TogglePolicyTypeActiveAsync(int id, int? adminId);

    // Chọn Amenity / Policy cho Hotel

    public Task<ApiResponse<bool>> SelectAmenityToHotelAsync(int hotelId, HotelAmenitiesDTO amenityId, int ownerId);
    public Task<ApiResponse<bool>> SelectPolicyToHotelAsync(int hotelId, HotelPoliciesDTO policyId, int ownerId);
    public Task<ApiResponse<bool>> SubmitHotelAsync(int hotelId, int ownerId);
    // // event
    // public Task<ApiResponse<List<EventTypeDTO>>> GetAllEventAsync();
    // public Task<ApiResponse<List<EventTypeDTO>>> GetByAccommodationTypeAsync(int accommodationTypeId);
    // // accommodation event
    // public Task<ApiResponse<List<AccommodationEventDTO>>> GetAllAccommodationEventAsync();
    // // booking event
    // public Task<ApiResponse<List<EventBookingDTO>>> GetAllEventBookingsAsync();
    // public Task<ApiResponse<EventBookingDTO>> GetEventBookingByCustomerIdAsync(int id);

    // hotel customer
    public Task<List<HotelListItemDTO>> GetAllHotelsAsync(int? userId = null);
    // Lấy hotel theo id
    public Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null, DateTime? checkIn = null, DateTime? checkOut = null);
    // Lấy hotel theo loại lưu trú
    public Task<ApiResponse<IEnumerable<HotelListItemDTO>>> GetHotelsByAccommodationTypeAsync(int accTypeId, int? userId = null);
    // Lấy hotel rate cao
    public Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null);
    public Task<string> GetOwnerDashBoard(int ownerId);
    // search hotel theo name
    public Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string? destination, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, int? userId);
    // kiểm tra phòng trống
    public Task<int> CheckAvailableRoomsAsync(int hotelId, DateOnly checkIn, DateOnly checkOut, int adults = 1, int children = 0);
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
    private readonly IPolicyRepository _policyRepository;
    private readonly IPolicyTypeRepository _policyTypeRepository;
    private readonly IWishlistService _wishlistService;
    private readonly IMemoryCache _cache;
    private readonly JwtAuthService _jwtAuthService;
    private readonly IFileUploadService _uploadService;
    private readonly IAuthorizationService _authService;
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
        IPolicyRepository policyRepository,
        IPolicyTypeRepository policyTypeRepository,
        IMemoryCache cache,
        IUnitOfWork dbu,
        IWebHostEnvironment env,
        JwtAuthService jwtAuth,
        IFileUploadService uploadService,
        IAuthorizationService authService
    )
    {
        _context = context;
        _hotelRepository = hotelRepository;
        _amenityRepository = amenityRepository;
        _wishlistService = wishlistService;
        _accommodationRepository = accommodationRepository;
        _policyRepository = policyRepository;
        _policyTypeRepository = policyTypeRepository;
        _cache = cache;
        _dbu = dbu;
        _env = env;
        _jwtAuthService = jwtAuth;
        _uploadService = uploadService;
        _userRepository = userRepository;
        _authService = authService;
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
            HighlightAmenities = string.IsNullOrEmpty(t.Amenities)
            ? new List<AmenityDTO>()
            : JsonSerializer.Deserialize<List<AmenityDTO>>(t.Amenities)!
                .Take(3).ToList(),
            MinPricePerNight = t.MinPrice,
            MaxPricePerNight = t.MaxPrice,
            AvgPricePerNight = t.AvgPrice,
            AvailableRooms = t.AvailableRooms = 0,
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
                HighlightAmenities = string.IsNullOrEmpty(h.Amenities)
                ? new List<AmenityDTO>()
                : JsonSerializer.Deserialize<List<AmenityDTO>>(h.Amenities)!
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
        catch (Exception)
        {
            return new ApiResponse<IEnumerable<HotelListItemDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER
            };
        }
    }

    public async Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null, DateTime? checkIn = null, DateTime? checkOut = null)
    {
        if (hotelId < 0)
        {
            throw new ArgumentException("Invalid hotel ID");
        }

        var checkInDate = checkIn.HasValue ? DateOnly.FromDateTime(checkIn.Value.Date) : DateOnly.FromDateTime(DateTime.Today);
        var checkOutDate = checkOut.HasValue ? DateOnly.FromDateTime(checkOut.Value.Date) : checkInDate.AddDays(1);

        var cacheKey = $"HotelDetail_{hotelId}_{userId ?? 0}_{checkInDate:ddMMyyy}_{checkOutDate:ddMMyyy}";

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
            Description = h.Description ?? string.Empty,
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
            // RoomTypes sẽ được xử lý riêng bên dưới
            RoomTypes = new List<RoomTypeDTO>(),
            MinPricePerNight = h.RoomTypes
                    .Where(rt => rt.IsDeleted == false)
                    .Min(rt => (decimal?)rt.PricePerNight),
            AverageRating = h.Reviews.Any(r => r.IsDeleted == false && r.Rating.HasValue)
                    ? Math.Round((decimal)h.Reviews.Where(r => r.IsDeleted == false).Average(r => r.Rating ?? 0), 1)
                    : 0m,
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
            AvailableRooms = 0,
            IsWishlist = false,
            IsVerified = false, // Adjust based on your logic
            Status = "Active" // Adjust based on your logic
        })
        .FirstOrDefaultAsync();

        if (hotel == null) return null!;

        // === TÍNH SỐ PHÒNG TRỐNG THEO NGÀY (realtime) - dùng SP ===
        hotel.AvailableRooms = await CheckAvailableRoomsAsync(hotelId, checkInDate, checkOutDate);

        // === LẤY DANH SÁCH ROOMTYPE + TÍNH SỐ PHÒNG TRỐNG RIÊNG CHO TỪNG LOẠI ===
        var roomTypes = await _context.RoomTypes
            .AsNoTracking()
            .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
            .Select(rt => new RoomTypeDTO
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
                Amenities = rt.RoomAmenities
                    .Where(ra => ra.Amenity.IsDeleted == false)
                    .Select(ra => new AmenityDTO
                    {
                        Id = ra.Amenity.Id,
                        Name = ra.Amenity.Name,
                        Additional = ra.Amenity.Additional
                    }).ToList(),
                // AvailableRooms sẽ được tính realtime bên dưới
                AvailableRooms = 0
            })
            .ToListAsync();

        // Tính AvailableRooms cho từng RoomType (realtime theo ngày)
        var roomTypeIds = roomTypes.Select(rt => rt.Id).ToList();

        var availableCounts = await _context.Rooms
            .Where(r => roomTypeIds.Contains(r.RoomTypeId)
                     && r.IsDeleted == false
                     && r.Status == "Available"
                     && !_context.BookingRooms.Any(br =>
                         br.RoomTypeId == r.Id &&
                         _context.Bookings.Any(b =>
                             b.Id == br.BookingId &&
                             b.Status != "Cancelled" &&
                             b.CheckInDate < checkOutDate &&
                             b.CheckOutDate > checkInDate)))
            .GroupBy(r => r.RoomTypeId)
            .Select(g => new { RoomTypeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoomTypeId, x => x.Count);

        foreach (var rt in roomTypes)
        {
            rt.AvailableRooms = availableCounts.GetValueOrDefault(rt.Id, 0);
        }

        hotel.RoomTypes = roomTypes;

        // === Xử lý Amenity Additional (IconClass, IconColor) ===
        hotel.Amenities = hotel.Amenities.Select(ha =>
        {
            var additional = TryParseAdditional(ha.Additional);
            return new AmenityDTO
            {
                Id = ha.Id,
                Name = ha.Name,
                IconClass = additional.GetValueOrDefault("IconClass") ?? "fa-bed",
                IconColor = additional.GetValueOrDefault("IconColor") ?? "#54a9ffff"
            };
        }).ToList();

        foreach (var rt in hotel.RoomTypes)
        {
            rt.Amenities = rt.Amenities.Select(a =>
            {
                var additional = TryParseAdditional(a.Additional);
                return new AmenityDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    IconClass = additional.GetValueOrDefault("IconClass") ?? "fa-bed",
                    IconColor = additional.GetValueOrDefault("IconColor") ?? "#54a9ffff"
                };
            }).ToList();
        }

        // === Wishlist ===
        if (userId.HasValue)
        {
            var wishlistKey = $"Wishlist_{userId.Value}_{hotelId}";
            if (!_cache.TryGetValue(wishlistKey, out bool isWishlist))
            {
                isWishlist = await _wishlistService.IsInWishlistAsync(userId.Value, hotelId);
                _cache.Set(wishlistKey, isWishlist, TimeSpan.FromMinutes(10));
            }
            hotel.IsWishlist = isWishlist;
        }

        // Cache theo ngày
        _cache.Set(cacheKey, hotel, TimeSpan.FromMinutes(10));

        return hotel;
    }

    // Helper để parse JSON Additional
    private static Dictionary<string, string> TryParseAdditional(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, string>();
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    // kiểm tra phòng trống
    public async Task<int> CheckAvailableRoomsAsync(int hotelId, DateOnly checkIn, DateOnly checkOut, int adults = 1, int children = 0)
    {
        var result = await _context.Database
            .SqlQueryRaw<int>(
                "EXEC sp_CheckHotelAvailability @HotelId = {0}, @CheckIn = {1}, @CheckOut = {2}, @Adults = {3}, @Children = {4}",
                hotelId, checkIn.ToDateTime(TimeOnly.MinValue), checkOut.ToDateTime(TimeOnly.MinValue), adults, children)
            .ToListAsync();

        return result.FirstOrDefault();
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
            HighlightAmenities = string.IsNullOrEmpty(h.Amenities)
            ? new List<AmenityDTO>()
            : JsonSerializer.Deserialize<List<AmenityDTO>>(h.Amenities)!
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

    #region HOTELS OWNER
    public async Task<ApiResponse<HotelDetailForOwnerDTO>> GetHotelDetailForOwnerAsync(int hotelId, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<HotelDetailForOwnerDTO>();

            var cacheKey = $"OwnerHotelDetail_{hotelId}_{ownerId}";

            if (_cache.TryGetValue(cacheKey, out HotelDetailForOwnerDTO? cached))
                return ApiResponseHelper.Ok(cached!);

            // Correct: Tạo mảng tham số đúng cách
            var parameters = new[]
            {
            new SqlParameter("@HotelId", hotelId),
            new SqlParameter("@OwnerId", ownerId)
            };

            // Gọi stored procedure với tham số
            var jsonList = await _context.Database
                .SqlQueryRaw<string>("EXEC sp_GetHotelDetailForOwner @HotelId, @OwnerId", parameters)
                .ToListAsync();

            var json = jsonList.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(json) || json.Contains("error"))
                return ApiResponseHelper.NotFound<HotelDetailForOwnerDTO>("Không tìm thấy khách sạn hoặc bạn không có quyền.");

            var data = JsonSerializer.Deserialize<HotelDetailForOwnerDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
                return ApiResponseHelper.ServerError<HotelDetailForOwnerDTO>("Dữ liệu không hợp lệ.");

            // Cache 15 phút
            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(15));

            return ApiResponseHelper.Ok(data);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<HotelDetailForOwnerDTO>($"Lấy chi tiết khách sạn thất bại: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<HotelOwnerSummaryDTO>>> GetOwnerHotelsSummaryAsync(int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<List<HotelOwnerSummaryDTO>>();

            var cacheKey = $"{CacheKey.OwnerHotelsKeyPrefix}{ownerId}";
            if (_cache.TryGetValue(cacheKey, out List<HotelOwnerSummaryDTO> cached))
                return ApiResponseHelper.Ok(cached);

            var result = await _context.Database
                .SqlQueryRaw<HotelOwnerSummaryDTO>(
                    "EXEC sp_GetOwnerHotelsSummary @OwnerId",
                    new SqlParameter("@OwnerId", ownerId))
                .ToListAsync();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<HotelOwnerSummaryDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<HotelDraftFullDTO>> GetHotelDraftAsync(int hotelId, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<HotelDraftFullDTO>();

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId || hotel.Status != "Draft")
                return ApiResponseHelper.NotFound<HotelDraftFullDTO>("Không phải draft hoặc không thuộc về bạn.");

            // === BASIC INFO ===
            var basic = new HotelResponseDTO
            {
                HotelId = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                CityId = hotel.CityId,
                Description = hotel.Description,
                ContactName = hotel.ContactName,
                ContactPhone = hotel.ContactPhone,
                ContactEmail = hotel.ContactEmail,
                AccommodationTypeId = hotel.AccommodationTypeId ?? 0,
                ChainId = hotel.ChainId ?? 0
            };

            // === IMAGES ===
            var images = new HotelImagesResponseDTO
            {
                CoverImageUrl = hotel.CoverImageUrl,
                GalleryImageUrls = await _context.HotelImages
                    .Where(hi => hi.HotelId == hotelId && hi.IsDeleted == false)
                    .OrderBy(hi => hi.SortOrder)
                    .Select(hi => hi.ImageUrl)
                    .ToListAsync()
            };

            // === AMENITIES ===
            var amenityIds = await _context.HotelAmenities
                .Where(ha => ha.HotelId == hotelId)
                .Select(ha => ha.AmenityId)
                .ToListAsync();

            // === POLICIES ===
            var policyIds = await _context.HotelPolicies
                .Where(hp => hp.HotelId == hotelId && hp.PolicyId > 0)
                .Select(hp => hp.PolicyId)
                .ToListAsync();

            var customPolicyJsons = await _context.HotelPolicies
            .Where(hp => hp.HotelId == hotelId && hp.PolicyId == 0 && hp.Additional != null)
            .Select(hp => hp.Additional)
            .ToListAsync();

            // SAU ĐÓ MỚI deserialize ở C#
            var customPolicies = customPolicyJsons
                .Select(json => JsonSerializer.Deserialize<OwnerCustomPolicyDTO>(json)!)
                .ToList();

            var dto = new HotelDraftFullDTO
            {
                BasicInfo = basic,
                Images = images,
                SelectedAmenityIds = amenityIds,
                SelectedPolicyIds = policyIds,
                CustomPolicies = customPolicies
            };

            return ApiResponseHelper.Ok(dto);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<HotelDraftFullDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<HotelResponseDTO>> CreateHotelAsync(HotelCreateOrUpdateDTO ownerHotel, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<HotelResponseDTO>();
            // Kiểm tra trùng tên + địa chỉ
            var exists = await _hotelRepository.AnyAsync(h =>
                h.Name.Trim().ToLower() == ownerHotel.Name.Trim().ToLower() &&
                h.Address.Trim().ToLower() == ownerHotel.Address.Trim().ToLower() &&
                h.CityId == ownerHotel.CityId &&
                h.IsDeleted == false);

            if (exists)
                return ApiResponseHelper.Conflict<HotelResponseDTO>("Khách sạn với tên và địa chỉ này đã tồn tại.");

            // tạo hotel draft
            var hotel = new Hotel
            {
                Name = ownerHotel.Name.Trim(),
                Address = ownerHotel.Address.Trim(),
                CityId = ownerHotel.CityId,
                Description = ownerHotel.Description?.Trim(),
                OwnerId = ownerId,
                AccommodationTypeId = ownerHotel.AccommodationTypeId > 0 ? ownerHotel.AccommodationTypeId : null,
                ChainId = ownerHotel.ChainId > 0 ? ownerHotel.ChainId : null,
                ContactName = ownerHotel.ContactName?.Trim(),
                ContactPhone = ownerHotel.ContactPhone?.Trim(),
                ContactEmail = ownerHotel.ContactEmail?.Trim(),
                Status = "Draft",
                IsVerified = false,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _context.Hotels.AddAsync(hotel);
            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"{CacheKey.OwnerHotelsKeyPrefix}{ownerId}");

            // Trả về chi tiết hotel vừa tạo 
            return ApiResponseHelper.Ok(new HotelResponseDTO
            {
                HotelId = hotel.Id,
                Name = hotel.Name,
                Status = hotel.Status
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<HotelResponseDTO>("Tạo khách sạn thất bại.");
        }
    }

    public async Task<ApiResponse<HotelResponseDTO>> UpdateHotelAsync(int hotelId, HotelCreateOrUpdateDTO ownerHotel, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<HotelResponseDTO>();

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<HotelResponseDTO>("Khách sạn không tồn tại hoặc không thuộc về bạn.");

            if (hotel.Status != "Draft" && hotel.Status != "Active")
                return ApiResponseHelper.BadRequest<HotelResponseDTO>("Chỉ có thể cập nhật khi khách sạn ở trạng thái Draft hoặc Active.");

            // Kiểm tra trùng tên + địa chỉ (ngoại trừ chính nó)
            var exists = await _hotelRepository.AnyAsync(h =>
                h.Id != hotelId &&
                h.Name.Trim().ToLower() == ownerHotel.Name.Trim().ToLower() &&
                h.Address.Trim().ToLower() == ownerHotel.Address.Trim().ToLower() &&
                h.IsDeleted == false);

            if (exists)
                return ApiResponseHelper.Conflict<HotelResponseDTO>("Tên và địa chỉ này đã tồn tại.");

            hotel.Name = ownerHotel.Name;
            hotel.Address = ownerHotel.Address;
            hotel.CityId = ownerHotel.CityId;
            hotel.Description = ownerHotel.Description;
            hotel.AccommodationTypeId = ownerHotel.AccommodationTypeId;
            hotel.ChainId = ownerHotel.ChainId;
            hotel.ContactName = ownerHotel.ContactName;
            hotel.ContactPhone = ownerHotel.ContactPhone;
            hotel.ContactEmail = ownerHotel.ContactEmail;
            hotel.UpdatedAt = DateTime.Now;

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();
            // Xóa cache
            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            _cache.Remove($"{CacheKey.OwnerHotelsKeyPrefix}{ownerId}");

            return ApiResponseHelper.Ok(new HotelResponseDTO
            {
                HotelId = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                CityId = hotel.CityId,
                Description = hotel.Description,
                AccommodationTypeId = hotel.AccommodationTypeId,
                ChainId = hotel.ChainId,
                ContactName = hotel.ContactName,
                ContactPhone = hotel.ContactPhone,
                ContactEmail = hotel.ContactEmail,
                Status = hotel.Status,
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<HotelResponseDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<HotelResponseDTO>> DeleteHotelAsync(int hotelId, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<HotelResponseDTO>("Khách sạn không tồn tại hoặc không thuộc về bạn.");

            if (hotel.OwnerId != ownerId)
                return ApiResponseHelper.Forbidden<HotelResponseDTO>("Bạn không có quyền xóa khách sạn này.");

            // CHỈ CHO PHÉP XÓA KHI KHÔNG ACTIVE
            if (hotel.Status == "Active")
                return ApiResponseHelper.BadRequest<HotelResponseDTO>(
                    "Không thể xóa khách sạn đang hoạt động. Vui lòng liên hệ admin.");

            if (hotel.Status == "PendingVerification")
                return ApiResponseHelper.BadRequest<HotelResponseDTO>(
                    "Không thể xóa khi đang chờ duyệt.");

            hotel.IsDeleted = true;
            hotel.UpdatedAt = DateTime.Now;

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            var folderPath = $"hotels/{ownerId}/{hotelId}";
            await _uploadService.DeleteFolderAsync(folderPath);

            // Xóa cache
            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            _cache.Remove($"{CacheKey.OwnerHotelsKeyPrefix}{ownerId}");

            return ApiResponseHelper.Ok(new HotelResponseDTO
            {
                HotelId = hotelId,
                Message = "Xóa khách sạn thành công."
            });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<HotelResponseDTO>();
        }
    }

    // Upload ảnh chính cho hotel 
    public async Task<ApiResponse<string>> UploadHotelCoverImageAsync(int hotelId, IFormFile image, int ownerId)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<string>("Khách sạn không tồn tại hoặc không thuộc về bạn.");

            if (image == null || image.Length == 0)
                return ApiResponseHelper.BadRequest<string>("Vui lòng chọn ảnh.");

            // Xóa ảnh cũ
            if (!string.IsNullOrEmpty(hotel.CoverImageUrl))
                await _uploadService.DeleteImageAsync(hotel.CoverImageUrl);

            var folder = $"hotels/{ownerId}/{hotelId}/cover";
            var newUrl = await _uploadService.SaveImageAsync(image, folder, "cover");

            hotel.CoverImageUrl = newUrl;
            hotel.UpdatedAt = DateTime.Now;
            await _dbu.SaveChangesAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            return ApiResponseHelper.Ok(newUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UploadCover error: {ex.Message}");
            return ApiResponseHelper.ServerError<string>();
        }
    }

    public async Task<ApiResponse<HotelImagesResponseDTO>> GetHotelImagesAsync(int hotelId, int ownerId)
    {
        try
        {
            var hotel = await _context.Hotels
                .Where(h => h.Id == hotelId && h.OwnerId == ownerId && h.IsDeleted == false)
                .Select(h => new HotelImagesResponseDTO
                {
                    CoverImageUrl = h.CoverImageUrl,
                    GalleryImageUrls = h.HotelImages
                        .Where(hi => hi.IsDeleted == false)
                        .OrderBy(hi => hi.SortOrder)
                        .ThenBy(hi => hi.Id)
                        .Select(hi => hi.ImageUrl)
                        .Take(4)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (hotel == null)
                return ApiResponseHelper.NotFound<HotelImagesResponseDTO>();

            return ApiResponseHelper.Ok(hotel);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<HotelImagesResponseDTO>(ex.Message);
        }
    }

    // Upload ảnh gallery (done)
    public async Task<ApiResponse<string>> UploadHotelGalleryImageAsync(int hotelId, IFormFile image, int ownerId)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<string>("Khách sạn không tồn tại hoặc không thuộc về bạn.");

            if (image == null || image.Length == 0)
                return ApiResponseHelper.BadRequest<string>("Vui lòng chọn ảnh.");

            // ĐẾM ẢNH GALLERY HIỆN TẠI (chưa xóa mềm)
            var currentCount = await _context.HotelImages
                .CountAsync(hi => hi.HotelId == hotelId && hi.IsDeleted == false);
            if (currentCount >= 4)
                return ApiResponseHelper.BadRequest<string>("Chỉ được upload tối đa 4 ảnh phụ cho khách sạn.");

            var folder = $"hotels/{ownerId}/{hotelId}/gallery";
            var url = await _uploadService.SaveImageAsync(image, folder);
            if (url == null) return ApiResponseHelper.ServerError<string>("Upload thất bại.");

            _context.HotelImages.Add(new HotelImage
            {
                HotelId = hotelId,
                ImageUrl = url,
                SortOrder = currentCount + 1,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            });

            await _dbu.SaveChangesAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");

            return ApiResponseHelper.Ok(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UploadGallery error: {ex.Message}");
            return ApiResponseHelper.ServerError<string>();
        }
    }

    public async Task<ApiResponse<bool>> DeleteHotelImageAsync(int hotelId, string imageUrl, int ownerId)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<bool>("Khách sạn không tồn tại hoặc không thuộc về bạn.");

            var image = await _context.HotelImages
                .FirstOrDefaultAsync(hi => hi.HotelId == hotelId && hi.ImageUrl == imageUrl && hi.IsDeleted == false);

            if (image == null)
                return ApiResponseHelper.NotFound<bool>("Ảnh không tồn tại.");

            image.IsDeleted = true;
            await _dbu.SaveChangesAsync();

            await _uploadService.DeleteImageAsync(imageUrl);
            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");

            return ApiResponseHelper.Ok(true);
        }
        catch (Exception)
        {
            return ApiResponseHelper.ServerError<bool>();
        }
    }

    public async Task<ApiResponse<bool>> SelectAmenityToHotelAsync(int hotelId, HotelAmenitiesDTO amenityId, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<bool>();

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<bool>("Khách sạn không tồn tại hoặc không thuộc về bạn.");
            if (hotel.Status != "Draft")
                return ApiResponseHelper.BadRequest<bool>("Chỉ được chỉnh sửa khi ở trạng thái Draft.");

            // Xóa tất cả amenity cũ
            var oldAmenities = _context.HotelAmenities.Where(x => x.HotelId == hotelId);
            _context.HotelAmenities.RemoveRange(oldAmenities);

            // Thêm mới (batch)
            if (amenityId.AmenityIds?.Any() == true)
            {
                var validIds = await _context.Amenities
                    .Where(a => amenityId.AmenityIds.Contains(a.Id) && a.IsDeleted == false)
                    .Select(a => a.Id)
                    .ToListAsync();

                var newAmenities = validIds.Select(id => new HotelAmenity
                {
                    HotelId = hotelId,
                    AmenityId = id
                });

                _context.HotelAmenities.AddRange(newAmenities);
            }

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            return ApiResponseHelper.Ok(true);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>();
        }
    }

    public async Task<ApiResponse<bool>> SelectPolicyToHotelAsync(int hotelId, HotelPoliciesDTO policyId, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<bool>();

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<bool>("Khách sạn không tồn tại.");

            if (hotel.Status != "Draft")
                return ApiResponseHelper.BadRequest<bool>("Chỉ được chỉnh sửa khi ở trạng thái Draft.");
            // Kiểm tra tên custom trùng
            var customNames = policyId.OwnerCustomPolicies?.Select(c => c.Name.Trim().ToLower()).ToList() ?? new List<string>();
            if (customNames.Count != customNames.Distinct().Count())
                return ApiResponseHelper.Conflict<bool>("Tên chính sách tùy chỉnh không được trùng nhau.");

            // Xóa tất cả policy cũ
            var oldPolicies = _context.HotelPolicies.Where(x => x.HotelId == hotelId);
            _context.HotelPolicies.RemoveRange(oldPolicies);

            // Thêm mới ( hệ thống )
            if (policyId.PolicyIds?.Any() == true)
            {
                var validIds = await _context.Policies
                    .Where(p => policyId.PolicyIds.Contains(p.Id) && p.IsDeleted == false && p.PolicyType.IsActive == true)
                    .Select(p => p.Id)
                    .ToListAsync();

                var newPolicies = validIds.Select(id => new HotelPolicy
                {
                    HotelId = hotelId,
                    PolicyId = id
                });

                _context.HotelPolicies.AddRange(newPolicies);
            }

            // owner custom policy
            if (policyId.OwnerCustomPolicies?.Any() == true)
            {
                await AddCustomPoliciesAsync(hotelId, policyId.OwnerCustomPolicies, ownerId);
            }

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            return ApiResponseHelper.Ok(true);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>();
        }
    }

    private async Task AddCustomPoliciesAsync(int hotelId, List<OwnerCustomPolicyDTO> customs, int ownerId)
    {
        foreach (var c in customs)
        {
            var pt = await _policyTypeRepository.GetByIdAsync(c.PolicyTypeId);
            if (pt == null || pt.IsActive == false) continue;

            var jsonData = new
            {
                c.Name,
                c.Description,
                c.PolicyTypeId,
                PolicyTypeName = pt.TypeName,
                CreatedBy = ownerId,
                CreatedAt = DateTime.Now
            };

            _context.HotelPolicies.Add(new HotelPolicy
            {
                HotelId = hotelId,
                PolicyId = 0,
                Additional = JsonSerializer.Serialize(jsonData),
            });
        }
    }

    // Gửi duyệt khách sạn
    public async Task<ApiResponse<bool>> SubmitHotelAsync(int hotelId, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<bool>("Khách sạn không tồn tại.");

            if (hotel.Status != "Draft")
                return ApiResponseHelper.BadRequest<bool>("Chỉ gửi duyệt khi ở trạng thái Draft.");

            // Kiểm tra bắt buộc 
            if (string.IsNullOrWhiteSpace(hotel.Name) || string.IsNullOrWhiteSpace(hotel.Address))
                return ApiResponseHelper.BadRequest<bool>("Tên và địa chỉ không được để trống.");

            if (hotel.CityId <= 0)
                return ApiResponseHelper.BadRequest<bool>("Vui lòng chọn thành phố.");

            if (string.IsNullOrEmpty(hotel.CoverImageUrl))
                return ApiResponseHelper.BadRequest<bool>("Vui lòng upload ảnh bìa.");

            if (!await _context.HotelImages.AnyAsync(x => x.HotelId == hotelId && x.IsDeleted == false))
                return ApiResponseHelper.BadRequest<bool>("Vui lòng upload ít nhất 1 ảnh phụ.");

            var roomTypeCount = await _context.RoomTypes.CountAsync(rt => rt.HotelId == hotelId && rt.IsDeleted == false);
            if (roomTypeCount == 0)
                return ApiResponseHelper.BadRequest<bool>("Vui lòng tạo ít nhất 1 loại phòng.");

            var hasInvalidRoomType = await _context.RoomTypes
                .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
                .AnyAsync(rt =>
                    rt.PricePerNight <= 0 ||
                    rt.AdultCapacity < 1 ||
                    !_context.RoomImages.Any(ri => ri.RoomTypeId == rt.Id && ri.IsDeleted == false)
                );
            if (hasInvalidRoomType)
                return ApiResponseHelper.BadRequest<bool>("Một số loại phòng chưa có ảnh hoặc giá hợp lệ.");

            hotel.Status = "PendingVerification";
            hotel.UpdatedAt = DateTime.Now;

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            _cache.Remove($"{CacheKey.OwnerHotelsKeyPrefix}{ownerId}");

            return ApiResponseHelper.Ok(true, "Gửi duyệt thành công. Vui lòng chờ admin xét duyệt.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>("Gửi duyệt thất bại:" + ex.Message);
        }
    }

    #endregion
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
        catch (Exception)
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
            if (!await _authService.IsAdminAsync(userId) && uploadAccImg.CreatedBy != userId)
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
                _uploadService.DeleteImageAsync(uploadAccImg.ImagePath);
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
        catch (Exception)
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
            if (!await _authService.IsAdminAsync(userId) && updateAcc.CreatedBy != userId)
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
            if (!await _authService.IsAdminOrOwnerAsync(userId) && delAcc.CreatedBy != userId)
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
                _uploadService.DeleteImageAsync(delAcc.ImagePath);
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

    #region ROOMTYPE
    // RoomType
    public async Task<ApiResponse<RoomTypeForOwnerDTO>> GetRoomTypeDetailAsync(int roomTypeId, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<RoomTypeForOwnerDTO>();

            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);

            if (roomType == null || roomType.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<RoomTypeForOwnerDTO>("Không tìm thấy loại phòng.");

            var dto = new RoomTypeForOwnerDTO
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                PricePerNight = roomType.PricePerNight,
                AdultCapacity = roomType.AdultCapacity,
                ChildCapacity = roomType.ChildCapacity,
                TotalRooms = roomType.Quantity,
                AvailableRooms = await _context.Rooms.CountAsync(r => r.RoomTypeId == roomTypeId && r.Status == "Available" && r.IsDeleted == false),
                Area = roomType.Area,
                IsActive = roomType.IsActive,
                SortOrder = roomType.SortOrder,
                DefaultImageUrl = roomType.DefaultImageUrl,
                CreatedAt = roomType.CreatedAt!.Value,
                UpdatedAt = roomType.UpdatedAt!.Value
            };

            // Load Beds
            dto.Beds = await _context.RoomBedTypes
                .Where(rb => rb.RoomTypeId == roomTypeId)
                .Include(rb => rb.BedType)
                .Select(rb => new RoomBedTypeDTO
                {
                    BedTypeId = rb.BedType.Id,
                    BedTypeName = rb.BedType.Name,
                    Additional = rb.BedType.Additional,
                    Quantity = rb.Quantity,
                    IsPrimary = rb.IsPrimary.HasValue
                })
                .ToListAsync();

            // Load Views
            dto.Views = await _context.RoomViewTypes
                .Where(rv => rv.RoomTypeId == roomTypeId)
                .Include(rv => rv.ViewType)
                .Select(rv => new RoomViewTypeDTO
                {
                    ViewTypeId = rv.ViewType.Id,
                    ViewTypeName = rv.ViewType.Name,
                    Additional = rv.ViewType.Additional,
                    IsPrimary = rv.IsPrimary.HasValue
                })
                .ToListAsync();

            // Load Amenities
            dto.Amenities = await _context.RoomAmenities
                .Where(ra => ra.RoomTypeId == roomTypeId)
                .Include(ra => ra.Amenity)
                .Select(ra => new AmenityDTO
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name,
                    IconClass = ra.Amenity.IconClass,
                    Additional = ra.Additional
                })
                .ToListAsync();

            // Load Images
            dto.RoomImages = await _context.RoomImages
                .Where(ri => ri.RoomTypeId == roomTypeId && ri.IsDeleted == false)
                .OrderBy(ri => ri.SortOrder)
                .Select(ri => ri.ImageUrl)
                .ToListAsync();

            return ApiResponseHelper.Ok(dto);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<RoomTypeForOwnerDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<List<RoomTypeForOwnerDTO>>> GetRoomTypesAsync(int hotelId, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<List<RoomTypeForOwnerDTO>>();

            var parameters = new[]
            {
            new SqlParameter("@HotelId", hotelId)
            };

            var jsonList = await _context.Database
             .SqlQueryRaw<string>("EXEC sp_GetRoomTypesByHotel @HotelId", parameters)
             .ToListAsync();

            var json = jsonList.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(json) || json == "[]" || json.Contains("error") || json.Trim() == "null")
            {
                // TRẢ VỀ DANH SÁCH RỖNG + 200 OK + THÔNG BÁO THÂN THIỆN
                return ApiResponseHelper.Ok(new List<RoomTypeForOwnerDTO>(), "Chưa có loại phòng nào.");
            }
            var result = JsonSerializer.Deserialize<List<RoomTypeForOwnerDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<RoomTypeForOwnerDTO>();

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetRoomTypes] Error: {ex}");
            return ApiResponseHelper.ServerError<List<RoomTypeForOwnerDTO>>();
        }
    }

    public async Task<ApiResponse<WizardRoomTypeResponseDTO>> WizardCreateFirstRoomTypeAsync(
    int hotelId,
    WizardRoomTypeCreateDTO dto,
    int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<WizardRoomTypeResponseDTO>();

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<WizardRoomTypeResponseDTO>("Khách sạn không tồn tại.");

            if (hotel.Status != "Draft")
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>(
                    "Chỉ có thể tạo loại phòng khi khách sạn đang ở trạng thái Draft.");

            // NGĂN TẠO NHIỀU ROOMTYPE TRONG WIZARD
            var hasRoomType = await _context.RoomTypes.AnyAsync(rt =>
                rt.HotelId == hotelId && rt.IsDeleted == false);

            if (hasRoomType)
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>(
                    "Bạn đã tạo loại phòng. Vui lòng hoàn tất gửi duyệt hoặc quản lý trong trang chi tiết khách sạn.");

            // Validate bắt buộc cho wizard
            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>("Tên loại phòng là bắt buộc.");

            if (dto.PricePerNight <= 0)
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>("Giá phòng phải lớn hơn 0.");

            if (dto.AdultCapacity < 1)
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>("Sức chứa người lớn phải ≥ 1.");

            if (dto.Quantity < 1)
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>("Số lượng phòng phải ≥ 1.");

            if (!dto.Beds?.Any() ?? true)
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>("Vui lòng chọn ít nhất 1 loại giường.");

            // Validate BedType hợp lệ
            var validBedIds = await _context.BedTypes
                .Where(b => dto.Beds!.Select(x => x.BedTypeId).Contains(b.Id) && b.IsDeleted == false)
                .Select(b => b.Id)
                .ToListAsync();

            if (validBedIds.Count != dto.Beds!.Count)
                return ApiResponseHelper.BadRequest<WizardRoomTypeResponseDTO>("Một số loại giường không hợp lệ.");

            // Tạo RoomType
            var roomType = new RoomType
            {
                HotelId = hotelId,
                Name = dto.Name.Trim(),
                PricePerNight = dto.PricePerNight,
                AdultCapacity = dto.AdultCapacity,
                ChildCapacity = dto.ChildCapacity,
                Area = dto.Area,
                Quantity = dto.Quantity,
                IsActive = true,
                SortOrder = 1,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.RoomTypes.Add(roomType);
            await _dbu.SaveChangesAsync(); // cần Id

            // Tạo liên kết giường
            var bedLinks = dto.Beds.Select(b => new RoomBedType
            {
                RoomTypeId = roomType.Id,
                BedTypeId = b.BedTypeId,
                Quantity = b.Quantity,
            });
            await _context.RoomBedTypes.AddRangeAsync(bedLinks);

            // Tạo phòng vật lý
            var rooms = Enumerable.Range(1, dto.Quantity).Select(i => new Room
            {
                RoomTypeId = roomType.Id,
                RoomNumber = $"{roomType.Id:D3}-{i:D3}",
                Status = "Available",
                IsDeleted = false,
                CreatedAt = DateTime.Now
            });
            await _context.Rooms.AddRangeAsync(rooms);

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(new WizardRoomTypeResponseDTO
            {
                RoomTypeId = roomType.Id,
                Name = roomType.Name,
                PricePerNight = roomType.PricePerNight,
                Quantity = dto.Quantity,
                Message = "Tạo loại phòng thành công! Bạn đã đủ điều kiện gửi duyệt khách sạn."
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<WizardRoomTypeResponseDTO>($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<WizardSubmitValidationDTO>> ValidateHotelForSubmitAsync(int hotelId, int ownerId)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.OwnerId != ownerId || hotel.Status != "Draft")
                return ApiResponseHelper.NotFound<WizardSubmitValidationDTO>();

            var result = new WizardSubmitValidationDTO { CanSubmit = true };

            if (string.IsNullOrWhiteSpace(hotel.Name)) result.AddError("Tên khách sạn không được để trống");
            if (string.IsNullOrWhiteSpace(hotel.Address)) result.AddError("Địa chỉ không được để trống");
            if (hotel.CityId <= 0) result.AddError("Vui lòng chọn thành phố");
            if (string.IsNullOrEmpty(hotel.CoverImageUrl)) result.AddError("Chưa upload ảnh bìa");
            if (!await _context.HotelImages.AnyAsync(x => x.HotelId == hotelId && x.IsDeleted == false))
                result.AddError("Cần ít nhất 1 ảnh phụ");

            var roomTypeCount = await _context.RoomTypes.CountAsync(rt => rt.HotelId == hotelId && rt.IsDeleted == false);
            if (roomTypeCount == 0)
                result.AddError("Chưa tạo loại phòng nào");
            else if (roomTypeCount >= 3)
                result.AddError(
                    "Trong bước khởi tạo, bạn chỉ được tạo tối đa 3 loại phòng. Vui lòng hoàn tất gửi duyệt để thêm nhiều hơn.");

            if (roomTypeCount > 0)
            {
                var invalid = await _context.RoomTypes
                    .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
                    .AnyAsync(rt =>
                        rt.PricePerNight <= 0 ||
                        rt.AdultCapacity < 1 ||
                        !_context.RoomImages.Any(ri => ri.RoomTypeId == rt.Id && ri.IsDeleted == false));

                if (invalid) result.AddError("Loại phòng chưa có ảnh hoặc thông tin chưa hợp lệ");
            }

            result.CanSubmit = !result.Errors.Any();
            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<WizardSubmitValidationDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<CreateRoomTypeResponseDTO>> CreateRoomTypeAsync(int hotelId, RoomTypeCreateOrUpdateDTO roomType, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<CreateRoomTypeResponseDTO>();

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<CreateRoomTypeResponseDTO>("Khách sạn không tồn tại hoặc không thuộc về bạn.");

            if (hotel.Status != "Draft" && hotel.Status != "Active")
                return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Chỉ thêm loại phòng khi Draft hoặc Active.");

            if (string.IsNullOrWhiteSpace(roomType.Name))
                return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Tên loại phòng là bắt buộc.");

            if (!roomType.PricePerNight.HasValue || roomType.PricePerNight <= 0)
                return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Giá phòng phải lớn hơn 0.");

            if (!roomType.AdultCapacity.HasValue || roomType.AdultCapacity < 1)
                return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Sức chứa người lớn phải ≥ 1.");

            if (!roomType.Quantity.HasValue || roomType.Quantity < 1)
                return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Số lượng phòng phải ≥ 1.");

            if (roomType.Beds == null || !roomType.Beds.Any())
                return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Vui lòng chọn ít nhất 1 loại giường.");

            // Kiểm tra trùng tên RoomType trong cùng khách sạn
            var exists = await _context.RoomTypes
                .AnyAsync(rt => rt.HotelId == hotelId && rt.Name.Trim().ToLower() == roomType.Name.Trim().ToLower() && rt.IsDeleted == false);

            if (exists)
                return ApiResponseHelper.Conflict<CreateRoomTypeResponseDTO>("Loại phòng này đã tồn tại.");

            var newRoomType = new RoomType
            {
                HotelId = hotelId,
                Name = roomType.Name.Trim(),
                Description = roomType.Description?.Trim(),
                PricePerNight = roomType.PricePerNight.Value,
                AdultCapacity = roomType.AdultCapacity.Value,
                ChildCapacity = roomType.ChildCapacity ?? 0,
                Area = roomType.Area,
                Quantity = roomType.Quantity.Value,
                IsActive = true,
                SortOrder = await _context.RoomTypes.CountAsync(rt => rt.HotelId == hotelId && rt.IsDeleted == false) + 1,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.RoomTypes.Add(newRoomType);
            await _dbu.SaveChangesAsync();

            // 7. Tạo liên kết Bed
            if (roomType.Beds?.Any() == true)
            {
                var validBedIds = await _context.BedTypes
                .Where(b => roomType.Beds.Select(bt => bt.BedTypeId).Contains(b.Id) && b.IsDeleted == false)
                .CountAsync();

                if (validBedIds != roomType.Beds.Count)
                    return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Một số loại giường không hợp lệ");

                var roomBedTypes = roomType.Beds.Select(b => new RoomBedType
                {
                    RoomTypeId = newRoomType.Id,
                    BedTypeId = b.BedTypeId,
                    Quantity = b.Quantity,
                    IsPrimary = roomType.Beds.IndexOf(b) == 0

                });
                await _context.RoomBedTypes.AddRangeAsync(roomBedTypes);
            }

            // 8. Tạo liên kết View
            if (roomType.Views?.Any() == true)
            {
                var validViewCount = await _context.ViewTypes
                .Where(v => roomType.Views.Select(x => x.ViewTypeId).Contains(v.Id) && v.IsDeleted == false)
                .CountAsync();

                if (validViewCount != roomType.Views.Count)
                    return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Một số View không hợp lệ.");

                var roomViewTypes = roomType.Views.Select(v => new RoomViewType
                {
                    RoomTypeId = newRoomType.Id,
                    ViewTypeId = v.ViewTypeId,
                    IsPrimary = roomType.Views.First() == v
                });
                await _context.RoomViewTypes.AddRangeAsync(roomViewTypes);
            }

            var rooms = Enumerable.Range(1, roomType.Quantity.Value).Select(i => new Room
            {
                RoomTypeId = newRoomType.Id,
                RoomNumber = $"{newRoomType.Id:D3}-{i:D3}", // 001-001, 001-002...
                Status = "Available",
                IsDeleted = false,
                CreatedAt = DateTime.Now
            }).ToList();

            await _context.Rooms.AddRangeAsync(rooms);
            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();


            return ApiResponseHelper.Ok(new CreateRoomTypeResponseDTO
            {
                RoomTypeId = newRoomType.Id
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<CreateRoomTypeResponseDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<RoomTypeForOwnerDTO>> UpdateRoomTypeAsync(int roomTypeId, RoomTypeCreateOrUpdateDTO roomType, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Kiểm tra Owner
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<RoomTypeForOwnerDTO>();

            // 2. Lấy RoomType + Hotel
            var roomTypeHotel = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);

            if (roomTypeHotel == null || roomTypeHotel.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<RoomTypeForOwnerDTO>("Loại phòng không tồn tại.");

            // 3. CHỈ CHO PHÉP EDIT KHI HOTEL DRAFT HOẶC ACTIVE
            if (roomTypeHotel.Hotel.Status != "Draft" && roomTypeHotel.Hotel.Status != "Active")
                return ApiResponseHelper.BadRequest<RoomTypeForOwnerDTO>(
                    "Không thể chỉnh sửa khi khách sạn đang chờ duyệt hoặc đã bị từ chối.");

            bool isUpdated = false;

            // 4. Kiểm tra trùng tên (ngoại trừ chính nó)
            if (!string.IsNullOrWhiteSpace(roomType.Name))
            {
                var trimmed = roomType.Name.Trim();
                var nameExists = await _context.RoomTypes.AnyAsync(rt =>
                    rt.HotelId == roomTypeHotel.HotelId &&
                    rt.Id != roomTypeId &&
                    rt.Name.Trim().ToLower() == trimmed.ToLower() &&
                    rt.IsDeleted == false);

                if (nameExists)
                    return ApiResponseHelper.Conflict<RoomTypeForOwnerDTO>("Tên loại phòng đã tồn tại.");

                roomTypeHotel.Name = trimmed;
                isUpdated = true;
            }

            if (roomType.Description != null)
            {
                roomTypeHotel.Description = string.IsNullOrWhiteSpace(roomType.Description) ? null : roomTypeHotel.Description!.Trim();
                isUpdated = true;
            }

            if (roomType.PricePerNight.HasValue)
            {
                roomTypeHotel.PricePerNight = roomType.PricePerNight.Value;
                isUpdated = true;
            }

            if (roomType.AdultCapacity.HasValue)
            {
                roomTypeHotel.AdultCapacity = roomType.AdultCapacity.Value;
                isUpdated = true;
            }

            if (roomType.ChildCapacity.HasValue)
            {
                roomTypeHotel.ChildCapacity = roomType.ChildCapacity.Value;
                isUpdated = true;
            }

            if (roomType.Area.HasValue)
            {
                roomTypeHotel.Area = roomType.Area.Value;
                isUpdated = true;
            }
            if (roomType.Beds != null)
            {
                _context.RoomBedTypes.RemoveRange(
                    _context.RoomBedTypes.Where(x => x.RoomTypeId == roomTypeId));

                if (roomType.Beds.Any())
                {
                    var validBedIds = await _context.BedTypes
                        .Where(b => roomType.Beds.Select(x => x.BedTypeId).Contains(b.Id) && b.IsDeleted == false)
                        .Select(b => b.Id)
                        .ToListAsync();

                    if (validBedIds.Count != roomType.Beds.Count)
                        return ApiResponseHelper.BadRequest<RoomTypeForOwnerDTO>("Một số loại giường không hợp lệ.");

                    var newBeds = roomType.Beds.Select(b => new RoomBedType
                    {
                        RoomTypeId = roomTypeId,
                        BedTypeId = b.BedTypeId,
                        Quantity = b.Quantity
                    });
                    await _context.RoomBedTypes.AddRangeAsync(newBeds);
                }
                isUpdated = true;
            }

            if (roomType.Views != null)
            {
                _context.RoomViewTypes.RemoveRange(
                    _context.RoomViewTypes.Where(x => x.RoomTypeId == roomTypeId));

                if (roomType.Views.Any())
                {
                    var validViewIds = await _context.ViewTypes
                        .Where(v => roomType.Views.Select(x => x.ViewTypeId).Contains(v.Id) && v.IsDeleted == false)
                        .Select(v => v.Id)
                        .ToListAsync();

                    if (validViewIds.Count != roomType.Views.Count)
                        return ApiResponseHelper.BadRequest<RoomTypeForOwnerDTO>("Một số hướng nhìn không hợp lệ.");

                    var newViews = roomType.Views.Select(v => new RoomViewType
                    {
                        RoomTypeId = roomTypeId,
                        ViewTypeId = v.ViewTypeId
                    });
                    await _context.RoomViewTypes.AddRangeAsync(newViews);
                }
                isUpdated = true;
            }

            if (roomType.Quantity.HasValue)
            {
                var target = roomType.Quantity.Value;
                var currentRooms = await _context.Rooms
                    .Where(r => r.RoomTypeId == roomTypeId && r.IsDeleted == false)
                    .ToListAsync();

                var currentCount = currentRooms.Count;

                if (target < currentCount)
                {
                    var toDelete = currentRooms.OrderByDescending(r => r.Id).Take(currentCount - target);
                    foreach (var r in toDelete)
                    {
                        r.IsDeleted = true;
                        r.UpdatedAt = DateTime.Now;
                    }
                }
                else if (target > currentCount)
                {
                    var toAdd = Enumerable.Range(currentCount + 1, target - currentCount)
                        .Select(i => new Room
                        {
                            RoomTypeId = roomTypeId,
                            RoomNumber = $"{roomTypeId:D3}-{i:D3}",
                            Status = "Available",
                            IsDeleted = false,
                            CreatedAt = DateTime.Now
                        });
                    await _context.Rooms.AddRangeAsync(toAdd);
                }

                roomTypeHotel.Quantity = target;
                isUpdated = true;
            }
            if (!isUpdated)
                return ApiResponseHelper.BadRequest<RoomTypeForOwnerDTO>("Không có thông tin nào được cập nhật.");

            roomTypeHotel.UpdatedAt = DateTime.Now;

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            // 7. Trả về chi tiết mới nhất
            var updatedRoomtype = new RoomTypeForOwnerDTO
            {
                Id = roomTypeHotel.Id,
                Name = roomTypeHotel.Name,
                Description = roomTypeHotel.Description,
                PricePerNight = roomTypeHotel.PricePerNight,
                AdultCapacity = roomTypeHotel.AdultCapacity,
                ChildCapacity = roomTypeHotel.ChildCapacity,
                Quantity = roomTypeHotel.Quantity,
                Area = roomTypeHotel.Area,
                IsActive = roomTypeHotel.IsActive,
                SortOrder = roomTypeHotel.SortOrder,
                DefaultImageUrl = roomTypeHotel.DefaultImageUrl,
                UpdatedAt = roomTypeHotel.UpdatedAt
                // Beds, Views, Amenities, Images → load nhanh từ context (đã có trong transaction)
            };

            // Load Beds
            updatedRoomtype.Beds = await _context.RoomBedTypes
                .Where(rb => rb.RoomTypeId == roomTypeId)
                .Include(rb => rb.BedType)
                .Select(rb => new RoomBedTypeDTO
                {
                    BedTypeId = rb.BedType.Id,
                    Quantity = rb.Quantity,
                    Additional = rb.BedType.Additional
                })
                .ToListAsync();

            updatedRoomtype.Views = await _context.RoomViewTypes
            .Where(rv => rv.RoomTypeId == roomTypeId)
            .Include(rv => rv.ViewType)
            .Select(rv => new RoomViewTypeDTO
            {
                ViewTypeId = rv.ViewType.Id,
                Additional = rv.ViewType.Additional
            })
            .ToListAsync();

            // Load Amenities
            updatedRoomtype.Amenities = await _context.RoomAmenities
            .Where(ra => ra.RoomTypeId == roomTypeId)
            .Include(ra => ra.Amenity)
            .Select(ra => new AmenityDTO
            {
                Id = ra.Amenity.Id,
                Name = ra.Amenity.Name,
                Additional = ra.Amenity.Additional
            })
            .ToListAsync();

            // Load Images
            updatedRoomtype.RoomImages = await _context.RoomImages
            .Where(ri => ri.RoomTypeId == roomTypeId && ri.IsDeleted == false)
            .OrderBy(ri => ri.SortOrder)
            .Select(ri => ri.ImageUrl)
            .ToListAsync();

            return ApiResponseHelper.Ok(updatedRoomtype);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<RoomTypeForOwnerDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<DeleteRoomTypeResponseDTO>> DeleteRoomTypeAsync(int roomTypeId, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<DeleteRoomTypeResponseDTO>();

            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);

            if (roomType == null || roomType.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<DeleteRoomTypeResponseDTO>("Loại phòng không tồn tại hoặc không thuộc về bạn.");

            if (roomType.Hotel.Status == "PendingVerification")
                return ApiResponseHelper.BadRequest<DeleteRoomTypeResponseDTO>("Không thể xóa khi khách sạn đang chờ duyệt.");
            // Xóa mềm
            roomType.IsDeleted = true;
            roomType.UpdatedAt = DateTime.Now;

            var bedLinks = _context.RoomBedTypes.Where(x => x.RoomTypeId == roomTypeId);
            var viewLinks = _context.RoomViewTypes.Where(x => x.RoomTypeId == roomTypeId);
            var amenities = _context.RoomAmenities.Where(x => x.RoomTypeId == roomTypeId);
            var rooms = _context.Rooms.Where(r => r.RoomTypeId == roomTypeId);

            foreach (var room in rooms)
            {
                room.IsDeleted = true;
                room.UpdatedAt = DateTime.Now;
            }
            // Xóa ảnh
            var imageUrls = await _context.RoomImages
                .Where(ri => ri.RoomTypeId == roomTypeId && ri.IsDeleted == false)
                .Select(ri => ri.ImageUrl)
                .ToListAsync();

            foreach (var url in imageUrls)
            {
                await _uploadService.DeleteImageAsync(url);
            }

            _context.RoomBedTypes.RemoveRange(_context.RoomBedTypes.Where(x => x.RoomTypeId == roomTypeId));
            _context.RoomViewTypes.RemoveRange(_context.RoomViewTypes.Where(x => x.RoomTypeId == roomTypeId));
            _context.RoomAmenities.RemoveRange(_context.RoomAmenities.Where(x => x.RoomTypeId == roomTypeId));
            _context.Rooms.RemoveRange(_context.Rooms.Where(r => r.RoomTypeId == roomTypeId));
            _context.RoomImages.RemoveRange(_context.RoomImages.Where(ri => ri.RoomTypeId == roomTypeId));

            await _dbu.SaveChangesAsync();
            _cache.Remove($"OwnerRoomTypes_{roomType.HotelId}_{ownerId}");
            _cache.Remove($"OwnerHotelDetail_{roomType.HotelId}_{ownerId}");

            return ApiResponseHelper.Ok(new DeleteRoomTypeResponseDTO
            {
                RoomTypeId = roomTypeId
            });
        }
        catch (Exception)
        {
            return ApiResponseHelper.ServerError<DeleteRoomTypeResponseDTO>();
        }
    }

    public async Task<ApiResponse<string>> UploadRoomImageAsync(int roomTypeId, IFormFile image, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<string>();

            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);

            if (roomType == null || roomType.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<string>("Loại phòng không tồn tại.");

            if (roomType.Hotel.Status != "Draft" && roomType.Hotel.Status != "Active")
                return ApiResponseHelper.BadRequest<string>("Chỉ được upload ảnh khi khách sạn ở trạng thái Draft hoặc Active.");

            if (image == null || image.Length == 0)
                return ApiResponseHelper.BadRequest<string>("Vui lòng chọn ảnh.");

            var currentCount = await _context.RoomImages
                .CountAsync(ri => ri.RoomTypeId == roomTypeId && ri.IsDeleted == false);

            if (currentCount >= 5)
                return ApiResponseHelper.BadRequest<string>("Chỉ được upload tối đa 5 ảnh cho loại phòng.");

            var folder = $"hotels/{ownerId}/{roomType.HotelId}/rooms/{roomTypeId}";
            var isFirst = currentCount == 0;
            var publicIdHint = isFirst ? "default" : null;

            var url = await _uploadService.SaveImageAsync(image, folder, publicIdHint);
            if (url == null)
                return ApiResponseHelper.ServerError<string>("Upload thất bại.");

            // 
            if (isFirst)
            {
                roomType.DefaultImageUrl = url;
                roomType.UpdatedAt = DateTime.Now;
            }

            // Thêm ảnh với SortOrder đúng thứ tự
            _context.RoomImages.Add(new RoomImage
            {
                RoomTypeId = roomTypeId,
                ImageUrl = url,
                IsDefault = isFirst,
                SortOrder = currentCount + 1,  // Quan trọng: thứ tự tăng dần
                IsDeleted = false,
                CreatedAt = DateTime.Now
            });

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            // Xóa cache
            _cache.Remove($"OwnerHotelDetail_{roomType.HotelId}_{ownerId}");

            return ApiResponseHelper.Ok(url);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<string>("Upload ảnh thất bại.");
        }
    }

    public async Task<ApiResponse<bool>> DeleteRoomImageAsync(int roomTypeId, string imageUrl, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<bool>();

            var roomImage = await _context.RoomImages
                .Include(ri => ri.RoomType.Hotel)
                .FirstOrDefaultAsync(ri =>
                ri.RoomTypeId == roomTypeId &&
                ri.ImageUrl == imageUrl &&
                ri.IsDeleted == false);

            if (roomImage == null || roomImage.RoomType.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<bool>("Ảnh không tồn tại hoặc không thuộc về bạn.");

            roomImage.IsDeleted = true;
            await _dbu.SaveChangesAsync();

            await _uploadService.DeleteImageAsync(imageUrl);

            _cache.Remove($"OwnerHotelDetail_{roomImage.RoomType.HotelId}_{ownerId}");

            return ApiResponseHelper.Ok(true);
        }
        catch (Exception)
        {
            return ApiResponseHelper.ServerError<bool>();
        }
    }


    public async Task<ApiResponse<bool>> SelectAmenityToRoomTypeAsync(int roomTypeId, List<int> amenityIds, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<bool>();

            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);

            if (roomType == null || roomType.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<bool>("Loại phòng không tồn tại hoặc không thuộc về bạn.");

            if (roomType.Hotel.Status != "Draft" && roomType.Hotel.Status != "Active")
                return ApiResponseHelper.BadRequest<bool>("Chỉ chọn tiện ích khi khách sạn đang Draft hoặc Active.");

            // XÓA CŨ
            _context.RoomAmenities.RemoveRange(
                _context.RoomAmenities.Where(ra => ra.RoomTypeId == roomTypeId));

            // THÊM MỚI
            if (amenityIds?.Any() == true)
            {
                var validIds = await _context.Amenities
                    .Where(a => amenityIds.Contains(a.Id) && a.IsDeleted == false)
                    .Select(a => a.Id)
                    .ToListAsync();

                if (validIds.Count != amenityIds.Count)
                    return ApiResponseHelper.BadRequest<bool>("Tiện ích không hợp lệ.");

                _context.RoomAmenities.AddRange(validIds.Select(id => new RoomAmenity
                {
                    RoomTypeId = roomTypeId,
                    AmenityId = id
                }));
            }

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"OwnerHotelDetail_{roomType.HotelId}_{ownerId}");
            return ApiResponseHelper.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }


    public async Task<ApiResponse<RoomOptionsDTO>> GetRoomOptionsAsync()
    {
        try
        {
            var bedRes = await GetAllBedTypesAsync();
            var viewRes = await GetAllViewTypesAsync();

            if (bedRes.StatusCode != StatusCodeResponse.Success || viewRes.StatusCode != StatusCodeResponse.Success)
                return ApiResponseHelper.ServerError<RoomOptionsDTO>();

            var result = new RoomOptionsDTO
            {
                BedTypes = bedRes.Content ?? new List<BedTypeDTO>(),
                ViewTypes = viewRes.Content ?? new List<ViewTypeDTO>()
            };

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<RoomOptionsDTO>($"Lỗi tải tùy chọn phòng: {ex.Message}");
        }
    }

    #endregion

    #region MANAGE POLICIES
    public async Task<ApiResponse<List<PolicyTypeDTO>>> GetAllPolicyAsync()
    {
        try
        {
            // Cách dùng hoàn toàn giống các method khác trong project của bạn
            var jsonList = await _context.Database
                .SqlQueryRaw<string>("EXEC sp_GetPoliciesByActiveTypes")
                .ToListAsync();

            var jsonResult = jsonList.FirstOrDefault(); // Lấy chuỗi JSON đầu tiên (SP chỉ trả 1 dòng)

            if (string.IsNullOrWhiteSpace(jsonResult))
            {
                return ApiResponseHelper.Ok(new List<PolicyTypeDTO>(), "Chưa có chính sách nào.");
            }

            // Deserialize JSON → List<PolicyTypeDTO>
            var policyTypes = JsonSerializer.Deserialize<List<PolicyTypeDTO>>(jsonResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PolicyTypeDTO>();

            // Xử lý PoliciesJson + chuẩn hóa dữ liệu (giống hệt các phần khác)
            foreach (var pt in policyTypes)
            {
                pt.Policies ??= new List<PolicyDTO>();

                if (!string.IsNullOrWhiteSpace(pt.PoliciesJson))
                {
                    var subPolicies = JsonSerializer.Deserialize<List<PolicyDTO>>(pt.PoliciesJson);
                    if (subPolicies?.Any() == true)
                        pt.Policies.AddRange(subPolicies);
                }

                pt.Code = pt.Code?.Trim().Slugify() ?? string.Empty;
                pt.Name = pt.Name?.Trim() ?? string.Empty;
                pt.PolicyCount = pt.Policies.Count;
            }

            var result = policyTypes
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.Name)
                .ToList();

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<PolicyTypeDTO>>(
                $"Lỗi tải danh sách chính sách: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PolicyDTO>> CreatePolicyAsync(PolicyCreateOrUpdateDTO newPolicy, int userId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(userId))
                return ApiResponseHelper.Forbidden<PolicyDTO>();

            var policyType = await _policyTypeRepository.AnyAsync(pt => pt.Id == newPolicy.PolicyTypeId && pt.IsActive == true);
            if (!policyType)
                return ApiResponseHelper.BadRequest<PolicyDTO>("Loại chính sách không tồn tại hoặc đã bị vô hiệu hóa.");

            // Người dùng mà tạo trùng tên tiện ích thì không cho phép
            var exists = await _policyRepository.AnyAsync(p => p.Name.ToLower() == newPolicy.Name.ToLower() && p.IsDeleted == false);
            if (exists) return new ApiResponse<PolicyDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            // var additional = JsonSerializer.Serialize(new
            // {  });

            var policy = new Policy
            {
                Name = newPolicy.Name.Trim(),
                Description = newPolicy.Description.Trim(),
                IsDeleted = false,
                PolicyTypeId = newPolicy.PolicyTypeId,
                IsSystemPolicy = true,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            // Map entity sang DTO để trả về cho FE
            var resultDTO = new PolicyDTO
            {
                Id = policy.Id,
                Name = policy.Name,
                Description = newPolicy.Description,
                PolicyTypeId = newPolicy.PolicyTypeId,
                IsDeleted = false,
                IsSystemPolicy = true
            };

            await _policyRepository.AddAsync(policy);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<PolicyDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.CREATE_SUCCESSFULLY,
                Content = resultDTO
            };
        }
        catch (Exception)
        {
            return new ApiResponse<PolicyDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }
    }

    public async Task<ApiResponse<PolicyDTO>> UpdatePolicyAsync(int id, PolicyCreateOrUpdateDTO policy, int? userId)
    {
        try
        {
            var existingPolicy = await _policyRepository.GetByIdAsync(id);
            if (existingPolicy == null)
                return new ApiResponse<PolicyDTO>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = MessageResponse.UPDATE_FAILED,
                    Content = null
                };

            // Người dùng mà đổi tên trùng với tên của 1 tiện ích khác thì cũng không cho phép
            var nameExists = await _policyRepository.AnyAsync(p => p.Id != id && p.Name.ToLower() == policy.Name.ToLower());
            if (nameExists) return new ApiResponse<PolicyDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            existingPolicy.Name = policy.Name.Trim();
            existingPolicy.Description = policy.Description?.Trim();
            existingPolicy.PolicyTypeId = policy.PolicyTypeId;
            existingPolicy.UpdatedBy = userId;
            existingPolicy.UpdatedAt = DateTime.Now;

            var resultDTO = new PolicyDTO
            {
                Id = existingPolicy.Id,
                Name = existingPolicy.Name,
                Description = policy.Description,
                PolicyTypeId = existingPolicy.PolicyTypeId,
                IsDeleted = existingPolicy.IsDeleted,
                IsSystemPolicy = existingPolicy.IsSystemPolicy

            };

            await _policyRepository.UpdateAsync(existingPolicy);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<PolicyDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.UPDATE_SUCCESSFULLY,
                Content = resultDTO
            }; ;
        }
        catch (Exception)
        {
            return new ApiResponse<PolicyDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            }; ;
        }
    }

    public async Task<ApiResponse<bool>> DeletePolicyAsync(int id, int? userId)
    {
        try
        {
            var policy = await _policyRepository.GetByIdAsync(id);

            if (policy == null) return new ApiResponse<bool>
            {
                StatusCode = StatusCodeResponse.NotFound,
                Message = MessageResponse.NOT_FOUND,
                Content = false
            };

            policy.IsDeleted = true;
            policy.UpdatedAt = DateTime.Now;
            policy.UpdatedBy = userId;

            await _policyRepository.UpdateAsync(policy);
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

    public async Task<ApiResponse<PolicyTypeDTO>> CreatePolicyTypeAsync(PolicyTypeCreateOrUpdateDTO policyType, int adminId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<PolicyTypeDTO>();

            var exists = await _policyTypeRepository.AnyAsync(pt =>
                pt.TypeName.Trim().ToLower() == policyType.TypeName.Trim().ToLower());

            if (exists)
                return ApiResponseHelper.Conflict<PolicyTypeDTO>("Tên loại chính sách đã tồn tại.");
            // tạo mới
            var policyTypes = new PolicyType
            {
                Code = policyType.Code.Trim().Slugify(),
                TypeName = policyType.TypeName.Trim(),
                IsActive = true,
                CreatedBy = adminId,
                CreatedAt = DateTime.Now
            };

            await _policyTypeRepository.AddAsync(policyTypes);
            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();
            // trả về kết quả
            return ApiResponseHelper.Ok(new PolicyTypeDTO
            {
                Id = policyTypes.Id,
                Code = policyTypes.Code.Trim().Slugify(),
                Name = policyTypes.TypeName.Trim(),
                IsActive = policyTypes.IsActive,
                PolicyCount = 0
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<PolicyTypeDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<PolicyTypeDTO>> UpdatePolicyTypeAsync(int id, PolicyTypeCreateOrUpdateDTO pt, int? adminId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var policyType = await _policyTypeRepository.GetByIdAsync(id);
            if (policyType == null)
                return ApiResponseHelper.NotFound<PolicyTypeDTO>("Loại chính sách không tồn tại.");

            if (!await _authService.IsAdminAsync(adminId ?? 0) && policyType.CreatedBy != adminId)
                return ApiResponseHelper.Forbidden<PolicyTypeDTO>();

            var nameExists = await _policyTypeRepository.AnyAsync(pt =>
                pt.Id != id && pt.TypeName.Trim().ToLower() == pt.TypeName.Trim().ToLower());

            if (nameExists)
                return ApiResponseHelper.Conflict<PolicyTypeDTO>("Tên loại chính sách đã tồn tại.");

            policyType.Code = pt.Code.Trim().Slugify();
            policyType.TypeName = pt.TypeName.Trim();

            await _policyTypeRepository.UpdateAsync(policyType);
            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(new PolicyTypeDTO
            {
                Id = policyType.Id,
                Code = policyType.Code,
                Name = policyType.TypeName,
                IsActive = policyType.IsActive,
                PolicyCount = 0
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<PolicyTypeDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<PolicyTypeDTO>> TogglePolicyTypeActiveAsync(int id, int? adminId)
    {
        try
        {

            var policyType = await _policyTypeRepository.GetByIdAsync(id);
            if (policyType == null)
                return ApiResponseHelper.NotFound<PolicyTypeDTO>("Loại chính sách không tồn tại.");

            if (!await _authService.IsAdminAsync(adminId ?? 0))
                return ApiResponseHelper.Forbidden<PolicyTypeDTO>("Bạn không có quyền thực hiện hành động này.");

            policyType.IsActive = !policyType.IsActive;

            await _policyTypeRepository.UpdateAsync(policyType);
            await _dbu.SaveChangesAsync();

            return ApiResponseHelper.Ok(new PolicyTypeDTO
            {
                Id = policyType.Id,
                Code = policyType.Code?.Trim().Slugify(),
                Name = policyType.TypeName?.Trim(),
                IsActive = policyType.IsActive,
                PolicyCount = policyType.Policies?.Count(p => p.IsDeleted == false) ?? 0
            }, policyType.IsActive ? "Kích hoạt thành công." : "Vô hiệu hóa thành công.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PolicyTypeDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeletePolicyTypeAsync(int id, int? adminId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var policyType = await _policyTypeRepository.GetByIdAsync(id);
            if (policyType == null)
                return ApiResponseHelper.NotFound<bool>("Loại chính sách không tồn tại.");

            if (!await _authService.IsAdminAsync(adminId ?? 0))
                return ApiResponseHelper.Forbidden<bool>();

            // Kiểm tra có policy nào đang dùng không
            var hasPolicies = await _policyRepository.AnyAsync(p => p.PolicyTypeId == id && p.IsDeleted == false);
            if (hasPolicies)
                return ApiResponseHelper.BadRequest<bool>("Không thể xóa vì còn chính sách thuộc loại này.");

            _context.PolicyTypes.Remove(policyType);
            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();


            return ApiResponseHelper.Ok(true, "Xóa loại chính sách thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    #endregion

    #region Bedtype
    public async Task<ApiResponse<List<BedTypeDTO>>> GetAllBedTypesAsync()
    {
        try
        {

            var bedTypes = await _context.BedTypes
                .AsNoTracking()
                .Where(b => b.IsDeleted == false)
                .OrderBy(b => b.SortOrder)
                .ToListAsync();

            var result = new List<BedTypeDTO>();
            foreach (var b in bedTypes)
            {
                BedTypeAdditional? add = null;
                if (!string.IsNullOrWhiteSpace(b.Additional))
                {
                    add = JsonSerializer.Deserialize<BedTypeAdditional>(b.Additional);
                }

                result.Add(new BedTypeDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = add?.Description,
                    IconClass = add?.IconClass ?? "fa-bed",
                    IconColor = add?.IconColor ?? "#54a9ffff"
                });
            }

            return new ApiResponse<List<BedTypeDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = result
            };
        }
        catch (Exception)
        {
            return new ApiResponse<List<BedTypeDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }
    }

    public async Task<ApiResponse<BedTypeDTO>> CreateBedTypeAsync(BedTypeCreateOrUpdateDTO newBedType, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<BedTypeDTO>();

        try
        {

            var exists = await _context.BedTypes.AnyAsync(b => b.Name.ToLower() == newBedType.Name.ToLower() && b.IsDeleted == false);
            if (exists) return new ApiResponse<BedTypeDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            var additional = JsonSerializer.Serialize(new BedTypeAdditional
            {
                Description = newBedType.Description,
                IconClass = newBedType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newBedType.IconColor) ? "#4CAF50" : newBedType.IconColor
            });

            var bedType = new BedType
            {
                Name = newBedType.Name.Trim(),
                Additional = additional,
                // DefaultQuantity = newBedType.Quantity,
                IsDeleted = false
            };

            await _context.BedTypes.AddAsync(bedType);
            await _dbu.SaveChangesAsync();

            var resultDTO = new BedTypeDTO
            {
                Id = bedType.Id,
                Name = bedType.Name,
                Description = newBedType.Description,
                IconClass = newBedType.IconClass,
            };

            return new ApiResponse<BedTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.CREATE_SUCCESSFULLY,
                Content = resultDTO
            };
        }
        catch (Exception)
        {
            return new ApiResponse<BedTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }
    }

    public async Task<ApiResponse<BedTypeDTO>> UpdateBedTypeAsync(int id, BedTypeCreateOrUpdateDTO bedType, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<BedTypeDTO>();
        try
        {
            var exitsBedtype = await _context.BedTypes.FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);
            if (exitsBedtype == null)
                return new ApiResponse<BedTypeDTO>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = MessageResponse.UPDATE_FAILED,
                    Content = null
                };

            // Người dùng mà đổi tên trùng với tên của 1 tiện ích khác thì cũng không cho phép
            var nameExists = await _context.BedTypes.AnyAsync(a => a.Id != id && a.Name.ToLower() == bedType.Name.ToLower() && a.IsDeleted == false);
            if (nameExists) return new ApiResponse<BedTypeDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            exitsBedtype.Name = bedType.Name.Trim();
            exitsBedtype.Additional = JsonSerializer.Serialize(new BedTypeAdditional
            {
                Description = string.IsNullOrWhiteSpace(bedType.Description) ? null : bedType.Description,
                IconClass = bedType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(bedType.IconColor) ? "blue" : bedType.IconColor
            });

            var resultDTO = new BedTypeDTO
            {
                Id = exitsBedtype.Id,
                Name = exitsBedtype.Name,
                Description = bedType.Description,
                IconClass = bedType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(bedType.IconColor) ? "blue" : bedType.IconColor
            };

            _context.BedTypes.Update(exitsBedtype);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<BedTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.UPDATE_SUCCESSFULLY,
                Content = resultDTO
            }; ;
        }
        catch (Exception)
        {
            return new ApiResponse<BedTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            }; ;
        }
    }

    public async Task<ApiResponse<bool>> DeleteBedTypeAsync(int id, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<bool>();

        try
        {
            var bedType = await _context.BedTypes
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (bedType == null)
                return ApiResponseHelper.NotFound<bool>();

            // Kiểm tra đang được dùng không
            var inUse = await _context.RoomBedTypes.AnyAsync(x => x.BedTypeId == id);
            if (inUse)
                return ApiResponseHelper.BadRequest<bool>("Không thể xóa vì loại giường đang được sử dụng trong phòng.");

            bedType.IsDeleted = true;
            await _dbu.SaveChangesAsync();

            return ApiResponseHelper.Ok(true, MessageResponse.DELETE_SUCCESSFULLY);
        }
        catch (Exception)
        {
            return ApiResponseHelper.ServerError<bool>();
        }
    }

    #endregion
    #region ViewType
    public async Task<ApiResponse<List<ViewTypeDTO>>> GetAllViewTypesAsync()
    {
        try
        {

            var viewTypes = await _context.ViewTypes
                .AsNoTracking()
                .Where(b => b.IsDeleted == false)
                .OrderBy(b => b.SortOrder)
                .ToListAsync();

            var result = new List<ViewTypeDTO>();
            foreach (var v in viewTypes)
            {
                ViewTypeAdditional? add = null;
                if (!string.IsNullOrWhiteSpace(v.Additional))
                {
                    add = JsonSerializer.Deserialize<ViewTypeAdditional>(v.Additional);
                }

                result.Add(new ViewTypeDTO
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = add?.Description,
                    IconClass = add?.IconClass ?? "fa fa-eye",
                    IconColor = add?.IconColor ?? "#54a9ffff"
                });
            }

            return new ApiResponse<List<ViewTypeDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = result
            };
        }
        catch (Exception)
        {
            return new ApiResponse<List<ViewTypeDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }
    }

    public async Task<ApiResponse<ViewTypeDTO>> CreateViewTypeAsync(ViewTypeCreateOrUpdateDTO newViewType, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<ViewTypeDTO>();

        try
        {

            var exists = await _context.ViewTypes.AnyAsync(b => b.Name.ToLower() == newViewType.Name.ToLower() && b.IsDeleted == false);
            if (exists) return new ApiResponse<ViewTypeDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            var additional = JsonSerializer.Serialize(new ViewTypeAdditional
            {
                Description = newViewType.Description,
                IconClass = newViewType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newViewType.IconColor) ? "#4CAF50" : newViewType.IconColor
            });

            var viewType = new ViewType
            {
                Name = newViewType.Name.Trim(),
                Additional = additional,
                IsDeleted = false
            };

            await _context.ViewTypes.AddAsync(viewType);
            await _dbu.SaveChangesAsync();

            var resultDTO = new ViewTypeDTO
            {
                Id = viewType.Id,
                Name = viewType.Name,
                Description = newViewType.Description,
                IconClass = newViewType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(newViewType.IconColor) ? "#4CAF50" : newViewType.IconColor
            };

            return new ApiResponse<ViewTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.CREATE_SUCCESSFULLY,
                Content = resultDTO
            };
        }
        catch (Exception)
        {
            return new ApiResponse<ViewTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            };
        }
    }

    public async Task<ApiResponse<ViewTypeDTO>> UpdateViewTypeAsync(int id, ViewTypeCreateOrUpdateDTO viewType, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<ViewTypeDTO>();

        try
        {
            var exitsViewType = await _context.ViewTypes.FirstOrDefaultAsync(v => v.Id == id && v.IsDeleted == false);
            if (exitsViewType == null)
                return new ApiResponse<ViewTypeDTO>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = MessageResponse.UPDATE_FAILED,
                    Content = null
                };

            // Người dùng mà đổi tên trùng với tên của 1 tiện ích khác thì cũng không cho phép
            var nameExists = await _context.ViewTypes.AnyAsync(a => a.Id != id && a.Name.ToLower() == viewType.Name.ToLower() && a.IsDeleted == false);
            if (nameExists) return new ApiResponse<ViewTypeDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
                Content = null
            };

            exitsViewType.Name = viewType.Name.Trim();
            exitsViewType.Additional = JsonSerializer.Serialize(new ViewTypeAdditional
            {
                Description = string.IsNullOrWhiteSpace(viewType.Description) ? null : viewType.Description,
                IconClass = viewType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(viewType.IconColor) ? "blue" : viewType.IconColor
            });

            var resultDTO = new ViewTypeDTO
            {
                Id = exitsViewType.Id,
                Name = exitsViewType.Name,
                Description = viewType.Description,
                IconClass = viewType.IconClass,
                IconColor = string.IsNullOrWhiteSpace(viewType.IconColor) ? "blue" : viewType.IconColor
            };

            _context.ViewTypes.Update(exitsViewType);
            await _dbu.SaveChangesAsync();

            return new ApiResponse<ViewTypeDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.UPDATE_SUCCESSFULLY,
                Content = resultDTO
            }; ;
        }
        catch (Exception)
        {
            return new ApiResponse<ViewTypeDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = MessageResponse.ERROR_IN_SERVER,
                Content = null
            }; ;
        }
    }

    public async Task<ApiResponse<bool>> DeleteViewTypeAsync(int id, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<bool>();

        var viewType = await _context.ViewTypes.FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

        if (viewType == null) return new ApiResponse<bool>
        {
            StatusCode = StatusCodeResponse.NotFound,
            Message = MessageResponse.NOT_FOUND,
            Content = false
        };

        try
        {
            viewType.IsDeleted = true;

            _context.ViewTypes.Update(viewType);
            await _dbu.SaveChangesAsync();

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