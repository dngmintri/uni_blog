namespace UniBlog.API.DTOs.Comment;

public class CommentDto
{
    public int CommentId { get; set; }
    public int? PostId { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

