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
		Console.WriteLine($"Register attempt for: {req.Username}, {req.Email}");
		Console.WriteLine($"DateOfBirth: {req.DateOfBirth}, Gender='{req.Gender}'");
		Console.WriteLine($"Gender type: {req.Gender?.GetType()}, IsNull: {req.Gender == null}, IsEmpty: {string.IsNullOrEmpty(req.Gender)}");

		var exists = await _users.ExistsByUsernameOrEmailAsync(req.Username, req.Email);
		if (exists)
		{
			Console.WriteLine("User already exists");
			return null;
		}

		var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
		Console.WriteLine($"Password hashed: {passwordHash.Substring(0, 20)}...");

		var user = new User
		{
			Username = req.Username,
			Email = req.Email,
			FullName = req.FullName,
			PasswordHash = passwordHash,
			Role = User.UserRoleEnum.User,
			CreatedAt = DateTime.Now,
			IsActive = true,
			DateOfBirth = req.DateOfBirth,
			Gender = req.Gender
		};

		Console.WriteLine($"User created with DateOfBirth: {user.DateOfBirth}, Gender: {user.Gender}");

		await _users.AddAsync(user);
		await _users.SaveChangesAsync();

		Console.WriteLine($"User registered successfully: {user.Username}");

		var (token, exp) = _jwt.CreateToken(user);
		var refreshToken = _jwt.CreateRefreshToken(user);
		return new AuthResponse { 
			AccessToken = token, 
			RefreshToken = refreshToken,
			ExpiresAt = exp, 
			Username = user.Username, 
			Email = user.Email,
			FullName = user.FullName,
			Role = user.Role.ToString(), 
			AvatarUrl = user.AvatarUrl,
			DateOfBirth = user.DateOfBirth,
			Gender = user.Gender
		};
	}

	public async Task<AuthResponse?> LoginAsync(LoginRequest req)
	{
		Console.WriteLine($"Login attempt for: {req.Username}");
		
		// Tìm user bằng username hoặc email
		var user = await _users.GetByUsernameAsync(req.Username);
		if (user == null)
		{
			Console.WriteLine($"User not found by username: {req.Username}");
			// Nếu không tìm thấy bằng username, thử tìm bằng email
			user = await _users.GetByEmailAsync(req.Username);
			if (user != null)
			{
				Console.WriteLine($"User found by email: {req.Username}");
			}
		}
		else
		{
			Console.WriteLine($"User found by username: {req.Username}");
		}
		
		if (user is null)
		{
			Console.WriteLine("User not found");
			return null;
		}
		
		Console.WriteLine($"Checking password for user: {user.Username}");
		var passwordValid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
		Console.WriteLine($"Password valid: {passwordValid}");
		
		if (!passwordValid)
		{
			Console.WriteLine("Invalid password");
			return null;
		}

		user.LastLogin = DateTime.Now;
		await _users.SaveChangesAsync();

		var (token, exp) = _jwt.CreateToken(user);
		var refreshToken = _jwt.CreateRefreshToken(user);
		Console.WriteLine($"Login successful for user: {user.Username}");
		return new AuthResponse { 
			AccessToken = token, 
			RefreshToken = refreshToken,
			ExpiresAt = exp, 
			Username = user.Username, 
			Email = user.Email,
			FullName = user.FullName,
			Role = user.Role.ToString(), 
			AvatarUrl = user.AvatarUrl,
			DateOfBirth = user.DateOfBirth,
			Gender = user.Gender
		};
	}

	// Thêm vào AuthService.cs
	public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
	{
		try
		{
			// Validate refresh token (có thể lưu trong DB hoặc cache)
			var userId = _jwt.ValidateRefreshToken(refreshToken);
			if (userId == null) return null;

			var user = await _users.GetByIdAsync(userId.Value);
			if (user == null) return null;

			var (token, exp) = _jwt.CreateToken(user);
			var newRefreshToken = _jwt.CreateRefreshToken(user);
			return new AuthResponse 
			{ 
				AccessToken = token, 
				RefreshToken = newRefreshToken,
				ExpiresAt = exp, 
				Username = user.Username, 
				Email = user.Email,
				FullName = user.FullName,
				Role = user.Role.ToString(), 
				AvatarUrl = user.AvatarUrl,
				DateOfBirth = user.DateOfBirth,
				Gender = user.Gender
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"RefreshToken error: {ex.Message}");
			return null;
		}
	}
}