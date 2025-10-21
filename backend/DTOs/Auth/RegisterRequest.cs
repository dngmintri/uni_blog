using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Auth;

public class RegisterRequest
{
    [Required, StringLength(50)] public string Username { get; set; } = null!;
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; } = null!;
    [Required, StringLength(100)] public string FullName { get; set; } = null!;
    [Required, StringLength(100, MinimumLength = 6)] public string Password { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
}