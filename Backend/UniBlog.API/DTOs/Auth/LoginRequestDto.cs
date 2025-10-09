using System.ComponentModel.DataAnnotations;

namespace UniBlog.API.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Username là bắt buộc")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password là bắt buộc")]
    public string Password { get; set; } = string.Empty;
}

