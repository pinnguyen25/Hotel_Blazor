using System.Security.Claims;
using HotelBooking.application.Helpers; // Để dùng ApiResponseHandlerHelper
using HotelBooking.application.Services;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")] // Chỉ Owner được phép truy cập toàn bộ controller này
    public class WalletController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public WalletController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // 1. Xem thông tin ví
        [HttpGet]
        public async Task<IActionResult> GetMyWallet()
        {
            // Lấy ID từ Token, không lấy từ tham số truyền lên để tránh hack
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int ownerId))
            {
                return Unauthorized("Không xác định được người dùng.");
            }
        
            var response = await _hotelService.GetOwnerWalletAsync(ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        // 2. Yêu cầu rút tiền
        [HttpPost("withdraw")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawRequestDTO request)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int ownerId))
            {
                return Unauthorized("Không xác định được người dùng.");
            }

            // Validate cơ bản
            if (request.Amount <= 0)
                return BadRequest(ApiResponseHelper.BadRequest<bool>("Số tiền rút phải lớn hơn 0."));

            if (string.IsNullOrWhiteSpace(request.BankAccountNumber) || string.IsNullOrWhiteSpace(request.BankName))
                 return BadRequest(ApiResponseHelper.BadRequest<bool>("Vui lòng cung cấp đầy đủ thông tin ngân hàng."));

            var response = await _hotelService.RequestWithdrawalAsync(request, ownerId);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
    }
}