using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniBlog.API.DTOs.User;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy người dùng" });
        }

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var user = await _userService.GetUserByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy người dùng" });
        }

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = int.Parse(userIdClaim!.Value);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId != id)
        {
            return Forbid();
        }

        var updatedUser = await _userService.UpdateUserAsync(id, userDto);
        if (updatedUser == null)
        {
            return NotFound(new { message = "Không tìm thấy người dùng" });
        }

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Không tìm thấy người dùng" });
        }

        return Ok(new { message = "Xóa người dùng thành công" });
    }
}

