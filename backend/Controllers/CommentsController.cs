using Backend.DTOs.Comments;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/posts/{postId:int:min(1)}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _comments;
    public CommentsController(ICommentService comments) => _comments = comments;

    private int GetUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(sub!);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> Get(int postId)
    {
        var items = await _comments.GetByPostAsync(postId);
        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentDto>> Create(int postId, [FromBody] CreateCommentRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var dto = await _comments.CreateAsync(postId, GetUserId(), req);
        if (dto is null) return NotFound(); // post not exists
        return CreatedAtAction(nameof(Get), new { postId }, dto);
    }
}