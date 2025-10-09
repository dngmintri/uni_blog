namespace UniBlog.API.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int? PostId { get; set; }
    public int? UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public Post? Post { get; set; }
    public User? User { get; set; }
}

