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
        [HttpPost("create-amenity")]
        public async Task<IActionResult> CreateAmenityAsync([FromBody] AmenityCreateOrUpdateDTO newAmenity)
        {
            var response = await _hotelService.CreateAmenityAsync(newAmenity);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // [Authorize(Roles = "Admin")]
        [HttpPut("update-amenity/{id}")]
        public async Task<IActionResult> UpdateAmenityAsync(int id, [FromBody] AmenityCreateOrUpdateDTO amenity)
        {

            var response = await _hotelService.UpdateAmenityAsync(id, amenity);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // [Authorize(Roles = "Admin")]
        [HttpDelete("delete-amenity/{id}")]
        public async Task<IActionResult> DeleteAmenityAsync(int id)
        {
            var response = await _hotelService.DeleteAmenityAsync(id);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion
    }
}