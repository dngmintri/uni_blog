using Backend.DTOs.Common;
using Backend.DTOs.Posts;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class PostService : IPostService
{
	private readonly IPostRepository _posts;
	private readonly IUserRepository _users;

	public PostService(IPostRepository posts, IUserRepository users)
	{
		_posts = posts;
		_users = users;
	}

	public async Task<PagedResult<PostDto>> GetPagedAsync(int page, int pageSize, bool? published)
	{
		page = page < 1 ? 1 : page;
		pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

		var (items, total) = await _posts.GetPagedAsync(page, pageSize, published);
		var resultItems = items.Select(p => new PostDto
		{
			PostId = p.PostId,
			UserId = p.UserId,
			Title = p.Title,
			Content = p.Content,
			ImageUrl = p.ImageUrl,
			CreatedAt = p.CreatedAt,
			UpdatedAt = p.UpdatedAt,
			Views = p.Views,
			IsPublished = p.IsPublished,
			IsDeleted = p.IsDeleted,
			AuthorName = p.User?.FullName,
			AuthorAvatarUrl = p.User?.AvatarUrl
		}).ToList();

		return new PagedResult<PostDto> { Total = total, Items = resultItems };
	}

	public async Task<PostDto?> GetByIdAndIncreaseViewAsync(int id)
	{
		var post = await _posts.GetByIdWithUserAsync(id);
		if (post is null) return null;

		post.Views += 1;
		await _posts.SaveChangesAsync();

		return new PostDto
		{
			PostId = post.PostId,
			UserId = post.UserId,
			Title = post.Title,
			Content = post.Content,
			ImageUrl = post.ImageUrl,
			CreatedAt = post.CreatedAt,
			UpdatedAt = post.UpdatedAt,
			Views = post.Views,
			IsPublished = post.IsPublished,
			IsDeleted = post.IsDeleted,
			AuthorName = post.User?.FullName,
			AuthorAvatarUrl = post.User?.AvatarUrl
		};
	}

	public async Task<PostDto> CreateAsync(int userId, CreatePostRequest req)
	{
		var entity = new Post
		{
			UserId = userId,
			Title = req.Title,
			Content = req.Content,
			ImageUrl = req.ImageUrl,
			CreatedAt = DateTime.Now,
			Views = 0,
			IsPublished = true,
			IsDeleted = false
		};
		await _posts.AddAsync(entity);
		await _posts.SaveChangesAsync();

		var user = await _users.GetByIdAsync(entity.UserId);
		return new PostDto
		{
			PostId = entity.PostId,
			UserId = entity.UserId,
			Title = entity.Title,
			Content = entity.Content,
			ImageUrl = entity.ImageUrl,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			Views = entity.Views,
			IsPublished = entity.IsPublished,
			IsDeleted = entity.IsDeleted,
			AuthorName = user?.FullName
		};
	}

	public async Task<bool> UpdateAsync(int id, int currentUserId, bool isAdmin, UpdatePostRequest req)
	{
		var entity = await _posts.GetByIdAsync(id);
		if (entity is null) return false;

		if (!isAdmin && entity.UserId != currentUserId) return false;

		entity.Title = req.Title;
		entity.Content = req.Content;
		entity.ImageUrl = req.ImageUrl;
		entity.IsPublished = req.IsPublished;
		entity.UpdatedAt = DateTime.Now;

		await _posts.SaveChangesAsync();
		return true;
	}

	public async Task<bool> SoftDeleteAsync(int id, int currentUserId, bool isAdmin)
	{
		var entity = await _posts.GetByIdAsync(id);
		if (entity is null) return false;

		if (!isAdmin && entity.UserId != currentUserId) return false;

		entity.IsDeleted = true;
		await _posts.SaveChangesAsync();
		return true;
	}

	public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId)
	{
		Console.WriteLine($"🔄 PostService.GetByUserIdAsync: Called with userId = {userId}");
		var posts = await _posts.GetByUserIdAsync(userId);
		Console.WriteLine($"📦 PostService.GetByUserIdAsync: Retrieved {posts.Count()} posts from repository");
		
		var result = posts.Select(p => new PostDto
		{
			PostId = p.PostId,
			UserId = p.UserId,
			Title = p.Title,
			Content = p.Content,
			ImageUrl = p.ImageUrl,
			CreatedAt = p.CreatedAt,
			UpdatedAt = p.UpdatedAt,
			Views = p.Views,
			IsPublished = p.IsPublished,
			IsDeleted = p.IsDeleted,
			AuthorName = p.User?.FullName,
			AuthorAvatarUrl = p.User?.AvatarUrl
		}).ToList();
		
		Console.WriteLine($"✅ PostService.GetByUserIdAsync: Returning {result.Count} posts");
		return result;
	}
}