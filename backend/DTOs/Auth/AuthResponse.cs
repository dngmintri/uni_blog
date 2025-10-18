namespace Backend.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}