using Backend.DTOs.Auth;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

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
        var res = await _auth.LoginAsync(req);
        if (res is null) return Unauthorized("Invalid credentials");
        return Ok(res);
    }
}