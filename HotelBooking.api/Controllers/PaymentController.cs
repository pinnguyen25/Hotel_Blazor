using System.Security.Claims;
using System.Threading.Tasks;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IHotelService _hotelService; // Để lấy booking detail

        public PaymentController(IPaymentService paymentService, IHotelService hotelService)
        {
            _paymentService = paymentService;
            _hotelService = hotelService;
        }

        [Authorize]
        [HttpPost("create-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] CreatePaymentReq req)
        {
            // Lấy thông tin booking từ DB để đảm bảo số tiền chính xác (không lấy từ FE gửi lên)
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var bookingResponse = await _hotelService.GetBookingDetailForUserAsync(req.BookingId, userId);

            if (bookingResponse.Content == null) return NotFound("Đơn hàng không tồn tại");

            // Map DTO sang Entity Booking (tạm thời để dùng hàm CreateVnpayUrl)
            var bookingEntity = new Booking
            {
                Id = bookingResponse.Content.Id,
                TotalPrice = bookingResponse.Content.TotalPrice
            };

            var url = _paymentService.CreateVnpayUrl(HttpContext, bookingEntity);
            return Ok(url);
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            // Truyền toàn bộ QueryString từ VNPAY về Service xử lý
            var response = await _paymentService.PaymentExecuteAsync(Request.Query);
            return Ok(response);
        }
    }

}
