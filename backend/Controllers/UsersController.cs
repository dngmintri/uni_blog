using Backend.DTOs.Auth;
using Backend.Services.Interfaces;
using Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IFileUploadService _fileUploadService;

    public UsersController(IUserRepository userRepository, IAuthService authService, IFileUploadService fileUploadService)
    {
        _userRepository = userRepository;
        _authService = authService;
        _fileUploadService = fileUploadService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<AuthResponse>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound();

            var authResponse = new AuthResponse
            {
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                AvatarUrl = user.AvatarUrl,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            };

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            Console.WriteLine($"Updating profile for user ID: {userId}");
            Console.WriteLine($"Request data: FullName={request.FullName}, Email={request.Email}, DateOfBirth={request.DateOfBirth}, Gender='{request.Gender}'");
            Console.WriteLine($"Gender type: {request.Gender?.GetType()}, IsNull: {request.Gender == null}, IsEmpty: {string.IsNullOrEmpty(request.Gender)}");

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null) return NotFound();

            Console.WriteLine($"Before update: FullName={user.FullName}, Email={user.Email}, DateOfBirth={user.DateOfBirth}, Gender={user.Gender}");

            // Update user info (không cập nhật Username)
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;

            Console.WriteLine($"After update: FullName={user.FullName}, Email={user.Email}, DateOfBirth={user.DateOfBirth}, Gender={user.Gender}");

            await _userRepository.SaveChangesAsync();

            Console.WriteLine("Profile updated successfully");

            // Verify database update by reloading user
            var updatedUser = await _userRepository.GetByIdAsync(userId);
            if (updatedUser != null)
            {
                Console.WriteLine($"Database verification: FullName={updatedUser.FullName}, Email={updatedUser.Email}, DateOfBirth={updatedUser.DateOfBirth}, Gender={updatedUser.Gender}");
            }

            return Ok(new { message = "Cập nhật thông tin thành công" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating profile: {ex.Message}");
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpPut("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null) return NotFound();

            Console.WriteLine($"Changing password for user ID: {userId}");
            Console.WriteLine($"Current password verification: {!string.IsNullOrEmpty(request.CurrentPassword)}");

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                Console.WriteLine("Current password verification failed");
                return BadRequest(new { message = "Mật khẩu hiện tại không đúng" });
            }

            Console.WriteLine("Current password verification successful");

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            Console.WriteLine("Password hashed and updated in entity");

            await _userRepository.SaveChangesAsync();
            Console.WriteLine("Password change saved to database successfully");

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpPut("avatar")]
    public async Task<ActionResult> UpdateAvatar([FromBody] UpdateAvatarRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            Console.WriteLine($"Updating avatar for user ID: {userId}");
            Console.WriteLine($"New avatar URL: {request.AvatarUrl}");

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null) return NotFound();

            user.AvatarUrl = request.AvatarUrl;
            await _userRepository.SaveChangesAsync();

            Console.WriteLine("Avatar URL updated in database successfully");

            return Ok(new { message = "Cập nhật ảnh đại diện thành công" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating avatar: {ex.Message}");
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpPost("upload-avatar")]
    public async Task<ActionResult> UploadAvatar(IFormFile file)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Không có file được chọn" });

            if (file.Length > 5 * 1024 * 1024) // 5MB
                return BadRequest(new { message = "File quá lớn (tối đa 5MB)" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Chỉ chấp nhận file JPG, PNG, GIF" });

            var avatarUrl = await _fileUploadService.UploadFileAsync(file, "avatars");
            if (string.IsNullOrEmpty(avatarUrl))
                return BadRequest(new { message = "Upload thất bại" });

            // Update user avatar
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.AvatarUrl = avatarUrl;
                await _userRepository.SaveChangesAsync();
            }

            return Ok(new { message = "Upload ảnh đại diện thành công", avatarUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
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

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return 0;
    }
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class UpdateAvatarRequest
{
    public string AvatarUrl { get; set; } = string.Empty;
}