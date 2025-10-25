namespace frontend.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorAvatarUrl { get; set; }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = "";
}
