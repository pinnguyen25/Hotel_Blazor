using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelBooking.application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using HotelBooking.api.Models;

namespace HotelBooking.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IUpgradeRequestService _upgradeRequestService;
        public RequestController(IUpgradeRequestService upgradeRequestService)
        {
            _upgradeRequestService = upgradeRequestService;
        }

        private int GetCurrentAdminId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }


        [HttpGet("get-user-upgrade")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserForUpgrade()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var dto = await _upgradeRequestService.GetUserForUpgradeAsync(userId);

            if (dto == null) return NotFound();

            return Ok(dto);
        }

        [HttpPost("create-request")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateRequestAsync([FromBody] CreateUpgradeRequestDTO request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _upgradeRequestService.CreateRequestAsync(userId, request.Address, request.TaxCode);
            if (result)
            {
                return Ok(new { Message = "Request created successfully." });
            }
            return BadRequest(new { Message = "Failed to create request." });
        }

        [HttpGet("get-all-requests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRequestsAsync([FromQuery] string? status)
        {
            var requests = await _upgradeRequestService.GetAllRequestAsync(status);
            return Ok(requests);
        }

        [HttpGet("request-detail/{requestId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByRequestIdAsync(int requestId)
        {
            var request = await _upgradeRequestService.GetByRequestIdAsync(requestId);
            if (request == null) return NotFound();
            return Ok(request);
        }

        [HttpPost("approve/{requestId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveUpgradeAsync(int requestId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("nameid");
            if (claim == null) return BadRequest("AdminId claim is missing.");

            var adminId = int.Parse(claim.Value);
            var success = await _upgradeRequestService.ApproveRequestAsync(requestId, adminId);
            if (!success) return BadRequest("Cannot approve upgrade request.");
            else
                return Ok("Approved upgrade request successfully.");
        }

        [HttpPost("reject/{requestId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectUpgradeAsync(int requestId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("nameid");
            if (claim == null) return BadRequest("AdminId claim is missing.");

            var adminId = int.Parse(claim.Value);
            var success = await _upgradeRequestService.RejectRequestAsync(requestId, adminId);
            if (!success) return BadRequest("Cannot reject upgrade request.");
            else
                return Ok("Rejected upgrade request successfully.");
        }

        #region Hotel Request

        [HttpGet("hotel-stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHotelStats()
        {
            // Kết quả trả về sẽ là AdminDashboardStatsDTO
            var stats = await _upgradeRequestService.GetHotelStatsAsync();
            return Ok(stats);
        }

        // Lấy danh sách khách sạn chờ duyệt
        [HttpGet("pending-hotels")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingHotels()
        {
            var hotels = await _upgradeRequestService.GetPendingHotelsAsync();
            return Ok(hotels);
        }

        // Xem chi tiết một khách sạn (Dành cho Admin - Bỏ qua check OwnerId)
        [HttpGet("hotel-preview/{hotelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHotelPreview(int hotelId)
        {
            var detail = await _upgradeRequestService.GetHotelDetailForReviewAsync(hotelId);
            if (detail == null) return NotFound("Hotel not found.");
            return Ok(detail);
        }

        // Duyệt khách sạn -> Lên sàn (Active)
        [HttpPost("approve-hotel/{hotelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveHotel(int hotelId)
        {
            var adminId = GetCurrentUserId();
            var result = await _upgradeRequestService.ApproveHotelAsync(hotelId, adminId);
            if (result) return Ok(new { Message = "Hotel approved successfully. Email notification sent." });
            return BadRequest(new { Message = "Failed to approve hotel." });
        }

        // Từ chối khách sạn -> Quay về Rejected/Draft + Lý do
        [HttpPost("reject-hotel/{hotelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectHotel(int hotelId, [FromQuery] string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return BadRequest(new { Message = "Rejection reason is required." });

            var adminId = GetCurrentUserId();
            var result = await _upgradeRequestService.RejectHotelAsync(hotelId, adminId, reason);
            if (result) return Ok(new { Message = "Hotel rejected. Email notification sent." });
            return BadRequest(new { Message = "Failed to reject hotel." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("hotel-updates")] // api/admin/hotel-updates
        public async Task<IActionResult> GetHotelUpdateRequests(
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
        {
            // Gọi service lấy danh sách
            var response = await _upgradeRequestService.GetHotelUpdateRequestsAsync(status, pageIndex, pageSize);
            return ApiResponseHandlerHelper.HandleResponse(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("hotel-updates/{requestId}/process")] // api/admin/hotel-updates/5/process
        public async Task<IActionResult> ProcessHotelUpdateRequest(int requestId, [FromBody] HotelVerificationRequestDTO request)
        {
            // Tái sử dụng DTO HotelVerificationRequestDTO (IsApproved, Reason)
            var response = await _upgradeRequestService.ProcessHotelUpdateAsync(
                requestId,
                GetCurrentAdminId(),
                request.IsApproved,
                request.Reason
            );
            return ApiResponseHandlerHelper.HandleResponse(response);
        }
        #endregion

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            // Nếu không tìm thấy hoặc parse thất bại -> Trả về lỗi 401 Unauthorized (Chuẩn hơn là crash 500)
            if (claim == null || !int.TryParse(claim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found.");
                // Hoặc return 0 tùy logic
            }
            return userId;
        }
    }

}