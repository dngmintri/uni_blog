using UniBlog.Client.Models;

namespace UniBlog.Client.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId);
    Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto);
    Task<bool> DeleteCommentAsync(int id);
}

