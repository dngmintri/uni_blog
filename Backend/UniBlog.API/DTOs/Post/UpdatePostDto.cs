using System.ComponentModel.DataAnnotations;

namespace UniBlog.API.DTOs.Post;

public class UpdatePostDto
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung là bắt buộc")]
    public string Content { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "URL hình ảnh tối đa 255 ký tự")]
    public string? ImageUrl { get; set; }

    public bool IsPublished { get; set; }
}

