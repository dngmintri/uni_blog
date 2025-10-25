using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class CommentRepository : ICommentRepository
{
	private readonly BlogDbContext _db;
	public CommentRepository(BlogDbContext db) => _db = db;

	public Task<bool> PostExistsAsync(int postId) =>
		_db.Posts.AnyAsync(p => p.PostId == postId);

	public async Task<IEnumerable<Comment>> GetByPostAsync(int postId) =>
		await _db.Comments
			.AsNoTracking()
			.Include(c => c.User)
			.Where(c => c.PostId == postId && !c.IsDeleted)
			.OrderBy(c => c.CreatedAt)
			.ToListAsync();

	public async Task<Comment?> GetByIdWithUserAsync(int id) =>
		await _db.Comments
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.CommentId == id);

	public async Task AddAsync(Comment comment) => await _db.Comments.AddAsync(comment);

	public Task SaveChangesAsync() => _db.SaveChangesAsync();
}