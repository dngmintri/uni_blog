using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;

namespace frontend.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<AuthResponse?> GetCurrentUserAsync();
    Task UpdateLocalUserInfo(AuthResponse? userInfo);
    Task<bool> IsTokenExpiredAsync();
    Task<bool> RefreshTokenAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            Console.WriteLine($"AuthService: Attempting login for {request.Username}");
            
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            Console.WriteLine($"AuthService: Response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"AuthService: Response content: {responseContent}");
                
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
                Console.WriteLine($"AuthService: Deserialized response: {authResponse?.Username}");
                
                if (authResponse != null)
                {
                    await _authStateProvider.MarkUserAsAuthenticated(authResponse);
                    Console.WriteLine("AuthService: User marked as authenticated");
                }
                
                return authResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"AuthService: Error response: {errorContent}");
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: Exception: {ex.Message}");
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        try
        {
            var userInfo = await GetCurrentUserAsync();
            if (userInfo?.ExpiresAt == null)
            {
                Console.WriteLine("AuthService: No ExpiresAt found, considering token not expired");
                return false;
            }
            
            var isExpired = userInfo.ExpiresAt <= DateTime.UtcNow;
            Console.WriteLine($"AuthService: Token expiration check - ExpiresAt: {userInfo.ExpiresAt}, Now: {DateTime.UtcNow}, Expired: {isExpired}");
            return isExpired;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: Error checking token expiration: {ex.Message}");
            return true; // Nếu có lỗi, coi như token đã hết hạn
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            // TODO: Implement với backend refresh endpoint
            // Hiện tại chỉ logout user
            Console.WriteLine("AuthService: Token refresh not implemented - logging out user");
            await LogoutAsync();
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: Error during token refresh: {ex.Message}");
            await LogoutAsync();
            return false;
        }
    }

    public async Task<AuthResponse?> GetCurrentUserAsync()
    {
        try
        {
            var userInfoJson = await _localStorage.GetItemAsync<string>("userInfo");
            if (string.IsNullOrEmpty(userInfoJson))
                return null;

            var userInfo = JsonSerializer.Deserialize<AuthResponse>(userInfoJson, _jsonOptions);
            
            // Đảm bảo token được lấy từ localStorage riêng biệt
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token) && userInfo != null)
            {
                userInfo.AccessToken = token;
            }
            
            // Convert relative avatar URL to full URL if needed
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.AvatarUrl))
            {
                Console.WriteLine($"AuthService GetCurrentUser original avatar URL: {userInfo.AvatarUrl}");
                if (userInfo.AvatarUrl.StartsWith("/"))
                {
                    userInfo.AvatarUrl = $"http://localhost:5000{userInfo.AvatarUrl}";
                    Console.WriteLine($"AuthService GetCurrentUser converted avatar URL: {userInfo.AvatarUrl}");
                }
            }
            
            return userInfo;
        }
        catch
        {
            return null;
        }
    }

    public async Task UpdateLocalUserInfo(AuthResponse? userInfo)
    {
        if (userInfo != null)
        {
            // Convert relative avatar URL to full URL if needed
            if (!string.IsNullOrEmpty(userInfo.AvatarUrl) && userInfo.AvatarUrl.StartsWith("/"))
            {
                userInfo.AvatarUrl = $"http://localhost:5000{userInfo.AvatarUrl}";
                Console.WriteLine($"AuthService converted avatar URL: {userInfo.AvatarUrl}");
            }
            
            // Lưu userInfo và đảm bảo token được lưu riêng biệt
            await _localStorage.SetItemAsync("userInfo", JsonSerializer.Serialize(userInfo, _jsonOptions));
            
            // Đảm bảo AccessToken được lưu riêng biệt nếu có
            if (!string.IsNullOrEmpty(userInfo.AccessToken))
            {
                await _localStorage.SetItemAsync("authToken", userInfo.AccessToken);
                Console.WriteLine("AuthService: Token updated in localStorage");
            }
        }
    }
}

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
