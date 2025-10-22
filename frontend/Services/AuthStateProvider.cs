using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;

namespace frontend.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public AuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userInfo = await GetUserFromStorage();
        
        if (userInfo != null)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Role, userInfo.Role),
                new Claim("FullName", userInfo.FullName),
                new Claim("AvatarUrl", userInfo.AvatarUrl ?? ""),
                new Claim("DateOfBirth", userInfo.DateOfBirth?.ToString() ?? ""),
                new Claim("Gender", userInfo.Gender ?? "")
            }, "apiauth");
            
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        
        return new AuthenticationState(new ClaimsPrincipal());
    }

    public async Task MarkUserAsAuthenticated(AuthResponse userInfo)
    {
        await _localStorage.SetItemAsync("userInfo", JsonSerializer.Serialize(userInfo));
        await _localStorage.SetItemAsync("authToken", userInfo.AccessToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("userInfo");
        await _localStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<AuthResponse?> GetUserFromStorage()
    {
        try
        {
            var userInfoJson = await _localStorage.GetItemAsync<string>("userInfo");
            if (string.IsNullOrEmpty(userInfoJson))
                return null;

            return JsonSerializer.Deserialize<AuthResponse>(userInfoJson);
        }
        catch
        {
            return null;
        }
    }
}
