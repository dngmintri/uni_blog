using Backend.DTOs.Comments;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class CommentService : ICommentService
{
	private readonly ICommentRepository _comments;

	public CommentService(ICommentRepository comments)
	{
		_comments = comments;
	}

	public async Task<IEnumerable<CommentDto>> GetByPostAsync(int postId)
	{
		var items = await _comments.GetByPostAsync(postId);
		return items.Select(c => new CommentDto
		{
			CommentId = c.CommentId,
			PostId = c.PostId,
			UserId = c.UserId,
			Content = c.Content,
			CreatedAt = c.CreatedAt,
			AuthorName = c.User?.FullName,
			AuthorAvatarUrl = c.User?.AvatarUrl
		});
	}

	public async Task<CommentDto?> CreateAsync(int postId, int userId, CreateCommentRequest req)
	{
		var exists = await _comments.PostExistsAsync(postId);
		if (!exists) return null;

		var entity = new Comment
		{
			PostId = postId,
			UserId = userId,
			Content = req.Content,
			CreatedAt = DateTime.Now,
			IsDeleted = false
		};
		await _comments.AddAsync(entity);
		await _comments.SaveChangesAsync();

		// Get the created comment with user info
		var createdComment = await _comments.GetByIdWithUserAsync(entity.CommentId);
		return new CommentDto
		{
			CommentId = entity.CommentId,
			PostId = entity.PostId,
			UserId = entity.UserId,
			Content = entity.Content,
			CreatedAt = entity.CreatedAt,
			AuthorName = createdComment?.User?.FullName,
			AuthorAvatarUrl = createdComment?.User?.AvatarUrl
		};
	}
}