using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register(object req) => StatusCode(501);

    [HttpPost("login")]
    public IActionResult Login(object req) => StatusCode(501);
}