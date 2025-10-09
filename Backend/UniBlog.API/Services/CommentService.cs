using UniBlog.API.DTOs.Comment;
using UniBlog.API.Models;
using UniBlog.API.Repositories.Interfaces;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId)
    {
        var comments = await _commentRepository.GetByPostIdAsync(postId);
        return comments.Select(c => new CommentDto
        {
            CommentId = c.CommentId,
            PostId = c.PostId,
            UserId = c.UserId,
            Username = c.User?.Username,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            IsDeleted = c.IsDeleted
        });
    }

    public async Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto, int userId)
    {
        var comment = new Comment
        {
            PostId = createCommentDto.PostId,
            UserId = userId,
            Content = createCommentDto.Content,
            CreatedAt = DateTime.Now,
            IsDeleted = false
        };

        var createdComment = await _commentRepository.CreateAsync(comment);
        var result = await _commentRepository.GetByIdAsync(createdComment.CommentId);

        if (result == null) return null;

        return new CommentDto
        {
            CommentId = result.CommentId,
            PostId = result.PostId,
            UserId = result.UserId,
            Username = result.User?.Username,
            Content = result.Content,
            CreatedAt = result.CreatedAt,
            IsDeleted = result.IsDeleted
        };
    }

    public async Task<bool> DeleteCommentAsync(int id, int userId, bool isAdmin)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null) return false;

        if (!isAdmin && comment.UserId != userId) return false;

        await _commentRepository.DeleteAsync(id);
        return true;
    }
}

