using System.Net;
using System.Net.Mail;
using HotelBooking.infrastructure.Models;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendBookingSuccessEmailAsync(Booking booking, decimal totalAmount);
    Task SendBookingUpdateEmailAsync(Booking booking, List<BookingService> newServices, decimal additionalAmount);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var fromEmail = emailSettings["Mail"];
            var password = emailSettings["Password"];
            var host = emailSettings["Host"];
            var port = int.Parse(emailSettings["Port"]);
            var displayName = emailSettings["DisplayName"];

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Để gửi được format đẹp (HTML)
            };

            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true // Gmail bắt buộc cái này
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Log lỗi ra console để debug nếu gửi thất bại
            Console.WriteLine($"Gửi mail lỗi: {ex.Message}");
            throw; // Hoặc bỏ qua tùy logic
        }
    }

    private (decimal Total, decimal Paid, decimal Remaining) CalculateFinance(Booking booking)
    {
        decimal total = booking.TotalPrice;

        // Cộng tổng các giao dịch đã thành công
        // Lưu ý: Code gọi hàm này phải .Include(b => b.Payments) từ trước
        decimal paid = booking.Payments?
            .Where(p => p.Status == "Completed")
            .Sum(p => p.Amount) ?? 0;

        decimal remaining = total - paid;
        return (total, paid, remaining > 0 ? remaining : 0);
    }

    // Gửi email xác nhận đặt phòng thành công
    public async Task SendBookingSuccessEmailAsync(Booking booking, decimal justPaidAmount)
    {
        var fin = CalculateFinance(booking);
        string subject = $"[ChillZone] ✅ Xác nhận đặt phòng #{booking.Id} thành công";
        string body = GenerateSuccessHtml(booking, fin.Total, fin.Paid, fin.Remaining);

        if (!string.IsNullOrEmpty(booking.ContactEmail))
        {
            await SendEmailAsync(booking.ContactEmail, subject, body);
        }
    }

    // Tạo nội dung HTML cho email thành công

    private string GenerateSuccessHtml(Booking booking, decimal total, decimal paid, decimal remaining)
    {
        // Logic hiển thị dòng ghi chú nếu còn nợ
        string remainingNote = remaining > 0
            ? $@"<p style='color: #d35400; font-style: italic; margin-top: 10px;'>
                    * Quý khách vui lòng thanh toán khoản còn lại <b>({remaining:N0} VNĐ)</b> tại quầy lễ tân.
                 </p>"
            : "<p style='color: #28a745; font-weight: bold; margin-top: 10px;'>✨ Đơn hàng đã được thanh toán đầy đủ.</p>";

        return $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px;'>
            <div style='background-color: #28a745; padding: 20px; text-align: center; color: white;'>
                <h2 style='margin: 0;'>ĐẶT PHÒNG THÀNH CÔNG</h2>
                <p>Mã đơn: <b>#{booking.Id}</b></p>
            </div>
            <div style='padding: 20px;'>
                <p>Xin chào <b>{booking.ContactName}</b>,</p>
                <p>Cảm ơn quý khách đã hoàn tất đặt phòng. Dưới đây là thông tin chi tiết:</p>
                
                <table style='width: 100%; margin-top: 15px; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 5px 0;'><b>Khách sạn:</b></td>
                        <td style='text-align: right;'>{booking.Hotel?.Name}</td>
                    </tr>
                    <tr>
                        <td style='padding: 5px 0;'><b>Check-in:</b></td>
                        <td style='text-align: right;'>{booking.CheckInDate:dd/MM/yyyy}</td>
                    </tr>
                    
                    <tr style='border-top: 1px solid #eee;'>
                        <td style='padding: 10px 0;'>Tổng giá trị:</td>
                        <td style='text-align: right;'>{total:N0} VNĐ</td>
                    </tr>
                    <tr>
                        <td style='padding: 5px 0; color: #28a745;'>Đã thanh toán (Cọc):</td>
                        <td style='text-align: right; color: #28a745; font-weight: bold;'>-{paid:N0} VNĐ</td>
                    </tr>
                    <tr style='border-top: 1px dashed #ccc;'>
                        <td style='padding: 10px 0; font-weight: bold; color: #d9534f;'>CÒN LẠI CẦN TRẢ:</td>
                        <td style='text-align: right; color: #d9534f; font-weight: bold; font-size: 16px;'>{remaining:N0} VNĐ</td>
                    </tr>
                </table>

                {remainingNote}

                <hr>
                <p style='font-size: 12px; color: #777; text-align: center;'>Email tự động từ ChillZone.</p>
            </div>
        </div>";
    }

    // Gửi email khi có cập nhật dịch vụ/thay đổi đơn hàng
    public async Task SendBookingUpdateEmailAsync(Booking booking, List<BookingService> newServices, decimal additionalAmount)
    {
        // Tính toán lại tài chính mới nhất
        var fin = CalculateFinance(booking);

        string subject = $"[ChillZone] 🔔 Cập nhật dịch vụ đơn hàng #{booking.Id}";

        string serviceListHtml = "<ul style='padding-left: 20px;'>";
        foreach (var s in newServices)
        {
            serviceListHtml += $"<li>{s.Service?.Name ?? "Dịch vụ"} (x{s.Quantity}): {s.Price:N0} VNĐ</li>";
        }
        serviceListHtml += "</ul>";

        string body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px;'>
            <div style='background-color: #17a2b8; padding: 20px; text-align: center; color: white;'>
                <h2 style='margin: 0;'>CẬP NHẬT ĐƠN HÀNG</h2>
                <p>Mã đơn: <b>#{booking.Id}</b></p>
            </div>
            <div style='padding: 20px;'>
                <p>Xin chào <b>{booking.ContactName}</b>,</p>
                <p>Hệ thống vừa ghi nhận các dịch vụ được bổ sung vào đơn đặt phòng của bạn:</p>
                
                <div style='background-color: #f8f9fa; padding: 10px; border-left: 4px solid #17a2b8;'>
                    {serviceListHtml}
                    <p><b>Cộng thêm: <span style='color: #d9534f;'>+{additionalAmount:N0} VNĐ</span></b></p>
                </div>

                <br/>
                <p><b>THÔNG TIN THANH TOÁN MỚI:</b></p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 5px 0;'>Tổng tiền mới:</td>
                        <td style='text-align: right;'>{fin.Total:N0} VNĐ</td>
                    </tr>
                    <tr>
                        <td style='padding: 5px 0; color: #28a745;'>Đã thanh toán:</td>
                        <td style='text-align: right; color: #28a745;'>-{fin.Paid:N0} VNĐ</td>
                    </tr>
                    <tr style='border-top: 1px solid #ccc;'>
                        <td style='padding-top: 5px; font-weight: bold; color: #d9534f;'>CẦN THANH TOÁN:</td>
                        <td style='text-align: right; padding-top: 5px; color: #d9534f; font-weight: bold;'>{fin.Remaining:N0} VNĐ</td>
                    </tr>
                </table>

                <hr>
                <p style='font-size: 12px; color: #777;'>Nếu bạn không thực hiện yêu cầu này, vui lòng liên hệ khách sạn ngay lập tức.</p>
            </div>
        </div>";

        if (!string.IsNullOrEmpty(booking.ContactEmail))
        {
            await SendEmailAsync(booking.ContactEmail, subject, body);
        }
    }


}