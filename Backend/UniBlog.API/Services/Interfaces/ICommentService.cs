using UniBlog.API.DTOs.Comment;

namespace UniBlog.API.Services.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId);
    Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto, int userId);
    Task<bool> DeleteCommentAsync(int id, int userId, bool isAdmin);
}


