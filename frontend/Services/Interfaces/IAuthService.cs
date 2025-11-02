using frontend.Models;
namespace frontend.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<(AuthResponse? result, string? errorMessage)> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync();
    Task<AuthResponse?> GetCurrentUserAsync();
    Task UpdateLocalUserInfo(AuthResponse? userInfo);
    Task<bool> IsTokenExpiredAsync();
    Task<bool> RefreshTokenAsync();
}