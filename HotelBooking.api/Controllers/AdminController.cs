using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HotelBooking.application.Helpers;
using HotelBooking.application.Services;

namespace HotelBooking.api.Controllers
{
    [Route("api/[controller]")] // URL gốc sẽ gọn hơn: api/admin/...
    [ApiController]
    [Authorize(Roles = "Admin")] // Bảo mật toàn bộ Controller này
    public class AdminController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ISystemSettingService _settingService;

        public AdminController(IHotelService hotelService, ISystemSettingService settingService)
        {
            _hotelService = hotelService;
            _settingService = settingService;
        }

        // Helper function để lấy ID Admin từ Token 
        private int GetCurrentAdminId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // service clean up
        [HttpGet("settings")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _settingService.GetAllSettingsAsync();
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> Update([FromBody] UpdateSettingRequestDTO req)
        {
            var success = await _settingService.UpdateSettingAsync(req.KeyWord, req.Value);
            if (!success) return NotFound("Không tìm thấy cấu hình.");
            return Ok(new { message = "Cập nhật thành công" });
        }

        //
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] string? keyword,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var adminId = GetCurrentAdminId(); // Hàm lấy ID admin từ Token
            var response = await _hotelService.GetCustomersForAdminAsync(keyword, pageIndex, pageSize, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 2. Lấy danh sách Chủ khách sạn (Owners)
        [HttpGet("owners")]
        public async Task<IActionResult> GetOwners(
            [FromQuery] string? keyword,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var adminId = GetCurrentAdminId();
            var response = await _hotelService.GetOwnersForAdminAsync(keyword, pageIndex, pageSize, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 3. Xem chi tiết User (Dùng chung cho cả Owner và Customer)
        // GET: api/admin/users/5
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetail(int userId)
        {
            var adminId = GetCurrentAdminId();
            var response = await _hotelService.GetUserDetailForAdminAsync(userId, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 4. Khóa/Mở khóa tài khoản (Ban/Unban)
        // PATCH: api/admin/users/5/toggle-status
        [HttpPatch("users/{userId}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            var adminId = GetCurrentAdminId();
            var response = await _hotelService.ToggleUserStatusAsync(userId, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 1. DASHBOARD & REVENUE (Thống kê)
        // ==========================================

        [HttpGet("dashboard")] // api/admin/dashboard
        public async Task<IActionResult> GetDashboardStats()
        {
            var response = await _hotelService.GetAdminDashboardStatsAsync(GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpGet("withdrawals")]
        public async Task<IActionResult> GetWithdrawalRequests(
            [FromQuery] string? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _hotelService.GetWithdrawalRequestsAsync(status, pageIndex, pageSize);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("withdrawals/process")]
        public async Task<IActionResult> ProcessWithdrawalRequest([FromBody] AdminProcessWithdrawalDTO request)
        {
            var response = await _hotelService.ProcessWithdrawalRequestAsync(request, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // [HttpPatch("users/{userId}/toggle-lock")]
        // public async Task<IActionResult> ToggleUserLock(int userId)
        // {
        //     var response = await _hotelService.ToggleUserLockAsync(userId, GetCurrentAdminId());
        //     return ApiResponseHandlerHelper.HandleResponse(response);
        // }

        // [HttpDelete("reviews/{reviewId}")]
        // public async Task<IActionResult> DeleteReview(int reviewId, [FromQuery] string reason)
        // {
        //     var response = await _hotelService.DeleteReviewByAdminAsync(reviewId, GetCurrentAdminId(), reason);
        //     return ApiResponseHandlerHelper.HandleResponse(response);
        // }

        [HttpGet("revenue-stats")] 
        public async Task<IActionResult> GetRevenueStats([FromQuery] int year)
        {
            if (year <= 0) year = DateTime.Now.Year;
            var response = await _hotelService.GetPlatformRevenueStatsAsync(year);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpGet("revenue/top-hotels")]
        public async Task<IActionResult> GetTopHotelsRevenue([FromQuery] int year)
        {
            if (year <= 0) year = DateTime.Now.Year;
            var response = await _hotelService.GetTopRevenueHotelsAsync(year);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 2. QUẢN LÝ TIỆN ÍCH (AMENITIES)
        // ==========================================

        [HttpPost("create-amenity")]
        public async Task<IActionResult> CreateAmenity([FromBody] AmenityCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreateAmenityAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("update-amenity/{id}")]
        public async Task<IActionResult> UpdateAmenity(int id, [FromBody] AmenityCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdateAmenityAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("del-amenity/{id}")]
        public async Task<IActionResult> DeleteAmenity(int id)
        {
            var response = await _hotelService.DeleteAmenityAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPatch("amenity/{id}/toggle-filter")]
        public async Task<IActionResult> ToggleAmenityFilter(int id)
        {
            var response = await _hotelService.ToggleFilterableAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 3. QUẢN LÝ DỊCH VỤ HỆ THỐNG (SERVICES)
        // ==========================================

        [HttpGet("services/{id}")]
        public async Task<IActionResult> GetServiceDetail(int id)
        {
            var response = await _hotelService.GetServiceDetailWithUsageAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("service")]
        public async Task<IActionResult> CreateService([FromBody] ServiceCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreateServicesAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("service/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdateServicesAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("service/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var response = await _hotelService.DeleteServiceAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 4. QUẢN LÝ LOẠI LƯU TRÚ (ACCOMMODATIONS)
        // ==========================================

        [HttpPost("accommodation")]
        public async Task<IActionResult> CreateAccommodation([FromBody] AccommodationCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreateAccommodationAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("accommodation/{id}")]
        public async Task<IActionResult> UpdateAccommodation(int id, [FromBody] AccommodationCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdateAccommodationAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("accommodation/{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAccommodationImage(int id, [FromForm] FileUploadDTO dto)
        {
            var response = await _hotelService.UploadImageAsync(id, dto.Image, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("accommodation/{id}")]
        public async Task<IActionResult> DeleteAccommodation(int id)
        {
            var response = await _hotelService.DeleteAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 5. QUẢN LÝ CHÍNH SÁCH (POLICIES)
        // ==========================================

        [HttpPost("policy-type")]
        public async Task<IActionResult> CreatePolicyType([FromBody] PolicyTypeCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreatePolicyTypeAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("policy-type/{id}")]
        public async Task<IActionResult> UpdatePolicyType(int id, [FromBody] PolicyTypeCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdatePolicyTypeAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPatch("policy-type/{id}/toggle")]
        public async Task<IActionResult> TogglePolicyType(int id)
        {
            var response = await _hotelService.TogglePolicyTypeActiveAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("policy-type/{id}")]
        public async Task<IActionResult> DeletePolicyType(int id)
        {
            var response = await _hotelService.DeletePolicyTypeAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // --- Policy Items ---
        [HttpPost("policy")]
        public async Task<IActionResult> CreatePolicy([FromBody] PolicyCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreatePolicyAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("policy/{id}")]
        public async Task<IActionResult> UpdatePolicy(int id, [FromBody] PolicyCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdatePolicyAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("policy/{id}")]
        public async Task<IActionResult> DeletePolicy(int id)
        {
            var response = await _hotelService.DeletePolicyAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 6. QUẢN LÝ BANNER & PROMOTION
        // ==========================================

        [HttpGet("banners")]
        public async Task<IActionResult> GetAllBanners()
        {
            var response = await _hotelService.GetAllBannersAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("banner")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateBanner([FromForm] BannerCreateDTO dto)
        {
            var response = await _hotelService.CreateBannersAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("banner/{id}")]
        public async Task<IActionResult> UpdateBanner(int id, [FromBody] BannerUpdateDTO dto)
        {
            var response = await _hotelService.UpdateBannerAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPatch("banner/{id}/toggle")]
        public async Task<IActionResult> ToggleBanner(int id)
        {
            var response = await _hotelService.ToggleActiveBannerAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("banner/{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var response = await _hotelService.DeleteBannerAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // --- Promotion ---
        [HttpGet("promotions")]
        public async Task<IActionResult> GetAllPromotions()
        {
            var response = await _hotelService.GetAllPromotionsAsync(GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("promotion")]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionCreateDTO dto)
        {
            var response = await _hotelService.CreatePromotionAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("promotion/{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] PromotionCreateDTO dto)
        {
            var response = await _hotelService.UpdatePromotionAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPatch("promotion/{id}/toggle")]
        public async Task<IActionResult> TogglePromotion(int id)
        {
            var response = await _hotelService.TogglePromotionStatusAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("promotion/{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var response = await _hotelService.DeletePromotionAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ==========================================
        // 7. BED TYPES & VIEW TYPES
        // ==========================================

        [HttpPost("bedtype")]
        public async Task<IActionResult> CreateBedType([FromBody] BedTypeCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreateBedTypeAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("bedtype/{id}")]
        public async Task<IActionResult> UpdateBedType(int id, [FromBody] BedTypeCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdateBedTypeAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("bedtype/{id}")]
        public async Task<IActionResult> DeleteBedType(int id)
        {
            var response = await _hotelService.DeleteBedTypeAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("viewtype")]
        public async Task<IActionResult> CreateViewType([FromBody] ViewTypeCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.CreateViewTypeAsync(dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPut("viewtype/{id}")]
        public async Task<IActionResult> UpdateViewType(int id, [FromBody] ViewTypeCreateOrUpdateDTO dto)
        {
            var response = await _hotelService.UpdateViewTypeAsync(id, dto, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpDelete("viewtype/{id}")]
        public async Task<IActionResult> DeleteViewType(int id)
        {
            var response = await _hotelService.DeleteViewTypeAsync(id, GetCurrentAdminId());
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
    }
}