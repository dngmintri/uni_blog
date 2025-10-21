using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly FileUploadService _fileService;

    public UploadController(FileUploadService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("avatar")]
    public async Task<ActionResult<object>> UploadAvatar(IFormFile file)
    {
        try
        {
            var url = await _fileService.UploadFileAsync(file, "avatars");
            if (url == null)
                return BadRequest("No file uploaded");

            return Ok(new { url });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("post-image")]
    public async Task<ActionResult<object>> UploadPostImage(IFormFile file)
    {
        try
        {
            var url = await _fileService.UploadFileAsync(file, "posts");
            if (url == null)
                return BadRequest("No file uploaded");

            return Ok(new { url });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}