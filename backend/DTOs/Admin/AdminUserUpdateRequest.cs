using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Admin;

public class AdminUserUpdateRequest
{
    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    [Required]
    public string Role { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
