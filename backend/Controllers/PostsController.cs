using Backend.Data;
using Backend.DTOs.Common;
using Backend.DTOs.Posts;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogDbContext _db;
    public PostsController(BlogDbContext db) => _db = db;

    private int GetUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(sub!);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PostDto>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? published = null)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var query = _db.Posts.AsNoTracking();

        if (published.HasValue)
            query = query.Where(p => p.IsPublished == published.Value);

        var total = await query.CountAsync();

        var items = await query
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostDto
            {
                PostId = p.PostId,
                UserId = p.UserId,
                Title = p.Title,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Views = p.Views,
                IsPublished = p.IsPublished,
                IsDeleted = p.IsDeleted,
                AuthorName = p.User.FullName
            })
            .ToListAsync();

        return Ok(new PagedResult<PostDto> { Total = total, Items = items });
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<PostDto>> GetById(int id)
    {
        var post = await _db.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post is null) return NotFound();

        // tăng view
        post.Views += 1;
        await _db.SaveChangesAsync();

        var dto = new PostDto
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Views = post.Views,
            IsPublished = post.IsPublished,
            IsDeleted = post.IsDeleted,
            AuthorName = post.User.FullName
        };
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = GetUserId();

        var entity = new Post
        {
            UserId = userId,
            Title = req.Title,
            Content = req.Content,
            ImageUrl = req.ImageUrl,
            CreatedAt = DateTime.Now,
            Views = 0,
            IsPublished = true,
            IsDeleted = false
        };
        _db.Posts.Add(entity);
        await _db.SaveChangesAsync();

        // load author name
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == entity.UserId);

        var dto = new PostDto
        {
            PostId = entity.PostId,
            UserId = entity.UserId,
            Title = entity.Title,
            Content = entity.Content,
            ImageUrl = entity.ImageUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Views = entity.Views,
            IsPublished = entity.IsPublished,
            IsDeleted = entity.IsDeleted,
            AuthorName = user?.FullName
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.PostId }, dto);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var entity = await _db.Posts.FirstOrDefaultAsync(p => p.PostId == id);
        if (entity is null) return NotFound();

        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && entity.UserId != userId) return Forbid();

        entity.Title = req.Title;
        entity.Content = req.Content;
        entity.ImageUrl = req.ImageUrl;
        entity.IsPublished = req.IsPublished;
        entity.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Posts.FirstOrDefaultAsync(p => p.PostId == id);
        if (entity is null) return NotFound();

        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && entity.UserId != userId) return Forbid();


        entity.IsDeleted = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}