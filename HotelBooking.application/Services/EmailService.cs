using System.Net;
using System.Net.Mail;
using HotelBooking.infrastructure.Models;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendBookingSuccessEmailAsync(Booking booking, decimal totalAmount);
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

    public async Task SendBookingSuccessEmailAsync(Booking booking, decimal totalAmount)
    {
        string subject = $"[ChillZone] ✅ Xác nhận đặt phòng #{booking.Id} thành công";
        string body = GenerateSuccessHtml(booking, totalAmount);

        if (!string.IsNullOrEmpty(booking.ContactEmail))
        {
            await SendEmailAsync(booking.ContactEmail, subject, body);
        }
    }

    private string GenerateSuccessHtml(Booking booking, decimal totalAmount)
    {
        // Template đơn giản chỉ cho trường hợp thành công
        return $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px;'>
            <div style='background-color: #28a745; padding: 20px; text-align: center; color: white;'>
                <h2 style='margin: 0;'>ĐẶT PHÒNG THÀNH CÔNG</h2>
                <p>Mã đơn: <b>#{booking.Id}</b></p>
            </div>
            <div style='padding: 20px;'>
                <p>Xin chào <b>{booking.ContactName}</b>,</p>
                <p>Cảm ơn quý khách đã hoàn tất đặt phòng. Chúc quý khách có kỳ nghỉ tuyệt vời!</p>
                
                <table style='width: 100%; margin-top: 15px; border-collapse: collapse;'>
                    <tr><td><b>Khách sạn:</b></td><td style='text-align: right;'>{booking.Hotel?.Name}</td></tr>
                    <tr><td><b>Check-in:</b></td><td style='text-align: right;'>{booking.CheckInDate:dd/MM/yyyy}</td></tr>
                    <tr><td><b>Tổng tiền:</b></td><td style='text-align: right; color: #d9534f; font-weight: bold;'>{totalAmount:N0} VNĐ</td></tr>
                </table>
                <hr>
                <p style='font-size: 12px; color: #777; text-align: center;'>Email tự động từ ChillZone.</p>
            </div>
        </div>";
    }
}