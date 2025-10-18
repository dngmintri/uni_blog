using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Repositories.Interfaces;
using Backend.Services;
using System.Security.Claims;

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
            _fileService.DeleteImage(user.AvatarUrl);

            // Save new avatar
            var newAvatarUrl = await _fileService.SaveImageAsync(file, "avatars");
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
}