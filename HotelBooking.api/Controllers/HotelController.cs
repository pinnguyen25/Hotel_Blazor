using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("get-search-options")]
        public async Task<IActionResult> GetSearchOptionsAsync(
        [FromQuery] string cityName,
        [FromQuery] DateTime? checkIn,
        [FromQuery] DateTime? checkOut,
        [FromQuery] int? adults,
        [FromQuery] int? children,
        [FromQuery] int? rooms)
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;

            var response = await _hotelService.GetSearchOptionsAsync(cityName, checkIn, checkOut, adults, children, rooms, userId);
            return Ok(response);
        }

        [HttpGet("highly-rated")]
        public async Task<IActionResult> GetHighlyRatedHotelsAsync()
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;

            var response = await _hotelService.GetHighlyRatedHotelsAsync(userId);
            return Ok(response);
        }

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelByIdAsync(int hotelId)
        {
            int? userId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uid))
                userId = uid;

            var response = await _hotelService.GetHotelByIdAsync(hotelId, userId);
            if (response == null) return NotFound();
            return Ok(response);
        }

    }
}