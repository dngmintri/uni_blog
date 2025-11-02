using Backend.DTOs.Auth;
using Backend.DTOs.Admin;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly IPostService _postService;

    public AdminController(IUserRepository userRepository, IPostRepository postRepository, IPostService postService)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _postService = postService;
    }

    [HttpGet("users")]
    public async Task<ActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var adminUsers = users.Select(u => new
            {
                Id = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                AvatarUrl = u.AvatarUrl,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive
            }).ToList();

            return Ok(adminUsers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpPut("users/{userId}")]
    public async Task<ActionResult> UpdateUser(int userId, [FromBody] AdminUserUpdateRequest request)
    {
        try
        {
            Console.WriteLine($"AdminController: Updating user {userId} with data: FullName={request.FullName}, Email={request.Email}, Role={request.Role}, IsActive={request.IsActive}");
            
            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null) 
            {
                Console.WriteLine($"AdminController: User {userId} not found");
                return NotFound();
            }

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;
            user.Role = Enum.Parse<Backend.Models.User.UserRoleEnum>(request.Role);
            user.IsActive = request.IsActive;

            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"AdminController: User {userId} updated successfully");
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AdminController: Error updating user {userId}: {ex.Message}");
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }


    [HttpGet("posts")]
    public async Task<ActionResult> GetAllPosts()
    {
        try
        {
            var posts = await _postRepository.GetAllAsync();
            var adminPosts = posts.Select(p => new
            {
                Id = p.PostId,
                Title = p.Title,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                AuthorId = p.UserId,
                AuthorName = p.User?.FullName ?? "Unknown",
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return Ok(adminPosts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpPut("posts/{postId}")]
    public async Task<ActionResult> UpdatePost(int postId, [FromBody] AdminPostUpdateRequest request)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return NotFound();

            post.Title = request.Title;
            post.Content = request.Content;
            post.ImageUrl = request.ImageUrl;
            post.UpdatedAt = DateTime.Now;

            await _postRepository.UpdateAsync(post);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }

    [HttpDelete("posts/{postId}")]
    public async Task<ActionResult> DeletePost(int postId)
    {
        try
        {
            // Lấy userId từ token (admin có thể xóa bất kỳ post nào)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var adminUserId))
            {
                return Unauthorized();
            }

            // Sử dụng SoftDeleteAsync từ service - admin có thể xóa bất kỳ post nào
            var success = await _postService.SoftDeleteAsync(postId, adminUserId, isAdmin: true);
            
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy bài viết hoặc không thể xóa" });
            }

            return Ok(new { message = "Đã xóa bài viết thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Lỗi: {ex.Message}" });
        }
    }
}