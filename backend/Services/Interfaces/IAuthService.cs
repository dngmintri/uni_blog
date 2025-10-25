using Backend.DTOs.Auth;

namespace Backend.Services.Interfaces;

public interface IAuthService
{
	Task<AuthResponse?> RegisterAsync(RegisterRequest req);
	Task<AuthResponse?> LoginAsync(LoginRequest req);
	Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
}