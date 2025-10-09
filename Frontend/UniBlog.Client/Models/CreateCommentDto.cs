using System.ComponentModel.DataAnnotations;

namespace UniBlog.Client.Models;

public class CreateCommentDto
{
    [Required(ErrorMessage = "Post ID là bắt buộc")]
    public int PostId { get; set; }

    [Required(ErrorMessage = "Nội dung comment là bắt buộc")]
    public string Content { get; set; } = string.Empty;
}

