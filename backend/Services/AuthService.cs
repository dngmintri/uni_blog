using Backend.DTOs.Auth;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class AuthService : IAuthService
{
	private readonly IUserRepository _users;
	private readonly JwtTokenService _jwt;

	public AuthService(IUserRepository users, JwtTokenService jwt)
	{
		_users = users;
		_jwt = jwt;
	}

	public async Task<AuthResponse?> RegisterAsync(RegisterRequest req)
	{
		var exists = await _users.ExistsByUsernameOrEmailAsync(req.Username, req.Email);
		if (exists) return null;

		var user = new User
		{
			Username = req.Username,
			Email = req.Email,
			FullName = req.FullName,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
			Role = User.UserRoleEnum.User,
			CreatedAt = DateTime.Now,
			IsActive = true
		};

		await _users.AddAsync(user);
		await _users.SaveChangesAsync();

		var (token, exp) = _jwt.CreateToken(user);
		return new AuthResponse { AccessToken = token, ExpiresAt = exp, Username = user.Username, Role = user.Role.ToString(), AvatarUrl = user.AvatarUrl };
	}

	public async Task<AuthResponse?> LoginAsync(LoginRequest req)
	{
		var user = await _users.GetByUsernameAsync(req.Username);
		if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
			return null;

		user.LastLogin = DateTime.Now;
		await _users.SaveChangesAsync();

		var (token, exp) = _jwt.CreateToken(user);
		return new AuthResponse { AccessToken = token, ExpiresAt = exp, Username = user.Username, Role = user.Role.ToString(), AvatarUrl = user.AvatarUrl };
	}
}