using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public interface IPaymentService
{
    string CreateVnpayUrl(HttpContext context, Booking booking);
    Task<PaymentResponseDTO> PaymentExecuteAsync(IQueryCollection collections);
}

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly HotelBookingContext _context;

    public PaymentService(IConfiguration configuration, HotelBookingContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    // 1. TẠO URL THANH TOÁN
    public string CreateVnpayUrl(HttpContext context, Booking booking)
    {
        var pay = new VnPayLibrary();
        var urlCallBack = _configuration["Vnpay:ReturnUrl"]; // Cấu hình link FE nhận kết quả

        pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
        pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
        pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);

        // Số tiền phải nhân 100 (VNPAY quy định)
        // Ví dụ: 100,000 VND => 10000000
        pay.AddRequestData("vnp_Amount", ((long)(booking.TotalPrice * 100)).ToString());

        pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);

        // Xử lý IP Address (Tránh lỗi Invalid Data Format trên Localhost)
        // Defensive check for IP Address
        var ipAddr = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddr) || ipAddr == "::1")
        {
            ipAddr = "127.0.0.1";
        }
        pay.AddRequestData("vnp_IpAddr", ipAddr);

        pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);

        // Thông tin đơn hàng
        pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {booking.Id}");
        pay.AddRequestData("vnp_OrderType", "other");

        // URL trả về khi thanh toán xong
        pay.AddRequestData("vnp_ReturnUrl", urlCallBack);

        // Mã tham chiếu giao dịch (phải là duy nhất) -> Dùng BookingId hoặc Tick
        pay.AddRequestData("vnp_TxnRef", booking.Id.ToString());

        var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

        return paymentUrl;
    }

    // 2. XỬ LÝ KẾT QUẢ TRẢ VỀ (CALLBACK)
    public async Task<PaymentResponseDTO> PaymentExecuteAsync(IQueryCollection collections)
    {
        var pay = new VnPayLibrary();
        var response = new PaymentResponseDTO();

        // Lấy tất cả dữ liệu VNPAY trả về
        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                pay.AddResponseData(key, value.ToString());
            }
        }

        // Lấy BookingId từ vnp_TxnRef
        var vnp_TxnRef = pay.GetResponseData("vnp_TxnRef");

        // Lấy mã phản hồi (00 là thành công)
        var vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");

        // Lấy chữ ký bảo mật từ VNPAY gửi về
        var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        var vnp_TransactionNo = pay.GetResponseData("vnp_TransactionNo");
        var vnp_OrderInfo = pay.GetResponseData("vnp_OrderInfo");

        // KIỂM TRA CHỮ KÝ (Tránh giả mạo dữ liệu)
        bool checkSignature = pay.ValidateSignature(vnp_SecureHash, _configuration["Vnpay:HashSecret"]);
        if (!checkSignature)
        {
            return new PaymentResponseDTO
            {
                Success = false,
                VnPayResponseCode = vnp_ResponseCode,
            };
        }

        if (!long.TryParse(vnp_TxnRef, out long bookingId))
        {
            return new PaymentResponseDTO
            {
                Success = false,
                VnPayResponseCode = vnp_ResponseCode,
                OrderDescription = "Lỗi mã đơn hàng không hợp lệ"
            };
        }
        response.OrderId = vnp_TxnRef;
        response.VnPayResponseCode = vnp_ResponseCode;
        response.TransactionId = pay.GetResponseData("vnp_TransactionNo");
        response.OrderDescription = pay.GetResponseData("vnp_OrderInfo");
        response.PaymentId = pay.GetResponseData("vnp_TransactionNo");

        // Nếu mã trả về là "00" => Thanh toán thành công
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Load Booking (Include Hotel để lấy thông tin gửi mail)
            var booking = await _context.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            // Nếu đơn không tồn tại hoặc đã xử lý rồi (tránh call 2 lần)
            if (booking == null)
            {
                response.Success = false;
                response.OrderDescription = "Đơn hàng không tồn tại";
                return response;
            }

            // Nếu đơn đã Confirmed hoặc Cancelled rồi thì không làm gì nữa (Idempotency)
            if (booking.Status != "PendingPayment")
            {
                response.Success = booking.Status == "Confirmed";
                response.OrderDescription = "Đơn hàng đã được xử lý trước đó";
                return response;
            }

            // --- TRƯỜNG HỢP 1: THANH TOÁN THÀNH CÔNG (00) ---
            if (vnp_ResponseCode == "00")
            {
                response.Success = true;
                
                // A. Update trạng thái Booking
                booking.Status = "Confirmed";
                booking.UpdatedAt = DateTime.Now;

                // B. Lưu lịch sử thanh toán
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    PaymentMethod = "VNPAY",
                    Amount = booking.TotalPrice,
                    Status = "Completed",
                    TransactionId = vnp_TransactionNo,
                    PaidAt = DateTime.Now,
                    Additional = JsonConvert.SerializeObject(new { vnp_ResponseCode, vnp_OrderInfo })
                };
                _context.Payments.Add(payment);

                // C. [NEW] Cập nhật số lượng Voucher đã dùng (Nếu có)
                if (booking.PromotionId.HasValue)
                {
                    var promo = await _context.Promotions.FindAsync(booking.PromotionId.Value);
                    if (promo != null)
                    {
                        promo.UsedCount += 1;
                        // Tự động tắt nếu hết lượt
                        if (promo.UsageLimit > 0 && promo.UsedCount >= promo.UsageLimit)
                        {
                            promo.IsActive = false; 
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            // --- TRƯỜNG HỢP 2: KHÁCH HỦY HOẶC LỖI (Mã != 00) ---
            else
            {
                response.Success = false;

                // A. Hủy đơn ngay để nhả phòng (Vì khách đã hủy ở cổng thanh toán rồi)
                booking.Status = "Cancelled"; 
                booking.Note += $" [Thanh toán thất bại/Hủy - Code: {vnp_ResponseCode}]";
                booking.UpdatedAt = DateTime.Now;

                // B. Lưu lịch sử thanh toán thất bại (để tra soát)
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    PaymentMethod = "VNPAY",
                    Amount = 0, // Chưa thu được tiền
                    Status = "Failed",
                    TransactionId = vnp_TransactionNo, // Có thể null nếu lỗi sớm
                    PaidAt = DateTime.Now,
                    Additional = "Lỗi/Hủy: " + GetVnpayResponseMessage(vnp_ResponseCode)
                };
                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.Success = false;
            response.OrderDescription = "Exception: " + ex.Message;
        }

        return response;
    }

    private string GetVnpayResponseMessage(string code)
    {
        return code switch
        {
            "00" => "Giao dịch thành công",
            "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
            "09" => "Giao dịch không thành công: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
            "10" => "Giao dịch không thành công: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
            "11" => "Giao dịch không thành công: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
            "12" => "Giao dịch không thành công: Thẻ/Tài khoản của khách hàng bị khóa.",
            "13" => "Giao dịch không thành công: Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.",
            "24" => "Giao dịch không thành công: Khách hàng hủy giao dịch", // <--- QUAN TRỌNG NHẤT
            "51" => "Giao dịch không thành công: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
            "65" => "Giao dịch không thành công: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
            "75" => "Ngân hàng thanh toán đang bảo trì.",
            "79" => "Giao dịch không thành công: Khách hàng nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch",
            "99" => "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)",
            _ => "Lỗi không xác định"
        };
    }
}