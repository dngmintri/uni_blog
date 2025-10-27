using Backend.DTOs.Auth;
using Backend.Services.Interfaces;
using Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserRepository _userRepository;
    
    public AuthController(IAuthService auth, IUserRepository userRepository)
    {
        _auth = auth;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var res = await _auth.RegisterAsync(req);
        if (res is null) return Conflict("Username or Email already exists");
        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        
        try
        {
            var res = await _auth.LoginAsync(req);
            if (res is null) return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            return Ok(res);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("test-users")]
    public async Task<ActionResult> TestUsers()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(new { 
                count = users.Count(), 
                users = users.Select(u => new { 
                    u.UserId, 
                    u.Username, 
                    u.Email, 
                    u.FullName,
                    u.DateOfBirth,
                    u.Gender,
                    u.AvatarUrl
                }) 
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        
        try
        {
            var res = await _auth.RefreshTokenAsync(req.RefreshToken);
            if (res is null) return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn" });
            return Ok(res);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}