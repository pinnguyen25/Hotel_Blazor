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
    #region Admin Manage
    public Task<ApiResponse<PagedResult<CustomerDTO>>> GetCustomersForAdminAsync(string? keyword, int pageIndex, int pageSize, int adminId);
    public Task<ApiResponse<PagedResult<OwnerDTO>>> GetOwnersForAdminAsync(string? keyword, int pageIndex, int pageSize, int adminId);
    public Task<ApiResponse<bool>> ToggleUserStatusAsync(int userId, int adminId);
    public Task<ApiResponse<UserDetailDTO>> GetUserDetailForAdminAsync(int targetUserId, int adminId);

    // dashboard
    public Task<ApiResponse<AdminDashboardStatsDTO>> GetAdminDashboardStatsAsync(int adminId);
    public Task<ApiResponse<AdminRevenueDTO>> GetPlatformRevenueStatsAsync(int year);
    public Task<ApiResponse<PagedResult<WithdrawalRequestAdminDTO>>> GetWithdrawalRequestsAsync(string status, int pageIndex, int pageSize);
    public Task<ApiResponse<bool>> ProcessWithdrawalRequestAsync(AdminProcessWithdrawalDTO request, int adminId);
    public Task<ApiResponse<List<HotelRevenueRankDTO>>> GetTopRevenueHotelsAsync(int year, int top = 10);
    // review
    // public Task<ApiResponse<PagedResult<ReviewDetailAdminDTO>>> GetAllReviewsForAdminAsync(string? keyword, int pageIndex, int pageSize);
    // public Task<ApiResponse<bool>> DeleteReviewByAdminAsync(int reviewId, int adminId, string reason);
    // amenity
    public Task<ApiResponse<List<AmenityDTO>>> GetAllAmenitiesAsync();
    public Task<ApiResponse<AmenityDTO>> CreateAmenityAsync(AmenityCreateOrUpdateDTO newAmenity, int? userId);
    public Task<ApiResponse<AmenityDTO>> UpdateAmenityAsync(int id, AmenityCreateOrUpdateDTO amenity, int? userId);
    public Task<ApiResponse<bool>> DeleteAmenityAsync(int id, int? userId);
    public Task<ApiResponse<bool>> ToggleFilterableAsync(int id, int? userId);
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
    public Task<ApiResponse<RoomTypeForOwnerDTO>> GetRoomTypeDetailAsync(int roomTypeId);
    public Task<ApiResponse<List<RoomTypeForOwnerDTO>>> GetRoomTypesAsync(int hotelId, int ownerId);
    public Task<ApiResponse<WizardSubmitValidationDTO>> ValidateHotelForSubmitAsync(int hotelId, int ownerId);
    public Task<ApiResponse<CreateRoomTypeResponseDTO>> CreateRoomTypeAsync(int hotelId, RoomTypeCreateOrUpdateDTO roomType, int ownerId); // chung với new và old
    public Task<ApiResponse<RoomTypeForOwnerDTO>> UpdateRoomTypeAsync(int roomTypeId, RoomTypeCreateOrUpdateDTO roomType, int ownerId);
    public Task<ApiResponse<DeleteRoomTypeResponseDTO>> DeleteRoomTypeAsync(int roomTypeId, int ownerId);
    public Task<ApiResponse<bool>> SelectAmenityToRoomTypeAsync(int roomTypeId, List<int> amenityIds, int ownerId);
    public Task<ApiResponse<string>> UploadRoomImageAsync(int roomTypeId, IFormFile image, int ownerId);
    public Task<ApiResponse<bool>> DeleteRoomImageAsync(int roomTypeId, string imageUrl, int ownerId);
    public Task<ApiResponse<RoomOptionsDTO>> GetRoomOptionsAsync();
    // Clone RoomType
    public Task<ApiResponse<RoomTypeForOwnerDTO>> CloneRoomTypeAsync(int sourceRoomTypeId, int ownerId);

    // Service Owner
    public Task<ApiResponse<List<OwnerHotelServiceDTO>>> GetOwnerHotelServicesAsync(int hotelId, int ownerId);
    public Task<ApiResponse<List<AvailableServiceDTO>>> GetAvailableServicesToAddAsync(int hotelId, int ownerId);
    public Task<ApiResponse<OwnerHotelServiceDTO>> AddServiceToHotelAsync(OwnerAddServiceDTO ownerService, int ownerId);
    public Task<ApiResponse<OwnerHotelServiceDTO>> UpdateHotelServiceAsync(int id, OwnerUpdateServiceDTO ownerService, int ownerId);
    public Task<ApiResponse<bool>> RemoveServiceFromHotelAsync(int id, int ownerId);
    public Task<ApiResponse<bool>> ToggleActiveOwnerServiceAsync(int id, int ownerId);
    // Booking Service in Booking
    public Task<ApiResponse<bool>> AddServiceToBookingAsync(AddServiceRequestDTO request, int userId);
    public Task<ApiResponse<bool>> UpdateServiceQuantityAsync(UpdateServiceQuantityDTO request, int requesterId);
    public Task<ApiResponse<bool>> DeleteServiceFromBookingAsync(int bookingServiceId, int requesterId);

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

    // banner
    public Task<ApiResponse<List<BannerDTO>>> CreateBannersAsync(BannerCreateDTO img, int adminId);
    public Task<ApiResponse<BannerDTO>> UpdateBannerAsync(int id, BannerUpdateDTO img, int adminId);
    public Task<ApiResponse<bool>> DeleteBannerAsync(int id, int adminId);
    public Task<ApiResponse<List<BannerDTO>>> GetAllBannersAsync();
    // 5. Lấy theo trang (Cho Frontend hiển thị - Home, AboutUs...)
    public Task<ApiResponse<List<BannerDTO>>> GetBannersByPageAsync(string pageName);
    public Task<ApiResponse<bool>> ToggleActiveBannerAsync(int id, int adminId);

    // service
    public Task<ApiResponse<PagedResult<ServicesDTO>>> GetAllServicesAsync(string? keyword = null, int pageIndex = 1, int pageSize = 10);
    public Task<ApiResponse<List<ServicesDTO>>> GetFeaturedServicesAsync();
    public Task<ApiResponse<ServiceDetailDTO>> GetServiceDetailWithUsageAsync(int id, int adminId);
    public Task<ApiResponse<ServicesDTO>> CreateServicesAsync(ServiceCreateOrUpdateDTO sv, int adminId);
    public Task<ApiResponse<ServicesDTO>> UpdateServicesAsync(int id, ServiceCreateOrUpdateDTO sv, int adminId);
    public Task<ApiResponse<bool>> DeleteServiceAsync(int id, int adminId);
    #endregion

    #region Owner 
    public Task<ApiResponse<OwnerDashboardStatsDTO>> GetOwnerDashboardStatsAsync(int hotelId, int ownerId);
    public Task<ApiResponse<List<RevenueChartDTO>>> GetRevenueChartAsync(int hotelId, int ownerId, string viewType);
    public Task<ApiResponse<bool>> SelectAmenityToHotelAsync(int hotelId, HotelAmenitiesDTO amenityId, int ownerId);
    public Task<ApiResponse<bool>> SelectPolicyToHotelAsync(int hotelId, HotelPoliciesDTO policyId, int ownerId);
    public Task<ApiResponse<bool>> SubmitHotelAsync(int hotelId, int ownerId);

    // hotel customer
    // booking
    public Task<ApiResponse<List<ServiceAddOnDTO>>> GetAddOnServicesForBookingAsync(int hotelId);
    public Task<ApiResponse<List<BookingRoomDetailDTO>>> GetBookingRoomsDetailsAsync(int bookingId, int requesterId);
    public Task<ApiResponse<InvoiceDTO>> GetInvoicePreviewAsync(int bookingId, int requesterId);

    // rooms
    // Quản lý phòng vật lý
    // guest
    public Task<ApiResponse<bool>> UpdateGuestNameAsync(UpdateGuestNameDTO request, int requesterId);
    public Task<ApiResponse<List<RoomPhysicalDTO>>> GetPhysicalRoomsAsync(int hotelId, int requesterId, int? roomTypeId = null);
    // 
    public Task<ApiResponse<RoomPhysicalDTO>> CreatePhysicalRoomAsync(int roomTypeId, CreateRoomRequestDTO roomNumber, int ownerId);
    public Task<ApiResponse<RoomPhysicalDTO>> UpdatePhysicalRoomAsync(int roomId, UpdateRoomPhysicalDTO request, int requesterId);
    public Task<ApiResponse<bool>> DeletePhysicalRoomAsync(int roomId, int ownerId);

    //  booking
    public Task<ApiResponse<List<SchedulerBookingDTO>>> GetBookingsForCalendarAsync(int hotelId, int ownerId, DateTime start, DateTime end);
    public Task<ApiResponse<bool>> AssignRoomToBookingAsync(AssignRoomRequestDTO request, int requesterId);
    public Task<ApiResponse<List<BookingRoomDetailDTO>>> GetPendingBookingsAsync(int hotelId, int requesterId);
    public Task<ApiResponse<bool>> MoveBookingAsync(MoveBookingRequestDTO request, int ownerId);
    // Tạo booking nhanh (Walk-in)
    public Task<ApiResponse<bool>> CreateWalkInBookingAsync(WalkInBookingRequestDTO request, int ownerId);
    #endregion

    #region Staff Manage
    public Task<ApiResponse<List<StaffDTO>>> GetHotelStaffsAsync(int hotelId, int ownerId);
    public Task<ApiResponse<StaffDTO>> CreateStaffAsync(CreateStaffRequestDTO request, int ownerId);
    public Task<ApiResponse<bool>> UpdateStaffAsync(int staffId, UpdateStaffRequestDTO request, int ownerId);
    public Task<ApiResponse<bool>> DeleteStaffAsync(int staffId, int ownerId);
    // housekeeping task
    public Task<ApiResponse<List<HousekeepingTaskDTO>>> GetHousekeepingTasksAsync(int hotelId, int requesterId, int? staffId = null);
    public Task<ApiResponse<bool>> ProcessCheckoutPaymentAsync(PaymentRequestDTO request, int requesterId);
    public Task<ApiResponse<bool>> AssignHousekeepingTaskAsync(AssignTaskRequestDTO request, int ownerId);
    public Task<ApiResponse<bool>> UpdateTaskStatusAsync(UpdateTaskStatusRequestDTO request, int requesterId);

    #endregion

    #region User
    #region booking User
    public Task<ApiResponse<BookingDetailDTO>> GetBookingDetailForUserAsync(int bookingId, int requesterId); // chi tiết đơn cho user xem
    public Task<ApiResponse<BookingShortDTO>> GetUpcomingTripAsync(int userId);
    public Task<ApiResponse<List<BookingListItemDTO>>> GetCustomerBookingsAsync(int userId); // danh sách đơn của user
    public Task<ApiResponse<BookingResponseDTO>> CreateBookingAsync(BookingCreateDTO request, int userId); // tạo đơn
    // hủy đơn
    public Task<ApiResponse<bool>> CancelBookingAsync(int bookingId, int requesterId);

    #endregion
    #region Payment
    public Task<ApiResponse<bool>> ConfirmBookingPaymentAsync(PaymentRequestDTO request, int userId);

    #endregion
    public Task<List<HotelListItemDTO>> GetAllHotelsAsync(int? userId = null);
    // Lấy hotel theo id
    public Task<HotelDetailDTO> GetHotelByIdAsync(int hotelId, int? userId = null, DateTime? checkIn = null, DateTime? checkOut = null);
    // Lấy hotel theo loại lưu trú
    public Task<ApiResponse<IEnumerable<HotelListItemDTO>>> GetHotelsByAccommodationTypeAsync(int accTypeId, int? userId = null);
    // Lấy hotel rate cao
    public Task<List<HotelListItemDTO>> GetHighlyRatedHotelsAsync(int? userId = null);

    // search hotel theo name
    public Task<List<HotelListItemDTO>> GetSearchOptionsAsync(string? destination, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms, decimal? priceMin, decimal? priceMax, decimal? ratingMin, string? accommodationTypeIds, string? amenityIds,
    string? bedTypeIds, string? viewTypeIds, string? chainIds, string? policyIds, string? serviceIds,
        string? sortBy, int? userId);
    // kiểm tra phòng trống
    public Task<int> CheckAvailableRoomsAsync(int hotelId, DateOnly checkIn, DateOnly checkOut, int? roomTypeId = null, int adults = 1, int children = 0);
    public Task<List<CityDTO>> GetCityNameAsync();
    public Task<List<AutocompleteDTO>> GetAutocompleteAsync(string keyword);
    #endregion

    #region Reviews
    public Task<ApiResponse<bool>> ReplyReviewAsync(ReplyReviewDTO request, int ownerId);
    public Task<ApiResponse<List<ReviewDTO>>> GetReviewsByHotelAsync(int hotelId);
    public Task<ApiResponse<bool>> CreateReviewAsync(ReviewCreateDTO request, int userId);
    #endregion
    #region Promotions
    public Task<ApiResponse<List<PromotionDTO>>> GetAllPromotionsAsync(int adminId);
    public Task<ApiResponse<PromotionDTO>> CreatePromotionAsync(PromotionCreateDTO request, int adminId);
    public Task<ApiResponse<PromotionDTO>> UpdatePromotionAsync(int id, PromotionCreateDTO request, int adminId);
    // Delete
    public Task<ApiResponse<bool>> DeletePromotionAsync(int id, int adminId);
    public Task<ApiResponse<bool>> TogglePromotionStatusAsync(int id, int adminId);

    // user
    public Task<ApiResponse<List<PromotionDTO>>> GetAvailablePromotionsForUserAsync();

    // Xử lý Logic tính tiền (Quan trọng nhất)
    public Task<ApiResponse<PriceResultDTO>> CalculateBookingPriceAsync(PriceCalculationDTO request);

    #endregion

    #region Wallet
    public Task<ApiResponse<OwnerWalletDTO>> GetOwnerWalletAsync(int ownerId);
    public Task<ApiResponse<bool>> RequestWithdrawalAsync(WithdrawRequestDTO request, int ownerId);
    #endregion
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
    private readonly IServiceRepository _serviceRespository;
    private readonly IMemoryCache _cache;
    private readonly JwtAuthService _jwtAuthService;
    private readonly IFileUploadService _uploadService;
    private readonly IAuthorizationService _authService;
    private readonly IEmailService _emailService;
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
        IAuthorizationService authService,
        IServiceRepository serviceRepository,
        IEmailService emailService
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
        _serviceRespository = serviceRepository;
        _emailService = emailService;
    }

    public async Task<ApiResponse<List<HotelRevenueRankDTO>>> GetTopRevenueHotelsAsync(int year, int top = 10)
    {
        try
        {
            // Logic: Tính toán dựa trên bảng Bookings (đã hoàn thành)
            // Vì WalletTransaction lưu số âm dương lẫn lộn khó group, nên dùng Bookings chuẩn hơn.

            var query = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == "Completed" && b.IsDeleted == false) // Chỉ tính đơn thành công
                .Where(b => b.CreatedAt.HasValue && b.CreatedAt.Value.Year == year)
                .GroupBy(b => b.HotelId)
                .Select(g => new
                {
                    HotelId = g.Key,
                    TotalRevenue = g.Sum(b => b.TotalPrice), // GMV
                    TotalBookings = g.Count(),
                    // Giả sử phí sàn là 10% (0.1). Nếu bạn lưu phí cứng trong Booking thì sum cột đó.
                    // Ở đây mình tính tạm theo công thức chung hoặc join transaction nếu cần chính xác tuyệt đối.
                    EstimatedCommission = g.Sum(b => b.TotalPrice) * 0.1m
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(top)
                .ToListAsync();

            // Lấy thông tin chi tiết Hotel và Owner
            var hotelIds = query.Select(x => x.HotelId).ToList();
            var hotelsInfo = await _context.Hotels
                .Include(h => h.Owner)
                .Where(h => hotelIds.Contains(h.Id))
                .ToDictionaryAsync(h => h.Id);

            var result = query.Select(item =>
            {
                var hotel = hotelsInfo.GetValueOrDefault(item.HotelId);
                return new HotelRevenueRankDTO
                {
                    HotelId = item.HotelId,
                    HotelName = hotel?.Name ?? "Unknown",
                    OwnerName = hotel?.Owner?.FullName ?? "Unknown",
                    TotalRevenue = item.TotalRevenue,
                    CommissionPaid = item.EstimatedCommission,
                    TotalBookings = item.TotalBookings
                };
            }).ToList();

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<HotelRevenueRankDTO>>(ex.Message);
        }
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

        // 2. Query bằng LINQ (An toàn & Chính xác hơn SP)
        var hotels = await _context.Hotels
            .AsNoTracking()
            .Where(h => h.IsDeleted == false && h.Status == "Active") // Chỉ lấy khách sạn đang hoạt động
                                                                      // Sắp xếp theo Rating giảm dần (tính trung bình)
            .OrderByDescending(h => h.Reviews.Any() ? h.Reviews.Average(r => r.Rating) : 0)
            .Take(10)
            .Select(h => new HotelListItemDTO
            {
                HotelId = h.Id,
                HotelName = h.Name,
                Address = h.Address,
                City = h.City.Name,
                // Country = h.Country.Name,
                CoverImageUrl = h.CoverImageUrl,

                // --- FIX: LẤY DANH SÁCH ẢNH TỪ BẢNG HOTELIMAGES ---
                ImageUrls = h.HotelImages
                    .Where(hi => hi.IsDeleted == false)
                    .OrderBy(hi => hi.SortOrder)
                    .Select(hi => hi.ImageUrl)
                    .Take(5) // Lấy 5 ảnh để slide
                    .ToList(),
                // --------------------------------------------------

                HighlightAmenities = h.HotelAmenities
                    .Where(ha => ha.Amenity.IsDeleted == false)
                    .OrderByDescending(ha => ha.Amenity.IsFilterable)
                    .Select(ha => new AmenityDTO
                    {
                        Id = ha.AmenityId,
                        Name = ha.Amenity.Name,
                        Additional = ha.Amenity.Additional
                    })
                    .Take(3).ToList(),

                MinPricePerNight = h.RoomTypes.Where(rt => rt.IsDeleted == false).Min(rt => (decimal?)rt.PricePerNight) ?? 0,
                MaxPricePerNight = h.RoomTypes.Where(rt => rt.IsDeleted == false).Max(rt => (decimal?)rt.PricePerNight) ?? 0,

                AverageRating = h.Reviews.Any(r => r.IsDeleted == false)
                    ? Math.Round((decimal)h.Reviews.Where(r => r.IsDeleted == false).Average(r => r.Rating ?? 0), 1)
                    : 0m,
                ReviewCount = h.Reviews.Count(r => r.IsDeleted == false),
                IsWishlist = wishlistIds.Contains(h.Id),
                AvailableRooms = h.RoomTypes
                    .SelectMany(rt => rt.Rooms)
                    .Count(r => r.Status == "Available" && r.IsDeleted == false),
                IsBookable = h.RoomTypes.Any(rt => rt.Rooms.Any(r => r.Status == "Available" && r.IsDeleted == false)) // Logic đơn giản
            })
            .ToListAsync();

        return hotels;
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

            var hotels = await _context.Hotels
            .AsNoTracking()
            .Where(h => h.AccommodationTypeId == accTypeId && h.IsDeleted == false && h.Status == "Active")
            .Select(h => new HotelListItemDTO
            {
                HotelId = h.Id,
                HotelName = h.Name,
                Address = h.Address,
                City = h.City.Name,
                // Country = h.Country.Name,
                CoverImageUrl = h.CoverImageUrl,
                AccommodationType = h.AccommodationType.Name,

                // --- FIX ẢNH ---
                ImageUrls = h.HotelImages
                    .Where(hi => hi.IsDeleted == false)
                    .OrderBy(hi => hi.SortOrder)
                    .Select(hi => hi.ImageUrl)
                    .Take(5)
                    .ToList(),

                HighlightAmenities = h.HotelAmenities
                    .Where(ha => ha.Amenity.IsDeleted == false)
                    .Select(ha => new AmenityDTO { Name = ha.Amenity.Name })
                    .Take(3).ToList(),

                MinPricePerNight = h.RoomTypes.Where(rt => rt.IsDeleted == false).Min(rt => (decimal?)rt.PricePerNight),
                MaxPricePerNight = h.RoomTypes.Where(rt => rt.IsDeleted == false).Max(rt => (decimal?)rt.PricePerNight),

                AverageRating = h.Reviews.Any(r => r.IsDeleted == false)
                    ? Math.Round((decimal)h.Reviews.Where(r => r.IsDeleted == false).Average(r => r.Rating ?? 0), 1)
                    : 0m,
                ReviewCount = h.Reviews.Count(r => r.IsDeleted == false),
                IsWishlist = wishlistIds.Contains(h.Id),
                IsBookable = true
            })
            .ToListAsync();

            return ApiResponseHelper.Ok<IEnumerable<HotelListItemDTO>>(hotels);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<IEnumerable<HotelListItemDTO>>(ex.Message);
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

        // var cacheKey = $"HotelDetail_{hotelId}_{userId ?? 0}_{checkInDate:ddMMyyy}_{checkOutDate:ddMMyyy}";

        // if (_cache.TryGetValue(cacheKey, out HotelDetailDTO? cachedHotel) && cachedHotel != null)
        // {
        //     return cachedHotel;
        // }

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
            City = h.City.Name,     // [Nên thêm] Lấy tên thành phố
            // Country = h.Country.Name,
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

            // RoomTypes sẽ được xử lý riêng bên dưới

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
                    CustomerName = r.Customer.FullName,
                    CustomerAvatar = r.Customer.AvatarUrl,
                    Rating = (decimal)r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                    Reply = r.Reply
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

        // === XỬ LÝ POLICIES (System + Custom) ===
        var rawPolicies = await _context.HotelPolicies
            .AsNoTracking()
            .Where(hp => hp.HotelId == hotelId)
            .Include(hp => hp.Policy)
            .ToListAsync();
        // Xử lý Custom Policies
        var finalPolicies = new List<PolicyDTO>();
        foreach (var hp in rawPolicies)
        {
            if (hp.PolicyId > 0) // System Policy
            {
                finalPolicies.Add(new PolicyDTO
                {
                    Id = hp.PolicyId,
                    Name = hp.Policy.Name,
                    Description = hp.Policy.Description,
                    IsSystemPolicy = true
                });
            }
            else if (!string.IsNullOrEmpty(hp.Additional)) // Custom Policy
            {
                try
                {
                    // Parse JSON để lấy Name & Description
                    var custom = JsonSerializer.Deserialize<OwnerCustomPolicyDTO>(hp.Additional);
                    if (custom != null)
                    {
                        finalPolicies.Add(new PolicyDTO
                        {
                            Id = 0, // Đánh dấu là custom
                            Name = custom.Name,
                            Description = custom.Description,
                            IsSystemPolicy = false
                        });
                    }
                }
                catch { }
            }
        }
        hotel.Policies = finalPolicies;

        // === LẤY DANH SÁCH ROOMTYPE + TÍNH SỐ PHÒNG TRỐNG RIÊNG CHO TỪNG LOẠI ===
        var roomTypes = await _context.RoomTypes
            .AsNoTracking()
            .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
            .Include(rt => rt.RoomImages)
            .Include(rt => rt.RoomBedTypes).ThenInclude(rb => rb.BedType)
            .Include(rt => rt.RoomViewTypes).ThenInclude(rv => rv.ViewType)
            .Include(rt => rt.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Include(rt => rt.RoomTypeServices).ThenInclude(rts => rts.Service)
            .Select(rt => new RoomTypeDTO
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description,
                PricePerNight = rt.PricePerNight,
                AdultCapacity = rt.AdultCapacity,
                ChildCapacity = rt.ChildCapacity,
                Area = rt.Area ?? 0,

                IsFreeCancellation = rt.IsFreeCancellation,
                IsBreakfastIncluded = rt.IsBreakfastIncluded,

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

                Beds = rt.RoomBedTypes.Select(rb => new RoomBedTypeDTO
                {
                    BedTypeName = rb.BedType.Name,
                    Quantity = rb.Quantity,
                }).ToList(),

                Views = rt.RoomViewTypes.Select(rv => new RoomViewTypeDTO
                {
                    ViewTypeName = rv.ViewType.Name,
                }).ToList(),

                RoomTypeServices = rt.RoomTypeServices
                    .Where(rts => rts.Service.IsDeleted == false)
                    .Select(rts => new RoomTypeServiceDTO
                    {
                        ServiceId = rts.ServiceId,
                        ServiceName = rts.Service.Name, // Lấy tên dịch vụ từ bảng Service
                        Quantity = rts.Quantity ?? 1,   // Số lượng (mặc định 1)
                        Note = rts.Note                 // Ghi chú
                    })
                .ToList(),

            })
            .ToListAsync();

        // Tính AvailableRooms cho từng RoomType (realtime theo ngày)
        var roomTypeIds = roomTypes.Select(rt => rt.Id).ToList();
        var availableCounts = await _context.Rooms
            .Where(r => roomTypeIds.Contains(r.RoomTypeId)
                     && r.IsDeleted == false
                     && r.Status == "Available"
                     && !_context.BookingRooms.Any(br =>
                         br.RoomId == r.Id &&
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
            rt.Amenities = rt.Amenities.Select(a =>
            {
                var add = TryParseAdditional(a.Additional);
                a.IconClass = add.GetValueOrDefault("IconClass", "fa-bed");
                a.IconColor = add.GetValueOrDefault("IconColor", "#54a9ffff");
                return a;
            }).ToList();
        }

        hotel.RoomTypes = roomTypes;

        // === Xử lý Amenity Additional (IconClass, IconColor) ===
        hotel.Amenities = hotel.Amenities.Select(a =>
        {
            var additional = TryParseAdditional(a.Additional);
            a.IconClass = additional.GetValueOrDefault("IconClass", "fa-hotel");
            a.IconColor = additional.GetValueOrDefault("IconColor", "#54a9ffff");
            return a;
        }).ToList();


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


        // // Cache theo ngày
        // _cache.Set(cacheKey, hotel, TimeSpan.FromMinutes(10));

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
    public async Task<int> CheckAvailableRoomsAsync(int hotelId, DateOnly checkIn, DateOnly checkOut, int? roomTypeId = null, int adults = 1, int children = 0)
    {
        var result = await _context.Database
            .SqlQueryRaw<int>(
                "EXEC sp_CheckHotelAvailability @HotelId = {0}, @CheckIn = {1}, @CheckOut = {2}, @RoomTypeId = {3}, @Adults = {4}, @Children = {5}",
                hotelId, checkIn.ToDateTime(TimeOnly.MinValue), checkOut.ToDateTime(TimeOnly.MinValue), roomTypeId, adults, children)
            .ToListAsync();

        return result.FirstOrDefault();
    }


    public async Task<List<HotelListItemDTO>> GetSearchOptionsAsync(
        string? destination,
        DateTime? checkIn,
        DateTime? checkOut,
        int? adults,
        int? children,
        int? rooms,
        decimal? priceMin,
        decimal? priceMax,
        decimal? ratingMin,
        string? accommodationTypeIds,
        string? amenityIds,
        string? bedTypeIds,
        string? viewTypeIds,
        string? chainIds,
        string? policyIds,
        string? serviceIds,
        string? sortBy = "Recommended",
        int? userId = null)
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
            // destination = destination.ToLowerInvariant(); // chuyển sang chữ thường
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
            @"EXEC sp_SearchHotels 
                @destination={0}, 
                @CheckIn={1}, 
                @CheckOut={2}, 
                @Adults={3}, 
                @Children={4}, 
                @Rooms={5}, 
                @PriceMin = {6}, 
                @PriceMax = {7},  
                @RatingMin = {8}, 
                @AccommodationTypeIds = {9},  
                @AmenityIds = {10}, 
                @BedTypeIds={11}, 
                @ViewTypeIds={12},
                @ChainIds={13}, 
                @PolicyIds={14},
                @ServiceIds={15},
                @SortBy = {16}",
            destination!,
            checkIn.Value,
            checkOut.Value,
            adults.Value,
            children.Value,
            rooms.Value,
            priceMin ?? (object)DBNull.Value,
            priceMax ?? (object)DBNull.Value,
            ratingMin ?? (object)DBNull.Value,
            accommodationTypeIds ?? (object)DBNull.Value,
            amenityIds ?? (object)DBNull.Value,
            bedTypeIds ?? (object)DBNull.Value, // BedTypeIds
            viewTypeIds ?? (object)DBNull.Value, // ViewTypeIds
            chainIds ?? (object)DBNull.Value, // ChainIds
            policyIds ?? (object)DBNull.Value, // PolicyIds
            serviceIds ?? (object)DBNull.Value,
            sortBy!)
        .ToListAsync();

        if (!hotelRes.Any()) return new List<HotelListItemDTO>();

        // 4. --- LOGIC HYBRID: Lấy ảnh và Amenities từ EF Core ---
        var hotelIds = hotelRes.Select(h => h.HotelId).ToList();

        // Lấy Ảnh
        var imagesDict = await _context.HotelImages
            .AsNoTracking()
            .Where(hi => hotelIds.Contains(hi.HotelId) && hi.IsDeleted == false)
            .OrderBy(hi => hi.SortOrder)
            .GroupBy(hi => hi.HotelId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.ImageUrl).Take(5).ToList());

        // Lấy Amenities (3 cái nổi bật)
        // 5. Map dữ liệu trả về
        return hotelRes.Select(h =>
        {
            var galleryImages = imagesDict.ContainsKey(h.HotelId) ? imagesDict[h.HotelId] : new List<string>();
            return new HotelListItemDTO
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                Address = h.Address,
                City = h.CityName,
                Country = h.CountryName,

                // Lấy từ Dictionary đã query ở bước 4 (Nhanh và Chuẩn)
                CoverImageUrl = !string.IsNullOrEmpty(h.CoverImageUrl)
                        ? h.CoverImageUrl
                        : galleryImages.FirstOrDefault(),
                ImageUrls = galleryImages,
                // Lấy từ Dictionary hoặc fallback
                HighlightAmenities = string.IsNullOrEmpty(h.Amenities)
                ? new List<AmenityDTO>()
                : JsonSerializer.Deserialize<List<AmenityDTO>>(h.Amenities)!
                    .Take(3).ToList(),

                MinPricePerNight = h.MinPrice,
                MaxPricePerNight = h.MaxPrice,
                AvgPricePerNight = h.AvgPrice,

                AvailableRooms = h.AvailableRooms,
                IsBookable = h.AvailableRooms >= rooms.Value,

                AverageRating = h.AvgRating > 0 ? Math.Round((decimal)h.AvgRating, 1) : 0m,
                ReviewCount = h.ReviewCount,

                AccommodationType = string.IsNullOrWhiteSpace(h.AccommodationType) ? "Khác" : h.AccommodationType.Trim(),
                ChainName = h.ChainName?.Trim() ?? string.Empty,

                IsWishlist = wishlistIds.Contains(h.HotelId)
            };
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
        return await GetSearchOptionsAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, userId);
    }

    #region Admin Dashboard
    public async Task<ApiResponse<AdminDashboardStatsDTO>> GetAdminDashboardStatsAsync(int adminId)
    {
        try
        {
            // 1. Vẫn giữ bảo mật ở BE
            if (!await _authService.IsAdminAsync(adminId))
            {
                return ApiResponseHelper.Forbidden<AdminDashboardStatsDTO>("Không có quyền truy cập.");
            }

            // 2. Gọi SP duy nhất - Trả về 1 dòng duy nhất chứa tất cả các trường
            // Sử dụng SqlQueryRaw để map thẳng vào DTO mới đã bổ sung trường
            var result = await _context.Database
                .SqlQueryRaw<AdminDashboardStatsDTO>("EXEC sp_GetAdminDashboardStats")
                .ToListAsync();

            var data = result.FirstOrDefault();

            if (data == null)
                return ApiResponseHelper.NotFound<AdminDashboardStatsDTO>("Không có dữ liệu thống kê.");

            // 3. Trả về dữ liệu
            return ApiResponseHelper.Ok(data);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<AdminDashboardStatsDTO>($"Lỗi Dashboard: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AdminRevenueDTO>> GetPlatformRevenueStatsAsync(int year)
    {
        try
        {
            // 1. Lấy tất cả giao dịch là Commission
            var query = _context.WalletTransactions
                .AsNoTracking()
                .Where(t => t.TransactionType == "CommissionFee");

            // 2. Tổng trọn đời 
            var netTotal = await query.SumAsync(t => t.Amount);
            var totalLifetime = await query.SumAsync(t => t.Amount);

            // 3. Tháng này
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var thisMonth = await query
                .Where(t => t.CreatedAt >= startOfMonth)
                .SumAsync(t => t.Amount);

            // 4. Biểu đồ theo tháng (Của năm được chọn)
            var monthlyStats = await query
                .Where(t => t.CreatedAt.HasValue && t.CreatedAt.Value.Year == year)
                .GroupBy(t => t.CreatedAt.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            // Fill đủ 12 tháng (kể cả tháng 0đ)
            var chartData = Enumerable.Range(1, 12).Select(month => new RevenueChartDTO
            {
                Label = $"Tháng {month}",
                Value = Math.Abs(monthlyStats.FirstOrDefault(m => m.Month == month)?.Amount ?? 0)
            }).ToList();

            // 5. Giao dịch gần đây (Join bảng Booking/Hotel để biết tiền từ đâu)
            // Phần này bạn có thể làm thêm nếu muốn hiện bảng chi tiết bên dưới
            var recentTransactions = await _context.WalletTransactions
            .AsNoTracking()
            .Where(t => t.TransactionType == "CommissionFee")
            .OrderByDescending(t => t.CreatedAt)
            .Take(10) // Lấy 10 giao dịch mới nhất
            .Select(t => new WalletTransactionDTO
            {
                // Vì WalletTransaction không trực tiếp link Hotel, ta dùng subquery hoặc join thủ công
                // Cách an toàn nhất trong EF Core với cấu trúc của bạn:
                Description = _context.Bookings
                    .Where(b => b.Id == t.ReferenceId)
                    .Select(b => $"{b.Hotel.Name} - Đơn #{b.Id} ({t.TransactionType})")
                    .FirstOrDefault() ?? t.Description, // Fallback nếu ko tìm thấy

                Amount = Math.Abs(t.Amount), // Hiển thị số dương cho Admin vui mắt
                Type = t.TransactionType == "CommissionReversal" ? "Hoàn phí" : "Thu phí",
                Date = t.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync();

            return ApiResponseHelper.Ok(new AdminRevenueDTO
            {
                TotalLifetimeRevenue = Math.Abs(totalLifetime),
                ThisMonthRevenue = Math.Abs(thisMonth),
                MonthlyData = chartData,
                RecentTransactions = recentTransactions
            });
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<AdminRevenueDTO>(ex.Message);
        }
    }
    #endregion

    #region Admin manage user/owner
    public async Task<ApiResponse<PagedResult<CustomerDTO>>> GetCustomersForAdminAsync(string? keyword, int pageIndex, int pageSize, int adminId)
    {
        try
        {
            // 1. Check quyền Admin
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<PagedResult<CustomerDTO>>();

            // 2. Query cơ bản
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.IsDeleted == false)
                // Lọc chỉ lấy Role Customer
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer"));

            // 3. Tìm kiếm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(k) ||
                                         u.Email.ToLower().Contains(k) ||
                                         u.PhoneNumber.Contains(k));
            }

            var totalCount = await query.CountAsync();

            // 4. Projection (Map sang DTO & Tính toán Aggregate)
            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CustomerDTO
                {
                    Id = u.Id,
                    FullName = u.FullName ?? "N/A",
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    AvatarUrl = u.AvatarUrl,
                    IsActive = u.IsActive ?? true,
                    CreatedAt = u.CreatedAt ?? DateTime.MinValue,

                    // Subquery tính tổng đơn và tổng tiền (EF Core sẽ dịch thành SQL sub-select tối ưu)
                    TotalBookings = _context.Bookings.Count(b => b.CustomerId == u.Id && b.Status == "Completed"),
                    TotalSpent = _context.Bookings
                        .Where(b => b.CustomerId == u.Id && b.Status == "Completed")
                        .Sum(b => b.TotalPrice)
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(new PagedResult<CustomerDTO>
            {
                Items = items,
                TotalRecords = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PagedResult<CustomerDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<PagedResult<OwnerDTO>>> GetOwnersForAdminAsync(string? keyword, int pageIndex, int pageSize, int adminId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<PagedResult<OwnerDTO>>();

            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.IsDeleted == false)
                // Lọc chỉ lấy Role Owner
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Owner"));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(k) ||
                                         u.Email.ToLower().Contains(k) ||
                                         u.TaxCode.Contains(k));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new OwnerDTO
                {
                    Id = u.Id,
                    FullName = u.FullName ?? "N/A",
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    TaxCode = u.TaxCode, // Trường riêng của Owner
                    IsActive = u.IsActive ?? true,
                    CreatedAt = u.CreatedAt ?? DateTime.MinValue,

                    // Đếm số khách sạn (Không tính cái đã xóa)
                    TotalHotels = _context.Hotels.Count(h => h.OwnerId == u.Id && h.IsDeleted == false),

                    // Lấy số dư ví (Dùng FirstOrDefault vì Left Join - có thể chưa có ví)
                    WalletBalance = _context.OwnerWallets
                        .Where(w => w.OwnerId == u.Id)
                        .Select(w => w.Balance)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(new PagedResult<OwnerDTO>
            {
                Items = items,
                TotalRecords = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PagedResult<OwnerDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<UserDetailDTO>> GetUserDetailForAdminAsync(int targetUserId, int adminId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<UserDetailDTO>();

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role) // Join lấy Role
                .FirstOrDefaultAsync(u => u.Id == targetUserId);

            if (user == null)
                return ApiResponseHelper.NotFound<UserDetailDTO>("Người dùng không tồn tại.");

            var roleNames = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            var detail = new UserDetailDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName ?? "",
                Address = user.Address,
                TaxCode = user.TaxCode,
                DateOfBirth = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue), // Convert DateOnly -> DateTime
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive ?? false,
                IsDeleted = user.IsDeleted ?? false,
                CreatedAt = user.CreatedAt ?? DateTime.MinValue,
                // Lấy danh sách tên Role (VD: ["Owner", "Customer"])
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            if (roleNames.Contains("Owner"))
            {
                detail.TaxCode = user.TaxCode; // TaxCode nằm trong bảng User nên map luôn

                // Lấy ví (chưa chắc đã có ví nếu chưa có booking nào, nên dùng FirstOrDefault)
                var wallet = await _context.OwnerWallets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(w => w.OwnerId == user.Id);

                detail.WalletBalance = wallet?.Balance ?? 0;

                // Đếm số khách sạn
                detail.OwnedHotelsCount = await _context.Hotels
                    .CountAsync(h => h.OwnerId == user.Id && h.IsDeleted == false);
            }

            if (roleNames.Contains("Customer"))
            {
                // Thống kê nhanh (Booking đã hoàn thành)
                var stats = await _context.Bookings
                    .Where(b => b.CustomerId == user.Id && b.Status == "Completed")
                    .GroupBy(b => b.CustomerId)
                    .Select(g => new
                    {
                        Count = g.Count(),
                        Total = g.Sum(x => x.TotalPrice)
                    })
                    .FirstOrDefaultAsync();

                detail.TotalBookings = stats?.Count ?? 0;
                detail.TotalSpent = stats?.Total ?? 0;
            }

            return ApiResponseHelper.Ok(detail);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<UserDetailDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> ToggleUserStatusAsync(int userId, int adminId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<bool>();

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted == true)
                return ApiResponseHelper.NotFound<bool>("Người dùng không tồn tại.");

            // Không cho phép Admin tự khóa chính mình (Optional)
            if (user.Id == adminId)
                return ApiResponseHelper.BadRequest<bool>("Không thể tự khóa tài khoản của mình.");

            // Đảo ngược trạng thái
            bool currentStatus = user.IsActive ?? true;
            user.IsActive = !currentStatus;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = adminId;

            await _context.SaveChangesAsync();

            string msg = user.IsActive.Value ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.";
            return ApiResponseHelper.Ok(user.IsActive.Value, msg);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }
    #endregion


    #region HOTELS OWNER
    // Thống kê dashboard cho Owner
    public async Task<ApiResponse<OwnerDashboardStatsDTO>> GetOwnerDashboardStatsAsync(int hotelId, int ownerId)
    {
        try
        {
            // 1. Validate: Khách sạn có thuộc về Owner này không?
            var hotel = await _context.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == hotelId && h.OwnerId == ownerId && h.IsDeleted == false);

            if (hotel == null)
            {
                return ApiResponseHelper.Forbidden<OwnerDashboardStatsDTO>("Khách sạn không tồn tại hoặc bạn không có quyền truy cập.");
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // 1. Lấy danh sách HotelId của Owner này
            var bookingsQuery = _context.Bookings
            .AsNoTracking()
            .Where(b => b.HotelId == hotelId && b.IsDeleted == false);

            // 2. Tính toán (Query tối ưu)

            // Doanh thu (Chỉ tính đơn đã thanh toán/hoàn tất)
            var totalRevenue = await bookingsQuery
                .Where(b => b.Status == "Completed" || b.Status == "Confirmed")
                .Where(b => b.CreatedAt >= DateTime.MinValue)
                .SumAsync(b => b.TotalPrice);

            var revenueToday = await bookingsQuery
            .Where(b => (b.Status == "Completed" || b.Status == "Confirmed")
                     && b.CreatedAt.Value.Date == DateTime.Today)
            .SumAsync(b => b.TotalPrice);

            var revenueMonth = await bookingsQuery
                .Where(b => (b.Status == "Completed" || b.Status == "Confirmed")
                         && b.CreatedAt >= startOfMonth)
                .SumAsync(b => b.TotalPrice);

            // Vận hành
            var newBookingsToday = await bookingsQuery
            .CountAsync(b => b.CreatedAt.Value.Date == DateTime.Today);

            var checkInsToday = await _context.Bookings
                .CountAsync(b => b.CheckInDate == today && (b.Status == "Confirmed" || b.Status == "PendingPayment"));

            var checkOutsToday = await bookingsQuery
            .CountAsync(b => b.CheckOutDate == today && b.Status == "CheckedIn");

            // Phòng ốc
            var totalRooms = await _context.Rooms
            .CountAsync(r => r.RoomType.HotelId == hotelId && r.IsDeleted == false && r.Status != "OutOfOrder");

            // Tính Occupied
            var occupiedRooms = await _context.BookingRooms
                .CountAsync(br => br.Booking.HotelId == hotelId
                              && br.Booking.IsDeleted == false
                              && (br.Booking.Status == "CheckedIn" || br.Booking.Status == "Confirmed")
                              && br.Booking.CheckInDate <= today
                              && br.Booking.CheckOutDate > today);

            var reviewsQuery = _context.Reviews.AsNoTracking()
            .Where(r => r.HotelId == hotelId && r.IsDeleted == false);

            var totalReviews = await reviewsQuery.CountAsync();
            var avgRating = totalReviews > 0
                ? await reviewsQuery.AverageAsync(r => (double?)r.Rating) ?? 0
                : 0;

            var recentBookings = await bookingsQuery
                .OrderByDescending(b => b.CreatedAt) // Sắp xếp mới nhất lên đầu
                .Take(5) // Lấy 10 đơn
                .Include(b => b.Customer) // Join User (nếu có)
                .Include(b => b.BookingRooms).ThenInclude(br => br.RoomType) // Lấy tên phòng
                .Select(b => new BookingShortDTO
                {
                    Id = b.Id,
                    GuestName = !string.IsNullOrEmpty(b.ContactName) ? b.ContactName : (b.Customer.FullName ?? "Khách vãng lai"),

                    TotalPrice = b.TotalPrice,
                    Status = b.Status, // Confirmed, CheckedIn, Completed...
                    CreatedAt = b.CreatedAt ?? DateTime.Now,
                    CheckInDate = b.CheckInDate.ToDateTime(TimeOnly.MinValue),   // Map ngày
                    CheckOutDate = b.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                    // Nối tên các loại phòng lại (nếu đặt nhiều phòng)
                    RoomTypeNames = string.Join(", ", b.BookingRooms.Select(br => br.RoomType.Name).Distinct())
                })
            .ToListAsync();

            var todayCheckInsList = await bookingsQuery
            .Where(b => b.CheckInDate == today && (b.Status == "Confirmed" || b.Status == "PendingPayment"))
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms).ThenInclude(br => br.RoomType)
            .Select(b => new BookingShortDTO
            {
                Id = b.Id,
                GuestName = !string.IsNullOrEmpty(b.ContactName) ? b.ContactName : (b.Customer.FullName ?? "Khách vãng lai"),
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                CreatedAt = b.CreatedAt ?? DateTime.Now,
                CheckInDate = b.CheckInDate.ToDateTime(TimeOnly.MinValue),
                CheckOutDate = b.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                RoomTypeNames = string.Join(", ", b.BookingRooms.Select(br => br.RoomType.Name).Distinct())
            })
            .ToListAsync();

            // C. Khách ĐI hôm nay (CheckOut == today)
            var todayCheckOutsList = await bookingsQuery
                .Where(b => b.CheckOutDate == today && b.Status == "CheckedIn")
                .Include(b => b.Customer)
                .Include(b => b.BookingRooms).ThenInclude(br => br.RoomType)
                .Select(b => new BookingShortDTO
                {
                    Id = b.Id,
                    GuestName = !string.IsNullOrEmpty(b.ContactName) ? b.ContactName : (b.Customer.FullName ?? "Khách vãng lai"),
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt ?? DateTime.Now,
                    CheckInDate = b.CheckInDate.ToDateTime(TimeOnly.MinValue),
                    CheckOutDate = b.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                    RoomTypeNames = string.Join(", ", b.BookingRooms.Select(br => br.RoomType.Name).Distinct())
                })
                .ToListAsync();

            var result = new OwnerDashboardStatsDTO
            {
                RevenueToday = revenueToday,
                RevenueThisMonth = revenueMonth,
                TotalRevenue = totalRevenue,

                NewBookingsToday = newBookingsToday,
                CheckInsToday = checkInsToday,
                CheckOutsToday = checkOutsToday,

                TotalRooms = totalRooms,
                AvailableRooms = Math.Max(0, totalRooms - occupiedRooms),
                OccupiedRooms = occupiedRooms,

                TotalReviews = totalReviews,
                AverageRating = Math.Round((double)avgRating, 1),
                RecentBookings = recentBookings,
                TodayCheckIns = todayCheckInsList,
                TodayCheckOuts = todayCheckOutsList
            };
            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<OwnerDashboardStatsDTO>(ex.Message);
        }

    }

    public async Task<ApiResponse<List<RevenueChartDTO>>> GetRevenueChartAsync(int hotelId, int ownerId, string viewType)
    {
        try
        {
            // Gọi SP vừa tạo
            var data = await _context.Database
                .SqlQueryRaw<RevenueChartDTO>(
                    "EXEC sp_GetRevenueChartData @HotelId={0}, @OwnerId={1}, @ViewType={2}",
                    hotelId, ownerId, viewType)
                .ToListAsync();

            return ApiResponseHelper.Ok(data);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<RevenueChartDTO>>(ex.Message);
        }
    }

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
            if (hotel == null || hotel.IsDeleted == true || hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<HotelDraftFullDTO>("Không tồn tại hoặc không thuộc về bạn.");

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
            var systemPolicies = await _context.HotelPolicies
                .Where(hp => hp.HotelId == hotelId && hp.PolicyId > 0)
                .Include(hp => hp.Policy)
                .Select(hp => new PolicyDTO
                {
                    Id = hp.PolicyId,
                    Name = hp.Policy.Name, // Lấy tên hiển thị
                    Description = hp.Policy.Description,
                    PolicyTypeId = hp.Policy.PolicyTypeId,
                    IsSystemPolicy = true
                })
                .ToListAsync();

            // Lấy custom policies dưới dạng JSON từ DB
            var customPolicyJsons = await _context.HotelPolicies
                .Where(hp => hp.HotelId == hotelId && hp.PolicyId == 0 && hp.Additional != null)
                .Select(hp => hp.Additional)
                .ToListAsync();

            // SAU ĐÓ MỚI deserialize ở C#
            var customPolicies = new List<OwnerCustomPolicyDTO>();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            foreach (var json in customPolicyJsons)
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        var policy = JsonSerializer.Deserialize<OwnerCustomPolicyDTO>(json, options);
                        if (policy != null)
                        {
                            customPolicies.Add(policy);
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Lỗi parse JSON custom policy: {json}");
                    }
                }
            }

            // === SERVICES ===
            var hotelServices = await _context.HotelServiceConfigs
                .Where(hs => hs.HotelId == hotelId)
                .Include(hs => hs.Service) // Join để lấy tên
                .Select(hs => new OwnerHotelServiceDTO
                {
                    Id = hs.Id,
                    HotelId = hs.HotelId,
                    ServiceId = hs.ServiceId,
                    ServiceName = hs.Service.Name,
                    Description = hs.Service.Description,
                    Price = hs.Price,
                    Unit = hs.Unit,
                    IsActive = hs.IsActive ?? true
                })
                .ToListAsync();

            // === ROOM TYPES ===
            var roomTypes = await _context.RoomTypes
                .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
                .Include(rt => rt.RoomImages) // Include ảnh nếu cần
                .OrderBy(rt => rt.SortOrder)
                .Select(rt => new RoomTypeForOwnerDTO
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    PricePerNight = rt.PricePerNight,
                    IsFreeCancellation = rt.IsFreeCancellation,
                    IsBreakfastIncluded = rt.IsBreakfastIncluded,
                    Quantity = rt.Quantity,
                    DefaultImageUrl = rt.DefaultImageUrl,
                    AdultCapacity = rt.AdultCapacity,
                    ChildCapacity = rt.ChildCapacity ?? 0,
                    Area = rt.Area ?? 0,
                    Description = rt.Description,
                    Amenities = rt.RoomAmenities
                        .Where(ra => ra.Amenity.IsDeleted == false)
                        .Select(ra => new AmenityDTO
                        {
                            Id = ra.Amenity.Id,
                            Name = ra.Amenity.Name,
                            Additional = ra.Amenity.Additional
                        }).ToList(),
                    RoomTypeServices = rt.RoomTypeServices
                        .Where(rts => rts.Service.IsDeleted == false)
                        .Select(rts => new RoomTypeServiceDTO
                        {
                            ServiceName = rts.Service.Name, // <--- LẤY TÊN Ở ĐÂY  
                        }).ToList()

                })
                .ToListAsync();
            var dto = new HotelDraftFullDTO
            {
                BasicInfo = basic,
                Images = images,
                SelectedAmenityIds = amenityIds,
                SelectedPolicyIds = systemPolicies
                    .Where(p => p.Id.HasValue)
                    .Select(p => p.Id!.Value)
                    .ToList(),
                SystemPolicies = systemPolicies,
                CustomPolicies = customPolicies,
                HotelServices = hotelServices,
                RoomTypes = roomTypes
            };
            if (!string.IsNullOrEmpty(hotel.Additional))
            {
                try
                {
                    var addData = JsonSerializer.Deserialize<Dictionary<string, object>>(hotel.Additional);
                    if (addData != null && addData.ContainsKey("RejectionReason"))
                    {
                        dto.RejectionReason = addData["RejectionReason"].ToString();
                    }
                }
                catch { }
            }
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

            // if (hotel.Status == "PendingVerification")
            // {
            //     // Cho phép sửa trực tiếp, nhưng đá về Draft
            //     hotel.Status = "Draft";
            //     // ... update các trường ...
            //     // Return: "Đã cập nhật thông tin. Khách sạn đã chuyển về trạng thái Nháp, vui lòng Gửi duyệt lại sau khi hoàn tất."
            // }
            // if (hotel.Status == "Active" || hotel.Status == "Verified" || hotel.Status == "PendingVerification")
            // {
            //     var existingReq = await _context.HotelUpdateRequests
            //         .FirstOrDefaultAsync(r => r.HotelId == hotelId && r.Status == "Pending");

            //     string jsonContent = JsonSerializer.Serialize(ownerHotel);

            //     if (existingReq != null)
            //     {
            //         existingReq.UpdateContentJson = jsonContent;
            //         existingReq.CreatedAt = DateTime.Now;
            //     }
            //     else
            //     {
            //         var req = new HotelUpdateRequest
            //         {
            //             HotelId = hotelId,
            //             OwnerId = ownerId,
            //             UpdateContentJson = jsonContent,
            //             Status = "Pending",
            //             CreatedAt = DateTime.Now
            //         };
            //         _context.HotelUpdateRequests.Add(req);
            //     }

            //     await _context.SaveChangesAsync();

            //     // --- FIX QUAN TRỌNG: PHẢI COMMIT TRƯỚC KHI RETURN ---
            //     await transaction.CommitAsync();
            //     // ----------------------------------------------------

            //     return ApiResponseHelper.Ok(new HotelResponseDTO
            //     {
            //         HotelId = hotel.Id,
            //         Name = hotel.Name,
            //         Status = hotel.Status,
            //         Message = "Yêu cầu chỉnh sửa đã được gửi tới Admin."
            //     });
            // }

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
            hotel.AccommodationTypeId = ownerHotel.AccommodationTypeId > 0 ? ownerHotel.AccommodationTypeId : null;
            hotel.ChainId = (ownerHotel.ChainId.HasValue && ownerHotel.ChainId.Value > 0)
                ? ownerHotel.ChainId
                : null;
            hotel.ContactName = ownerHotel.ContactName;
            hotel.ContactPhone = ownerHotel.ContactPhone;
            hotel.ContactEmail = ownerHotel.ContactEmail;
            hotel.UpdatedAt = DateTime.Now;

            if (hotel.Status == "Rejected")
            {
                hotel.Status = "Draft";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            // Xóa cache
            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            _cache.Remove(CacheKey.AmenitiesKey);
            return ApiResponseHelper.Ok(new HotelResponseDTO
            {
                HotelId = hotel.Id,
                Name = hotel.Name,
                Status = hotel.Status,
                Message = "Cập nhật thành công."
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

            // // CHỈ CHO PHÉP XÓA KHI KHÔNG ACTIVE
            // if (hotel.Status == "Active")
            //     return ApiResponseHelper.BadRequest<HotelResponseDTO>(
            //         "Không thể xóa khách sạn đang hoạt động. Vui lòng liên hệ admin.");

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

            // if (hotel.Status != "Draft" && hotel.Status != "Active")
            //     return ApiResponseHelper.BadRequest<bool>("Chỉ được chỉnh sửa khi khách sạn chưa gửi duyệt.");
            // Kiểm tra tên custom trùng
            var customNames = policyId.OwnerCustomPolicies?.Select(c => c.Name.Trim().ToLower()).ToList() ?? new List<string>();
            if (customNames.Count != customNames.Distinct().Count())
                return ApiResponseHelper.Conflict<bool>("Tên chính sách tùy chỉnh không được trùng nhau.");

            // Xóa tất cả policy cũ
            var oldPolicies = await _context.HotelPolicies
                .Where(x => x.HotelId == hotelId && x.PolicyId > 0)
                .ToListAsync();
            _context.HotelPolicies.RemoveRange(oldPolicies);

            // Thêm mới ( hệ thống )
            if (policyId.PolicyIds?.Any() == true)
            {
                var newSystemPolicies = policyId.PolicyIds.Distinct().Select(id => new HotelPolicy
                {
                    HotelId = hotelId,
                    PolicyId = id, // ID của Policy trong bảng Policies
                    Additional = null
                });

                await _context.HotelPolicies.AddRangeAsync(newSystemPolicies);
            }

            // 2. XỬ LÝ CUSTOM POLICIES
            var dbCustomPolicies = await _context.HotelPolicies
                .Where(x => x.HotelId == hotelId && x.PolicyId == 0)
                .ToListAsync();

            // 2. Lấy danh sách FE gửi lên (đảm bảo không null)
            var requestCustoms = policyId.OwnerCustomPolicies ?? new List<OwnerCustomPolicyDTO>();

            // --- A. DELETE: Những cái có trong DB mà KHÔNG có trong Request (dựa vào Id) ---
            // Lưu ý: FE phải gửi Id của HotelPolicy (chứ không phải PolicyId=0)
            var requestIds = requestCustoms.Where(x => x.Id > 0).Select(x => x.Id).ToList();
            var toDelete = dbCustomPolicies.Where(x => !requestIds.Contains(x.Id)).ToList();

            if (toDelete.Any())
            {
                _context.HotelPolicies.RemoveRange(toDelete);
            }

            // --- B. UPDATE: Những cái có ID trùng khớp ---
            foreach (var reqItem in requestCustoms.Where(x => x.Id > 0))
            {
                var dbItem = dbCustomPolicies.FirstOrDefault(x => x.Id == reqItem.Id);
                if (dbItem != null)
                {
                    // Parse JSON cũ để giữ lại các trường không đổi (nếu có)
                    // Hoặc ghi đè toàn bộ JSON mới
                    var jsonData = new
                    {
                        Name = reqItem.Name,
                        Description = reqItem.Description,
                        PolicyTypeId = reqItem.PolicyTypeId,
                        UpdatedAt = DateTime.Now // Tracking thời gian sửa
                    };
                    dbItem.Additional = JsonSerializer.Serialize(jsonData);
                    // dbItem.UpdatedBy = ownerId; // Nếu bảng có cột này
                }
            }

            // --- C. CREATE: Những cái Id = 0 ---
            var toAdd = requestCustoms.Where(x => x.Id == 0).ToList();
            foreach (var newItem in toAdd)
            {
                // Logic lấy tên Type giống code cũ của bạn
                string typeName = "Khác";
                if (newItem.PolicyTypeId > 0)
                {
                    var pt = await _policyTypeRepository.GetByIdAsync(newItem.PolicyTypeId);
                    if (pt != null) typeName = pt.TypeName;
                }

                var jsonData = new
                {
                    Name = newItem.Name,
                    Description = newItem.Description,
                    PolicyTypeId = newItem.PolicyTypeId,
                    PolicyTypeName = typeName,
                    CreatedBy = ownerId,
                    CreatedAt = DateTime.Now
                };

                await _context.HotelPolicies.AddAsync(new HotelPolicy
                {
                    HotelId = hotelId,
                    PolicyId = null, // Đánh dấu là Custom
                    Additional = JsonSerializer.Serialize(jsonData)
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            return ApiResponseHelper.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
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

            // Kiểm tra bắt buộc 
            if (string.IsNullOrWhiteSpace(hotel.Name) || string.IsNullOrWhiteSpace(hotel.Address))
                return ApiResponseHelper.BadRequest<bool>("Tên và địa chỉ không được để trống.");

            if (hotel.CityId <= 0)
                return ApiResponseHelper.BadRequest<bool>("Vui lòng chọn thành phố.");
            if (hotel.AccommodationTypeId == null || hotel.AccommodationTypeId <= 0)
                return ApiResponseHelper.BadRequest<bool>("Vui lòng chọn loại hình lưu trú (Khách sạn, Villa...).");

            if (string.IsNullOrEmpty(hotel.CoverImageUrl))
                return ApiResponseHelper.BadRequest<bool>("Vui lòng upload ảnh bìa.");

            if (!await _context.HotelImages.AnyAsync(x => x.HotelId == hotelId && x.IsDeleted == false))
                return ApiResponseHelper.BadRequest<bool>("Vui lòng upload ít nhất 1 ảnh phụ.");
            var hasAmenities = await _context.HotelAmenities.AnyAsync(x => x.HotelId == hotelId);
            if (!hasAmenities)
                return ApiResponseHelper.BadRequest<bool>("Vui lòng chọn ít nhất 1 tiện ích cho khách sạn.");
            // Kiểm tra xem đã có chính sách nào chưa (đặc biệt là nhóm Check-in/Check-out)
            var hasPolicies = await _context.HotelPolicies.AnyAsync(x => x.HotelId == hotelId);
            if (!hasPolicies)
                return ApiResponseHelper.BadRequest<bool>("Vui lòng thiết lập chính sách (Giờ nhận/trả phòng).");

            var roomTypes = await _context.RoomTypes
            .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
            .Include(rt => rt.RoomImages)
            .Include(rt => rt.RoomBedTypes) // Include để check ảnh phòng
            .ToListAsync();
            if (!roomTypes.Any())
                return ApiResponseHelper.BadRequest<bool>("Vui lòng tạo ít nhất 1 loại phòng.");
            foreach (var rt in roomTypes)
            {
                if (rt.PricePerNight <= 0) return ApiResponseHelper.BadRequest<bool>($"Phòng '{rt.Name}': Giá chưa hợp lệ.");

                // CHECK AREA
                if ((rt.Area ?? 0) <= 0) return ApiResponseHelper.BadRequest<bool>($"Phòng '{rt.Name}': Vui lòng nhập diện tích (m2).");

                // CHECK BEDS
                if (!rt.RoomBedTypes.Any()) return ApiResponseHelper.BadRequest<bool>($"Phòng '{rt.Name}': Chưa cấu hình loại giường.");

                if (!rt.RoomImages.Any(ri => ri.IsDeleted == false)) return ApiResponseHelper.BadRequest<bool>($"Phòng '{rt.Name}': Chưa có hình ảnh.");

                // Check số lượng phòng vật lý (Optional: Nếu logic của bạn yêu cầu phải có phòng vật lý mới bán được)
                // var roomCount = await _context.Rooms.CountAsync(r => r.RoomTypeId == rt.Id && !r.IsDeleted);
                // if (roomCount < rt.Quantity) ...
            }

            // Lấy tất cả ServiceId được gán vào các loại phòng (Inclusions)
            var includedServiceIds = await _context.RoomTypeServices
                .Where(rts => rts.RoomType.HotelId == hotelId) // Join nhẹ qua RoomType để lấy HotelId
                .Select(rts => rts.ServiceId)
                .Distinct()
                .ToListAsync();

            if (includedServiceIds.Any())
            {
                // Lấy danh sách ServiceId mà khách sạn đang kinh doanh (Active)
                var activeHotelServiceIds = await _context.HotelServiceConfigs
                    .Where(hs => hs.HotelId == hotelId && hs.IsActive == true)
                    .Select(hs => hs.ServiceId)
                    .ToListAsync();

                // Tìm những thằng "lạc loài" (Có trong phòng nhưng Hotel không cung cấp)
                var invalidServices = includedServiceIds.Except(activeHotelServiceIds).ToList();

                if (invalidServices.Any())
                {
                    // Lấy tên để báo lỗi cho chi tiết
                    var serviceNames = await _context.Services
                        .Where(s => invalidServices.Contains(s.Id))
                        .Select(s => s.Name)
                        .ToListAsync();

                    var strNames = string.Join(", ", serviceNames);
                    return ApiResponseHelper.BadRequest<bool>(
                        $"Lỗi logic: Một số loại phòng đang bao gồm dịch vụ '{strNames}' nhưng khách sạn chưa kích hoạt dịch vụ này. Vui lòng kiểm tra lại mục Dịch vụ.");
                }
            }

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
            .OrderByDescending(a => a.IsFilterable) // Đưa cái nào filter được lên đầu cho dễ nhìn
            .ThenBy(a => a.Name)
            .ToListAsync();

            var result = new List<AmenityDTO>();
            // query 1l tránh nhiều lần query
            var userIds = amenities.Where(a => a.CreatedBy.HasValue).Select(a => a.CreatedBy!.Value)
                          .Union(amenities.Where(a => a.UpdatedBy.HasValue).Select(a => a.UpdatedBy!.Value))
                          .Distinct().ToList();

            var users = await _context.Users.Where(u => userIds.Contains(u.Id))
                                      .ToDictionaryAsync(u => u.Id, u => u.FullName);

            foreach (var a in amenities)
            {
                AmenityAdditional? add = null;
                if (!string.IsNullOrWhiteSpace(a.Additional))
                {
                    add = JsonSerializer.Deserialize<AmenityAdditional>(a.Additional);
                }

                result.Add(new AmenityDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsFilterable = a.IsFilterable,
                    IconClass = add?.IconClass ?? "default-icon",
                    IconColor = add?.IconColor ?? "#54a9ffff",
                    Description = add?.Description,
                    CreatedByName = a.CreatedBy.HasValue && users.ContainsKey(a.CreatedBy.Value) ? users[a.CreatedBy.Value] : null,
                    UpdatedByName = a.UpdatedBy.HasValue && users.ContainsKey(a.UpdatedBy.Value) ? users[a.UpdatedBy.Value] : null,
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
                IsFilterable = false,
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
                IsFilterable = amenity.IsFilterable,
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


            await _amenityRepository.UpdateAsync(existingAmenity);
            await _dbu.SaveChangesAsync();
            _cache.Remove(CacheKey.AmenitiesKey);

            return new ApiResponse<AmenityDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.UPDATE_SUCCESSFULLY,
                Content = new AmenityDTO
                {
                    Id = existingAmenity.Id,
                    Name = existingAmenity.Name,
                    IconClass = amenity.IconClass,
                }
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

    public async Task<ApiResponse<bool>> ToggleFilterableAsync(int id, int? userId)
    {
        var amenity = await _amenityRepository.GetByIdAsync(id);
        if (amenity == null)
            return new ApiResponse<bool> { StatusCode = StatusCodeResponse.NotFound };

        // Đảo ngược giá trị hiện tại (True -> False, False -> True)
        amenity.IsFilterable = !amenity.IsFilterable;

        amenity.UpdatedBy = userId;
        amenity.UpdatedAt = DateTime.Now;

        await _amenityRepository.UpdateAsync(amenity);
        await _dbu.SaveChangesAsync();
        _cache.Remove(CacheKey.AmenitiesKey); // Nhớ xóa cache để trang chủ cập nhật ngay

        return new ApiResponse<bool>
        {
            StatusCode = StatusCodeResponse.Success,
            Message = "Cập nhật trạng thái bộ lọc thành công",
            Content = amenity.IsFilterable // Trả về trạng thái mới
        };
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
    public async Task<ApiResponse<RoomTypeForOwnerDTO>> GetRoomTypeDetailAsync(int roomTypeId)
    {
        try
        {

            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);


            var dto = new RoomTypeForOwnerDTO
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                PricePerNight = roomType.PricePerNight,
                IsFreeCancellation = roomType.IsFreeCancellation,
                IsBreakfastIncluded = roomType.IsBreakfastIncluded,
                AdultCapacity = roomType.AdultCapacity,
                ChildCapacity = roomType.ChildCapacity,
                Quantity = roomType.Quantity,
                AvailableRooms = await _context.Rooms.CountAsync(r => r.RoomTypeId == roomTypeId && r.Status == "Available" && r.IsDeleted == false),
                Area = roomType.Area,
                IsActive = roomType.IsActive,
                SortOrder = roomType.SortOrder,
                DefaultImageUrl = roomType.DefaultImageUrl,
                CreatedAt = roomType.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = roomType.UpdatedAt ?? DateTime.MinValue
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

            // load RoomtypeServices
            dto.RoomTypeServices = await _context.RoomTypeServices
                .Where(rs => rs.RoomTypeId == roomTypeId)
                .Include(rs => rs.Service) // Include bảng Service để lấy Tên
                .Select(rs => new RoomTypeServiceDTO
                {
                    ServiceId = rs.ServiceId,
                    ServiceName = rs.Service.Name,
                    // Icon = rs.Service.Icon, // Nếu có
                    Quantity = rs.Quantity ?? 1,
                    Note = rs.Note
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

            // Dùng EF Core thay vì SP để dễ lấy dữ liệu liên quan (Include)
            var roomTypes = await _context.RoomTypes
                .AsNoTracking()
                .AsSplitQuery()
                .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
                .Include(rt => rt.RoomTypeServices)
                .ThenInclude(rts => rts.Service)
                .Include(rt => rt.RoomImages)
                .OrderBy(rt => rt.SortOrder)
                .Select(rt => new RoomTypeForOwnerDTO
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    Description = rt.Description,
                    PricePerNight = rt.PricePerNight,

                    IsFreeCancellation = rt.IsFreeCancellation,
                    IsBreakfastIncluded = rt.IsBreakfastIncluded,
                    AdultCapacity = rt.AdultCapacity,
                    ChildCapacity = rt.ChildCapacity,
                    Quantity = rt.Quantity,
                    Area = rt.Area, // Đảm bảo lấy Area tại đây
                    IsActive = rt.IsActive,
                    SortOrder = rt.SortOrder ?? 0,
                    DefaultImageUrl = rt.DefaultImageUrl ??
                                  rt.RoomImages.Where(ri => ri.IsDeleted == false)
                                               .OrderBy(ri => ri.SortOrder)
                                               .Select(ri => ri.ImageUrl)
                                               .FirstOrDefault(),

                    // Lấy danh sách Giường
                    Beds = rt.RoomBedTypes.Select(rb => new RoomBedTypeDTO
                    {
                        BedTypeId = rb.BedTypeId,
                        BedTypeName = rb.BedType.Name,
                        Quantity = rb.Quantity
                    }).ToList(),

                    // Lấy danh sách Views
                    Views = rt.RoomViewTypes.Select(rv => new RoomViewTypeDTO
                    {
                        ViewTypeId = rv.ViewTypeId,
                        ViewTypeName = rv.ViewType.Name
                    }).ToList(),

                    RoomTypeServices = rt.RoomTypeServices.Select(rs => new RoomTypeServiceDTO
                    {
                        ServiceId = rs.ServiceId,
                        ServiceName = rs.Service.Name, // Cần đảm bảo EF Core load được tên Service
                        Quantity = rs.Quantity ?? 1
                    }).ToList(),
                    // Tính số phòng trống (nếu cần hiển thị luôn)
                    AvailableRooms = _context.Rooms.Count(r => r.RoomTypeId == rt.Id && r.Status == "Available" && r.IsDeleted == false)
                })
                .ToListAsync();

            if (!roomTypes.Any())
            {
                return ApiResponseHelper.Ok(new List<RoomTypeForOwnerDTO>(), "Chưa có loại phòng nào.");
            }

            return ApiResponseHelper.Ok(roomTypes);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<RoomTypeForOwnerDTO>>(ex.Message);
        }
    }


    public async Task<ApiResponse<WizardSubmitValidationDTO>> ValidateHotelForSubmitAsync(int hotelId, int ownerId)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            // if (hotel == null || hotel.OwnerId != ownerId || hotel.Status != "Draft")
            //     return ApiResponseHelper.NotFound<WizardSubmitValidationDTO>();

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

    private async Task SyncRoomBedsAsync(int roomTypeId, List<RoomBedRequestDTO>? beds)
    {
        var old = _context.RoomBedTypes.Where(x => x.RoomTypeId == roomTypeId);
        _context.RoomBedTypes.RemoveRange(old);

        if (beds != null && beds.Any())
        {
            // var bedIds = beds.Select(b => b.BedTypeId).Distinct().ToList();
            // var countValid = await _context.BedTypes.CountAsync(b => bedIds.Contains(b.Id) && b.IsDeleted == false);

            // if (countValid != bedIds.Count)
            //     throw new ArgumentException("Một số loại giường không hợp lệ.");

            // var selectBeds = beds.Select((b, index) => new RoomBedType
            // {
            //     RoomTypeId = roomTypeId,
            //     BedTypeId = b.BedTypeId,
            //     Quantity = b.Quantity,
            //     IsPrimary = index == 0
            // });
            // await _context.RoomBedTypes.AddRangeAsync(selectBeds);

            var validBeds = beds
            .Where(b => b.Quantity > 0)
            .GroupBy(b => b.BedTypeId) // Group lại nếu lỡ FE gửi trùng
            .Select(g => g.First())
            .ToList();

            var newEntities = validBeds.Select((b, index) => new RoomBedType
            {
                RoomTypeId = roomTypeId,
                BedTypeId = b.BedTypeId,
                Quantity = b.Quantity,
                IsPrimary = index == 0 // Cái đầu tiên mặc định là chính
            });

            await _context.RoomBedTypes.AddRangeAsync(newEntities);
        }
    }

    private async Task SyncRoomViewsAsync(int roomTypeId, List<int>? viewIds)
    {
        var old = _context.RoomViewTypes.Where(x => x.RoomTypeId == roomTypeId);
        _context.RoomViewTypes.RemoveRange(old);

        if (viewIds != null && viewIds.Any())
        {
            // Validate
            var distinctIds = viewIds.Distinct().ToList();
            var countValid = await _context.ViewTypes.CountAsync(v => distinctIds.Contains(v.Id) && v.IsDeleted == false);

            if (countValid != distinctIds.Count)
                throw new ArgumentException("Một số hướng nhìn không hợp lệ.");

            var selectView = distinctIds.Select(id => new RoomViewType
            {
                RoomTypeId = roomTypeId,
                ViewTypeId = id
            });
            await _context.RoomViewTypes.AddRangeAsync(selectView);
        }
    }

    private async Task SyncRoomAmenitiesAsync(int roomTypeId, List<int>? amenityIds)
    {
        var old = _context.RoomAmenities.Where(x => x.RoomTypeId == roomTypeId);
        _context.RoomAmenities.RemoveRange(old);

        if (amenityIds != null && amenityIds.Any())
        {
            var distinctIds = amenityIds.Distinct().ToList();
            var countValid = await _context.Amenities.CountAsync(a => distinctIds.Contains(a.Id) && a.IsDeleted == false);

            if (countValid != distinctIds.Count)
                throw new ArgumentException("Một số tiện ích không hợp lệ.");

            var newLinks = distinctIds.Select(id => new RoomAmenity
            {
                RoomTypeId = roomTypeId,
                AmenityId = id
            });
            await _context.RoomAmenities.AddRangeAsync(newLinks);
        }
    }

    private async Task SyncRoomServicesAsync(int roomTypeId, List<RoomTypeServiceRequestDTO>? roomTypeServices)
    {
        var old = _context.RoomTypeServices.Where(x => x.RoomTypeId == roomTypeId);
        _context.RoomTypeServices.RemoveRange(old);

        if (roomTypeServices != null && roomTypeServices.Any())
        {
            // 2. Validate xem ServiceId có tồn tại trong hệ thống không?
            var serviceIds = roomTypeServices.Select(s => s.ServiceId).Distinct().ToList();
            var countValid = await _context.Services.CountAsync(s => serviceIds.Contains(s.Id) && s.IsDeleted == false);
            if (countValid != serviceIds.Count)
                throw new ArgumentException("Một số dịch vụ không hợp lệ.");

            // 3. Map và Insert cái mới
            var newLinks = roomTypeServices.Select(dto => new RoomTypeService
            {
                RoomTypeId = roomTypeId,
                ServiceId = dto.ServiceId,
                Quantity = dto.Quantity > 0 ? dto.Quantity : 1, // Đảm bảo min là 1
                Note = dto.Note,
                // Các field audit
                // CreatedAt = DateTime.Now 
            });

            await _context.RoomTypeServices.AddRangeAsync(newLinks);
        }
    }


    private async Task AdjustPhysicalRoomsAsync(int roomTypeId, int currentQty, int newQty)
    {
        if (newQty > currentQty)
        {
            int countToAdd = newQty - currentQty;
            // Tăng số lượng: Thêm phòng mới
            // Tìm số phòng lớn nhất hiện tại để đặt tên tiếp theo (tránh trùng lặp P001, P001)
            var existingCount = await _context.Rooms.CountAsync(r => r.RoomTypeId == roomTypeId);

            // Logic sinh tên phòng đơn giản (có thể cải tiến sau)
            // Ví dụ: Tạo phòng "RT-{Id}-New-{i}" để Owner vào sửa sau
            var roomsToAdd = new List<Room>();
            for (int i = 1; i <= countToAdd; i++)
            {
                roomsToAdd.Add(new Room
                {
                    RoomTypeId = roomTypeId,
                    RoomNumber = $"New-{Guid.NewGuid().ToString().Substring(0, 4)}", // Tạm thời đặt tên theo ID loại phòng
                    Status = "Available",
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                });
            }
            await _context.Rooms.AddRangeAsync(roomsToAdd);
        }
        else if (newQty < currentQty)
        {
            int amountToDelete = currentQty - newQty;
            var today = DateOnly.FromDateTime(DateTime.Now);

            var safeRooms = await _context.Rooms
            .Where(r => r.RoomTypeId == roomTypeId && r.IsDeleted == false && r.Status == "Available")
            .Where(r => !_context.BookingRooms.Any(br =>
                br.RoomId == r.Id &&
                _context.Bookings.Any(b =>
                    b.Id == br.BookingId &&
                    b.Status != "Cancelled" && b.Status != "Completed" &&
                    b.CheckOutDate >= today // Còn hạn ở
                )
            ))
            .OrderByDescending(r => r.CreatedAt) // Xóa phòng mới tạo trước
            .Take(amountToDelete)
            .ToListAsync();

            if (safeRooms.Count < amountToDelete)
            {
                // Nếu không đủ phòng trống -> Chặn lại và báo lỗi
                throw new InvalidOperationException(
                    $"Bạn muốn giảm {amountToDelete} phòng, nhưng chỉ tìm thấy {safeRooms.Count} phòng trống không có lịch đặt. Vui lòng kiểm tra lại lịch booking.");
            }

            foreach (var room in safeRooms)
            {
                room.IsDeleted = true;
                room.UpdatedAt = DateTime.Now;
            }
        }

        await _context.SaveChangesAsync();
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

            // if (hotel.Status != "Draft" && hotel.Status != "Active")
            //     return ApiResponseHelper.BadRequest<CreateRoomTypeResponseDTO>("Chỉ thêm loại phòng khi khách sạn đang tạm ngưng hoạt động.");

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
                IsFreeCancellation = roomType.IsFreeCancellation ?? false,
                IsBreakfastIncluded = roomType.IsBreakfastIncluded ?? false,
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

            await _context.SaveChangesAsync(); // có id

            if (roomType.Beds != null) await SyncRoomBedsAsync(newRoomType.Id, roomType.Beds);
            if (roomType.ViewIds != null) await SyncRoomViewsAsync(newRoomType.Id, roomType.ViewIds);
            if (roomType.AmenityIds != null) await SyncRoomAmenitiesAsync(newRoomType.Id, roomType.AmenityIds);
            if (roomType.RoomTypeServices != null) await SyncRoomServicesAsync(newRoomType.Id, roomType.RoomTypeServices);

            await AdjustPhysicalRoomsAsync(newRoomType.Id, 0, roomType.Quantity.Value);
            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            _cache.Remove($"OwnerHotelDetail_{hotelId}_{ownerId}");
            _cache.Remove($"OwnerRoomTypes_{hotelId}_{ownerId}");
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

            // // 3. CHỈ CHO PHÉP EDIT KHI HOTEL DRAFT HOẶC ACTIVE
            // if (roomTypeHotel.Hotel.Status != "Draft" && roomTypeHotel.Hotel.Status != "Active")
            //     return ApiResponseHelper.BadRequest<RoomTypeForOwnerDTO>(
            //         "Không thể chỉnh sửa khi khách sạn đang chờ duyệt.");

            // 3. UPDATE THÔNG TIN CƠ BẢN (Dùng Null Coalescing cho gọn)
            if (roomType.Name != null) roomTypeHotel.Name = roomType.Name.Trim();
            if (roomType.Description != null) roomTypeHotel.Description = roomType.Description.Trim();
            if (roomType.PricePerNight.HasValue) roomTypeHotel.PricePerNight = roomType.PricePerNight.Value;
            if (roomType.AdultCapacity.HasValue) roomTypeHotel.AdultCapacity = roomType.AdultCapacity.Value;
            if (roomType.ChildCapacity.HasValue) roomTypeHotel.ChildCapacity = roomType.ChildCapacity.Value;
            if (roomType.Area.HasValue) roomTypeHotel.Area = roomType.Area.Value;

            if (roomType.IsFreeCancellation.HasValue) roomTypeHotel.IsFreeCancellation = roomType.IsFreeCancellation.Value;
            if (roomType.IsBreakfastIncluded.HasValue) roomTypeHotel.IsBreakfastIncluded = roomType.IsBreakfastIncluded.Value;

            roomTypeHotel.UpdatedAt = DateTime.Now;

            // 4. ĐỒNG BỘ DỮ LIỆU
            if (roomType.Beds != null) await SyncRoomBedsAsync(roomTypeId, roomType.Beds);
            if (roomType.ViewIds != null) await SyncRoomViewsAsync(roomTypeId, roomType.ViewIds);
            if (roomType.AmenityIds != null) await SyncRoomAmenitiesAsync(roomTypeId, roomType.AmenityIds);
            if (roomType.RoomTypeServices != null) await SyncRoomServicesAsync(roomTypeId, roomType.RoomTypeServices);
            // 5. CẬP NHẬT SỐ LƯỢNG PHÒNG VẬT LÝ 
            if (roomType.Quantity.HasValue && roomType.Quantity.Value != roomTypeHotel.Quantity)
            {
                try
                {
                    await AdjustPhysicalRoomsAsync(roomTypeId, roomTypeHotel.Quantity, roomType.Quantity.Value);
                    // Nếu thành công thì update số lượng trong RoomType
                    roomTypeHotel.Quantity = roomType.Quantity.Value;
                }
                catch (InvalidOperationException invEx)
                {
                    // Bắt lỗi logic nghiệp vụ từ hàm AdjustPhysicalRoomsAsync
                    await transaction.RollbackAsync(); // Rollback các thay đổi trước đó (Tên, Giá, Giường...)
                    return ApiResponseHelper.BadRequest<RoomTypeForOwnerDTO>(invEx.Message);
                }
            }

            await _dbu.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetRoomTypeDetailAsync(roomTypeId);
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
            _context.RoomTypeServices.RemoveRange(_context.RoomTypeServices.Where(x => x.RoomTypeId == roomTypeId));
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

            // if (roomType.Hotel.Status != "Draft" && roomType.Hotel.Status != "Active")
            //     return ApiResponseHelper.BadRequest<string>("Chỉ được upload ảnh khi khách sạn đang tạm ngưng hoạt động.");

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

            // if (roomType.Hotel.Status != "Draft" && roomType.Hotel.Status != "Active")
            //     return ApiResponseHelper.BadRequest<bool>("Chỉ chọn tiện ích khi khách sạn đang tạm ngưng hoạt động.");

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

    public async Task<ApiResponse<RoomTypeForOwnerDTO>> CloneRoomTypeAsync(int sourceRoomTypeId, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Kiểm tra quyền sở hữu
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<RoomTypeForOwnerDTO>();

            // 2. Lấy dữ liệu nguồn (Include TOÀN BỘ các bảng con)
            var source = await _context.RoomTypes
                .AsNoTracking() // Quan trọng: Để không bị track entity cũ
                .Include(rt => rt.RoomImages)
                .Include(rt => rt.RoomBedTypes)
                .Include(rt => rt.RoomViewTypes)
                .Include(rt => rt.RoomAmenities)
                .Include(rt => rt.RoomTypeServices)
                .FirstOrDefaultAsync(rt => rt.Id == sourceRoomTypeId && rt.IsDeleted == false);

            if (source == null)
                return ApiResponseHelper.NotFound<RoomTypeForOwnerDTO>("Loại phòng nguồn không tồn tại.");

            // Kiểm tra xem khách sạn có thuộc về Owner này không (thông qua HotelId trong source)
            var isOwnerHotel = await _context.Hotels.AnyAsync(h => h.Id == source.HotelId && h.OwnerId == ownerId);
            if (!isOwnerHotel)
                return ApiResponseHelper.Forbidden<RoomTypeForOwnerDTO>();

            // 3. Tạo Object mới (Sao chép thông tin)
            var newRoomType = new RoomType
            {
                HotelId = source.HotelId,
                Name = source.Name, // Giữ nguyên tên để Frontend Group lại (Fake iVIVU)
                Description = source.Description,
                PricePerNight = source.PricePerNight, // Giữ nguyên giá (Owner sửa sau)

                // Các thông số vật lý
                Area = source.Area,
                AdultCapacity = source.AdultCapacity,
                ChildCapacity = source.ChildCapacity,

                // Chính sách (Copy luôn)
                IsBreakfastIncluded = source.IsBreakfastIncluded,
                IsFreeCancellation = source.IsFreeCancellation,

                // Setup trạng thái
                IsActive = false, // Tắt trước, để Owner sửa xong mới bật
                Quantity = 0,     // Reset số lượng về 0 (An toàn)
                DefaultImageUrl = source.DefaultImageUrl, // Dùng lại link ảnh cũ

                // Meta
                SortOrder = source.SortOrder + 1,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            // Thêm vào DB để lấy ID mới trước
            _context.RoomTypes.Add(newRoomType);
            await _context.SaveChangesAsync();

            // 4. Sao chép dữ liệu bảng con (Bulk Copy)

            // A. Images (Dùng lại URL ảnh cũ -> Tiết kiệm dung lượng server)
            if (source.RoomImages != null && source.RoomImages.Any())
            {
                var newImages = source.RoomImages.Where(x => x.IsDeleted == false).Select(img => new RoomImage
                {
                    RoomTypeId = newRoomType.Id, // Link vào ID mới
                    ImageUrl = img.ImageUrl,
                    IsDefault = img.IsDefault,
                    SortOrder = img.SortOrder,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                });
                await _context.RoomImages.AddRangeAsync(newImages);
            }

            // B. Amenities
            if (source.RoomAmenities != null && source.RoomAmenities.Any())
            {
                var newAmenities = source.RoomAmenities.Select(a => new RoomAmenity
                {
                    RoomTypeId = newRoomType.Id,
                    AmenityId = a.AmenityId,
                    CreatedAt = DateTime.Now
                });
                await _context.RoomAmenities.AddRangeAsync(newAmenities);
            }

            // C. Beds
            if (source.RoomBedTypes != null && source.RoomBedTypes.Any())
            {
                var newBeds = source.RoomBedTypes.Select(b => new RoomBedType
                {
                    RoomTypeId = newRoomType.Id,
                    BedTypeId = b.BedTypeId,
                    Quantity = b.Quantity,
                    IsPrimary = b.IsPrimary,
                    CreatedAt = DateTime.Now
                });
                await _context.RoomBedTypes.AddRangeAsync(newBeds);
            }

            // D. Views
            if (source.RoomViewTypes != null && source.RoomViewTypes.Any())
            {
                var newViews = source.RoomViewTypes.Select(v => new RoomViewType
                {
                    RoomTypeId = newRoomType.Id,
                    ViewTypeId = v.ViewTypeId,
                    IsPrimary = v.IsPrimary,
                    CreatedAt = DateTime.Now
                });
                await _context.RoomViewTypes.AddRangeAsync(newViews);
            }

            // E. Services
            if (source.RoomTypeServices != null && source.RoomTypeServices.Any())
            {
                var newServices = source.RoomTypeServices.Where(x => x.IsDeleted == false).Select(s => new RoomTypeService
                {
                    RoomTypeId = newRoomType.Id,
                    ServiceId = s.ServiceId,
                    Quantity = s.Quantity,
                    Note = s.Note,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                });
                await _context.RoomTypeServices.AddRangeAsync(newServices);
            }

            // Lưu toàn bộ bảng con
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Xóa cache
            _cache.Remove($"OwnerHotelDetail_{source.HotelId}_{ownerId}");
            _cache.Remove($"OwnerRoomTypes_{source.HotelId}_{ownerId}");

            // 5. Trả về chi tiết thằng mới tạo (Để FE mở popup edit luôn)
            return await GetRoomTypeDetailAsync(newRoomType.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<RoomTypeForOwnerDTO>($"Lỗi nhân bản: {ex.Message}");
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

            var jsonResult = string.Join("", jsonList); // Lấy chuỗi JSON đầu tiên (SP chỉ trả 1 dòng)

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

    #region Manage Banner
    public async Task<ApiResponse<List<BannerDTO>>> GetAllBannersAsync()
    {
        try
        {
            var banner = await _context.Banners
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

            var res = banner.Select(b => new BannerDTO
            {
                Id = b.Id,
                Page = b.Page,
                ImageUrl = b.ImageUrl,
                Title = b.Title,
                LinkUrl = b.LinkUrl,
                SortOrder = b.SortOrder ?? 0,
                IsActive = b.IsActive ?? true
            }).ToList();

            return new ApiResponse<List<BannerDTO>> { StatusCode = StatusCodeResponse.Success, Content = res };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<BannerDTO>> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<List<BannerDTO>>> GetBannersByPageAsync(string pageName)
    {
        try
        {
            var banner = await _context.Banners
                .Where(b => b.Page == pageName && (b.IsActive ?? true))
                .OrderBy(b => b.SortOrder)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();

            var res = banner.Select(b => new BannerDTO
            {
                Id = b.Id,
                Page = b.Page,
                ImageUrl = b.ImageUrl,
                Title = b.Title,
                LinkUrl = b.LinkUrl,
                SortOrder = b.SortOrder ?? 0,
                IsActive = true
            }).ToList();

            return new ApiResponse<List<BannerDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = res
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<BannerDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<bool>> ToggleActiveBannerAsync(int id, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<bool>();

        var banner = await _context.Banners.FindAsync(id);
        if (banner == null) return new ApiResponse<bool> { StatusCode = StatusCodeResponse.NotFound };

        try
        {
            // Đảo ngược trạng thái
            var currentStatus = banner.IsActive;
            banner.IsActive = !currentStatus;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = banner.IsActive.Value,
                Message = "Cập nhật trạng thái thành công"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<List<BannerDTO>>> CreateBannersAsync(BannerCreateDTO img, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<List<BannerDTO>>();

        if (img.Images == null || !img.Images.Any())
            return new ApiResponse<List<BannerDTO>> { StatusCode = StatusCodeResponse.BadRequest, Message = "Vui lòng chọn ảnh" };

        var newBanners = new List<Banner>();
        var folderName = "chillzone/banners";

        try
        {
            foreach (var file in img.Images)
            {
                if (file.Length > 0)
                {
                    var imageUrl = await _uploadService.SaveImageAsync(file, folderName);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // lấy lại PublicId từ Url
                        var publicId = ExtractPublicIdFromUrl(imageUrl);
                        var banner = new Banner
                        {
                            Page = img.Page,
                            Title = img.Title,
                            LinkUrl = img.LinkUrl,
                            ImageUrl = imageUrl,
                            PublicId = publicId, // Lưu cái này để xóa sau này
                            IsActive = true,
                            SortOrder = 0,
                            CreatedAt = DateTime.Now
                        };
                        newBanners.Add(banner);
                    }
                }
            }

            if (newBanners.Any())
            {
                _context.Banners.AddRange(newBanners);
                await _context.SaveChangesAsync();
            }
            // map sang dto
            var res = newBanners.Select(b => new BannerDTO
            {
                Id = b.Id,
                ImageUrl = b.ImageUrl,
                Page = b.Page,
                Title = b.Title,
                IsActive = b.IsActive ?? true,
                SortOrder = b.SortOrder ?? 0
            }).ToList();

            return new ApiResponse<List<BannerDTO>> { StatusCode = StatusCodeResponse.Success, Content = res };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<BannerDTO>> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<BannerDTO>> UpdateBannerAsync(int id, BannerUpdateDTO img, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<BannerDTO>();

        var banner = await _context.Banners.FindAsync(id);
        if (banner == null) return new ApiResponse<BannerDTO>
        {
            StatusCode = StatusCodeResponse.NotFound,
            Message = MessageResponse.IMAGE_NOTFOUND
        };

        try
        {
            banner.Page = img.Page;
            banner.Title = img.Title;
            banner.LinkUrl = img.LinkUrl;
            banner.SortOrder = img.SortOrder;
            banner.IsActive = img.IsActive;

            await _context.SaveChangesAsync();

            return new ApiResponse<BannerDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = new BannerDTO
                {
                    Id = banner.Id,
                    Page = banner.Page,
                    ImageUrl = banner.ImageUrl,
                    Title = banner.Title,
                    LinkUrl = banner.LinkUrl,
                    IsActive = banner.IsActive ?? true,
                    SortOrder = banner.SortOrder ?? 0
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<BannerDTO> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    // 2. Xóa Banner (Cần xóa cả trên Cloud)
    public async Task<ApiResponse<bool>> DeleteBannerAsync(int id, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<bool>();

        var banner = await _context.Banners.FindAsync(id);
        if (banner == null) return new ApiResponse<bool> { StatusCode = StatusCodeResponse.NotFound };

        // Xóa trên Cloudinary trước
        if (!string.IsNullOrEmpty(banner.ImageUrl))
        {
            await _uploadService.DeleteImageAsync(banner.ImageUrl);
        }

        _context.Banners.Remove(banner);
        await _context.SaveChangesAsync();

        return new ApiResponse<bool> { StatusCode = StatusCodeResponse.Success, Content = true };
    }

    private static string? ExtractPublicIdFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;

            // Tìm vị trí sau /upload/
            // /v1234567890/hotels/1/2/cover.jpg → hotels/1/2/cover
            int start = path.IndexOf("upload/", StringComparison.Ordinal) + 8;
            if (start <= 7) return null;

            var publicId = path.Substring(start);
            publicId = publicId.Split('?')[0]; // bỏ query string
            return Path.ChangeExtension(publicId.Trim('/'), null); // bỏ .jpg
        }
        catch
        {
            return null;
        }
    }
    #endregion

    #region Manage Services

    public async Task<ApiResponse<PagedResult<ServicesDTO>>> GetAllServicesAsync(string? keyword = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        try
        {
            var query = _context.Services.AsQueryable();
            query = query.Where(s => s.IsDeleted == false);
            // 3. Xử lý Tìm kiếm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower(); // Chuẩn hóa keyword
                // EF Core sẽ dịch đoạn này thành: WHERE LOWER(Name) LIKE '%keyword%'
                query = query.Where(s => s.Name.ToLower().Contains(k));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.CreatedAt) // Mới nhất lên đầu
                .Skip((pageIndex - 1) * pageSize)    // Bỏ qua các trang trước
                .Take(pageSize)                      // Lấy số lượng của trang này
                .Select(s => new ServicesDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description ?? "",
                    CreatedBy = s.CreatedBy ?? 0,
                    UpdateBy = s.UpdatedBy ?? 0,
                    IsDeleted = s.IsDeleted
                })
                .ToListAsync();

            var result = new PagedResult<ServicesDTO>
            {
                Items = items,
                TotalRecords = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return new ApiResponse<PagedResult<ServicesDTO>>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedResult<ServicesDTO>>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = ex.Message
            };
        }
    }

    // 5. [QUAN TRỌNG] Get Detail & Usage Statistic (Logic xem ai đang dùng)
    public async Task<ApiResponse<ServiceDetailDTO>> GetServiceDetailWithUsageAsync(int id, int adminId)
    {
        try
        {
            // 1. Check quyền Admin
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<ServiceDetailDTO>();

            // 2. Dùng LINQ để lấy dữ liệu và Map thẳng ra DTO
            var serviceDetail = await _context.Services
                .Where(s => s.Id == id) // Chỉ lấy service cần tìm
                .Select(s => new ServiceDetailDTO
                {
                    // Map các trường cơ bản
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description!,
                    CreatedBy = s.CreatedBy ?? adminId,
                    IsDeleted = s.IsDeleted,

                    // --- PHẦN THỐNG KÊ (EF tự sinh SQL Count/Avg) ---
                    UsageCount = s.HotelServiceConfigs.Count(), // Đếm số khách sạn dùng

                    // Tính trung bình giá (xử lý null nếu chưa ai dùng)
                    AveragePrice = s.HotelServiceConfigs.Any()
                                   ? s.HotelServiceConfigs.Average(hs => hs.Price)
                                   : 0,

                    // --- PHẦN DANH SÁCH CHI TIẾT ---
                    UsedByHotels = s.HotelServiceConfigs.Select(hs => new HotelServiceUsageDTO
                    {
                        HotelId = hs.HotelId,
                        HotelName = hs.Hotel.Name,
                        // Lưu ý: Cần chắc chắn Hotel có Owner, nếu không phải check null
                        OwnerName = hs.Hotel.Owner != null ? hs.Hotel.Owner.FullName : "N/A",
                        Price = hs.Price,
                        Unit = hs.Unit,
                        IsActive = hs.IsActive ?? false
                    })
                    .OrderByDescending(hs => hs.Price) // Sắp xếp theo giá giảm dần
                    .ToList()
                })
                .FirstOrDefaultAsync(); // Lấy 1 bản ghi hoặc null

            // 3. Check kết quả
            if (serviceDetail == null)
                return new ApiResponse<ServiceDetailDTO> { StatusCode = StatusCodeResponse.NotFound };

            return new ApiResponse<ServiceDetailDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = serviceDetail
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ServiceDetailDTO> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    // hiển thị ở trang chủ
    public async Task<ApiResponse<List<ServicesDTO>>> GetFeaturedServicesAsync()
    {
        // Logic: Lấy 8-10 service được nhiều khách sạn sử dụng nhất
        var topServices = await _context.Services
            .Where(s => s.IsDeleted == false)
            .OrderByDescending(s => s.HotelServiceConfigs.Count()) // Sắp xếp theo độ phổ biến
            .Take(8)
            .Select(s => new ServicesDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                // Nếu bạn có cột Icon/Image trong bảng Service thì select ra luôn
                // Icon = s.Icon 
            })
            .ToListAsync();

        return ApiResponseHelper.Ok(topServices);
    }




    public async Task<ApiResponse<ServicesDTO>> CreateServicesAsync(ServiceCreateOrUpdateDTO sv, int adminId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<ServicesDTO>();

            var exists = await _context.Services.AnyAsync(s => s.Name.ToLower() == sv.Name.ToLower() && s.IsDeleted == false);
            if (exists) return new ApiResponse<ServicesDTO>
            {
                StatusCode = StatusCodeResponse.Conflict,
                Message = MessageResponse.NAME_ALREADY_EXISTS,
            };

            var service = new Service
            {
                Name = sv.Name.Trim(),
                Description = sv.Description?.Trim(),
                CreatedBy = adminId,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();

            var resDTO = new ServicesDTO
            {
                Id = service.Id,
                Name = service.Name,
                Description = sv.Description!,
                CreatedBy = service.CreatedBy,
                IsDeleted = service.IsDeleted
            };

            return new ApiResponse<ServicesDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.CREATE_SUCCESSFULLY,
                Content = resDTO
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ServicesDTO>
            {
                StatusCode = StatusCodeResponse.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<ServicesDTO>> UpdateServicesAsync(int id, ServiceCreateOrUpdateDTO sv, int adminId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<ServicesDTO>();

            var service = await _context.Services.FindAsync(id);
            if (service == null || service.IsDeleted == true)
                return new ApiResponse<ServicesDTO> { StatusCode = StatusCodeResponse.NotFound, Message = "Dịch vụ không tồn tại" };

            var isDuplicate = await _context.Services
                .AnyAsync(s => s.Name.ToLower() == sv.Name.Trim().ToLower() && s.Id != id && s.IsDeleted == false);

            if (isDuplicate)
            {
                return new ApiResponse<ServicesDTO>
                {
                    StatusCode = StatusCodeResponse.Conflict,
                    Message = MessageResponse.NAME_ALREADY_EXISTS
                };
            }

            service.Name = sv.Name.Trim();
            service.Description = sv.Description?.Trim();
            service.UpdatedBy = adminId;
            service.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<ServicesDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.SUCCESS,
                Content = new ServicesDTO
                {
                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description!,
                    UpdateBy = adminId
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ServicesDTO> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteServiceAsync(int id, int adminId)
    {
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<bool>();

            var service = await _context.Services.FindAsync(id);
            if (service == null || service.IsDeleted == true)
                return new ApiResponse<bool> { StatusCode = StatusCodeResponse.NotFound, Message = "Dịch vụ không tìm thấy" };

            // Logic quan trọng:
            // Nếu đang có khách sạn sử dụng Active, có cho xóa không?
            // Ở đây mình chọn phương án Soft Delete: Vẫn cho xóa, nhưng các khách sạn đang dùng vẫn hiển thị bình thường,
            // chỉ là các khách sạn mới sẽ không thấy Service này để chọn nữa.

            service.IsDeleted = true;
            service.UpdatedBy = adminId;
            service.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { StatusCode = StatusCodeResponse.Success, Content = true, Message = "Xóa dịch vụ thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }

    #endregion

    #region Owner Services
    public async Task<ApiResponse<List<OwnerHotelServiceDTO>>> GetOwnerHotelServicesAsync(int hotelId, int ownerId)
    {
        try
        {
            // Bảo mật: Check xem khách sạn này có phải của Owner này không
            var isOwner = await _context.Hotels.AnyAsync(h => h.Id == hotelId && h.OwnerId == ownerId);
            if (!isOwner) return ApiResponseHelper.Forbidden<List<OwnerHotelServiceDTO>>("Bạn không sở hữu khách sạn này");

            var list = await _context.HotelServiceConfigs
                .Where(hs => hs.HotelId == hotelId)
                .Include(hs => hs.Service) // Join bảng Service gốc
                .Select(hs => new OwnerHotelServiceDTO
                {
                    Id = hs.Id,
                    HotelId = hs.HotelId,
                    ServiceId = hs.ServiceId,
                    ServiceName = hs.Service.Name,
                    Description = hs.Service.Description,
                    Price = hs.Price,
                    Unit = hs.Unit,
                    IsActive = hs.IsActive ?? true,
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(list);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<OwnerHotelServiceDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<List<AvailableServiceDTO>>> GetAvailableServicesToAddAsync(int hotelId, int ownerId)
    {
        try
        {
            var isOwner = await _context.Hotels.AnyAsync(h => h.Id == hotelId && h.OwnerId == ownerId);
            if (!isOwner) return ApiResponseHelper.Forbidden<List<AvailableServiceDTO>>();

            // Lấy ID các service mà khách sạn đã có
            var existingServiceIds = await _context.HotelServiceConfigs
                .Where(hs => hs.HotelId == hotelId)
                .Select(hs => hs.ServiceId)
                .ToListAsync();

            // Lấy các service còn lại
            var available = await _context.Services
                .Where(s => s.IsDeleted == false && existingServiceIds.Contains(s.Id) == false)
                .Select(s => new AvailableServiceDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description!
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(available);
        }
        catch (Exception ex) { return ApiResponseHelper.ServerError<List<AvailableServiceDTO>>(ex.Message); }
    }

    public async Task<ApiResponse<OwnerHotelServiceDTO>> AddServiceToHotelAsync(OwnerAddServiceDTO ownerService, int ownerId)
    {
        try
        {
            // Check quyền sở hữu
            var isOwner = await _context.Hotels.AnyAsync(h => h.Id == ownerService.HotelId && h.OwnerId == ownerId);
            if (!isOwner) return ApiResponseHelper.Forbidden<OwnerHotelServiceDTO>();

            // Check đã tồn tại chưa (Double check)
            var exists = await _context.HotelServiceConfigs.AnyAsync(hs => hs.HotelId == ownerService.HotelId && hs.ServiceId == ownerService.ServiceId);
            if (exists) return ApiResponseHelper.Conflict<OwnerHotelServiceDTO>("Dịch vụ này đã có trong khách sạn");

            var ownerHotelService = new HotelServiceConfig
            {
                HotelId = ownerService.HotelId,
                ServiceId = ownerService.ServiceId,
                Price = ownerService.Price,
                Unit = ownerService.Unit,
                IsActive = true,
                CreatedBy = ownerId,
                CreatedAt = DateTime.Now
            };

            await _context.HotelServiceConfigs.AddAsync(ownerHotelService);
            await _context.SaveChangesAsync();

            // query vào bảng để lấy dc tên, vì chỉ lấy dc Id chưa lấy dc tên
            var serviceInfo = await _context.Services
                .Where(s => s.Id == ownerService.ServiceId)
                .Select(s => new { s.Name, s.Description })
                .FirstOrDefaultAsync();

            var resDTO = new OwnerHotelServiceDTO
            {
                HotelId = ownerHotelService.HotelId,
                ServiceId = ownerHotelService.ServiceId,
                ServiceName = serviceInfo?.Name ?? "N/A",
                Description = serviceInfo?.Description,
                Price = ownerHotelService.Price,
                Unit = ownerHotelService.Unit,
                IsActive = ownerHotelService.IsActive ?? true
            };
            return new ApiResponse<OwnerHotelServiceDTO>
            {
                StatusCode = StatusCodeResponse.Success,
                Message = MessageResponse.SUCCESS,
                Content = resDTO
            };
        }
        catch (Exception ex) { return ApiResponseHelper.ServerError<OwnerHotelServiceDTO>(ex.Message); }
    }

    public async Task<ApiResponse<OwnerHotelServiceDTO>> UpdateHotelServiceAsync(int id, OwnerUpdateServiceDTO ownerService, int ownerId)
    {
        try
        {
            var item = await _context.HotelServiceConfigs
            .Include(hs => hs.Hotel)
            .Include(hs => hs.Service)
            .FirstOrDefaultAsync(hs => hs.Id == id);

            if (item == null) return ApiResponseHelper.NotFound<OwnerHotelServiceDTO>();

            // Check quyền: Hotel của Service này phải thuộc về Owner
            if (item.Hotel.OwnerId != ownerId) return ApiResponseHelper.Forbidden<OwnerHotelServiceDTO>();

            item.Price = ownerService.Price;
            item.Unit = ownerService.Unit;
            item.IsActive = ownerService.IsActive;

            await _context.SaveChangesAsync();

            var resDto = new OwnerHotelServiceDTO
            {
                Id = item.Id,
                ServiceId = item.ServiceId,
                ServiceName = item.Service.Name, // Đã có nhờ Include ở trên
                Description = item.Service.Description,
                Price = item.Price,
                Unit = item.Unit,
                IsActive = item.IsActive ?? true
            };

            return ApiResponseHelper.Ok(resDto);
        }
        catch (Exception ex) { return ApiResponseHelper.ServerError<OwnerHotelServiceDTO>(ex.Message); }
    }

    // 5. Remove (Xóa khỏi khách sạn)
    public async Task<ApiResponse<bool>> RemoveServiceFromHotelAsync(int id, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var item = await _context.HotelServiceConfigs
                .Include(hs => hs.Hotel)
                .FirstOrDefaultAsync(hs => hs.Id == id);
            if (item == null) return ApiResponseHelper.NotFound<bool>();
            if (item.Hotel.OwnerId != ownerId) return ApiResponseHelper.Forbidden<bool>();

            var roomTypeIds = await _context.RoomTypes
                .Where(rt => rt.HotelId == item.HotelId)
                .Select(rt => rt.Id)
                .ToListAsync();

            if (roomTypeIds.Any())
            {
                var relatedInclusions = await _context.RoomTypeServices
                    .Where(rts => roomTypeIds.Contains(rts.RoomTypeId) && rts.ServiceId == item.ServiceId)
                    .ToListAsync();

                if (relatedInclusions.Any())
                {
                    _context.RoomTypeServices.RemoveRange(relatedInclusions);
                }
            }

            _context.HotelServiceConfigs.Remove(item);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ApiResponseHelper.Ok(true);
        }
        catch (Exception ex) { await transaction.RollbackAsync(); return ApiResponseHelper.ServerError<bool>(ex.Message); }
    }

    public async Task<ApiResponse<bool>> ToggleActiveOwnerServiceAsync(int id, int ownerId)
    {
        try
        {
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<bool>();

            var hotelService = await _context.HotelServiceConfigs
                .Include(hs => hs.Hotel)
                .FirstOrDefaultAsync(hs => hs.Id == id);

            if (hotelService == null) return new ApiResponse<bool> { StatusCode = StatusCodeResponse.NotFound, Message = "Dịch vụ không tồn tại" };
            if (hotelService.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền sửa dịch vụ của khách sạn này");
            // Đảo ngược trạng thái
            var currentStatus = hotelService.IsActive ?? true;
            hotelService.IsActive = !currentStatus;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = hotelService.IsActive!.Value,
                Message = currentStatus ? "Đã bật dịch vụ" : "Đã tắt dịch vụ"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { StatusCode = StatusCodeResponse.Error, Message = ex.Message };
        }
    }
    #endregion

    #region Rooms
    public async Task<ApiResponse<List<RoomPhysicalDTO>>> GetPhysicalRoomsAsync(int hotelId, int requesterId, int? roomTypeId = null)
    {
        // Check quyền Owner...
        if (!await _authService.CanOperateHotelAsync(requesterId, hotelId))
            return ApiResponseHelper.Forbidden<List<RoomPhysicalDTO>>();

        // 2. Lấy ngày hiện tại
        var today = DateOnly.FromDateTime(DateTime.Now);

        var query = _context.Rooms
            .AsNoTracking()
            .Include(r => r.RoomType)
            .Where(r => r.RoomType.HotelId == hotelId && r.IsDeleted == false);

        if (roomTypeId.HasValue)
        {
            query = query.Where(r => r.RoomTypeId == roomTypeId);
        }

        var roomsData = await query.OrderBy(r => r.RoomNumber).ToListAsync();

        // Lấy danh sách ID phòng
        var roomIds = roomsData.Select(r => r.Id).ToList();

        // Lấy thông tin Booking hiện tại (Đang ở)
        var currentBookings = await _context.BookingRooms
            .AsNoTracking()
            .Include(br => br.Booking)
            .Where(br => roomIds.Contains(br.RoomId ?? 0)
                        && br.Booking.IsDeleted == false
                        && (br.Booking.Status == "CheckedIn" || (br.Booking.Status == "Confirmed"
                            && br.Booking.CheckInDate <= today
                            && br.Booking.CheckOutDate >= today)
                        ))
            .OrderByDescending(br => br.Booking.CheckInDate)
            .ToDictionaryAsync(k => k.RoomId!.Value, v => v);

        var activeTasks = await _context.HousekeepingTasks
            .Include(t => t.Staff).ThenInclude(s => s.User)
            .Where(t => roomIds.Contains(t.RoomId) && t.Status != "Completed")
            .ToDictionaryAsync(k => k.RoomId, v => v);
        // Map kết quả
        var result = roomsData.Select(r =>
        {
            var activeBooking = currentBookings.GetValueOrDefault(r.Id);
            var activeTask = activeTasks.GetValueOrDefault(r.Id);
            // Logic xác định status hiển thị
            string displayStatus = r.Status; // Mặc định lấy status vật lý (Sạch/Bẩn/Bảo trì)
            // Nếu đang có khách thì ghi đè status hiển thị là Occupied
            if (activeBooking != null)
            {
                // Nếu hôm nay là ngày check-in -> Hiển thị "Arrival" (Sắp đến) hoặc "Occupied" (Đã đến)
                if (activeBooking.Booking.Status == "Confirmed")
                    displayStatus = "Reserved"; // Đã đặt trước
                else if (activeBooking.Booking.Status == "CheckedIn")
                    displayStatus = "Occupied"; // Đang ở
            }

            return new RoomPhysicalDTO
            {
                Id = r.Id,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType.Name,
                RoomNumber = r.RoomNumber,
                Status = displayStatus,
                Floor = r.Floor,

                CurrentCheckIn = activeBooking?.Booking.CheckInDate.ToDateTime(TimeOnly.MinValue),
                CurrentCheckOut = activeBooking?.Booking.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                // Thông tin khách
                CurrentGuestName = activeBooking?.GuestName ?? activeBooking?.Booking.ContactName,
                CurrentBookingId = activeBooking?.BookingId,
                CurrentBookingRoomId = activeBooking?.Id,
                HousekeeperName = activeTask?.Staff?.User?.FullName ?? "Chưa giao",
                MaintenanceReason = (displayStatus == "Maintenance") ? activeTask?.Note : null
            };
        }).ToList();

        return ApiResponseHelper.Ok(result);
    }

    public async Task<ApiResponse<bool>> AssignRoomToBookingAsync(AssignRoomRequestDTO request, int requesterId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Lấy thông tin BookingRoom (Đang chưa có phòng hoặc muốn đổi phòng)
            var bookingRoom = await _context.BookingRooms
                .Include(br => br.Booking)
                .FirstOrDefaultAsync(br => br.Id == request.BookingRoomId);

            if (bookingRoom == null)
                return ApiResponseHelper.NotFound<bool>("Không tìm thấy yêu cầu đặt phòng.");

            // 2. Check quyền Owner
            if (!await _authService.CanOperateHotelAsync(requesterId, bookingRoom.Booking.HotelId))
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền thực hiện check-in/gán phòng.");

            // 3. Lấy thông tin phòng vật lý đích
            var targetRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.PhysicalRoomId);
            if (targetRoom == null)
                return ApiResponseHelper.NotFound<bool>("Phòng vật lý không tồn tại.");

            // 4. Validate: Phòng vật lý phải cùng loại với loại khách đặt
            // (Trừ khi bạn muốn cho phép nâng hạng miễn phí - ở đây mình làm chặt chẽ trước)
            if (targetRoom.RoomTypeId != bookingRoom.RoomTypeId)
                return ApiResponseHelper.BadRequest<bool>("Phòng chọn không đúng loại phòng khách đã đặt.");

            // 5. Validate: Phòng đích có đang bị trùng lịch không?
            var checkIn = bookingRoom.Booking.CheckInDate;
            var checkOut = bookingRoom.Booking.CheckOutDate;

            var isRoomBusy = await _context.BookingRooms.AnyAsync(br =>
                br.RoomId == request.PhysicalRoomId && // Phòng này
                br.Id != request.BookingRoomId &&      // Không phải chính đơn này
                _context.Bookings.Any(b => b.Id == br.BookingId
                                        && b.Status != "Cancelled"
                                        && b.Status != "Completed"
                                        // Logic trùng ngày
                                        && b.CheckInDate < checkOut
                                        && b.CheckOutDate > checkIn));

            if (isRoomBusy)
                return ApiResponseHelper.Conflict<bool>("Phòng này đã có người đặt trong khoảng thời gian này.");

            // 6. Validate Status phòng vật lý (Nếu đang bảo trì thì không gán được)
            if (targetRoom.Status == "Maintenance" || targetRoom.Status == "OutOfOrder")
                return ApiResponseHelper.BadRequest<bool>("Phòng đang bảo trì/hỏng, không thể gán.");

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (checkIn <= today && targetRoom.Status != "Available")
            {
                // Nếu phòng đang bẩn (Cleaning) -> Cảnh báo hoặc Chặn
                if (targetRoom.Status == "Cleaning")
                    return ApiResponseHelper.BadRequest<bool>("Phòng đang dọn dẹp.");

                // Nếu phòng đang có khách khác (Occupied) -> Chặn (dù query trên đã check Booking, nhưng check thêm Status cho chắc)
                if (targetRoom.Status == "Occupied")
                    return ApiResponseHelper.Conflict<bool>("Phòng đang có khách ở.");
            }

            // 7. THỰC HIỆN GÁN
            bookingRoom.RoomId = request.PhysicalRoomId;
            if (!string.IsNullOrEmpty(request.GuestName))
            {
                bookingRoom.GuestName = request.GuestName;
            }

            if (checkIn <= today)
            {
                targetRoom.Status = "Occupied"; // Phòng vật lý: Có khách
                bookingRoom.Booking.Status = "CheckedIn"; // Đơn hàng: Đã nhận phòng
                bookingRoom.Booking.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Gán phòng thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<RoomPhysicalDTO>> CreatePhysicalRoomAsync(int roomTypeId, CreateRoomRequestDTO roomNumber, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Check quyền
            if (!await _authService.IsOwnerAsync(ownerId))
                return ApiResponseHelper.Forbidden<RoomPhysicalDTO>();

            // 2. Validate RoomType
            var roomType = await _context.RoomTypes
                .Include(rt => rt.Hotel)
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && rt.IsDeleted == false);

            if (roomType == null || roomType.Hotel.OwnerId != ownerId)
                return ApiResponseHelper.NotFound<RoomPhysicalDTO>("Loại phòng không tồn tại.");

            // 3. Check trùng tên phòng trong cùng khách sạn
            // (Phòng 101 Standard không thể trùng tên với 101 VIP)
            var exists = await _context.Rooms.AnyAsync(r =>
                r.RoomType.HotelId == roomType.HotelId &&
                r.RoomNumber == roomNumber.RoomNumber!.Trim() &&
                r.IsDeleted == false);

            if (exists)
                return ApiResponseHelper.Conflict<RoomPhysicalDTO>($"Số phòng '{roomNumber}' đã tồn tại trong khách sạn này.");

            // 4. Tạo phòng mới
            var newRoom = new Room
            {
                RoomTypeId = roomTypeId,
                RoomNumber = roomNumber.RoomNumber!.Trim(),
                Status = "Available",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Rooms.Add(newRoom);

            // 5. [QUAN TRỌNG] Cập nhật lại số lượng tổng trong RoomType
            roomType.Quantity += 1;
            roomType.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(new RoomPhysicalDTO
            {
                Id = newRoom.Id,
                RoomTypeId = newRoom.RoomTypeId,
                RoomTypeName = roomType.Name,
                RoomNumber = newRoom.RoomNumber,
                Status = newRoom.Status
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<RoomPhysicalDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<RoomPhysicalDTO>> UpdatePhysicalRoomAsync(int roomId, UpdateRoomPhysicalDTO request, int requesterId)
    {
        // 1. Lấy thông tin phòng trước để biết nó thuộc Hotel nào
        var room = await _context.Rooms
            .Include(r => r.RoomType)
            .ThenInclude(rt => rt.Hotel)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null) return ApiResponseHelper.NotFound<RoomPhysicalDTO>();

        // Sử dụng hàm CanManageHotelAsync bạn đã viết ở bước trước
        if (!await _authService.CanOperateHotelAsync(requesterId, room.RoomType.HotelId))
            return ApiResponseHelper.Forbidden<RoomPhysicalDTO>("Bạn không có quyền chỉnh sửa phòng của khách sạn này.");

        // Check trùng tên phòng trong cùng khách sạn
        var exists = await _context.Rooms.AnyAsync(r =>
            r.RoomType.HotelId == room.RoomType.HotelId &&
            r.RoomNumber == request.RoomNumber.Trim() &&
            r.Id != roomId &&
            r.IsDeleted == false);

        if (exists) return ApiResponseHelper.Conflict<RoomPhysicalDTO>("Tên phòng này đã tồn tại.");

        var reporterName = await _context.Users
                .Where(u => u.Id == requesterId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync() ?? $"ID {requesterId}";

        // Phải kiểm tra xem phòng có đang "Occupied" (Có khách ở) không?
        if (request.Status == "Maintenance")
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var isOccupied = await _context.BookingRooms.AnyAsync(br =>
                br.RoomId == roomId &&
                _context.Bookings.Any(b => b.Id == br.BookingId
                                        && (b.Status == "CheckedIn" || b.Status == "Confirmed") // Cả đang ở và sắp đến
                                        && b.CheckInDate <= today
                                        && b.CheckOutDate > today));

            if (isOccupied)
            {
                return ApiResponseHelper.Conflict<RoomPhysicalDTO>(
                    "Không thể bảo trì: Phòng đang có khách ở hoặc sắp nhận phòng hôm nay.");
            }
        }

        if (request.Status == "Cleaning" || request.Status == "Maintenance")
        {
            // Kiểm tra xem đã có task nào chưa xong chưa? (Tránh tạo trùng)
            var existingTask = await _context.HousekeepingTasks
                .AnyAsync(t => t.RoomId == roomId && t.Status != "Completed");

            if (!existingTask)
            {
                var newTask = new HousekeepingTask
                {
                    HotelId = room.RoomType.HotelId,
                    RoomId = roomId,
                    // Nếu là Maintenance thì status task là ReportedIssue, còn lại là Pending
                    Status = request.Status == "Maintenance" ? "ReportedIssue" : "Pending",
                    Priority = "Normal",
                    Note = request.Status == "Maintenance"
                           ? $"Chuyển sang bảo trì thủ công bởi {reporterName}"
                           : $"Yêu cầu dọn dẹp thủ công bởi {reporterName}",
                    CreatedAt = DateTime.Now
                };
                _context.HousekeepingTasks.Add(newTask);
            }
        }
        // Nếu chuyển sang Available -> Tự động hoàn thành các Task cũ (nếu có)
        else if (request.Status == "Available")
        {
            var pendingTasks = await _context.HousekeepingTasks
                .Where(t => t.RoomId == roomId && t.Status != "Completed")
                .ToListAsync();

            foreach (var t in pendingTasks)
            {
                t.Status = "Completed";
                t.CompletedAt = DateTime.Now;
                t.Note += " (Hoàn tất do đổi trạng thái phòng thủ công)";
            }
        }

        room.RoomNumber = request.RoomNumber.Trim();
        room.Status = request.Status;
        room.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return ApiResponseHelper.Ok(new RoomPhysicalDTO
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Status = room.Status,
            RoomTypeId = room.RoomTypeId,
            RoomTypeName = room.RoomType.Name,
            Floor = room.Floor
        });
    }

    public async Task<ApiResponse<bool>> DeletePhysicalRoomAsync(int roomId, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Check quyền & Lấy thông tin
            // Include RoomType để update Quantity sau này
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == roomId && r.IsDeleted == false);
            if (!await _authService.CanManageHotelAsync(ownerId, room.RoomType.HotelId))
                return ApiResponseHelper.Forbidden<bool>();

            if (room == null) return ApiResponseHelper.NotFound<bool>("Phòng không tồn tại.");
            if (room.RoomType.Hotel.OwnerId != ownerId) return ApiResponseHelper.Forbidden<bool>();

            // 2. [QUAN TRỌNG] Kiểm tra ràng buộc Booking tương lai
            // Nếu phòng này (RoomId) đã được gán cho một Booking nào đó trong tương lai -> KHÔNG ĐƯỢC XÓA
            var today = DateOnly.FromDateTime(DateTime.Now);

            var hasBooking = await _context.BookingRooms.AnyAsync(br =>
                br.RoomId == roomId &&
                _context.Bookings.Any(b => b.Id == br.BookingId
                                        && b.Status != "Cancelled"
                                        && b.Status != "Completed"
                                        && b.CheckOutDate >= today)); // Vẫn còn hạn ở

            if (hasBooking)
            {
                return ApiResponseHelper.BadRequest<bool>(
                    "Không thể xóa: Phòng này đang có khách ở hoặc đã được gán cho đơn đặt phòng trong tương lai. Vui lòng gỡ khách ra trước.");
            }

            // 3. Xóa mềm (Soft Delete)
            room.IsDeleted = true;
            room.UpdatedAt = DateTime.Now;

            // 4. [QUAN TRỌNG] Giảm số lượng tổng trong RoomType
            if (room.RoomType.Quantity > 0)
            {
                room.RoomType.Quantity -= 1;
                room.RoomType.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Xóa phòng thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }
    #endregion

    #region Booking Owner
    public async Task<ApiResponse<bool>> UpdateGuestNameAsync(UpdateGuestNameDTO request, int requesterId)
    {
        try
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(request.GuestName))
                return ApiResponseHelper.BadRequest<bool>("Tên khách không được để trống.");

            // 2. Tìm thông tin BookingRoom + Booking gốc
            var bookingRoom = await _context.BookingRooms
                .Include(br => br.Booking) // Include để lấy HotelId kiểm tra quyền
                .FirstOrDefaultAsync(br => br.Id == request.BookingRoomId);

            if (bookingRoom == null)
                return ApiResponseHelper.NotFound<bool>("Không tìm thấy phòng đặt này.");

            // 3. Check quyền Owner
            if (!await _authService.CanOperateHotelAsync(requesterId, bookingRoom.Booking.HotelId))
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền chỉnh sửa đơn đặt của khách sạn này.");

            // 4. Cập nhật tên khách
            bookingRoom.GuestName = request.GuestName.Trim();
            await _context.SaveChangesAsync();

            return ApiResponseHelper.Ok(true, "Cập nhật tên khách thành công.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<List<BookingRoomDetailDTO>>> GetBookingRoomsDetailsAsync(int bookingId, int requesterId)
    {
        try
        {
            // 1. Validate quyền sở hữu Booking
            var booking = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.IsDeleted == false);

            if (booking == null) return ApiResponseHelper.NotFound<List<BookingRoomDetailDTO>>("Đơn đặt không tồn tại.");

            if (!await _authService.CanOperateHotelAsync(requesterId, booking.HotelId))
                return ApiResponseHelper.Forbidden<List<BookingRoomDetailDTO>>("Bạn không có quyền truy cập.");

            // 2. Lấy danh sách các dòng trong BookingRooms
            var details = await _context.BookingRooms
                .AsNoTracking()
                .Include(br => br.RoomType) // Để lấy tên loại phòng
                .Include(br => br.Room)     // Để lấy tên phòng vật lý (nếu đã gán)
                .Include(br => br.SelectedBedType)  // Để lấy ngày checkin/checkout
                .Where(br => br.BookingId == bookingId)
                .Select(br => new BookingRoomDetailDTO
                {
                    Id = br.Id,
                    RoomTypeId = br.RoomTypeId,
                    RoomTypeName = br.RoomType.Name,
                    RoomId = br.RoomId,
                    RoomNumber = br.Room != null ? br.Room.RoomNumber : null, // Quan trọng: Null nghĩa là chưa gán
                    GuestName = br.GuestName,
                    Price = br.PricePerNight,
                    BedTypeName = br.SelectedBedTypeId != null ? br.SelectedBedType.Name : "Tiêu chuẩn",
                    CheckIn = booking.CheckInDate.ToDateTime(TimeOnly.MinValue),
                    CheckOut = booking.CheckOutDate.ToDateTime(TimeOnly.MinValue)
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(details);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<BookingRoomDetailDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<List<BookingRoomDetailDTO>>> GetPendingBookingsAsync(int hotelId, int requesterId)
    {
        try
        {
            if (!await _authService.CanOperateHotelAsync(requesterId, hotelId))
                return ApiResponseHelper.Forbidden<List<BookingRoomDetailDTO>>("Bạn không có quyền truy cập dữ liệu khách sạn này.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            var list = await _context.BookingRooms
                .AsNoTracking()
                .Include(br => br.Booking)
                .Include(br => br.RoomType)
                .Include(br => br.SelectedBedType)
                .Where(br => br.Booking.HotelId == hotelId
                             && br.RoomId == null
                             && (br.Booking.Status == "Confirmed" || br.Booking.Status == "PendingPayment") // Tùy logic
                             && br.Booking.CheckInDate <= today.AddDays(1) // Cho phép checkin sớm 1 ngày hoặc hôm nay
                             && br.Booking.CheckOutDate > today)
                .Select(br => new BookingRoomDetailDTO
                {
                    Id = br.Id, // BookingRoomId dùng để assign
                    RoomTypeId = br.RoomTypeId,
                    RoomTypeName = br.RoomType.Name,
                    BedTypeName = br.SelectedBedTypeId != null ? br.SelectedBedType.Name : "Không yêu cầu",
                    GuestName = br.GuestName ?? br.Booking.ContactName,
                    Price = br.PricePerNight,
                    CheckIn = br.Booking.CheckInDate.ToDateTime(TimeOnly.MinValue),
                    CheckOut = br.Booking.CheckOutDate.ToDateTime(TimeOnly.MinValue)
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(list);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<BookingRoomDetailDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<List<SchedulerBookingDTO>>> GetBookingsForCalendarAsync(int hotelId, int ownerId, DateTime start, DateTime end)
    {
        try
        {
            // 1. Check quyền
            if (!await _authService.CanOperateHotelAsync(ownerId, hotelId))
                return ApiResponseHelper.Forbidden<List<SchedulerBookingDTO>>();
            // 2. Query dữ liệu
            // Lấy từ BookingRooms (vì bảng này mới chứa RoomId cụ thể)
            var bookings = await _context.BookingRooms
                .AsNoTracking()
                .Include(br => br.Booking)
                .Include(br => br.Room)
                .Where(br => br.Booking.HotelId == hotelId
                             && br.RoomId != null // Chỉ lấy đơn ĐÃ XẾP PHÒNG
                             && br.Booking.IsDeleted == false
                             && br.Booking.Status != "Cancelled"
                             // Logic giao nhau thời gian:
                             // (Ngày đến của khách < Ngày cuối lịch) AND (Ngày đi của khách > Ngày đầu lịch)
                             && br.Booking.CheckInDate < DateOnly.FromDateTime(end)
                             && br.Booking.CheckOutDate > DateOnly.FromDateTime(start))
                .Select(br => new SchedulerBookingDTO
                {
                    Id = br.BookingId, // Link về đơn gốc để xem chi tiết
                    BookingId = br.BookingId,
                    RoomId = br.RoomId!.Value,
                    GuestName = br.GuestName ?? br.Booking.ContactName,
                    Status = br.Booking.Status,
                    RoomName = br.Room.RoomNumber,

                    // Radzen cần DateTime, ta convert từ DateOnly
                    // Mặc định Check-in 14:00, Check-out 12:00 để thanh hiển thị chuẩn
                    StartTime = br.Booking.CheckInDate.ToDateTime(new TimeOnly(14, 0)),
                    EndTime = br.Booking.CheckOutDate.ToDateTime(new TimeOnly(12, 0))
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(bookings);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<SchedulerBookingDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> MoveBookingAsync(MoveBookingRequestDTO request, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // A. Validate Booking
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null) return ApiResponseHelper.NotFound<bool>("Đơn đặt không tồn tại.");

            // Check quyền Owner
            if (!await _authService.CanOperateHotelAsync(ownerId, booking.HotelId))
                return ApiResponseHelper.Forbidden<bool>();

            // B. Validate Logic Ngày tháng
            if (request.NewCheckIn >= request.NewCheckOut)
                return ApiResponseHelper.BadRequest<bool>("Ngày Check-out phải sau ngày Check-in.");

            var newCheckInDate = DateOnly.FromDateTime(request.NewCheckIn);
            var newCheckOutDate = DateOnly.FromDateTime(request.NewCheckOut);

            // C. Kiểm tra phòng mới có trống trong khoảng thời gian mới không?
            // Logic: Tìm xem có đơn nào KHÁC đơn hiện tại đang chiếm chỗ không
            var isOccupied = await _context.BookingRooms
                .AnyAsync(br => br.RoomId == request.NewRoomId
                                && br.BookingId != request.BookingId // Bỏ qua chính nó
                                && br.Booking.Status != "Cancelled"
                                && br.Booking.Status != "Completed"
                                && br.Booking.IsDeleted == false
                                // Logic giao nhau: (A_Start < B_End) && (A_End > B_Start)
                                && br.Booking.CheckInDate < newCheckOutDate
                                && br.Booking.CheckOutDate > newCheckInDate);

            if (isOccupied)
                return ApiResponseHelper.Conflict<bool>("Phòng này đã bị vướng lịch của khách khác trong khoảng thời gian chọn.");

            // D. Cập nhật dữ liệu
            // 1. Cập nhật Booking Master (Ngày tháng)
            booking.CheckInDate = newCheckInDate;
            booking.CheckOutDate = newCheckOutDate;
            booking.UpdatedAt = DateTime.Now;

            // 2. Cập nhật BookingRoom (Phòng vật lý)
            // Giả định đơn này chỉ có 1 phòng (hoặc logic kéo thả chỉ áp dụng đơn 1 phòng)
            var bookingRoom = booking.BookingRooms.FirstOrDefault();
            if (bookingRoom != null)
            {
                bookingRoom.RoomId = request.NewRoomId;
                // Nếu đổi sang loại phòng khác, có thể cần update giá tiền (nhưng ở đây giữ nguyên giá cũ cho đơn giản)
            }

            // 3. Nếu đang ở ngày hiện tại -> Cập nhật trạng thái phòng vật lý
            // (Logic này hơi phức tạp, tạm thời bỏ qua để đơn giản hóa, chỉ update data Booking)

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Cập nhật lịch thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> CreateWalkInBookingAsync(WalkInBookingRequestDTO request, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // A. Validate Phòng
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == request.PhysicalRoomId);

            if (room == null) return ApiResponseHelper.NotFound<bool>("Phòng không tồn tại.");

            // Check quyền
            if (!await _authService.CanOperateHotelAsync(ownerId, room.RoomType.HotelId))
                return ApiResponseHelper.Forbidden<bool>();

            var checkInDate = DateOnly.FromDateTime(request.CheckIn);
            var checkOutDate = DateOnly.FromDateTime(request.CheckOut);
            var today = DateOnly.FromDateTime(DateTime.Now);

            if (checkInDate >= checkOutDate)
                return ApiResponseHelper.BadRequest<bool>("Ngày giờ không hợp lệ.");

            if (checkInDate <= today)
            {
                if (room.Status == "Maintenance" || room.Status == "OutOfOrder")
                    return ApiResponseHelper.BadRequest<bool>($"Phòng {room.RoomNumber} đang bảo trì, không thể đón khách.");

                if (room.Status == "Cleaning")
                    return ApiResponseHelper.Conflict<bool>($"Phòng {room.RoomNumber} đang dọn dẹp.");

                // Nếu status là Occupied nhưng check overlap bên dưới không thấy đơn nào 
                if (room.Status == "Occupied")
                    return ApiResponseHelper.Conflict<bool>($"Phòng {room.RoomNumber} hiện đang có khách.");
            }
            else
            {
                if (room.Status == "Maintenance" || room.Status == "OutOfOrder")
                    return ApiResponseHelper.BadRequest<bool>($"Phòng {room.RoomNumber} đang trong kế hoạch bảo trì.");
            }

            // Kiểm tra trống phòng
            var isOccupied = await _context.BookingRooms
                .AnyAsync(br => br.RoomId == request.PhysicalRoomId
                                && br.Booking.Status != "Cancelled"
                                && br.Booking.Status != "Completed"
                                && br.Booking.IsDeleted == false
                                && br.Booking.CheckInDate < checkOutDate
                                && br.Booking.CheckOutDate > checkInDate);

            if (isOccupied)
                return ApiResponseHelper.Conflict<bool>("Phòng này đã có lịch trong khoảng thời gian này.");

            // Khách vãng lai
            var walkInUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == "walkin@system.local");

            if (walkInUser == null)
            {
                walkInUser = new User
                {
                    UserName = "walkin@system.local", // <--- QUAN TRỌNG: Phải có dòng này
                    Email = "walkin@system.local",
                    FullName = "Khách Vãng Lai",
                    PhoneNumber = "0000000000",
                    PasswordHash = "NO_PASSWORD",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
                _context.Users.Add(walkInUser);
                await _context.SaveChangesAsync(); // Save để lấy ID ngay lập tức
            }


            // Tạo Booking Master
            var booking = new Booking
            {
                HotelId = room.RoomType.HotelId,
                CustomerId = walkInUser.Id, // Khách lẻ không cần account hệ thống
                ContactName = request.GuestName,
                ContactPhone = request.Phone,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                TotalPrice = request.Price,
                Note = request.Note + "Đặt phòng trực tiếp",
                Status = "Confirmed", // Xác nhận luôn
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            // Nếu check-in ngay hôm nay -> Set luôn là CheckedIn & Phòng là Occupied
            if (checkInDate <= today)
            {
                booking.Status = "CheckedIn";
                // room.Status = "Occupied";
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(); // Save để lấy ID

            // D. Tạo BookingRoom (Liên kết phòng vật lý)
            var bookingRoom = new BookingRoom
            {
                BookingId = booking.Id,
                RoomTypeId = room.RoomTypeId,
                RoomId = room.Id, // Gán cứng luôn
                PricePerNight = request.Price, // Giá thỏa thuận
                GuestName = request.GuestName,
                Quantity = 1,
                CreatedAt = DateTime.Now
            };
            _context.BookingRooms.Add(bookingRoom);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Tạo đơn thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }
    // thêm dịch vụ 
    public async Task<ApiResponse<bool>> AddServiceToBookingAsync(AddServiceRequestDTO request, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Kiểm tra Booking có tồn tại và chưa hoàn tất không
            var booking = await _context.Bookings.FindAsync(request.BookingId);
            if (booking == null) return ApiResponseHelper.NotFound<bool>("Đơn đặt phòng không tồn tại.");

            // Chỉ được thêm dịch vụ khi khách ĐANG Ở (CheckedIn) hoặc ĐÃ ĐẶT (Confirmed)
            if (booking.Status == "Cancelled" || booking.Status == "Completed")
                return ApiResponseHelper.BadRequest<bool>("Đơn đã kết thúc.");

            if (booking.CustomerId != userId)
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền thao tác trên đơn hàng này.");


            decimal totalAdded = 0;
            bool hasItemAdded = false;

            foreach (var item in request.Services.Where(x => x.Quantity > 0))
            {
                // Lấy giá hiện tại
                var config = await _context.HotelServiceConfigs
                    .FirstOrDefaultAsync(x => x.HotelId == booking.HotelId && x.ServiceId == item.ServiceId);

                if (config != null && config.IsActive == true)
                {
                    var newService = new BookingService
                    {
                        BookingId = booking.Id,
                        ServiceId = item.ServiceId,
                        Quantity = item.Quantity,
                        Price = config.Price,
                        IsPaid = false, // Chưa thanh toán
                        CreatedAt = DateTime.Now
                    };
                    _context.BookingServices.Add(newService);

                    totalAdded += (newService.Price * (newService.Quantity ?? 0));
                    hasItemAdded = true;
                }
            }

            if (!hasItemAdded)
            {
                return ApiResponseHelper.BadRequest<bool>("Không có dịch vụ nào hợp lệ để thêm.");
            }

            booking.TotalPrice += totalAdded;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Thêm dịch vụ thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> UpdateServiceQuantityAsync(UpdateServiceQuantityDTO request, int requesterId)
    {
        try
        {
            // 1. Lấy thông tin dòng dịch vụ đã đặt
            var bookingService = await _context.BookingServices
                .Include(bs => bs.Booking) // Để check quyền
                .FirstOrDefaultAsync(bs => bs.Id == request.BookingServiceId);

            if (bookingService == null)
                return ApiResponseHelper.NotFound<bool>("Dịch vụ không tồn tại trong đơn này.");
            bool isOwnerOrStaff = await _authService.CanOperateHotelAsync(requesterId, bookingService.Booking.HotelId);
            bool isCustomerOwner = bookingService.Booking.CustomerId == requesterId;
            // 2. Check quyền (Chỉ Lễ tân/Chủ mới được sửa)
            if (!isOwnerOrStaff && !isCustomerOwner)
            {
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền chỉnh sửa dịch vụ này.");
            }
            // 3. Logic chặn "gian lận"
            if (bookingService.IsPaid == true)
                return ApiResponseHelper.BadRequest<bool>("Dịch vụ này đã được thanh toán, không thể chỉnh sửa số lượng.");

            // 4. Nếu số lượng <= 0 -> Xóa luôn
            if (request.NewQuantity <= 0)
            {
                _context.BookingServices.Remove(bookingService);
                await _context.SaveChangesAsync();
                return ApiResponseHelper.Ok(true, "Đã xóa dịch vụ khỏi đơn hàng.");
            }

            // 5. Cập nhật số lượng
            // Lưu ý: Giá (Price) giữ nguyên giá lúc đặt, chỉ nhân lại số lượng
            bookingService.Quantity = request.NewQuantity;
            // (Optional) Update Audit Time
            // bookingService.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponseHelper.Ok(true, "Cập nhật số lượng thành công. Tổng tiền đã được tính lại.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteServiceFromBookingAsync(int bookingServiceId, int requesterId)
    {
        try
        {
            var bookingService = await _context.BookingServices
                .Include(bs => bs.Booking)
                .FirstOrDefaultAsync(bs => bs.Id == bookingServiceId);

            if (bookingService == null)
                return ApiResponseHelper.NotFound<bool>("Dịch vụ không tồn tại.");

            // Check quyền
            bool isOwnerOrStaff = await _authService.CanOperateHotelAsync(requesterId, bookingService.Booking.HotelId);
            bool isCustomerOwner = bookingService.Booking.CustomerId == requesterId;
            // 2. Check quyền (Chỉ Lễ tân/Chủ mới được sửa)
            if (!isOwnerOrStaff && !isCustomerOwner)
            {
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền chỉnh sửa dịch vụ này.");
            }

            // Check đã trả tiền chưa
            if (bookingService.IsPaid == true)
                return ApiResponseHelper.BadRequest<bool>("Dịch vụ đã thanh toán, không thể xóa.");

            _context.BookingServices.Remove(bookingService);
            await _context.SaveChangesAsync();

            return ApiResponseHelper.Ok(true, "Đã xóa dịch vụ khỏi đơn hàng.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }
    #endregion

    #region Booking User

    // xem lại lịch sử đặt phòng
    public async Task<ApiResponse<List<BookingListItemDTO>>> GetCustomerBookingsAsync(int userId)
    {
        try
        {
            var list = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Hotel) // Cần Include Hotel để lấy tên và ảnh
                .Include(b => b.Payments) // Cần Include BookingRooms để đếm số phòng
                .Include(b => b.BookingRooms)
                .Include(b => b.BookingServices)
                .Where(b => b.CustomerId == userId && b.IsDeleted == false)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingListItemDTO
                {
                    Id = b.Id,
                    HotelId = b.HotelId,
                    HotelName = b.Hotel.Name, // Cần Include Hotel
                    HotelImage = b.Hotel.CoverImageUrl,
                    CheckInDate = b.CheckInDate.ToDateTime(TimeOnly.MinValue),
                    CheckOutDate = b.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                    TotalPrice = b.TotalPrice + b.BookingServices
                                         .Where(bs => bs.IsPaid == false) // Chỉ cộng thêm những món nợ mới
                                         .Sum(bs => bs.Price * (bs.Quantity ?? 0)),
                    Status = b.Status,
                    TotalRooms = b.BookingRooms.Count, // Đếm số phòng
                    IsPaid = b.Payments.Any(p => p.Status == "Completed"),
                    HasReviewed = _context.Reviews.Any(r => r.BookingId == b.Id && r.IsDeleted == false)
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(list);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<BookingListItemDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<BookingShortDTO>> GetUpcomingTripAsync(int userId)
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Logic: Lấy đơn hàng SẮP TỚI (CheckIn >= Hôm nay) 
            // Trạng thái phải là đã xác nhận/đã thanh toán
            // Sắp xếp ngày gần nhất lên đầu -> Lấy 1 cái
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Hotel)
                .Include(b => b.BookingRooms).ThenInclude(br => br.RoomType)
                .Where(b => b.CustomerId == userId
                            && (b.Status == "Confirmed" || b.Status == "Paid" || b.Status == "CheckedIn")
                            && b.CheckInDate >= today)
                .OrderBy(b => b.CheckInDate)
                .Select(b => new BookingShortDTO
                {
                    Id = b.Id,
                    GuestName = b.ContactName,
                    HotelName = b.Hotel.Name,
                    HotelImage = b.Hotel.CoverImageUrl, // Map ảnh bìa

                    // Tính số ngày còn lại (CheckIn - Hôm nay)
                    TotalDays = b.CheckInDate.DayNumber - today.DayNumber,

                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt ?? DateTime.MinValue,
                    CheckInDate = b.CheckInDate.ToDateTime(TimeOnly.MinValue),
                    CheckOutDate = b.CheckOutDate.ToDateTime(TimeOnly.MinValue),

                    // Nối tên các loại phòng: "Deluxe, Standard"
                    RoomTypeNames = string.Join(", ", b.BookingRooms.Select(br => br.RoomType.Name).Distinct())
                })
                .FirstOrDefaultAsync();

            // Nếu không có đơn nào thì trả về Null (Frontend sẽ ẩn Widget đi)
            // Vẫn là Success code 200 nhưng Content là null
            return ApiResponseHelper.Ok(booking);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<BookingShortDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<BookingDetailDTO>> GetBookingDetailForUserAsync(int bookingId, int userId)
    {
        try
        {
            // Query Booking và Include tất cả các bảng con
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Hotel)
                .Include(b => b.BookingRooms).ThenInclude(br => br.Room)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.RoomType)
                    .ThenInclude(rt => rt.RoomTypeServices) // Giả sử bảng trung gian là RoomTypeServices
                    .ThenInclude(rts => rts.Service)
                .Include(b => b.BookingRooms).ThenInclude(br => br.SelectedBedType)
                .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.IsDeleted == false);

            if (booking == null) return ApiResponseHelper.NotFound<BookingDetailDTO>("Đơn đặt phòng không tồn tại.");

            // Bảo mật: Chỉ cho phép xem nếu là chính chủ (User) hoặc là Chủ KS/Lễ tân (Owner/Staff)
            // (Nếu dùng chung hàm này cho cả Owner xem thì thêm check CanOperateHotelAsync)
            bool isOwnerOrStaff = await _authService.CanOperateHotelAsync(userId, booking.HotelId);
            if (booking.CustomerId != userId && !isOwnerOrStaff)
                return ApiResponseHelper.Forbidden<BookingDetailDTO>("Bạn không có quyền xem đơn hàng này.");

            // Mapping
            var result = new BookingDetailDTO
            {
                Id = booking.Id,
                HotelId = booking.HotelId,
                HotelName = booking.Hotel.Name,
                HotelAddress = booking.Hotel.Address,
                HotelImage = booking.Hotel.CoverImageUrl,

                ContactName = booking.ContactName,
                ContactPhone = booking.ContactPhone,
                ContactEmail = booking.ContactEmail,
                Note = booking.Note,

                CheckIn = booking.CheckInDate.ToDateTime(TimeOnly.MinValue),
                CheckOut = booking.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                TotalNights = booking.CheckOutDate.DayNumber - booking.CheckInDate.DayNumber,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt ?? DateTime.MinValue,

                // Map List Rooms
                Rooms = booking.BookingRooms.Select(br => new BookingRoomDetailDTO
                {
                    Id = br.Id,
                    RoomTypeId = br.RoomTypeId,
                    RoomTypeName = br.RoomType.Name,
                    PricePerNight = br.PricePerNight,
                    BedTypeName = br.SelectedBedType != null ? br.SelectedBedType.Name : "Tiêu chuẩn",

                    RoomId = br.RoomId,
                    RoomNumber = br.Room != null ? br.Room.RoomNumber : "Chưa xếp phòng",
                    GuestName = br.GuestName ?? "Chưa cập nhật",

                    // Nếu muốn tính giá riêng từng phòng
                    Price = br.PricePerNight * (booking.CheckOutDate.DayNumber - booking.CheckInDate.DayNumber),

                    IncludedServices = br.RoomType?.RoomTypeServices?
                        .Select(rts => rts.Service.Name)
                        .ToList() ?? new List<string>()
                }).ToList(),

                // Map List Services
                Services = booking.BookingServices.Select(bs => new BookingServiceDTO
                {
                    Id = bs.Id,
                    ServiceId = bs.ServiceId,
                    ServiceName = bs.Service.Name,
                    Description = bs.Service.Description,
                    Price = bs.Price,
                    Quantity = bs.Quantity ?? 0,
                    IsPaid = bs.IsPaid ?? false,
                    // Unit lấy từ đâu cũng được, tạm thời bỏ qua hoặc query thêm
                }).ToList()
            };

            if (result.TotalNights <= 0) result.TotalNights = 1;

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<BookingDetailDTO>(ex.Message);
        }
    }

    // 1. Create Booking
    public async Task<ApiResponse<BookingResponseDTO>> CreateBookingAsync(BookingCreateDTO request, int userId)
    {
        if (request.CheckIn >= request.CheckOut)
            return ApiResponseHelper.BadRequest<BookingResponseDTO>("Ngày trả phòng phải sau ngày nhận phòng.");

        using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        try
        {
            await _context.SaveChangesAsync();
            // Check từng loại phòng trong vòng lặp
            foreach (var item in request.SelectedRooms)
            {
                // 1. Đếm tổng số phòng vật lý của loại này (đang hoạt động)
                var totalPhysicalRooms = await _context.Rooms
                    .CountAsync(r => r.RoomTypeId == item.RoomTypeId && r.IsDeleted == false && r.Status != "Maintenance");

                // 2. Đếm tổng số booking (Confirmed/CheckedIn/Pending) bị trùng lịch với đơn vừa tạo
                // (Bao gồm cả chính đơn hàng booking.Id vừa insert ở trên)
                var overlappingBookings = await _context.BookingRooms
                    .Where(br => br.RoomTypeId == item.RoomTypeId)
                    .Where(br => br.Booking.Status != "Cancelled" && br.Booking.Status != "Refunded" && br.Booking.IsDeleted == false)
                    .Where(br => br.Booking.CheckInDate < request.CheckOut && br.Booking.CheckOutDate > request.CheckIn)
                    .CountAsync();

                // 3. Nếu số lượng đã đặt > Tổng số phòng -> Rollback
                if ((overlappingBookings + item.Quantity) > totalPhysicalRooms)
                {
                    throw new Exception($"Rất tiếc, loại phòng {item.RoomTypeId} không đủ số lượng trống.");
                }
            }

            // áp dụng khuyens mãi
            var priceRequest = new PriceCalculationDTO
            {
                HotelId = request.HotelId,
                UserId = userId,
                CheckInDate = request.CheckIn.ToDateTime(TimeOnly.MinValue),
                CheckOutDate = request.CheckOut.ToDateTime(TimeOnly.MinValue),
                VoucherCode = request.VoucherCode, // FE phải gửi mã này lên

                // Map SelectedRooms
                SelectedRooms = request.SelectedRooms.Select(r => new BookingRoomDetailRequestDTO
                {
                    RoomTypeId = r.RoomTypeId,
                    Quantity = r.Quantity
                }).ToList(),

                // Map Services
                SelectedServices = request.Services?.Select(s => new BookingServiceRequestDTO
                {
                    ServiceId = s.ServiceId,
                    Quantity = s.Quantity
                }).ToList() ?? new()
            };

            // Gọi hàm tính toán tập trung (Centralized Logic)
            var priceResultResponse = await CalculateBookingPriceAsync(priceRequest);

            if (priceResultResponse.StatusCode != StatusCodeResponse.Success || priceResultResponse.Content == null)
            {
                throw new Exception("Lỗi khi tính toán giá phòng: " + priceResultResponse.Message);
            }

            var priceData = priceResultResponse.Content;

            string paymentMethod = string.IsNullOrEmpty(request.PaymentMethod) ? "Direct" : request.PaymentMethod;
            string bookingStatus = paymentMethod == "Direct" ? "Confirmed" : "PendingPayment";

            // 2. Tạo Booking Master
            var booking = new Booking
            {
                CustomerId = userId,
                HotelId = request.HotelId,
                CheckInDate = request.CheckIn,
                CheckOutDate = request.CheckOut,
                Status = bookingStatus, // Mặc định chờ thanh toán
                ContactName = request.ContactName,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                Note = request.Note,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                PromotionId = priceData.AppliedPromotionId,
                DiscountAmount = priceData.DiscountAmount,
                TotalPrice = priceData.FinalTotal
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(); // Để lấy booking.Id


            // 3. Tạo BookingRooms (TÁCH DÒNG)
            foreach (var item in request.SelectedRooms)
            {
                // Lấy giá hiện tại từ DB (Snapshot Price)
                var roomType = await _context.RoomTypes.FindAsync(item.RoomTypeId);
                if (roomType == null || roomType.HotelId != request.HotelId)
                    throw new Exception("Loại phòng không hợp lệ.");
                decimal currentPrice = roomType?.PricePerNight ?? 0;
                // Loop theo số lượng khách đặt (Quantity)
                for (int i = 0; i < item.Quantity; i++)
                {
                    var roomDetail = new BookingRoom
                    {
                        BookingId = booking.Id,
                        RoomTypeId = item.RoomTypeId,
                        RoomId = null, // Chưa xếp phòng
                        PricePerNight = roomType.PricePerNight, // Lưu giá tại thời điểm đặt
                        SelectedBedTypeId = item.SelectedBedTypeId,
                        Quantity = 1,  // Luôn là 1 vì đã tách dòng
                        GuestName = request.ContactName, // Mặc định lấy tên người đặt, sau này update tên người ở sau
                        CreatedAt = DateTime.Now
                    };
                    _context.BookingRooms.Add(roomDetail);
                }
            }

            if (request.Services != null && request.Services.Any())
            {
                foreach (var sReq in request.Services)
                {
                    // Lấy giá cấu hình tại khách sạn
                    var hotelService = await _context.HotelServiceConfigs
                        .FirstOrDefaultAsync(hs => hs.HotelId == request.HotelId && hs.ServiceId == sReq.ServiceId);

                    if (hotelService != null && (hotelService.IsActive ?? true))
                    {
                        _context.BookingServices.Add(new BookingService
                        {
                            BookingId = booking.Id,
                            ServiceId = sReq.ServiceId,
                            Quantity = sReq.Quantity,
                            Price = hotelService.Price, // Giá tại thời điểm đặt
                            IsPaid = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (paymentMethod == "Direct")
            {
                booking.Status = "Confirmed";
                if (priceData.AppliedPromotionId.HasValue)
                {
                    await IncrementPromotionUsageAsync(priceData.AppliedPromotionId);
                }

                var hotelInfo = await _context.Hotels.FindAsync(request.HotelId);
                booking.Hotel = hotelInfo;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendBookingSuccessEmailAsync(booking, booking.TotalPrice);
                    }
                    catch (Exception ex) { Console.WriteLine($"Mail Error: {ex.Message}"); }
                });
            }

            // 8. Trả về kết quả
            string msg = paymentMethod == "Direct"
                ? "Đặt phòng thành công! Email xác nhận đã được gửi."
                : "Đơn hàng đã được tạo. Vui lòng thanh toán trong 15 phút.";

            return ApiResponseHelper.Ok(new BookingResponseDTO
            {
                BookingId = booking.Id,
                TotalPrice = booking.TotalPrice,
                Message = msg
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<BookingResponseDTO>($"Lỗi đặt phòng: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ServiceAddOnDTO>>> GetAddOnServicesForBookingAsync(int hotelId)
    {
        try
        {
            // 2. Lấy tất cả dịch vụ KHÁCH SẠN đang kinh doanh (Active)
            // Lưu ý: Dùng AsNoTracking() để tối ưu tốc độ đọc
            var allHotelServices = await _context.HotelServiceConfigs
                .AsNoTracking()
                .Include(x => x.Service)
                .Where(x => x.HotelId == hotelId && x.IsActive == true)
                .Select(x => new ServiceAddOnDTO
                {
                    ServiceId = x.ServiceId,
                    Name = x.Service.Name,
                    // Giá riêng của khách sạn cấu hình
                    Price = x.Price,
                    Unit = x.Unit,
                    Description = x.Description ?? x.Service.Description
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(allHotelServices);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<ServiceAddOnDTO>>(ex.Message);
        }
    }

    // Thêm vào #region User hoặc Owner (Dùng chung logic)
    public async Task<ApiResponse<bool>> CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Payments)
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return ApiResponseHelper.NotFound<bool>("Đơn không tồn tại.");

        // Check quyền: Chính chủ đặt HOẶC là Owner/Lễ tân khách sạn đó
        bool isOwnerOrStaff = await _authService.CanOperateHotelAsync(userId, booking.HotelId);
        if (booking.CustomerId != userId && !isOwnerOrStaff)
            return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền hủy đơn này.");

        if (booking.Status == "Cancelled" || booking.Status == "Refunded")
            return ApiResponseHelper.BadRequest<bool>("Đơn này đã bị hủy trước đó.");

        // Chỉ hủy được khi chưa Check-in
        if (booking.Status == "CheckedIn" || booking.Status == "Completed")
            return ApiResponseHelper.BadRequest<bool>("Không thể hủy đơn đang ở hoặc đã hoàn tất.");

        var incomeTransaction = await _context.WalletTransactions
                .FirstOrDefaultAsync(t => t.ReferenceId == bookingId && t.TransactionType == "BookingRevenue");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (incomeTransaction != null)
            {
                // Nghĩa là Owner đã nhận tiền rồi -> Giờ phải trừ lại (Chargeback)

                // Tìm ví Owner
                var wallet = await _context.OwnerWallets.FirstOrDefaultAsync(w => w.OwnerId == booking.Hotel.OwnerId);

                if (wallet != null)
                {
                    // A. Lấy lại tiền doanh thu (Trừ tiền)
                    // incomeTransaction.Amount là số dương (VD: +1.000.000)
                    // Giờ ta tạo giao dịch âm (VD: -1.000.000)
                    var refundAmount = -incomeTransaction.Amount;

                    // B. Hoàn lại phí sàn (ChillZone trả lại hoa hồng cho Owner vì đơn bị hủy)
                    // Tìm transaction phí sàn cũ
                    var feeTransaction = await _context.WalletTransactions
                        .FirstOrDefaultAsync(t => t.ReferenceId == bookingId && t.TransactionType == "CommissionFee");

                    decimal feeReversal = 0;
                    if (feeTransaction != null)
                    {
                        // feeTransaction.Amount là số âm (VD: -100.000)
                        // Giờ hoàn lại là số dương (VD: +100.000) -> Đảo dấu trừ thành cộng
                        feeReversal = -feeTransaction.Amount;
                    }

                    // C. Tính toán tác động lên số dư
                    // Net deduction = (-1tr) + (+100k) = -900k
                    wallet.Balance += (refundAmount + feeReversal);
                    wallet.TotalEarnings += refundAmount; // Giảm tổng thu nhập
                    wallet.UpdatedAt = DateTime.Now;
                    // Lưu ý: C# cho phép decimal âm, nên nếu Balance < 0 nó tự ghi âm -> Ghi nợ thành công.

                    // D. Ghi log giao dịch đảo (Reversal Logs)
                    _context.WalletTransactions.Add(new WalletTransaction
                    {
                        WalletId = wallet.Id,
                        Amount = refundAmount, // Số âm
                        TransactionType = "RefundDeduction",
                        Description = $"Hoàn tiền/Hủy đơn hàng #{bookingId}",
                        ReferenceId = bookingId,
                        CreatedAt = DateTime.Now
                    });

                    if (feeReversal > 0)
                    {
                        _context.WalletTransactions.Add(new WalletTransaction
                        {
                            WalletId = wallet.Id,
                            Amount = feeReversal, // Số dương
                            TransactionType = "CommissionReversal", // Hoàn phí
                            Description = $"Hoàn lại phí dịch vụ đơn #{bookingId}",
                            ReferenceId = bookingId,
                            CreatedAt = DateTime.Now.AddSeconds(1)
                        });
                    }
                }
            }

            // Tính tổng tiền đã thanh toán thành công
            var totalPaid = booking.Payments
                .Where(p => p.Status == "Completed" || p.Status == "Success")
                .Sum(p => p.Amount);

            if (totalPaid > 0)
            {
                // CASE A: Khách đã trả tiền -> Không hủy ngay -> Chuyển sang chờ hoàn tiền
                // Bạn nên thêm trạng thái "PendingRefund" vào hệ thống status
                booking.Status = "PendingRefund";
                booking.Note += $" [Đã trừ tiền ví Owner. Cần hoàn {totalPaid:N0} cho khách.]";

            }
            else
            {
                // CASE B: Khách chưa trả tiền -> Hủy thoải mái
                booking.Status = "Cancelled";
            }

            booking.UpdatedAt = DateTime.Now;

            // Logic nhả phòng vật lý (nếu lỡ đã xếp phòng rồi)
            // Tìm các BookingRoom đã gán RoomId
            var assignedRooms = await _context.BookingRooms
                .Include(br => br.Room)
                .Where(br => br.BookingId == bookingId && br.RoomId != null)
                .ToListAsync();

            foreach (var br in assignedRooms)
            {
                // Trả trạng thái phòng vật lý về Available (nếu nó đang không có ai khác ở)
                if (br.Room != null && br.Room.Status != "Maintenance")
                {
                    br.Room.Status = "Available";
                }
                br.RoomId = null; // Gỡ link
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Hủy đặt phòng thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>($"Lỗi khi hủy đơn: {ex.Message}");
        }
    }
    #endregion

    #region Confirm Payment
    public async Task<ApiResponse<bool>> ConfirmBookingPaymentAsync(PaymentRequestDTO request, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Lấy đơn hàng
            var booking = await _context.Bookings
            .Include(b => b.Hotel) // Include Hotel để lấy tên cho vào Email
            .FirstOrDefaultAsync(b => b.Id == request.BookingId);
            if (booking == null) return ApiResponseHelper.NotFound<bool>("Đơn hàng không tồn tại.");

            // Check quyền (nếu cần): if (booking.CustomerId != userId) ...

            if (booking.Status != "PendingPayment")
                return ApiResponseHelper.BadRequest<bool>("Đơn hàng này không ở trạng thái chờ thanh toán.");

            // 2. Tạo bản ghi thanh toán
            var payment = new Payment
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                TransactionId = string.IsNullOrEmpty(request.TransactionId)
                            ? $"TRANS_{DateTime.Now.Ticks}" // Fallback nếu null (tránh lỗi DB)
                            : request.TransactionId,

                Status = "Completed",
                PaidAt = DateTime.Now,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                UpdatedBy = null,
            };
            _context.Payments.Add(payment);

            // 3. Cập nhật trạng thái Booking
            // Nếu trả đủ tiền -> Confirmed. Nếu trả 1 phần -> Có thể vẫn Confirmed nhưng ghi nợ.
            booking.Status = "Confirmed";
            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // 4. GỬI MAIL XÁC NHẬN
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendBookingSuccessEmailAsync(booking, booking.TotalPrice);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });

            return ApiResponseHelper.Ok(true, "Thanh toán thành công. Đơn phòng đã được xác nhận.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<InvoiceDTO>> GetInvoicePreviewAsync(int bookingId, int requesterId)
    {
        try
        {
            // 1. Query dữ liệu (Include BookingRooms, Services, Payments)
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.BookingRooms).ThenInclude(br => br.Room)     // <--- Lấy thông tin Phòng vật lý
                .Include(b => b.BookingRooms).ThenInclude(br => br.RoomType) // <--- QUAN TRỌNG: Lấy thông tin Loại phòng (để lấy Name)
                .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return ApiResponseHelper.NotFound<InvoiceDTO>("Đơn không tồn tại.");

            // Check quyền Owner/Staff
            if (!await _authService.CanOperateHotelAsync(requesterId, booking.HotelId))
                return ApiResponseHelper.Forbidden<InvoiceDTO>();

            // 2. Tính tiền phòng thực tế
            var bookingRooms = booking.BookingRooms ?? new List<BookingRoom>();
            var bookingServices = booking.BookingServices ?? new List<BookingService>();
            var payments = booking.Payments ?? new List<Payment>();

            int nights = booking.CheckOutDate.DayNumber - booking.CheckInDate.DayNumber;
            if (nights <= 0) nights = 1;

            decimal roomTotal = booking.BookingRooms.Sum(br => br.PricePerNight * nights * br.Quantity);

            // 3. Tính tiền dịch vụ (Chưa thanh toán)
            // Nếu IsPaid = true thì không cộng vào tổng thu nữa (vì khách trả rồi)
            var servicesDto = booking.BookingServices.Select(bs => new BookingServiceDTO
            {
                Id = bs.Id,
                ServiceId = bs.ServiceId,
                ServiceName = bs.Service.Name,
                Price = bs.Price,
                Quantity = bs.Quantity ?? 0,
                IsPaid = bs.IsPaid ?? false,
                // Total tự tính trong DTO: Price * Quantity
            }).ToList();

            decimal serviceTotalUnpaid = servicesDto.Where(s => !s.IsPaid).Sum(s => s.Total);

            // 4. Tổng cần thanh toán (Grand Total)
            decimal grandTotal = roomTotal + serviceTotalUnpaid;

            // 5. Đã thanh toán (Cọc) - Lấy từ bảng Payments (Completed)
            decimal paidAmount = booking.Payments
                .Where(p => p.Status == "Completed")
                .Sum(p => p.Amount);

            // 6. Còn lại phải thu
            decimal remaining = grandTotal - paidAmount;

            var invoice = new InvoiceDTO
            {
                BookingId = booking.Id,
                GuestName = booking.ContactName ?? "Khách vãng lai",
                PhoneNumber = booking.ContactPhone,
                CheckIn = booking.CheckInDate.ToDateTime(new TimeOnly(14, 0)),
                CheckOut = booking.CheckOutDate.ToDateTime(new TimeOnly(12, 0)),
                RoomDetails = booking.BookingRooms.Select(br => new BookingRoomDetailDTO
                {
                    RoomId = br.RoomId,
                    RoomNumber = br.Room != null ? br.Room.RoomNumber : "Chưa xếp",
                    RoomTypeName = (br.RoomType != null) ? br.RoomType.Name : "Loại phòng cũ",
                    Price = br.PricePerNight,
                }).ToList() ?? new List<BookingRoomDetailDTO>(),

                TotalNights = nights,
                Services = servicesDto,
                RoomTotal = roomTotal,
                ServiceTotal = serviceTotalUnpaid,
                GrandTotal = grandTotal,
                PaidAmount = paidAmount,
                RemainingAmount = remaining > 0 ? remaining : 0
            };

            return ApiResponseHelper.Ok(invoice);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<InvoiceDTO>(ex.Message);
        }
    }
    #endregion

    #region Staff Manage
    public async Task<ApiResponse<List<StaffDTO>>> GetHotelStaffsAsync(int hotelId, int ownerId)
    {
        // 1. Check quyền Owner
        if (!await _authService.IsOwnerAsync(ownerId))
            return ApiResponseHelper.Forbidden<List<StaffDTO>>();

        // 2. Query Staff join User
        var staffs = await _context.Staffs
            .AsNoTracking()
            .Include(s => s.User) // Join bảng User để lấy Tên, Email
            .Where(s => s.HotelId == hotelId) // Giả sử Staffs có cột IsDeleted chung pattern
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new StaffDTO
            {
                Id = s.Id,
                UserId = s.UserId,
                FullName = s.User.FullName!.Trim(),
                Email = s.User.Email,
                PhoneNumber = s.User.PhoneNumber,
                Position = s.Position,
                IsActive = s.IsActive,
                Salary = s.Salary,
                CreatedAt = s.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync();

        return ApiResponseHelper.Ok(staffs);
    }

    public async Task<ApiResponse<StaffDTO>> CreateStaffAsync(CreateStaffRequestDTO request, int ownerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Check quyền Owner sở hữu khách sạn
            var isOwner = await _context.Hotels.AnyAsync(h => h.Id == request.HotelId && h.OwnerId == ownerId);
            if (!isOwner) return ApiResponseHelper.Forbidden<StaffDTO>("Bạn không có quyền thêm nhân viên vào khách sạn này.");

            // 2. Kiểm tra Email đã tồn tại trong hệ thống chưa?
            // Logic đơn giản: Nếu email đã có -> Báo lỗi (hoặc logic nâng cao là Invite user cũ làm staff)
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                // Trong khuôn khổ đồ án, báo lỗi để tránh phức tạp
                return ApiResponseHelper.Conflict<StaffDTO>("Email này đã được đăng ký tài khoản. Vui lòng sử dụng email khác.");
            }

            // 3. Tạo User mới (Account để đăng nhập)
            var newUser = new User
            {
                UserName = request.Email!, // Username là Email
                Email = request.Email!.Trim(),
                FullName = request.FullName!.Trim(),
                PhoneNumber = request.PhoneNumber!,
                // Mật khẩu mặc định: Staff@123 (Nên mã hóa Bcrypt)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // Lưu để lấy Id

            var userRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = RoleTypeConstDTO.Staff // Dùng hằng số 4
            };
            _context.UserRoles.Add(userRole);

            // 4. Tạo Staff (Profile làm việc)
            var newStaff = new Staff
            {
                UserId = newUser.Id,
                HotelId = request.HotelId,
                Position = request.Position, // "Receptionist", "Housekeeper"
                Salary = request.Salary,
                IsActive = true,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Staffs.Add(newStaff);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(new StaffDTO
            {
                Id = newStaff.Id,
                UserId = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Position = newStaff.Position,
                IsActive = true
            }, "Tạo nhân viên thành công. Mật khẩu mặc định: Staff@123");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<StaffDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> UpdateStaffAsync(int staffId, UpdateStaffRequestDTO request, int ownerId)
    {
        var staff = await _context.Staffs.Include(s => s.Hotel).FirstOrDefaultAsync(s => s.Id == staffId);
        if (staff == null || staff.Hotel.OwnerId != ownerId) return ApiResponseHelper.Forbidden<bool>();

        if (!string.IsNullOrEmpty(request.Position)) staff.Position = request.Position;
        if (request.Salary.HasValue) staff.Salary = request.Salary;
        staff.IsActive = request.IsActive;

        // Đồng bộ trạng thái user (nếu staff bị khóa -> user cũng bị khóa)
        var user = await _context.Users.FindAsync(staff.UserId);
        if (user != null) user.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return ApiResponseHelper.Ok(true);
    }

    public async Task<ApiResponse<bool>> DeleteStaffAsync(int staffId, int ownerId)
    {
        var staff = await _context.Staffs.Include(s => s.Hotel).FirstOrDefaultAsync(s => s.Id == staffId);
        if (staff == null || staff.Hotel.OwnerId != ownerId) return ApiResponseHelper.Forbidden<bool>();

        staff.IsDeleted = true; // Soft delete staff

        // Khóa tài khoản đăng nhập luôn
        var user = await _context.Users.FindAsync(staff.UserId);
        if (user != null) user.IsActive = false; // Chặn login

        await _context.SaveChangesAsync();
        return ApiResponseHelper.Ok(true);
    }
    #endregion

    #region Housekeeping
    public async Task<ApiResponse<bool>> ProcessCheckoutPaymentAsync(PaymentRequestDTO request, int requesterId)
    {
        // Bắt đầu Transaction để đảm bảo: Tiền thu được thì Phòng phải nhả ra.
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Lấy dữ liệu
            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .Include(b => b.BookingRooms).ThenInclude(br => br.Room) // Lấy phòng vật lý để đổi trạng thái
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null) return ApiResponseHelper.NotFound<bool>("Đơn không tồn tại.");

            // Check quyền
            if (!await _authService.CanOperateHotelAsync(requesterId, booking.HotelId))
                return ApiResponseHelper.Forbidden<bool>();

            string finalTransId = request.TransactionId;
            if (string.IsNullOrEmpty(finalTransId))
            {
                finalTransId = $"POS-{DateTime.Now:yyyyMMddHHmmss}";
            }
            // XỬ LÝ THANH TOÁN 
            if (request.Amount > 0)
            {
                var payment = new Payment
                {
                    BookingId = request.BookingId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    Status = "Completed",
                    TransactionId = finalTransId, // Dùng mã đã xử lý
                    PaidAt = DateTime.Now,
                    CreatedBy = requesterId,
                    CreatedAt = DateTime.Now
                };
                _context.Payments.Add(payment);
            }

            // Đánh dấu tất cả dịch vụ là "Đã thanh toán"
            foreach (var sv in booking.BookingServices)
            {
                sv.IsPaid = true;
            }

            // XỬ LÝ TRẢ PHÒNG

            // Lặp qua từng phòng trong đơn đặt này (vì 1 booking có thể đặt nhiều phòng)
            foreach (var br in booking.BookingRooms)
            {
                if (br.RoomId != null && br.Room != null)
                {
                    br.Room.Status = "Cleaning";
                    br.Room.UpdatedAt = DateTime.Now;

                    // B. Kiểm tra xem đã có task dọn dẹp nào chưa (Tránh spam task)
                    var existingTask = await _context.HousekeepingTasks
                        .AnyAsync(t => t.RoomId == br.Room.Id
                            && (t.Status == "Pending" || t.Status == "Assigned" || t.Status == "Cleaning"));

                    if (!existingTask)
                    {
                        var bestCandidate = await _context.Staffs
                            .Where(s => s.HotelId == booking.HotelId &&
                                        s.Position == "Housekeeper" &&
                                        s.IsActive == true &&
                                        s.IsDeleted == false)
                            .Select(s => new
                            {
                                s.Id,
                                PendingTasks = s.HousekeepingTasks.Count(t => t.Status != "Completed")
                            })
                            .OrderBy(s => s.PendingTasks) // Ai ít việc nhất thì giao
                            .FirstOrDefaultAsync();

                        // D. Tạo Task Housekeeping mới
                        var newTask = new HousekeepingTask
                        {
                            HotelId = booking.HotelId,
                            RoomId = br.Room.Id,
                            StaffId = bestCandidate?.Id, // Gán luôn nếu tìm thấy nhân viên
                            Status = (bestCandidate?.Id != null) ? "Assigned" : "Pending",
                            Priority = "High", // Khách vừa trả phòng -> Ưu tiên dọn để bán tiếp
                            Note = (bestCandidate?.Id != null)
                                   ? $"Khách {booking.ContactName} trả phòng. Cần dọn dẹp."
                                   : $"Khách {booking.ContactName} trả phòng. Chưa tìm thấy nhân viên.",
                            AssignedAt = (bestCandidate?.Id != null) ? DateTime.Now : null,
                            CreatedAt = DateTime.Now
                        };
                        _context.HousekeepingTasks.Add(newTask);
                    }
                }
            }

            // CẬP NHẬT TRẠNG THÁI BOOKING
            booking.Status = "Completed";
            booking.UpdatedAt = DateTime.Now;

            // 1. Cấu hình mức hoa hồng (Ví dụ 10%)
            // (Thực tế nên lưu CommissionRate trong bảng Hotels hoặc SystemConfig)
            decimal commissionRate = 0.10m;

            // Tổng tiền đơn hàng
            decimal totalBookingValue = booking.TotalPrice;

            // Tiền sàn thu (Commission)
            decimal platformFee = totalBookingValue * commissionRate;

            // Tiền thực nhận của Owner
            decimal ownerIncome = totalBookingValue - platformFee;

            // 2. Tìm hoặc tạo Ví cho Owner
            var wallet = await _context.OwnerWallets.FirstOrDefaultAsync(w => w.OwnerId == booking.Hotel.OwnerId);
            if (wallet == null)
            {
                wallet = new OwnerWallet
                {
                    OwnerId = booking.Hotel.OwnerId,
                    Balance = 0,
                    TotalEarnings = 0,
                    UpdatedAt = DateTime.Now
                };
                _context.OwnerWallets.Add(wallet);
                await _context.SaveChangesAsync(); // Lưu để lấy Id
            }

            // 3. Cộng tiền vào ví
            wallet.Balance += ownerIncome;
            wallet.TotalEarnings += ownerIncome;
            wallet.UpdatedAt = DateTime.Now;

            // 4. Ghi lịch sử giao dịch (Transaction Logs)
            // a. Ghi nhận Doanh thu tổng (+)
            _context.WalletTransactions.Add(new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = totalBookingValue,
                TransactionType = "BookingRevenue",
                Description = $"Doanh thu đơn hàng #{booking.Id}",
                ReferenceId = booking.Id,
                CreatedAt = DateTime.Now
            });

            // b. Ghi nhận Phí sàn (-)
            _context.WalletTransactions.Add(new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = -platformFee,
                TransactionType = "CommissionFee",
                Description = $"Phí dịch vụ ChillZone (10%) đơn #{booking.Id}",
                ReferenceId = booking.Id,
                CreatedAt = DateTime.Now.AddSeconds(1) // trick để nó hiện sau dòng doanh thu
            });

            // Lưu tất cả thay đổi vào DB cùng lúc
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Thanh toán và trả phòng thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>($"Lỗi xử lý: {ex.Message}");
        }
    }


    public async Task<ApiResponse<List<HousekeepingTaskDTO>>> GetHousekeepingTasksAsync(int hotelId, int requesterId, int? staffId = null)
    {
        // 1. Lấy thông tin Staff của người đang request
        var requesterStaff = await _context.Staffs
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == requesterId && s.HotelId == hotelId && s.IsActive && s.IsDeleted == false);

        bool isOwnerOrAdmin = await _authService.IsAdminOrOwnerAsync(requesterId);

        bool canViewAll = isOwnerOrAdmin;

        // Nếu là Staff, check chức vụ
        if (!isOwnerOrAdmin)
        {
            if (requesterStaff == null)
                return ApiResponseHelper.Forbidden<List<HousekeepingTaskDTO>>("Bạn không thuộc khách sạn này.");

            // Manager và Receptionist được xem hết
            if (requesterStaff.Position == "Manager" || requesterStaff.Position == "Receptionist")
            {
                canViewAll = true;
            }
        }
        var query = _context.HousekeepingTasks
            .AsNoTracking()
            .Include(t => t.Room)
            .Include(t => t.Staff).ThenInclude(s => s.User)
            .Where(t => t.HotelId == hotelId);

        // Nếu có staffId (Nhân viên xem việc của mình)
        if (!canViewAll)
        {
            // Housekeeper chỉ xem việc của mình
            query = query.Where(t => t.StaffId == requesterStaff!.Id);
        }
        else if (staffId.HasValue)
        {
            // Manager muốn lọc theo nhân viên cụ thể
            query = query.Where(t => t.StaffId == staffId);
        }

        // Lấy task chưa xong hoặc mới xong trong vòng 24h
        query = query.Where(t => t.Status != "Completed" || t.CompletedAt > DateTime.Now.AddHours(-24));

        var tasks = await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .Select(t => new HousekeepingTaskDTO
            {
                Id = t.Id,
                RoomId = t.RoomId,
                RoomNumber = t.Room.RoomNumber,
                RoomStatus = t.Room.Status,
                StaffId = t.StaffId,
                StaffName = t.Staff != null ? t.Staff.User.FullName : "Chưa giao",
                TaskStatus = t.Status,
                Priority = t.Priority,
                Note = t.Note,
                CreatedAt = t.CreatedAt ?? DateTime.MinValue,
                CompletedAt = t.CompletedAt
            })
            .ToListAsync();

        return ApiResponseHelper.Ok(tasks);
    }

    public async Task<ApiResponse<bool>> AssignHousekeepingTaskAsync(AssignTaskRequestDTO request, int ownerId)
    {
        try
        {
            var task = await _context.HousekeepingTasks.FindAsync(request.TaskId);
            if (task == null) return ApiResponseHelper.NotFound<bool>("Công việc không tồn tại.");

            // Check quyền Owner
            if (!await _authService.CanOperateHotelAsync(ownerId, task.HotelId)) return ApiResponseHelper.Forbidden<bool>();
            // Check nhân viên
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == request.StaffId && s.HotelId == task.HotelId);
            if (staff == null) return ApiResponseHelper.BadRequest<bool>("Nhân viên không hợp lệ.");

            task.StaffId = request.StaffId;
            task.Status = "Assigned";
            task.Priority = request.Priority;
            if (!string.IsNullOrEmpty(request.Note)) task.Note = request.Note;
            task.AssignedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return ApiResponseHelper.Ok(true, $"Đã giao cho {staff.User?.FullName ?? "nhân viên"}");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> UpdateTaskStatusAsync(UpdateTaskStatusRequestDTO request, int requesterId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {

            var task = await _context.HousekeepingTasks
                .Include(t => t.Room)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId);

            var requesterStaff = await _context.Staffs.FirstOrDefaultAsync(s => s.UserId == requesterId && s.HotelId == task.HotelId);
            bool isManager = await _authService.CanOperateHotelAsync(requesterId, task.HotelId);

            if (!isManager && (requesterStaff == null || task.StaffId != requesterStaff.Id))
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền cập nhật công việc này.");

            var reporterName = await _context.Users
                .Where(u => u.Id == requesterId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync() ?? $"ID {requesterId}"; // Fallback nếu không tìm thấy

            // === CASE 1: BÁO HỎNG (MAINTENANCE) ===
            if (request.Status == "Maintenance")
            {
                // Cập nhật Task: Đã báo cáo
                task.Status = "ReportedIssue";
                task.Note = $"{request.IssueDescription ?? "Không rõ lý do"} (Báo bởi: {reporterName})";
                task.CompletedAt = DateTime.Now; // Kết thúc phiên dọn dẹp này (dù chưa sạch)

                // Cập nhật Phòng: BẢO TRÌ (Chặn khách mới)
                task.Room.Status = "Maintenance";
            }
            else // === CASE 2: DỌN DẸP BÌNH THƯỜNG ===
            {
                task.Status = request.Status; // Cleaning / Completed

                if (request.Status == "Cleaning")
                {
                    task.StartedAt = DateTime.Now;
                }
                else if (request.Status == "Completed")
                {
                    task.CompletedAt = DateTime.Now;

                    // Dọn xong -> Phòng Sạch -> Sẵn sàng đón khách
                    task.Room.Status = "Available";
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ApiResponseHelper.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }
    #endregion

    #region Reviews
    public async Task<ApiResponse<List<ReviewDTO>>> GetReviewsByHotelAsync(int hotelId)
    {
        try
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId && r.IsDeleted == false)
                .Include(r => r.Customer) // Join bảng User
                .Include(r => r.Booking)
                    .ThenInclude(b => b.BookingRooms)
                    .ThenInclude(br => br.RoomType) // Join sâu để lấy RoomType
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    BookingId = r.BookingId,

                    // Map thông tin khách
                    CustomerName = r.Customer.FullName ?? "Khách vãng lai",
                    // Giả sử bảng User có cột AvatarUrl, nếu chưa có thì để null hoặc placeholder
                    CustomerAvatar = r.Customer.AvatarUrl,

                    // Map nội dung review
                    Rating = r.Rating ?? 0,
                    Comment = r.Comment ?? "",
                    Reply = r.Reply,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue,

                    // Map thông tin phòng (Lấy phòng đầu tiên trong đơn đặt)
                    // Dùng toán tử điều kiện để tránh Null Reference nếu data cũ bị thiếu
                    RoomTypeName = r.Booking.BookingRooms.FirstOrDefault() != null
                        ? r.Booking.BookingRooms.First().RoomType.Name
                        : "Phòng tiêu chuẩn",

                    // Tính số đêm đã ở (Optional)
                    StayNights = (r.Booking.CheckOutDate.DayNumber - r.Booking.CheckInDate.DayNumber)
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(reviews);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<ReviewDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> ReplyReviewAsync(ReplyReviewDTO request, int ownerId)
    {
        var review = await _context.Reviews
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId);

        if (review == null) return ApiResponseHelper.NotFound<bool>("Review không tồn tại.");

        // Check quyền Owner của Hotel này
        if (!await _authService.CanOperateHotelAsync(ownerId, review.HotelId))
            return ApiResponseHelper.Forbidden<bool>();

        review.Reply = request.Reply;
        review.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return ApiResponseHelper.Ok(true, "Đã lưu phản hồi.");
    }

    public async Task<ApiResponse<bool>> CreateReviewAsync(ReviewCreateDTO request, int userId)
    {
        try
        {
            // 1. Tìm đơn đặt phòng
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null)
                return ApiResponseHelper.NotFound<bool>("Đơn đặt phòng không tồn tại.");

            // 2. CHECK CHÍNH CHỦ: Người review phải là người đặt
            if (booking.CustomerId != userId)
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền đánh giá đơn này.");

            // 3. CHECK TRẠNG THÁI: Phải hoàn tất mới được review
            if (booking.Status != "Completed")
                return ApiResponseHelper.BadRequest<bool>("Bạn chỉ có thể đánh giá sau khi đã trả phòng và hoàn tất thanh toán.");

            // 4. CHECK TRÙNG: Đã review chưa? (Dù DB đã chặn Unique, code vẫn nên check để báo lỗi thân thiện)
            bool exists = await _context.Reviews.AnyAsync(r => r.BookingId == request.BookingId && r.IsDeleted == false);
            if (exists)
                return ApiResponseHelper.Conflict<bool>("Bạn đã đánh giá chuyến đi này rồi.");

            // 5. Tạo Review
            var review = new Review
            {
                BookingId = request.BookingId,
                HotelId = booking.HotelId,   // Lấy HotelId từ Booking cho chuẩn
                CustomerId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Reviews.Add(review);

            // 6. TÍNH LẠI ĐIỂM TRUNG BÌNH CHO HOTEL (Quan trọng để hiển thị search)
            // Lưu tạm review trước để nó tính vào DB
            await _context.SaveChangesAsync();

            await UpdateHotelRatingStats(booking.HotelId); // Hàm phụ (xem bên dưới)

            return ApiResponseHelper.Ok(true, "Cảm ơn bạn đã đánh giá!");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    // Hàm phụ để cập nhật điểm số Hotel
    private async Task UpdateHotelRatingStats(int hotelId)
    {
        var stats = await _context.Reviews
            .Where(r => r.HotelId == hotelId && r.IsDeleted == false)
            .GroupBy(r => r.HotelId)
            .Select(g => new
            {
                Count = g.Count(),
                Avg = g.Average(r => r.Rating)
            })
            .FirstOrDefaultAsync();

        if (stats != null)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel != null)
            {
                hotel.ReviewCount = stats.Count;
                hotel.AverageRating = (decimal)stats.Avg; // Lưu ý ép kiểu nếu cần
                await _context.SaveChangesAsync();
            }
        }
    }
    #endregion

    #region Promotion
    // 1. Lấy danh sách (Cho Admin)
    public async Task<ApiResponse<List<PromotionDTO>>> GetAllPromotionsAsync(int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<List<PromotionDTO>>();

        try
        {
            var list = await _context.Promotions
                .AsNoTracking()
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.Id)
                .Select(p => new PromotionDTO
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    DiscountType = p.DiscountType ?? 1,
                    DiscountValue = p.DiscountValue ?? 0,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    MinBookingValue = p.MinBookingValue,

                    PromotionCategory = p.PromotionCategory ?? 0,

                    StartDate = p.StartDate ?? DateTime.MinValue,
                    EndDate = p.EndDate ?? DateTime.MinValue,

                    UsageLimit = p.UsageLimit ?? 0,
                    UsedCount = p.UsedCount ?? 0,
                    IsActive = p.IsActive ?? false
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(list);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<PromotionDTO>>(ex.Message);
        }
    }

    // 2. Tạo khuyến mãi
    public async Task<ApiResponse<PromotionDTO>> CreatePromotionAsync(PromotionCreateDTO request, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<PromotionDTO>();

        try
        {
            // Validate Code trùng
            if (await _context.Promotions.AnyAsync(p => p.Code == request.Code && p.IsDeleted == false))
                return ApiResponseHelper.Conflict<PromotionDTO>("Mã khuyến mãi này đã tồn tại.");

            var promo = new Promotion
            {
                Code = request.Code.Trim().ToUpper(),
                Name = request.Name.Trim(),
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinBookingValue = request.MinBookingValue,
                PromotionCategory = request.PromotionCategory, // 0, 1, 2, 3
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UsageLimit = request.UsageLimit,
                IsActive = true,
                IsDeleted = false
            };

            _context.Promotions.Add(promo);
            await _context.SaveChangesAsync();

            // Map lại DTO trả về (bạn tự map cho đầy đủ)
            var res = new PromotionDTO { Id = promo.Id, Code = promo.Code };
            return ApiResponseHelper.Ok(res, "Tạo khuyến mãi thành công.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PromotionDTO>(ex.Message);
        }
    }

    public async Task<ApiResponse<PromotionDTO>> UpdatePromotionAsync(int id, PromotionCreateDTO request, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<PromotionDTO>();

        try
        {
            var promo = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            if (promo == null)
                return ApiResponseHelper.NotFound<PromotionDTO>("Khuyến mãi không tồn tại.");

            // Kiểm tra trùng Code (nếu đổi code)
            var isDuplicate = await _context.Promotions
                .AnyAsync(p => p.Code == request.Code && p.Id != id && p.IsDeleted == false);

            if (isDuplicate)
                return ApiResponseHelper.Conflict<PromotionDTO>("Mã khuyến mãi này đã được sử dụng bởi chương trình khác.");

            // Cập nhật thông tin
            promo.Code = request.Code.Trim().ToUpper();
            promo.Name = request.Name.Trim();
            promo.DiscountType = request.DiscountType;
            promo.DiscountValue = request.DiscountValue;
            promo.MaxDiscountAmount = request.MaxDiscountAmount;
            promo.MinBookingValue = request.MinBookingValue;
            promo.PromotionCategory = request.PromotionCategory;
            promo.StartDate = request.StartDate;
            promo.EndDate = request.EndDate;
            promo.UsageLimit = request.UsageLimit;
            // Không update UsedCount, IsActive ở đây (IsActive dùng API Toggle riêng)

            await _context.SaveChangesAsync();

            // Map lại để trả về
            var res = new PromotionDTO
            {
                Id = promo.Id,
                Code = promo.Code,
                Name = promo.Name

            };
            return ApiResponseHelper.Ok(res, "Cập nhật thành công.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PromotionDTO>(ex.Message);
        }
    }

    // 5. Xóa khuyến mãi (Soft Delete)
    public async Task<ApiResponse<bool>> DeletePromotionAsync(int id, int adminId)
    {
        if (!await _authService.IsAdminAsync(adminId))
            return ApiResponseHelper.Forbidden<bool>();

        try
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null || promo.IsDeleted == true)
                return ApiResponseHelper.NotFound<bool>("Khuyến mãi không tồn tại.");

            promo.IsDeleted = true; // Xóa mềm
            promo.IsActive = false; // Tắt luôn cho chắc

            await _context.SaveChangesAsync();

            return ApiResponseHelper.Ok(true, "Đã xóa khuyến mãi.");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    // 3. Logic Tính Tiền & Áp dụng Khuyến mãi (CORE LOGIC)
    public async Task<ApiResponse<PriceResultDTO>> CalculateBookingPriceAsync(PriceCalculationDTO request)
    {
        var result = new PriceResultDTO { IsSuccess = true };

        try
        {
            // A. Tính số đêm
            int nights = (request.CheckOutDate.Date - request.CheckInDate.Date).Days;
            if (nights <= 0) nights = 1;

            // B. TÍNH TIỀN PHÒNG (giữ nguyên)
            decimal roomTotal = 0;
            if (request.SelectedRooms != null && request.SelectedRooms.Any())
            {
                var roomTypeIds = request.SelectedRooms.Select(r => r.RoomTypeId).ToList();
                var roomTypes = await _context.RoomTypes
                    .AsNoTracking()
                    .Where(rt => roomTypeIds.Contains(rt.Id)
                        && rt.HotelId == request.HotelId          // <<< THIẾU CÁI NÀY!
                        && rt.IsDeleted == false)
                    .ToDictionaryAsync(rt => rt.Id, rt => rt.PricePerNight);

                foreach (var item in request.SelectedRooms)
                {
                    if (roomTypes.TryGetValue(item.RoomTypeId, out var price))
                    {
                        roomTotal += price * item.Quantity * nights;
                    }

                }
            }

            // C. TÍNH TIỀN DỊCH VỤ (giữ nguyên)
            decimal serviceTotal = 0;
            if (request.SelectedServices != null && request.SelectedServices.Any())
            {
                var serviceIds = request.SelectedServices.Select(s => s.ServiceId).ToList();
                var services = await _context.HotelServiceConfigs
                    .AsNoTracking()
                    .Where(hs => hs.HotelId == request.HotelId
                        && serviceIds.Contains(hs.ServiceId)
                        && hs.IsActive == true)
                    .ToDictionaryAsync(hs => hs.ServiceId, hs => hs.Price);

                foreach (var s in request.SelectedServices)
                {
                    if (services.TryGetValue(s.ServiceId, out var price))
                    {
                        serviceTotal += price * s.Quantity;
                    }
                }
            }

            result.OriginalTotal = roomTotal + serviceTotal;
            result.FinalTotal = result.OriginalTotal;
            result.DiscountAmount = 0; // Reset

            // --- FIX: Lấy tất cả promo đang chạy (bao gồm manual để check hết hạn) ---
            var activePromos = await _context.Promotions
                .AsNoTracking()
                .Where(p => p.IsActive == true && p.IsDeleted == false)
                .Where(p => p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now) // Check hết hạn cho tất cả
                .ToListAsync();

            // --- ÁP PROMO: Hỗ trợ áp nhiều (welcome + manual) nếu user mới ---
            List<Promotion> appliedPromos = new List<Promotion>();

            // 1. Áp auto: owner/staff
            var isOwner = await _context.Hotels.AnyAsync(h => h.Id == request.HotelId && h.OwnerId == request.UserId);
            if (isOwner)
            {
                var ownerPromo = activePromos.FirstOrDefault(p => p.PromotionCategory == 1);
                if (ownerPromo != null) appliedPromos.Add(ownerPromo);
            }
            else
            {
                var isStaff = await _context.Staffs.AnyAsync(s => s.HotelId == request.HotelId && s.UserId == request.UserId && s.IsActive == true);
                if (isStaff)
                {
                    var staffPromo = activePromos.FirstOrDefault(p => p.PromotionCategory == 2);
                    if (staffPromo != null) appliedPromos.Add(staffPromo);
                }
            }

            // 2. Áp welcome nếu user mới
            var hasPreviousBooking = await _context.Bookings
                .AnyAsync(b => b.CustomerId == request.UserId && b.Status != "Cancelled");
            if (!hasPreviousBooking)
            {
                var welcomePromo = activePromos.FirstOrDefault(p => p.PromotionCategory == 3);
                if (welcomePromo != null) appliedPromos.Add(welcomePromo);
            }

            // 3. Áp manual nếu có code
            if (!string.IsNullOrEmpty(request.VoucherCode))
            {
                var code = request.VoucherCode.Trim().ToUpper();
                var manualPromo = activePromos.FirstOrDefault(p => p.Code == code && (p.PromotionCategory == 0 || p.PromotionCategory == null));
                if (manualPromo != null)
                {
                    // Validate manual (giữ nguyên code của bạn)
                    if (manualPromo.UsageLimit > 0 && manualPromo.UsedCount >= manualPromo.UsageLimit)
                    {
                        result.IsSuccess = false; result.Message = "Mã đã hết lượt sử dụng."; return ApiResponseHelper.Ok(result);
                    }
                    if (manualPromo.UserUsageLimit > 0)
                    {
                        var userUsed = await _context.Bookings.CountAsync(b => b.CustomerId == request.UserId && b.PromotionId == manualPromo.Id && b.Status != "Cancelled");
                        if (userUsed >= manualPromo.UserUsageLimit)
                        {
                            result.IsSuccess = false; result.Message = $"Bạn đã dùng mã này hết lượt."; return ApiResponseHelper.Ok(result);
                        }
                    }
                    if (manualPromo.MinBookingValue.HasValue && result.OriginalTotal < manualPromo.MinBookingValue.Value)
                    {
                        result.IsSuccess = false; result.Message = $"Đơn chưa đủ {manualPromo.MinBookingValue.Value:N0}đ."; return ApiResponseHelper.Ok(result);
                    }
                    appliedPromos.Add(manualPromo);
                }
                else
                {
                    result.IsSuccess = false; result.Message = "Mã không hợp lệ."; return ApiResponseHelper.Ok(result);
                }
            }

            // --- TÍNH GIẢM CHO TỪNG PROMO (Áp nhiều) ---
            foreach (var promo in appliedPromos)
            {
                decimal discount = 0;
                if (promo.DiscountType == 1) // %
                {
                    discount = result.OriginalTotal * (promo.DiscountValue ?? 0) / 100;
                    if (promo.MaxDiscountAmount.HasValue && discount > promo.MaxDiscountAmount.Value)
                        discount = promo.MaxDiscountAmount.Value;
                }
                else // Cash
                {
                    discount = promo.DiscountValue ?? 0;
                }

                if (discount > result.OriginalTotal) discount = result.OriginalTotal;

                result.DiscountAmount += discount; // Cộng dồn
                result.FinalTotal -= discount;

                // (Optional) Lưu promo áp dụng (nếu cần nhiều, dùng list)
                result.AppliedPromotionId = promo.Id; // Chỉ lưu 1 cho đơn giản, hoặc dùng list nếu cần
                result.Message += $"Áp dụng {promo.Name}: -{discount:N0}đ. ";
            }

            if (result.FinalTotal < 0) result.FinalTotal = 0;

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PriceResultDTO>(ex.Message);
        }
    }

    // helper:
    private async Task IncrementPromotionUsageAsync(int? promotionId)
    {
        if (promotionId.HasValue)
        {
            var promo = await _context.Promotions.FindAsync(promotionId.Value);
            if (promo != null)
            {
                promo.UsedCount += 1;
                // Optional: nếu vượt giới hạn → tự động tắt
                if (promo.UsageLimit > 0 && promo.UsedCount >= promo.UsageLimit)
                {
                    promo.IsActive = false;
                }
            }
        }
    }

    public async Task<ApiResponse<bool>> TogglePromotionStatusAsync(int id, int adminId)
    {
        try
        {
            // 1. Check quyền Admin
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<bool>("Bạn không có quyền thực hiện hành động này.");

            // 2. Tìm khuyến mãi
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (promotion == null)
                return ApiResponseHelper.NotFound<bool>("Chương trình khuyến mãi không tồn tại.");

            // 3. Đảo ngược trạng thái (True -> False, False -> True)
            // Lưu ý: IsActive trong DB có thể là null, nên cần xử lý ?? false
            bool currentStatus = promotion.IsActive ?? false;
            promotion.IsActive = !currentStatus;

            // 4. Lưu thay đổi
            await _context.SaveChangesAsync();

            // 5. Trả về kết quả
            // Trả về true/false đại diện cho trạng thái MỚI
            string msg = promotion.IsActive.Value ? "Đã kích hoạt khuyến mãi." : "Đã tạm dừng khuyến mãi.";
            return ApiResponseHelper.Ok(promotion.IsActive.Value, msg);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<List<PromotionDTO>>> GetAvailablePromotionsForUserAsync()
    {
        try
        {
            var today = DateTime.Now;
            var list = await _context.Promotions
                .AsNoTracking()
                .Where(p => p.IsDeleted == false && p.IsActive == true)
                .Where(p => p.PromotionCategory == 0) // Chỉ lấy Voucher thường (loại khách tự nhập)
                .Where(p => p.StartDate <= today && p.EndDate >= today) // Còn trong thời gian
                                                                        // Logic hiển thị: Có thể ẩn mã đã hết lượt dùng chung, hoặc vẫn hiện nhưng báo hết
                .Where(p => p.UsageLimit == 0 || p.UsedCount < p.UsageLimit)
                .OrderByDescending(p => p.Id)
                .Select(p => new PromotionDTO
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    DiscountType = p.DiscountType ?? 1,
                    DiscountValue = p.DiscountValue ?? 0,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    MinBookingValue = p.MinBookingValue,
                    EndDate = p.EndDate ?? DateTime.MaxValue
                })
                .ToListAsync();

            return ApiResponseHelper.Ok(list);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<List<PromotionDTO>>(ex.Message);
        }
    }
    #endregion

    #region Wallet
    // 1. Lấy thông tin ví & lịch sử
    public async Task<ApiResponse<OwnerWalletDTO>> GetOwnerWalletAsync(int ownerId)
    {
        var wallet = await _context.OwnerWallets
            .Include(w => w.WalletTransactions)
            .FirstOrDefaultAsync(w => w.OwnerId == ownerId);

        if (wallet == null) return ApiResponseHelper.Ok(new OwnerWalletDTO { Balance = 0, TotalEarnings = 0, RecentTransactions = new() });

        var transactions = wallet.WalletTransactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(50) // Lấy 50 giao dịch gần nhất
            .Select(t => new WalletTransactionDTO
            {
                Amount = t.Amount,
                Type = t.TransactionType,
                Description = t.Description,
                Date = t.CreatedAt ?? DateTime.MinValue
            }).ToList();

        return ApiResponseHelper.Ok(new OwnerWalletDTO
        {
            Balance = wallet.Balance,
            TotalEarnings = wallet.TotalEarnings,
            RecentTransactions = transactions
        });
    }

    // 2. Yêu cầu rút tiền
    public async Task<ApiResponse<bool>> RequestWithdrawalAsync(WithdrawRequestDTO request, int ownerId)
    {
        var wallet = await _context.OwnerWallets.FirstOrDefaultAsync(w => w.OwnerId == ownerId);
        if (wallet == null || wallet.Balance < request.Amount)
            return ApiResponseHelper.BadRequest<bool>("Số dư không đủ.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Trừ tiền trong ví ngay lập tức
            wallet.Balance -= request.Amount;
            wallet.UpdatedAt = DateTime.Now;

            // Tạo yêu cầu rút
            var req = new WithdrawalRequest
            {
                OwnerId = ownerId,
                WalletId = wallet.Id,
                Amount = request.Amount,
                BankName = request.BankName,
                BankAccountNumber = request.BankAccountNumber,
                BankAccountName = request.BankAccountName,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            _context.WithdrawalRequests.Add(req);
            await _context.SaveChangesAsync(); // Lấy ID

            // Ghi log giao dịch
            _context.WalletTransactions.Add(new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = -request.Amount,
                TransactionType = "Withdrawal",
                Description = $"Yêu cầu rút tiền #{req.Id}",
                ReferenceId = req.Id,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseHelper.Ok(true, "Yêu cầu rút tiền đã được gửi.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }

    // admin

    public async Task<ApiResponse<PagedResult<WithdrawalRequestAdminDTO>>> GetWithdrawalRequestsAsync(string? status, int pageIndex, int pageSize)
    {
        try
        {
            var query = _context.WithdrawalRequests
                .AsNoTracking()
                .Include(w => w.Owner)  // Join lấy tên chủ khách sạn
                .Include(w => w.Wallet) // Join lấy số dư ví hiện tại
                .AsQueryable();

            // 1. Lọc theo trạng thái (nếu có)
            if (!string.IsNullOrEmpty(status))
            {
                // Chuẩn hóa string để so sánh không phân biệt hoa thường
                query = query.Where(w => w.Status == status);
            }

            // 2. Sắp xếp: Ưu tiên đơn "Pending" lên đầu, sau đó là mới nhất
            query = query.OrderByDescending(w => w.Status == "Pending")
                         .ThenByDescending(w => w.CreatedAt);

            // 3. Phân trang
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WithdrawalRequestAdminDTO
                {
                    RequestId = w.Id,
                    OwnerId = w.OwnerId,
                    OwnerName = w.Owner.FullName ?? "N/A",
                    OwnerEmail = w.Owner.Email,

                    Amount = w.Amount,
                    // Lấy số dư hiện tại để Admin biết ví ông này còn bao nhiêu (sau khi đã trừ đơn này)
                    CurrentWalletBalance = w.Wallet.Balance,

                    BankName = w.BankName,
                    BankAccountNumber = w.BankAccountNumber,
                    BankAccountName = w.BankAccountName,

                    Status = w.Status,
                    CreatedAt = w.CreatedAt ?? DateTime.MinValue,
                    ProcessedAt = w.ProcessedAt,
                    AdminNote = w.AdminNote
                })
                .ToListAsync();

            var result = new PagedResult<WithdrawalRequestAdminDTO>
            {
                Items = items,
                TotalRecords = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponseHelper.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PagedResult<WithdrawalRequestAdminDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> ProcessWithdrawalRequestAsync(AdminProcessWithdrawalDTO request, int adminId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await _authService.IsAdminAsync(adminId))
                return ApiResponseHelper.Forbidden<bool>();

            var withdrawal = await _context.WithdrawalRequests
                .Include(w => w.Wallet) // Include Ví để hoàn tiền nếu từ chối
                .FirstOrDefaultAsync(w => w.Id == request.RequestId);

            if (withdrawal == null) return ApiResponseHelper.NotFound<bool>("Yêu cầu không tồn tại.");
            if (withdrawal.Status != "Pending") return ApiResponseHelper.BadRequest<bool>("Yêu cầu này đã được xử lý trước đó.");

            // Cập nhật thông tin xử lý
            withdrawal.ProcessedAt = DateTime.Now;
            withdrawal.ApprovedBy = adminId;
            withdrawal.AdminNote = request.AdminNote; // Lý do từ chối hoặc Mã giao dịch ngân hàng

            if (request.IsApproved)
            {
                // CASE 1: DUYỆT
                withdrawal.Status = "Approved";
                // Tiền đã trừ lúc tạo Request rồi, nên ở đây chỉ cập nhật trạng thái
            }
            else
            {
                // CASE 2: TỪ CHỐI -> PHẢI HOÀN TIỀN VÀO VÍ
                withdrawal.Status = "Rejected";

                // Cộng lại tiền vào ví
                withdrawal.Wallet.Balance += withdrawal.Amount;
                withdrawal.Wallet.UpdatedAt = DateTime.Now;

                // Ghi log giao dịch hoàn tiền
                _context.WalletTransactions.Add(new WalletTransaction
                {
                    WalletId = withdrawal.WalletId,
                    Amount = withdrawal.Amount, // Số dương (+)
                    TransactionType = "WithdrawalRefund",
                    Description = $"Hoàn tiền rút thất bại #{withdrawal.Id}. Lý do: {request.AdminNote}",
                    ReferenceId = withdrawal.Id,
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // TODO: Gửi email thông báo cho Owner (Rất quan trọng)

            return ApiResponseHelper.Ok(true, request.IsApproved ? "Đã duyệt yêu cầu rút tiền." : "Đã từ chối và hoàn tiền.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }
    #endregion

}