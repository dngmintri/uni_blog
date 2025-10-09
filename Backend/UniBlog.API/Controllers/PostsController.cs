using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniBlog.API.DTOs.Post;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] bool publishedOnly = false)
    {
        var posts = publishedOnly 
            ? await _postService.GetPublishedPostsAsync() 
            : await _postService.GetAllPostsAsync();
        
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound(new { message = "Không tìm thấy bài viết" });
        }

        return Ok(post);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPostsByUserId(int userId)
    {
        var posts = await _postService.GetPostsByUserIdAsync(userId);
        return Ok(posts);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdClaim!.Value);

        var post = await _postService.CreatePostAsync(createPostDto, userId);
        if (post == null)
        {
            return BadRequest(new { message = "Không thể tạo bài viết" });
        }

        return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, post);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdClaim!.Value);

        var post = await _postService.UpdatePostAsync(id, updatePostDto, userId);
        if (post == null)
        {
            return NotFound(new { message = "Không tìm thấy bài viết hoặc bạn không có quyền chỉnh sửa" });
        }

        return Ok(post);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdClaim!.Value);
        var isAdmin = User.IsInRole("Admin");

        var result = await _postService.DeletePostAsync(id, userId, isAdmin);
        if (!result)
        {
            return NotFound(new { message = "Không tìm thấy bài viết hoặc bạn không có quyền xóa" });
        }

        return Ok(new { message = "Xóa bài viết thành công" });
    }
}


