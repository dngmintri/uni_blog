using System.ComponentModel.DataAnnotations;

namespace UniBlog.Client.Models;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải từ 3-50 ký tự")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6-100 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string? FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }
}

