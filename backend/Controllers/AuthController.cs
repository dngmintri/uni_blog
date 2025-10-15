using Backend.Data;
using Backend.DTOs.Auth;
using Backend.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BlogDbContext _db;
    private readonly JwtTokenService _jwt;
    public AuthController(BlogDbContext db, JwtTokenService jwt) { _db = db; _jwt = jwt; }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var exists = await _db.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email);
        if (exists) return Conflict("Username or Email already exists");

        var user = new User
        {
            Username = req.Username,
            Email = req.Email,
            FullName = req.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = Backend.Models.User.UserRoleEnum.User,
            CreatedAt = DateTime.Now,
            IsActive = true
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (token, exp) = _jwt.CreateToken(user);
        return Ok(new AuthResponse { AccessToken = token, ExpiresAt = exp, Username = user.Username, Role = user.Role.ToString() });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        user.LastLogin = DateTime.Now;
        await _db.SaveChangesAsync();

        var (token, exp) = _jwt.CreateToken(user);
        return Ok(new AuthResponse { AccessToken = token, ExpiresAt = exp, Username = user.Username, Role = user.Role.ToString() });
    }
}