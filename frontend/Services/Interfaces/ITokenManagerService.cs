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