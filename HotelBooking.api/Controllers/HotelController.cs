using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelBooking.api.Helpers;
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
        public async Task<IActionResult> GetHotelByIdAsync(int hotelId)
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

                var response = await _hotelService.GetHotelByIdAsync(hotelId, userId);
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
                    Message = MessageResponse.SUCCESS_FIND_HOTEL,
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
            [FromQuery] int hotelId,
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            [FromQuery] int adults,
            [FromQuery] int children)
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
            var availableRooms = await _hotelService.CheckAvailableRoomsAsync(hotelId, checkIn, checkOut, adults, children);
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
    }
}