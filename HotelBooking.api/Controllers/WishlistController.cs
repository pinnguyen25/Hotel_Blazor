using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using HotelBooking.api.Models;
using HotelBooking.application.Services;

namespace HotelBooking.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("check/{hotelId}")]
        public async Task<ActionResult> CheckWishlist(int hotelId)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                return Unauthorized(new { Message = "User identifier is missing or invalid." });
            }

            var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, hotelId);
            return Ok(new { HotelId = hotelId, IsWishlist = isInWishlist });
        }

        [HttpPost("toggle/{hotelId}")]
        public async Task<ActionResult> ToggleWishlist(int hotelId)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                return Unauthorized(new { Message = "User identifier is missing or invalid." });
            }

            // Kiểm tra hotel tồn tại (tùy chọn, để validate)
            // var hotelExists = await _hotelService.ExistsAsync(hotelId); // Nếu cần
            // if (!hotelExists) return NotFound("Hotel not found.");

            var isAdded = await _wishlistService.ToggleWishlistAsync(userId, hotelId);
            return Ok(new { HotelId = hotelId, IsWishlist = isAdded });
        }

        [HttpGet("GetUserWishlist")]
        public async Task<ActionResult> GetUserWishlist()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                return Unauthorized(new { Message = "User identifier is missing or invalid." });
            }
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }
    }
}