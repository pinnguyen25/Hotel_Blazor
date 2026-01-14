using Microsoft.EntityFrameworkCore;
using HotelBooking.infrastructure.Models;

public class DraftCleanupWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DraftCleanupWorker> _logger;

    public DraftCleanupWorker(IServiceProvider serviceProvider, ILogger<DraftCleanupWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Draft Cleanup Worker started.");
        await DoCleanupAsync(stoppingToken);
        
        // Dùng PeriodicTimer (tốt hơn Task.Delay) - Chạy mỗi 24 giờ
        using PeriodicTimer timer = new(TimeSpan.FromHours(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await DoCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task DoCleanupAsync(CancellationToken cancellationToken)
    {
        
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HotelBookingContext>();
        
        int cleanupDays = 2;
        var setting = await context.SystemSettings.FindAsync("DraftCleanupDays");
        if (setting != null && int.TryParse(setting.SetTime, out int val))
        {
            cleanupDays = val;
        }
        // Logic: Xóa Hotel đang là DRAFT và tạo quá 2 ngày trước
        var thresholdDate = DateTime.UtcNow.AddDays(-cleanupDays);

        // Giả sử bảng Hotels có cột Status (Enum) và CreatedDate
        // Thay HotelStatus.Draft bằng enum thực tế của bạn
        var deletedCount = await context.Hotels
            .Where(h => h.Status == "Draft" && h.CreatedAt < thresholdDate)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
        {
            _logger.LogInformation($"Đã xóa {deletedCount} bài lưu trú chưa hoàn thiện quá {cleanupDays} ngày.");
        }
    }
}