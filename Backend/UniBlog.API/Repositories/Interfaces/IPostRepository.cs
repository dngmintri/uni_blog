using UniBlog.API.Models;

namespace UniBlog.API.Repositories.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetPublishedPostsAsync();
    Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task DeleteAsync(int id);
    Task IncrementViewsAsync(int id);
}

