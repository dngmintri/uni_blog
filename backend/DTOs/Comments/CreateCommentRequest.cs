using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Comments;

public class CreateCommentRequest
{
    [Required] public int UserId { get; set; }
    [Required] public string Content { get; set; } = null!;
}