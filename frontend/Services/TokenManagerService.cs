using Blazored.LocalStorage;
using System.Text.Json;

namespace frontend.Services;

public interface ITokenManagerService
{
    Task<string?> GetValidTokenAsync();
    Task<bool> IsTokenValidAsync();
    Task<bool> RefreshTokenIfNeededAsync();
    Task ClearTokenAsync();
    Task SetTokenAsync(string token, DateTime? expiresAt = null);
    Task<bool> EnsureTokenIsSetAsync(HttpClient httpClient);
}

public class TokenManagerService : ITokenManagerService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public TokenManagerService(ILocalStorageService localStorage, IAuthService authService)
    {
        _localStorage = localStorage;
        _authService = authService;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<string?> GetValidTokenAsync()
    {
        // Kiểm tra token có tồn tại không
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("TokenManager: No token found in localStorage");
            return null;
        }

        // Kiểm tra token có hết hạn không
        if (!await IsTokenValidAsync())
        {
            Console.WriteLine("TokenManager: Token is expired, attempting refresh");
            // Thử refresh token
            var refreshed = await RefreshTokenIfNeededAsync();
            if (!refreshed)
            {
                // Không clear token ngay, chỉ log warning
                Console.WriteLine("TokenManager: Token refresh not available, but keeping token for now");
                // TODO: Có thể implement logic khác như logout user sau một thời gian
            }
            else
            {
                // Lấy token mới sau khi refresh
                token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine("TokenManager: Token refreshed successfully");
            }
        }

        return token;
    }

    public async Task<bool> IsTokenValidAsync()
    {
        try
        {
            var userInfoJson = await _localStorage.GetItemAsync<string>("userInfo");
            if (string.IsNullOrEmpty(userInfoJson))
            {
                Console.WriteLine("TokenManager: No userInfo found");
                return false;
            }

            var userInfo = JsonSerializer.Deserialize<AuthResponse>(userInfoJson, _jsonOptions);
            if (userInfo?.ExpiresAt == null)
            {
                Console.WriteLine("TokenManager: No ExpiresAt found, considering token valid");
                return true; // Nếu không có ExpiresAt, coi như token luôn valid
            }

            var isValid = userInfo.ExpiresAt > DateTime.UtcNow.AddMinutes(5); // Buffer 5 phút
            Console.WriteLine($"TokenManager: Token valid check - ExpiresAt: {userInfo.ExpiresAt}, Now: {DateTime.UtcNow}, Valid: {isValid}");
            return isValid;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TokenManager: Error checking token validity: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RefreshTokenIfNeededAsync()
    {
        try
        {
            Console.WriteLine("TokenManager: Attempting to refresh token");
            var refreshed = await _authService.RefreshTokenAsync();
            
            if (refreshed)
            {
                Console.WriteLine("TokenManager: Token refreshed successfully");
                return true;
            }
            else
            {
                Console.WriteLine("TokenManager: Token refresh failed - clearing tokens");
                await ClearTokenAsync();
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TokenManager: Error during token refresh: {ex.Message}");
            await ClearTokenAsync();
            return false;
        }
    }

    public async Task ClearTokenAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userInfo");
            Console.WriteLine("TokenManager: Token cleared from localStorage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TokenManager: Error clearing token: {ex.Message}");
        }
    }

    public async Task SetTokenAsync(string token, DateTime? expiresAt = null)
    {
        try
        {
            await _localStorage.SetItemAsync("authToken", token);
            Console.WriteLine("TokenManager: Token saved to localStorage");
            
            if (expiresAt.HasValue)
            {
                var userInfoJson = await _localStorage.GetItemAsync<string>("userInfo");
                if (!string.IsNullOrEmpty(userInfoJson))
                {
                    var userInfo = JsonSerializer.Deserialize<AuthResponse>(userInfoJson, _jsonOptions);
                    if (userInfo != null)
                    {
                        userInfo.ExpiresAt = expiresAt;
                        await _localStorage.SetItemAsync("userInfo", JsonSerializer.Serialize(userInfo, _jsonOptions));
                        Console.WriteLine($"TokenManager: Token expiration set to {expiresAt}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TokenManager: Error setting token: {ex.Message}");
        }
    }

    public async Task<bool> EnsureTokenIsSetAsync(HttpClient httpClient)
    {
        var token = await GetValidTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("TokenManager: No valid token available for request");
            return false;
        }

        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        Console.WriteLine("TokenManager: Authorization header set for request");
        return true;
    }
}
