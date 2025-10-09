namespace UniBlog.API.Models;

public class Post
{
    public int PostId { get; set; }
    public int? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int Views { get; set; } = 0;
    public bool IsPublished { get; set; } = true;

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

