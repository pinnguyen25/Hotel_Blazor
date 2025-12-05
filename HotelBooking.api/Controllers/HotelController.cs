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
        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("get-owner-dashboard")]
        public async Task<IActionResult> GetOwnerDashboardAsync()
        {
            var ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var response = await _hotelService.GetOwnerDashBoard(ownerId);
            return Ok(response);
        }

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
        [FromQuery] int? rooms)
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;

            var response = await _hotelService.GetSearchOptionsAsync(destination, checkIn, checkOut, adults, children, rooms, userId);
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

        [HttpGet("owner/roomtype-detail/{roomTypeId}")]
        public async Task<IActionResult> GetRoomTypeDetailAsync(int roomTypeId)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.GetRoomTypeDetailAsync(roomTypeId, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [HttpGet("room-options")]
        public async Task<IActionResult> GetRoomOptions()
        {
            var result = await _hotelService.GetRoomOptionsAsync();
            return ApiResponseHandlerHelper.HandleResponse(result);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/{hotelId}/wizard/roomtype")]
        public async Task<IActionResult> WizardCreateFirstRoomTypeAsync(int hotelId, [FromBody] WizardRoomTypeCreateDTO roomTypeDto)
        {
            int ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.WizardCreateFirstRoomTypeAsync(hotelId, roomTypeDto, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("create-amenity")]
        public async Task<IActionResult> CreateAmenityAsync([FromBody] AmenityCreateOrUpdateDTO newAmenity)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateAmenityAsync(newAmenity, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-amenity/{id}")]
        public async Task<IActionResult> UpdateAmenityAsync(int id, [FromBody] AmenityCreateOrUpdateDTO amenity)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateAmenityAsync(id, amenity, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-amenity/{id}")]
        public async Task<IActionResult> DeleteAmenityAsync(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteAmenityAsync(id, userId);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("create-policyType")]
        public async Task<IActionResult> CreatePolicyTypeAsync([FromBody] PolicyTypeCreateOrUpdateDTO newPolicyType)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreatePolicyTypeAsync(newPolicyType, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-policyType/{id}")]
        public async Task<IActionResult> UpdatePolicyTypeAsync(int id, [FromBody] PolicyTypeCreateOrUpdateDTO policyType)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdatePolicyTypeAsync(id, policyType, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-policyType/{id}")]
        public async Task<IActionResult> DeletePolicyTypeAsync(int id)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeletePolicyTypeAsync(id, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-policy")]
        public async Task<IActionResult> CreatePolicyAsync([FromBody] PolicyCreateOrUpdateDTO newPolicyType)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreatePolicyAsync(newPolicyType, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-policy/{id}")]
        public async Task<IActionResult> UpdatePolicyAsync(int id, [FromBody] PolicyCreateOrUpdateDTO policyType)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdatePolicyAsync(id, policyType, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-policy/{id}")]
        public async Task<IActionResult> DeletePolicyAsync(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeletePolicyAsync(id, userId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("toggle-policyType-active/{id}")]
        public async Task<IActionResult> TogglePolicyTypeActiveAsync(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.TogglePolicyTypeActiveAsync(id, userId);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("create-bedtype")]
        public async Task<IActionResult> CreateBedTypeAsync([FromBody] BedTypeCreateOrUpdateDTO bedTypeDto)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateBedTypeAsync(bedTypeDto, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Admin: Cập nhật loại giường
        [Authorize(Roles = "Admin")]
        [HttpPut("update-bedtype/{id}")]
        public async Task<IActionResult> UpdateBedTypeAsync(int id, [FromBody] BedTypeCreateOrUpdateDTO bedTypeDto)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateBedTypeAsync(id, bedTypeDto, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Admin: Xóa loại giường (soft delete hoặc hard tùy bạn)
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-bedtype/{id}")]
        public async Task<IActionResult> DeleteBedTypeAsync(int id)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteBedTypeAsync(id, adminId);
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
        [Authorize(Roles = "Admin")]
        [HttpPost("create-viewtype")]
        public async Task<IActionResult> CreateViewTypeAsync([FromBody] ViewTypeCreateOrUpdateDTO viewTypeDto)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.CreateViewTypeAsync(viewTypeDto, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Admin: Cập nhật loại view
        [Authorize(Roles = "Admin")]
        [HttpPut("update-viewtype/{id}")]
        public async Task<IActionResult> UpdateViewTypeAsync(int id, [FromBody] ViewTypeCreateOrUpdateDTO viewTypeDto)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.UpdateViewTypeAsync(id, viewTypeDto, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // Admin: Xóa loại view
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-viewtype/{id}")]
        public async Task<IActionResult> DeleteViewTypeAsync(int id)
        {
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _hotelService.DeleteViewTypeAsync(id, adminId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion
    }
}