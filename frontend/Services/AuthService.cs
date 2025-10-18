using frontend.Models;
using System.Net.Http.Json;

namespace frontend.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authStateService;

    public AuthService(HttpClient httpClient, AuthStateService authStateService)
    {
        _httpClient = httpClient;
        _authStateService = authStateService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    await _authStateService.SetUserAsync(authResponse);
                }
                return authResponse;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    await _authStateService.SetUserAsync(authResponse);
                }
                return authResponse;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateService.ClearUserAsync();
    }
}
