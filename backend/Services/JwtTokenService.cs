using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public (string token, DateTime expires) CreateToken(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!));
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    // Thêm vào JwtTokenService.cs
public string CreateRefreshToken(User user)
{
    var jwt = _config.GetSection("Jwt");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Refresh token có thời hạn dài hơn (7 ngày)
    var expires = DateTime.UtcNow.AddDays(7);
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim("token_type", "refresh")
    };

    var token = new JwtSecurityToken(
        issuer: jwt["Issuer"],
        audience: jwt["Audience"],
        claims: claims,
        expires: expires,
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

    public int? ValidateRefreshToken(string refreshToken)
    {
        try
        {
            // Loại bỏ dấu ngoặc kép nếu có
            refreshToken = refreshToken.Trim('"');
            Console.WriteLine($"🔄 ValidateRefreshToken: Attempting to validate token: {refreshToken.Substring(0, 20)}...");
            
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            Console.WriteLine($"🔄 ValidateRefreshToken: Validation parameters - Issuer: {jwt["Issuer"]}, Audience: {jwt["Audience"]}");

            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);
            
            // Debug: Xem tất cả claims
            Console.WriteLine($"🔄 ValidateRefreshToken: All claims:");
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"  - {claim.Type}: {claim.Value}");
            }
            
            // Tìm userId bằng nhiều cách
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub) 
                            ?? principal.FindFirst(ClaimTypes.NameIdentifier)
                            ?? principal.FindFirst("sub");
            
            Console.WriteLine($"🔄 ValidateRefreshToken: UserId claim found: '{userIdClaim?.Value}'");
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                Console.WriteLine($"✅ ValidateRefreshToken: Success! UserId: {userId}");
                return userId;
            }
            
            Console.WriteLine("❌ ValidateRefreshToken: No valid userId found");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ValidateRefreshToken: Exception: {ex.Message}");
            return null;
        }
    }
}