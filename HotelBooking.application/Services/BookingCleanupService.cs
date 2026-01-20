using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HotelBooking.infrastructure.Models; // Namespace chứa DbContext

public class BookingCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingCleanupService> _logger;
    private DateTime _lastNoShowCheck = DateTime.MinValue;
    public BookingCleanupService(IServiceProvider serviceProvider, ILogger<BookingCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Booking Cleanup Service started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<HotelBookingContext>();
                    int timeoutMinutes = 15;
                    var setting = await context.SystemSettings.FindAsync("BookingTimeoutMinutes");
                    if (setting != null && int.TryParse(setting.SetTime, out int val))
                    {
                        timeoutMinutes = val;
                    }

                    // Tìm các đơn chờ quá 15 phút
                    var threshold = DateTime.Now.AddMinutes(-timeoutMinutes);

                    var affectedRows = await context.Bookings
                        .Where(b => b.Status == "PendingPayment" && b.CreatedAt < threshold)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(b => b.Status, "Cancelled")
                            .SetProperty(b => b.UpdatedAt, DateTime.Now)
                            // Cộng chuỗi trực tiếp trong SQL
                            .SetProperty(b => b.Note, b => (b.Note ?? "") + " [Hết hạn thanh toán]")
                        , stoppingToken);

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation($"[AutoCleanup] Đã hủy {affectedRows} đơn hết hạn.");
                    }

                    // Kiểm tra No-Show mỗi giờ
                    if ((DateTime.Now - _lastNoShowCheck).TotalHours >= 1)
                    {
                        var yesterday = DateTime.Now.Date.AddDays(-1);
                        
                        var noShowBookings = await context.Bookings
                            .Where(b => b.Status == "Confirmed" 
                                     && b.CheckInDate <= DateOnly.FromDateTime(yesterday)) // Quá ngày check-in 1 ngày
                            .ToListAsync(stoppingToken);

                        foreach (var booking in noShowBookings)
                        {
                            booking.Status = "NoShow";
                            booking.UpdatedAt = DateTime.Now;
                            booking.Note += " [Hệ thống: Khách hàng vắng mặt]";
                        }
                        
                        if (noShowBookings.Any()) 
                        {
                            await context.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation($"[NoShowCleanup] Đã đánh dấu {noShowBookings.Count} đơn vắng mặt.");
                        }

                        // Cập nhật lại mốc thời gian
                        _lastNoShowCheck = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutoCleanup Error] {ex.Message}");
            }

            // Chờ 1 phút trước khi chạy lại
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}