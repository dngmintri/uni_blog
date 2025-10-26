// backend/DTOs/Auth/RefreshTokenRequest.cs
namespace Backend.DTOs.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}