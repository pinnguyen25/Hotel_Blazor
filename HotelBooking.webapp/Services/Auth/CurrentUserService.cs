using System.Security.Claims;
using HotelBooking.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public interface ICurrentUserService
{
    Task<CurrentUserVM> GetCurrentUserAsync();
    Task LogoutAsync();
}
public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _nav;

    public CurrentUserService(AuthenticationStateProvider authStateProvider, NavigationManager nav)
    {
        _authStateProvider = authStateProvider;
        _nav = nav;
    }

    public async Task<CurrentUserVM> GetCurrentUserAsync()
    {
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
        }

        return vm;
    }

    public async Task LogoutAsync()
    {
        if (_authStateProvider is CustomAuthStateProvider customAuth)
        {
            await customAuth.MarkUserAsLoggedOut();
        }
        _nav.NavigateTo("/", true);
    }
}
