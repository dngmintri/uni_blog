namespace Backend.DTOs.Posts;

public class PostDto
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Views { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }
    public string? AuthorName { get; set; }
}