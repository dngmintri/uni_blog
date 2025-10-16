using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class PostRepository : IPostRepository
{
	private readonly BlogDbContext _db;
	public PostRepository(BlogDbContext db) => _db = db;

	public async Task<(IEnumerable<Post> items, int total)> GetPagedAsync(int page, int pageSize, bool? published)
	{
		var query = _db.Posts.AsNoTracking();
		if (published.HasValue)
			query = query.Where(p => p.IsPublished == published.Value);

		var total = await query.CountAsync();
		var items = await query
			.Include(p => p.User)
			.OrderByDescending(p => p.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return (items, total);
	}

	public Task<Post?> GetByIdWithUserAsync(int id) =>
		_db.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.PostId == id);

	public Task<Post?> GetByIdAsync(int id) =>
		_db.Posts.FirstOrDefaultAsync(p => p.PostId == id);

	public async Task AddAsync(Post post) => await _db.Posts.AddAsync(post);

	public Task SaveChangesAsync() => _db.SaveChangesAsync();
}