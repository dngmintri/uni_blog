using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LogsController : ControllerBase
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await _logService.GetAllLogsAsync();
        return Ok(logs);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetLogsByUserId(int userId)
    {
        var logs = await _logService.GetLogsByUserIdAsync(userId);
        return Ok(logs);
    }
}

