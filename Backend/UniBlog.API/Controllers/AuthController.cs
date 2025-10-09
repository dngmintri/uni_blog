using Microsoft.AspNetCore.Mvc;
using UniBlog.API.DTOs.Auth;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogService _logService;

    public AuthController(IAuthService authService, ILogService logService)
    {
        _authService = authService;
        _logService = logService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request);
        if (result == null)
        {
            await _logService.LogActionAsync(null, "LOGIN_FAILED", $"Failed login attempt for username: {request.Username}");
            return Unauthorized(new { message = "Username hoặc password không đúng" });
        }

        await _logService.LogActionAsync(null, "LOGIN_SUCCESS", $"User {request.Username} logged in successfully");
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request);
        if (!result)
        {
            return BadRequest(new { message = "Username hoặc email đã tồn tại" });
        }

        await _logService.LogActionAsync(null, "REGISTER_SUCCESS", $"New user registered: {request.Username}");
        return Ok(new { message = "Đăng ký thành công" });
    }
}


