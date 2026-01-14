using System.Security.Claims;
using HotelBooking.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public interface ICurrentUserService
{
    Task<CurrentUserVM> GetCurrentUserAsync();
    Task LogoutAsync();
    event Action OnChange; 
    void NotifyStateChanged();

    // --- [MỚI] Cập nhật thông tin tạm thời (để Header hiển thị ngay) ---
    void SetUserLocally(string? fullName, string? avatarUrl);
}
public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _nav;
    private CurrentUserVM? _cachedUser;

    // [MỚI] Sự kiện thay đổi
    public event Action? OnChange;

    public CurrentUserService(AuthenticationStateProvider authStateProvider, NavigationManager nav)
    {
        _authStateProvider = authStateProvider;
        _nav = nav;
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
    public void SetUserLocally(string? fullName, string? avatarUrl)
    {
        if (_cachedUser != null)
        {
            if (!string.IsNullOrEmpty(fullName)) _cachedUser.FullName = fullName;
            if (!string.IsNullOrEmpty(avatarUrl)) _cachedUser.Avatar = avatarUrl;
            
            // Quan trọng: Báo cho Header biết để render lại
            NotifyStateChanged();
        }
    }
    public async Task<CurrentUserVM> GetCurrentUserAsync()
    {
        if (_cachedUser != null) return _cachedUser;

        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var vm = new CurrentUserVM
        {
            IsAuthenticated = user.Identity?.IsAuthenticated ?? false
        };

        if (vm.IsAuthenticated)
        {
            vm.FullName = user.FindFirst("FullName")?.Value ?? "Người dùng";
            vm.Avatar = user.FindFirst("Avatar")?.Value ?? "/images/default-avatar.png";
            vm.Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
                vm.UserId = id;
            vm.StaffHotelId = user.FindFirst("HotelId")?.Value;
        }
        _cachedUser = vm;
        return vm;
    }

    public async Task LogoutAsync()
    {
        if (_authStateProvider is CustomAuthStateProvider customAuth)
        {
            await customAuth.MarkUserAsLoggedOut();
        }

        _cachedUser = null; 
        NotifyStateChanged();
        _nav.NavigateTo("/", true);
    }
}
