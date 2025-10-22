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

    public async Task<AuthResponse?> GetCurrentUserAsync()
    {
        try
        {
            var userInfoJson = await _localStorage.GetItemAsync<string>("userInfo");
            if (string.IsNullOrEmpty(userInfoJson))
                return null;

            var userInfo = JsonSerializer.Deserialize<AuthResponse>(userInfoJson, _jsonOptions);
            
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
            
            await _localStorage.SetItemAsync("userInfo", JsonSerializer.Serialize(userInfo, _jsonOptions));
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
