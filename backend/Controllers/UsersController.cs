using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Repositories.Interfaces;
using Backend.Services;
using System.Security.Claims;
using Backend.DTOs.Common;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly FileUploadService _fileService;

    public UsersController(IUserRepository users, FileUploadService fileService)
    {
        _users = users;
        _fileService = fileService;
    }

    private int GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(sub!);
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateAvatar(IFormFile file)
    {
        try
        {
            var userId = GetUserId();
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return NotFound();

            // Delete old avatar
            _fileService.DeleteFile(user.AvatarUrl);

            // Save new avatar
            var newAvatarUrl = await _fileService.UploadFileAsync(file, "avatars");
            if (newAvatarUrl == null)
                return BadRequest("Failed to upload avatar");

            // Update user
            user.AvatarUrl = newAvatarUrl;
            await _users.SaveChangesAsync();

            return Ok(new { avatarUrl = newAvatarUrl });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
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