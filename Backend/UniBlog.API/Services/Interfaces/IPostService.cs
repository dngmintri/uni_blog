using UniBlog.API.DTOs.Post;

namespace UniBlog.API.Services.Interfaces;

public interface IPostService
{
    Task<PostDto?> GetPostByIdAsync(int id);
    Task<IEnumerable<PostDto>> GetAllPostsAsync();
    Task<IEnumerable<PostDto>> GetPublishedPostsAsync();
    Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(int userId);
    Task<PostDto?> CreatePostAsync(CreatePostDto createPostDto, int userId);
    Task<PostDto?> UpdatePostAsync(int id, UpdatePostDto updatePostDto, int userId);
    Task<bool> DeletePostAsync(int id, int userId, bool isAdmin);
}

