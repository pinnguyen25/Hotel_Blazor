using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelBooking.application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using HotelBooking.api.Models;

namespace HotelBooking.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        IHotelService _hotelService;
        ISystemSettingService _settingService;
        public HotelController(IHotelService hotelService, ISystemSettingService settingService)
        {
            _hotelService = hotelService;
            _settingService = settingService;
        }


        #region Dashboard Owner
        [Authorize(Roles = "Owner")]
        [HttpGet("get-owner-dashboard")]
        public async Task<IActionResult> GetOwnerDashboardAsync(int hotelId)
        {
            var ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var response = await _hotelService.GetOwnerDashboardStatsAsync(hotelId, ownerId);
            return Ok(response);
        }

        [HttpGet("revenue-chart")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetRevenueChart(int hotelId, string viewType = "Week")
        {
            var ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetRevenueChartAsync(hotelId, ownerId, viewType);
            return Ok(response);
        }
        #endregion


        [Authorize(Roles = "Owner")]
        [HttpGet("owner-hotels")]
        public async Task<IActionResult> GetHotelsByOwnerAsync()
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var hotels = await _hotelService.GetAllHotelsAsync(ownerId);
            return Ok(hotels);
        }

        // ================= TÌM KIẾM KHÁCH SẠN THEO FILTER SearchForm.razor ================
        #region Hotel
        [HttpGet("get-search-options")]
        public async Task<IActionResult> GetSearchOptionsAsync(
        [FromQuery] string? destination,
        [FromQuery] DateTime? checkIn,
        [FromQuery] DateTime? checkOut,
        [FromQuery] int? adults,
        [FromQuery] int? children,
        [FromQuery] int? rooms,
        [FromQuery] decimal? priceMin,
        [FromQuery] decimal? priceMax,
        [FromQuery] decimal? ratingMin,
        [FromQuery] string? accommodationTypeIds,
        [FromQuery] string? amenityIds,
        [FromQuery] string? bedTypeIds,
        [FromQuery] string? viewTypeIds,
        [FromQuery] string? chainIds,
        [FromQuery] string? policyIds,
        [FromQuery] string? serviceIds

        )
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;

            var response = await _hotelService.GetSearchOptionsAsync(destination, checkIn, checkOut, adults, children, rooms, priceMin,                    // ← chưa dùng → để null
            priceMax,
            ratingMin,
            accommodationTypeIds,
            amenityIds,
            bedTypeIds,
            viewTypeIds,
            chainIds,
            policyIds,
            serviceIds,
            sortBy: "Recommended", userId);
            return Ok(response);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllHotelsAsync()
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
            {
                userId = uid;
            }
            var res = await _hotelService.GetAllHotelsAsync(userId);
            return Ok(res);
        }

        [HttpGet("get-hotel-by-accTypeId/{accTypeId}")]
        public async Task<IActionResult> GetHotelsByAccommodationTypeAsync(int accTypeId)
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;
            var response = await _hotelService.GetHotelsByAccommodationTypeAsync(accTypeId, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // ================= Lấy khách sạn có đánh giá cao ================
        [HttpGet("highly-rated")]
        public async Task<IActionResult> GetHighlyRatedHotelsAsync()
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;

            var response = await _hotelService.GetHighlyRatedHotelsAsync(userId);
            return Ok(response);
        }

        // ================= Lấy khách sạn theo id ================
        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelByIdAsync(
            int hotelId,
            [FromQuery] DateTime? checkIn = null,
            [FromQuery] DateTime? checkOut = null)
        {
            try
            {
                if (hotelId < 0)
                {
                    return BadRequest(new ApiResponse<HotelDetailDTO>
                    {
                        StatusCode = StatusCodeResponse.BadRequest,
                        Message = MessageResponse.INVALID_ID,
                        Content = null
                    });
                }

                int? userId = null;
                if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                    userId = uid;

                var response = await _hotelService.GetHotelByIdAsync(hotelId, userId, checkIn, checkOut);
                if (response == null)
                {
                    return NotFound(new ApiResponse<HotelDetailDTO>
                    {
                        StatusCode = StatusCodeResponse.NotFound,
                        Message = MessageResponse.NOT_FOUND,
                        Content = null
                    });
                }
                return Ok(new ApiResponse<HotelDetailDTO>
                {
                    StatusCode = StatusCodeResponse.Success,
                    Message = MessageResponse.SUCCESS,
                    Content = response
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ApiResponse<HotelDetailDTO>
                {
                    StatusCode = StatusCodeResponse.Error,
                    Message = MessageResponse.ERROR_IN_DB,
                    Content = null
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<HotelDetailDTO>
                {
                    StatusCode = StatusCodeResponse.Error,
                    Message = MessageResponse.ERROR_IN_SERVER,
                    Content = null
                });
            }
        }

        [HttpGet("get-cityName")]
        public async Task<IActionResult> GetCityNameAsync()
        {
            var cities = await _hotelService.GetCityNameAsync();
            return Ok(cities);
        }

        // ================= kiểm tra phòng trống ================
        [HttpGet("{hotelId}/check-availability")]
        public async Task<IActionResult> CheckAvailableRoomsAsync(
            int hotelId,
            [FromQuery] DateTime? checkIn,
            [FromQuery] DateTime? checkOut,
            [FromQuery] int adults = 1,
            [FromQuery] int children = 0)
        {
            if (checkIn >= checkOut)
            {
                return BadRequest(new ApiResponse<int>
                {
                    StatusCode = StatusCodeResponse.BadRequest,
                    Message = "Check-out date must be after check-in date.",
                    Content = 0
                });
            }
            var availableRooms = await _hotelService.CheckAvailableRoomsAsync(
                hotelId,
                DateOnly.FromDateTime(checkIn.Value.Date),
                DateOnly.FromDateTime(checkOut.Value.Date),
                adults,
                children);
            return Ok(availableRooms);
        }

        // ================= Autocomplete ================
        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string keyword)
        {
            var res = await _hotelService.GetAutocompleteAsync(keyword);
            return Ok(res);
        }

        #endregion




        #region Owner 

        // Lấy danh sách khách sạn của Owner (summary)
        [Authorize(Roles = "Owner")]
        [HttpGet("owner/summary")]
        public async Task<IActionResult> GetOwnerHotelsSummaryAsync()
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetOwnerHotelsSummaryAsync(ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Lấy chi tiết khách sạn của Owner (bao gồm room types, stats, images, amenities...)
        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{hotelId}/detail")]
        public async Task<IActionResult> GetHotelDetailForOwnerAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetHotelDetailForOwnerAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpGet("owner/{hotelId}/draft")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetHotelDraftAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetHotelDraftAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Tạo khách sạn mới
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/create-hotel")]
        public async Task<IActionResult> CreateHotelAsync([FromBody] HotelCreateOrUpdateDTO ownerHotel)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateHotelAsync(ownerHotel, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Cập nhật khách sạn
        [Authorize(Roles = "Owner")]
        [HttpPut("owner/update-hotel/{hotelId}")]
        public async Task<IActionResult> UpdateHotelAsync(int hotelId, [FromBody] HotelCreateOrUpdateDTO ownerHotel)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateHotelAsync(hotelId, ownerHotel, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Xóa khách sạn (soft delete)
        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/delete-hotel/{hotelId}")]
        public async Task<IActionResult> DeleteHotelAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteHotelAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Chọn tiện ích cho khách sạn
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/select-amenities")]
        public async Task<IActionResult> SelectAmenityToHotelAsync(int hotelId, HotelAmenitiesDTO amenityId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.SelectAmenityToHotelAsync(hotelId, amenityId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Thêm chính sách vào khách sạn
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/select-policies")]
        public async Task<IActionResult> SelectPolicyToHotelAsync(int hotelId, HotelPoliciesDTO policyId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.SelectPolicyToHotelAsync(hotelId, policyId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{hotelId}/images")]
        public async Task<IActionResult> GetHotelImagesAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetHotelImagesAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Upload ảnh bìa khách sạn
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/cover-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadHotelCoverImageAsync(int hotelId, [FromForm] FileUploadDTO image)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UploadHotelCoverImageAsync(hotelId, image.Image, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Upload ảnh gallery khách sạn
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/gallery-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadHotelGalleryImageAsync(int hotelId, [FromForm] FileUploadDTO image)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UploadHotelGalleryImageAsync(hotelId, image.Image, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Xóa ảnh khách sạn
        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/{hotelId}/del-image")]
        public async Task<IActionResult> DeleteHotelImageAsync(int hotelId, [FromQuery] string imageUrl)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteHotelImageAsync(hotelId, imageUrl, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        #region Staff Manage
        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{hotelId}/staffs")]
        public async Task<IActionResult> GetStaffs(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetHotelStaffsAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/staff/create")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequestDTO request)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateStaffAsync(request, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("owner/staff/{staffId}/update")]
        public async Task<IActionResult> UpdateStaff(int staffId, [FromBody] UpdateStaffRequestDTO request)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateStaffAsync(staffId, request, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/staff/{staffId}/delete")]
        public async Task<IActionResult> DeleteStaff(int staffId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteStaffAsync(staffId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        #region Roomtype
        //Lấy danh sách loại phòng của khách sạn
        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{hotelId}/roomtypes")]
        public async Task<IActionResult> GetRoomTypesByHotelAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetRoomTypesAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // cho khách
        [HttpGet("roomtype-detail/{roomTypeId}")]
        public async Task<IActionResult> GetRoomTypeDetailAsync(int roomTypeId)
        {
            // int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetRoomTypeDetailAsync(roomTypeId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpGet("room-options")]
        public async Task<IActionResult> GetRoomOptions()
        {
            var result = await _hotelService.GetRoomOptionsAsync();
            return ApiResponseHandlerHelper.HandleResponse(result);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{hotelId}/wizard/validate-submit")]
        public async Task<IActionResult> ValidateHotelForSubmitAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.ValidateHotelForSubmitAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Tạo loại phòng mới
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/roomtypes")]
        public async Task<IActionResult> CreateRoomTypeAsync(int hotelId, [FromBody] RoomTypeCreateOrUpdateDTO roomTypeDto)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateRoomTypeAsync(hotelId, roomTypeDto, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }


        // Cập nhật loại phòng
        [Authorize(Roles = "Owner")]
        [HttpPut("owner/roomtype/{roomTypeId}")]
        public async Task<IActionResult> UpdateRoomTypeAsync(int roomTypeId, [FromBody] RoomTypeCreateOrUpdateDTO roomTypeDto)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateRoomTypeAsync(roomTypeId, roomTypeDto, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Xóa loại phòng
        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/roomtype/{roomTypeId}")]
        public async Task<IActionResult> DeleteRoomTypeAsync(int roomTypeId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteRoomTypeAsync(roomTypeId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Upload ảnh loại phòng
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/roomtype/{roomTypeId}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRoomImageAsync(int roomTypeId, [FromForm] FileUploadDTO image)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UploadRoomImageAsync(roomTypeId, image.Image, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Xóa ảnh loại phòng
        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/roomtype/{roomTypeId}/image")]
        public async Task<IActionResult> DeleteRoomImageAsync(int roomTypeId, [FromQuery] string imageUrl)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteRoomImageAsync(roomTypeId, imageUrl, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        //Thêm tiện ích vào roomtype khách sạn 
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/roomtype/{roomTypeId}/select-amenity")]
        public async Task<IActionResult> SelectAmenityToRoomTypeAsync(int roomTypeId, List<int> amenityIds)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.SelectAmenityToRoomTypeAsync(roomTypeId, amenityIds, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/roomtype/clone/{sourceId}")]
        public async Task<IActionResult> CloneRoomType(int sourceId)
        {
            // Giả sử bạn có hàm lấy CurrentUserId từ Token
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CloneRoomTypeAsync(sourceId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #region Rooms (Physical Rooms Management)

        // 1. Lấy danh sách phòng vật lý (Dashboard)
        [Authorize(Roles = "Owner,Staff")]
        [HttpGet("owner/{hotelId}/rooms")]
        public async Task<IActionResult> GetPhysicalRoomsAsync(int hotelId, [FromQuery] int? roomTypeId)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetPhysicalRoomsAsync(hotelId, requesterId, roomTypeId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 2. Tạo phòng vật lý mới (Thêm lẻ)
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/room/{roomTypeId}/create")]
        public async Task<IActionResult> CreatePhysicalRoomAsync(int roomTypeId, [FromBody] CreateRoomRequestDTO roomNumber)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreatePhysicalRoomAsync(roomTypeId, roomNumber, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 3. Cập nhật phòng (Đổi tên, Đổi trạng thái Bảo trì)
        [Authorize(Roles = "Owner, Staff")]
        [HttpPut("owner/room/{roomId}/update")]
        public async Task<IActionResult> UpdatePhysicalRoomAsync(int roomId, [FromBody] UpdateRoomPhysicalDTO request)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdatePhysicalRoomAsync(roomId, request, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 4. Xóa phòng vật lý (Xóa lẻ)
        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/room/{roomId}/delete")]
        public async Task<IActionResult> DeletePhysicalRoomAsync(int roomId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeletePhysicalRoomAsync(roomId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 5. Gán phòng cho Booking (Check-in / Xếp phòng) - Quan trọng!
        [Authorize(Roles = "Owner, Staff")] // Hoặc "Owner,Staff" sau này
        [HttpPost("owner/room/assign")]
        public async Task<IActionResult> AssignRoomToBookingAsync([FromBody] AssignRoomRequestDTO request)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.AssignRoomToBookingAsync(request, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("owner/{hotelId}/pending-bookings")]
        public async Task<IActionResult> GetPendingBookings(int hotelId)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetPendingBookingsAsync(hotelId, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        // ================= Submit KHÁCH SẠN ĐỂ DUYỆT ================
        // Submit khách sạn để duyệt
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/submit")]
        public async Task<IActionResult> SubmitHotelAsync(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.SubmitHotelAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }


        #region Service Owner

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/get-owner-services/{hotelId}")]
        public async Task<IActionResult> GetOwnerServices(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var res = await _hotelService.GetOwnerHotelServicesAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(res);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/get-available-services/{hotelId}")]
        public async Task<IActionResult> GetAvailable(int hotelId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var res = await _hotelService.GetAvailableServicesToAddAsync(hotelId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(res);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/add-service")]
        public async Task<IActionResult> AddService([FromBody] OwnerAddServiceDTO dto)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var res = await _hotelService.AddServiceToHotelAsync(dto, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(res);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("owner/update-service/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] OwnerUpdateServiceDTO dto)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var res = await _hotelService.UpdateHotelServiceAsync(id, dto, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(res);
        }

        // 5. Xóa
        [Authorize(Roles = "Owner")]
        [HttpDelete("owner/remove-service/{id}")]
        public async Task<IActionResult> RemoveService(int id)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var res = await _hotelService.RemoveServiceFromHotelAsync(id, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(res);
        }

        [Authorize(Roles = "Owner")]
        [HttpPatch("owner/toggle-active-service/{id}")]
        public async Task<IActionResult> ToggleActiveOwnerService(int id)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.ToggleActiveOwnerServiceAsync(id, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion
        #endregion

        #endregion

        // =============== ĐỌC, THÊM, SỬA, XÓA TIỆN ÍCH CHO KHÁCH SẠN ================
        #region Amenity

        // [Authorize(Roles = "Admin")]
        [HttpGet("get-all-amenities")]
        public async Task<IActionResult> GetAllAmenitiesAsync()
        {
            var response = await _hotelService.GetAllAmenitiesAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        #endregion

        #region Polcicy
        [HttpGet("get-all-policy")]
        public async Task<IActionResult> GetAllPolicyAsync()
        {
            var response = await _hotelService.GetAllPolicyAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }


        #endregion

        #region Accommodation
        // lấy all accommodation
        [AllowAnonymous] // Cho phép tất cả người dùng truy cập
        [HttpGet("get-all-accommodations")]
        public async Task<IActionResult> GetAllAccommodationTypesAsync()
        {
            var response = await _hotelService.GetAllAccommodationTypesAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // lấy all accommodation của owner
        [Authorize(Roles = "Owner")]
        [HttpGet("owner-accommodation")]
        public async Task<IActionResult> GetOwnerAccommodationAsync()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetAccommodationsByUserAsync(userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Thêm loại lưu trú ( json )
        [Authorize(Roles = "Owner,Admin")]
        [HttpPost("create-accommodation")]
        public async Task<IActionResult> CreateAccommodationAsync([FromBody] AccommodationCreateOrUpdateDTO accommodation)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateAccommodationAsync(accommodation, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // upload ảnh loại lưu trú 
        [Authorize(Roles = "Owner,Admin")]
        [HttpPost("accommodation/{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImageAsync([FromForm] FileUploadDTO image, int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UploadImageAsync(id, image.Image, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Cập nhật loại lưu trú
        [Authorize(Roles = "Owner,Admin")]
        [HttpPut("update-accommodation/{id}")]
        public async Task<IActionResult> UpdateAccommodationAsync([FromBody] AccommodationCreateOrUpdateDTO accommodation, int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateAccommodationAsync(id, accommodation, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Xóa loại lưu trú
        [Authorize(Roles = "Owner,Admin")]
        [HttpDelete("delete-accommodation/{id}")]
        public async Task<IActionResult> DeleteAccommodationAsync(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteAsync(id, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        #region Bedtype
        [HttpGet("get-all-bedtypes")]
        [ResponseCache(Duration = 3600)] // Cache 1 giờ như room-options
        public async Task<IActionResult> GetAllBedTypesAsync()
        {
            var response = await _hotelService.GetAllBedTypesAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }


        #endregion

        #region ViewType
        [HttpGet("get-all-viewtypes")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetAllViewTypesAsync()
        {
            var response = await _hotelService.GetAllViewTypesAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Admin: Tạo loại view mới

        #endregion

        #region Banners
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-banners")]
        public async Task<IActionResult> GetAllBannersAsync()
        {
            var response = await _hotelService.GetAllBannersAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        // user xem
        [HttpGet("get-by-page/{page}")]
        public async Task<IActionResult> GetByPageAsync(string page)
        {
            var response = await _hotelService.GetBannersByPageAsync(page);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }


        #endregion

        #region Service Admin
        [HttpGet("get-all-services")]
        public async Task<IActionResult> GetAllServices([FromQuery] string? keyword, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _hotelService.GetAllServicesAsync(keyword, pageIndex, pageSize);
            return ApiResponseHandlerHelper.HandleResponse(response); ;
        }

        // trang chủ
        [HttpGet("get-featured-services")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedServices()
        {
            var response = await _hotelService.GetFeaturedServicesAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-detail-services/{id}")]
        public async Task<IActionResult> GetServiceDetailWithUsage(int id)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetServiceDetailWithUsageAsync(id, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response); ;
        }


        #endregion

        #region Housekeeping

        // 2. Lấy danh sách việc (Task) -> Owner hoặc Staff gọi
        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("housekeeping/tasks/{hotelId}")]
        public async Task<IActionResult> GetHousekeepingTasks(int hotelId, [FromQuery] int? staffId)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetHousekeepingTasksAsync(hotelId, requesterId, staffId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 3. Giao việc (Assign) -> Owner gọi
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/housekeeping/assign")]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskRequestDTO request)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.AssignHousekeepingTaskAsync(request, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 4. Cập nhật trạng thái (Cleaning/Completed/Maintenance) -> Staff gọi
        [Authorize(Roles = "Owner, Staff")]
        [HttpPut("housekeeping/update-status")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatusRequestDTO request)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateTaskStatusAsync(request, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        #endregion

        #region Booking Owner
        // 
        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("owner/booking/{bookingId}")]
        public async Task<IActionResult> GetBookingDetailForOwner(int bookingId)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetBookingDetailForOwnerAsync(bookingId, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Lấy danh sách booking của khách sạn (Owner/Staff)
        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("owner/booking/{bookingId}/rooms")]
        public async Task<IActionResult> GetBookingRoomsDetails(int bookingId)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetBookingRoomsDetailsAsync(bookingId, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPut("owner/booking-room/update-guest")]
        public async Task<IActionResult> UpdateGuestNameAsync([FromBody] UpdateGuestNameDTO request)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateGuestNameAsync(request, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpGet("owner/{hotelId}/calendar")]
        [Authorize(Roles = "Owner, Staff")]
        public async Task<IActionResult> GetCalendarBookings(
            [FromRoute] int hotelId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            // Lấy ID User từ Token
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var response = await _hotelService.GetBookingsForCalendarAsync(hotelId, userId, start, end);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("owner/booking/move")]
        [Authorize(Roles = "Owner, Staff")]
        public async Task<IActionResult> MoveBooking([FromBody] MoveBookingRequestDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.MoveBookingAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("owner/booking/walk-in")]
        [Authorize(Roles = "Owner, Staff")]
        public async Task<IActionResult> CreateWalkInBooking([FromBody] WalkInBookingRequestDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateWalkInBookingAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpPost("owner/booking/{bookingId}/mark-noshow")]
        [Authorize(Roles = "Owner, Staff")]
        public async Task<IActionResult> MarkNoShow(int bookingId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.MarkBookingAsNoShowAsync(bookingId, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        #region Booking User

        [Authorize]
        [HttpPost("booking/create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateBookingAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize]
        [HttpGet("booking/my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetCustomerBookingsAsync(userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize]
        [HttpGet("booking/detail/{bookingId}")]
        public async Task<IActionResult> GetBookingDetail(int bookingId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetBookingDetailForUserAsync(bookingId, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize] // Bắt buộc phải đăng nhập mới lấy được User Id
        [HttpGet("booking/upcoming-trip")]
        public async Task<IActionResult> GetUpcomingTrip()
        {
            // 1. Lấy UserId từ Token
            var userIdString = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);
            // 2. Gọi Service
            var response = await _hotelService.GetUpcomingTripAsync(userId);

            // 3. Trả về kết quả
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize]
        [HttpPost("booking/cancel/{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CancelBookingAsync(bookingId, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }


        [Authorize]
        [HttpPost("booking/service/add")]
        public async Task<IActionResult> AddServiceToBooking([FromBody] AddServiceRequestDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.AddServiceToBookingAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize]
        [HttpPut("booking/service/update-qty")]
        public async Task<IActionResult> UpdateServiceQuantity([FromBody] UpdateServiceQuantityDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateServiceQuantityAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize]
        [HttpDelete("booking/service/remove/{bookingServiceId}")]
        public async Task<IActionResult> DeleteServiceFromBooking(int bookingServiceId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteServiceFromBookingAsync(bookingServiceId, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        // add service khi user đang tạo booking
        [Authorize]
        [HttpGet("services-add/{hotelId}")]
        public async Task<IActionResult> GetAddOnServices(int hotelId)
        {
            var result = await _hotelService.GetAddOnServicesForBookingAsync(hotelId);
            return ApiResponseHandlerHelper.HandleResponse(result);
        }


        #endregion

        #region Payments
        [Authorize]
        [HttpPost("payment/confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentRequestDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.ConfirmBookingPaymentAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("owner/booking/{bookingId}/invoice")]
        public async Task<IActionResult> GetInvoicePreview([FromRoute] int bookingId)
        {
            // Lấy ID người đang thao tác
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            // Gọi Service
            var response = await _hotelService.GetInvoicePreviewAsync(bookingId, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // thanh toán và dọn dẹp
        [Authorize(Roles = "Owner, Staff")]
        [HttpPost("owner/booking/{bookingId}/checkout")]
        public async Task<IActionResult> ProcessCheckoutPayment(
        [FromRoute] int bookingId,
        [FromBody] PaymentRequestDTO request)
        {
            // Validate: Đảm bảo ID trên URL khớp với ID trong Body (nếu có gửi)
            if (request.BookingId != 0 && request.BookingId != bookingId)
            {
                return BadRequest(ApiResponseHelper.BadRequest<bool>("Mã đơn phòng không khớp."));
            }

            request.BookingId = bookingId;
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.ProcessCheckoutPaymentAsync(request, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPost("owner/booking/{bookingId}/refund-confirm")]
        public async Task<IActionResult> ConfirmRefund([FromRoute] int bookingId)
        {
            int requesterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.ConfirmRefundAsync(bookingId, requesterId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        #region Reviews
        [Authorize] // Owner role
        [HttpPost("owner/review/reply")]
        public async Task<IActionResult> ReplyReview([FromBody] ReplyReviewDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.ReplyReviewAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [AllowAnonymous]
        [HttpGet("{hotelId}/reviews")]
        public async Task<IActionResult> GetHotelReviews(int hotelId)
        {
            var response = await _hotelService.GetReviewsByHotelAsync(hotelId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize]
        [HttpPost("review/create")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDTO request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateReviewAsync(request, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        #region Poromotion
        [Authorize]
        [HttpPost("booking/preview-price")]
        public async Task<IActionResult> PreviewPrice([FromBody] PriceCalculationDTO request)
        {
            // Lấy UserId từ Token để đảm bảo bảo mật
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            request.UserId = int.Parse(userIdClaim.Value);

            var result = await _hotelService.CalculateBookingPriceAsync(request);

            // Trả về kết quả (PriceResultDTO)
            return ApiResponseHandlerHelper.HandleResponse(result);
        }



        [Authorize]
        [HttpGet("booking/available-promotions")]
        public async Task<IActionResult> GetAvailablePromotions()
        {
            var response = await _hotelService.GetAvailablePromotionsForUserAsync();
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion
        
        [HttpGet("booking-timeout")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetPublicBookingTimeout()
        {
            int minutes = await _settingService.GetBookingTimeoutAsync();
            return Ok(new ApiResponse<int>
            {
                StatusCode = StatusCodeResponse.Success,
                Content = minutes,
                Message = "Success"
            });
        }
    }
}