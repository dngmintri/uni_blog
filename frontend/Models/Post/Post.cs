namespace frontend.Models;
public class Post
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public string AuthorName { get; set; } = "";
    public string? AuthorAvatarUrl { get; set; }
}