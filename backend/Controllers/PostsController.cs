using Backend.DTOs.Common;
using Backend.DTOs.Posts;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _posts;
    public PostsController(IPostService posts) => _posts = posts;

    private int GetUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(sub!);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PostDto>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? published = null)
    {
        var result = await _posts.GetPagedAsync(page, pageSize, published);
        return Ok(result);
    }

    // Add new endpoint after the existing Get method:
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetByUserId(int userId)
    {
        var posts = await _posts.GetByUserIdAsync(userId);
        return Ok(posts);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<PostDto>> GetById(int id)
    {
        var dto = await _posts.GetByIdAndIncreaseViewAsync(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var userId = GetUserId();
        var dto = await _posts.CreateAsync(userId, req);
        return CreatedAtAction(nameof(GetById), new { id = dto.PostId }, dto);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var ok = await _posts.UpdateAsync(id, GetUserId(), User.IsInRole("Admin"), req);
        if (!ok) return Forbid();
        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _posts.SoftDeleteAsync(id, GetUserId(), User.IsInRole("Admin"));
        if (!ok) return Forbid();
        return NoContent();
    }
}