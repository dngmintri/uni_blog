using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Posts;

public class UpdatePostRequest
{
    [Required, StringLength(200)] public string Title { get; set; } = null!;
    [Required] public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
}