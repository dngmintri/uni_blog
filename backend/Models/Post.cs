using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models;
[Table("posts")]
public class Post
{
    [Key]
    public int PostId { get; set; }
    public int UserId { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = null!;
    [Required]
    public string Content { get; set; } = null!;
    [StringLength(255)]
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}