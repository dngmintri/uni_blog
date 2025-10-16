using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models;
[Table("comments")]
public class Comment
{
    [Key]
    public int CommentId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    [Required]
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;

    public Post? Post { get; set; }
    public User? User { get; set; } 
}