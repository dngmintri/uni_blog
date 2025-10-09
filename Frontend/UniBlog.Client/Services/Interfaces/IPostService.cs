using UniBlog.Client.Models;

namespace UniBlog.Client.Services.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetPostsAsync(bool publishedOnly = true);
    Task<PostDto?> GetPostByIdAsync(int id);
    Task<List<PostDto>> GetPostsByUserIdAsync(int userId);
    Task<PostDto?> CreatePostAsync(CreatePostDto createPostDto);
    Task<PostDto?> UpdatePostAsync(int id, CreatePostDto updatePostDto);
    Task<bool> DeletePostAsync(int id);
}

