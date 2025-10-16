using Backend.DTOs.Comments;

namespace Backend.Services.Interfaces;

public interface ICommentService
{
	Task<IEnumerable<CommentDto>> GetByPostAsync(int postId);
	Task<CommentDto?> CreateAsync(int postId, int userId, CreateCommentRequest req);
}