namespace Backend.DTOs.Auth;

using System.Text.Json.Serialization;

public class AuthResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
    
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }
    
    [JsonPropertyName("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }
    
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
}