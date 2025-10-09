using UniBlog.API.Models;

namespace UniBlog.API.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(int id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment> UpdateAsync(Comment comment);
    Task DeleteAsync(int id);
}

