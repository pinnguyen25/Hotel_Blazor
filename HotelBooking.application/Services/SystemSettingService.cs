using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface ISystemSettingService
{
    Task<List<SystemSetting>> GetAllSettingsAsync();
    Task<bool> UpdateSettingAsync(string keyWord, string newValue);
    Task<int> GetBookingTimeoutAsync();
}

public class SystemSettingService : ISystemSettingService
{
    private readonly HotelBookingContext _context;

    public SystemSettingService(HotelBookingContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSetting>> GetAllSettingsAsync()
    {
        return await _context.SystemSettings.ToListAsync();
    }

    public async Task<bool> UpdateSettingAsync(string keyWord, string newValue)
    {
        
        var setting = await _context.SystemSettings.FindAsync(keyWord);
        if (setting == null) return false;

        setting.SetTime = newValue; // Cập nhật giá trị mới
        setting.UpdateAt = DateTime.Now;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetBookingTimeoutAsync()
    {
        // 1. Tìm setting theo KeyWord
        var setting = await _context.SystemSettings
            .AsNoTracking() // Tối ưu hiệu năng vì chỉ đọc
            .FirstOrDefaultAsync(s => s.KeyWord == "BookingTimeoutMinutes");

        // 2. Logic xử lý: Nếu có thì parse, nếu không có hoặc lỗi thì trả về mặc định 15
        if (setting != null && int.TryParse(setting.SetTime, out int minutes))
        {
            return minutes > 0 ? minutes : 15; // Đảm bảo số dương
        }

        return 15; // Mặc định cứng nếu chưa cấu hình
    }        
}