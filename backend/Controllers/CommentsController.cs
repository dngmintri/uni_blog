using Backend.Data;
using Backend.DTOs.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Controllers;

[ApiController]
[Route("api/posts/{postId:int:min(1)}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly BlogDbContext _db;
    public CommentsController(BlogDbContext db) => _db = db;

    private int GetUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(sub!);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> Get(int postId)
    {
        var exists = await _db.Posts.AnyAsync(p => p.PostId == postId);
        if (!exists) return NotFound();

        var items = await _db.Comments
            .AsNoTracking()
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                UserId = c.UserId,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentDto>> Create(int postId, [FromBody] CreateCommentRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var exists = await _db.Posts.AnyAsync(p => p.PostId == postId);
        if (!exists) return NotFound();

        var userId = GetUserId();

        var entity = new Backend.Models.Comment
        {
            PostId = postId,
            UserId = userId,
            Content = req.Content,
            CreatedAt = DateTime.Now,
            IsDeleted = false
        };
        _db.Comments.Add(entity);
        await _db.SaveChangesAsync();

        var dto = new CommentDto
        {
            CommentId = entity.CommentId,
            PostId = entity.PostId,
            UserId = entity.UserId,
            Content = entity.Content,
            CreatedAt = entity.CreatedAt
        };
        return CreatedAtAction(nameof(Get), new { postId }, dto);
    }
}