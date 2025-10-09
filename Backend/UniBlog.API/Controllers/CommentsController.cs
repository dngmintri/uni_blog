using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniBlog.API.DTOs.Comment;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetCommentsByPostId(int postId)
    {
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);
        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdClaim!.Value);

        var comment = await _commentService.CreateCommentAsync(createCommentDto, userId);
        if (comment == null)
        {
            return BadRequest(new { message = "Không thể tạo comment" });
        }

        return Ok(comment);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdClaim!.Value);
        var isAdmin = User.IsInRole("Admin");

        var result = await _commentService.DeleteCommentAsync(id, userId, isAdmin);
        if (!result)
        {
            return NotFound(new { message = "Không tìm thấy comment hoặc bạn không có quyền xóa" });
        }

        return Ok(new { message = "Xóa comment thành công" });
    }
}

