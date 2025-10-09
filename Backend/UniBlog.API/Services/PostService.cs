using UniBlog.API.DTOs.Post;
using UniBlog.API.Models;
using UniBlog.API.Repositories.Interfaces;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostDto?> GetPostByIdAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null) return null;

        await _postRepository.IncrementViewsAsync(id);

        return MapToDto(post);
    }

    public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(MapToDto);
    }

    public async Task<IEnumerable<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await _postRepository.GetPublishedPostsAsync();
        return posts.Select(MapToDto);
    }

    public async Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(int userId)
    {
        var posts = await _postRepository.GetPostsByUserIdAsync(userId);
        return posts.Select(MapToDto);
    }

    public async Task<PostDto?> CreatePostAsync(CreatePostDto createPostDto, int userId)
    {
        var post = new Post
        {
            UserId = userId,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            ImageUrl = createPostDto.ImageUrl,
            IsPublished = createPostDto.IsPublished,
            CreatedAt = DateTime.Now,
            Views = 0
        };

        var createdPost = await _postRepository.CreateAsync(post);
        return await GetPostByIdAsync(createdPost.PostId);
    }

    public async Task<PostDto?> UpdatePostAsync(int id, UpdatePostDto updatePostDto, int userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null || post.UserId != userId) return null;

        post.Title = updatePostDto.Title;
        post.Content = updatePostDto.Content;
        post.ImageUrl = updatePostDto.ImageUrl;
        post.IsPublished = updatePostDto.IsPublished;

        await _postRepository.UpdateAsync(post);
        return await GetPostByIdAsync(id);
    }

    public async Task<bool> DeletePostAsync(int id, int userId, bool isAdmin)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null) return false;

        if (!isAdmin && post.UserId != userId) return false;

        await _postRepository.DeleteAsync(id);
        return true;
    }

    private static PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            PostId = post.PostId,
            UserId = post.UserId,
            AuthorName = post.User?.FullName ?? post.User?.Username,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Views = post.Views,
            IsPublished = post.IsPublished,
            CommentCount = post.Comments?.Count(c => !c.IsDeleted) ?? 0
        };
    }
}

