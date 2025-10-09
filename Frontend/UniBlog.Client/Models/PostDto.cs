namespace UniBlog.Client.Models;

public class PostDto
{
    public int PostId { get; set; }
    public int? UserId { get; set; }
    public string? AuthorName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Views { get; set; }
    public bool IsPublished { get; set; }
    public int CommentCount { get; set; }
}

