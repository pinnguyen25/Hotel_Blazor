using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface ISystemSettingService
{
    Task<List<SystemSetting>> GetAllSettingsAsync();
    Task<bool> UpdateSettingAsync(string keyWord, string newValue);
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
}