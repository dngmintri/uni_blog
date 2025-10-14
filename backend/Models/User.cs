using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = null!;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = null!;

    [Required, StringLength(100)]
    public string FullName { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }
    [StringLength(10)]
    public string? Gender { get; set; }
    public UserRoleEnum Role { get; set; } = UserRoleEnum.User;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;

    public enum UserRoleEnum
    {
        User,
        Admin
    }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}