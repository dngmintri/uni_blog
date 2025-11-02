using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class PostRepository : IPostRepository
{
	private readonly BlogDbContext _db;
	public PostRepository(BlogDbContext db) => _db = db;

	public async Task<(IEnumerable<Post> items, int total)> GetPagedAsync(int page, int pageSize)
	{
		var query = _db.Posts.AsNoTracking()
			.Where(p => !p.IsDeleted);

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
		_db.Posts.Where(p => !p.IsDeleted).Include(p => p.User).FirstOrDefaultAsync(p => p.PostId == id);

	public Task<Post?> GetByIdAsync(int id) =>
		_db.Posts.Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.PostId == id);

	public async Task<IEnumerable<Post>> GetAllAsync()
	{
		return await _db.Posts
			.Where(p => !p.IsDeleted)
			.Include(p => p.User)
			.Where(p => !p.IsDeleted)
			.OrderByDescending(p => p.CreatedAt)
			.ToListAsync();
	}

	public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
	{
		Console.WriteLine($"🔄 PostRepository.GetByUserIdAsync: Querying for userId = {userId}");
		var posts = await _db.Posts
			.Include(p => p.User)
			.Where(p => p.UserId == userId && !p.IsDeleted)
			.OrderByDescending(p => p.CreatedAt)
			.ToListAsync();
		Console.WriteLine($"📦 PostRepository.GetByUserIdAsync: Found {posts.Count} posts");
		return posts;
	}

	public async Task AddAsync(Post post) => await _db.Posts.AddAsync(post);

	public async Task UpdateAsync(Post post)
	{
		_db.Posts.Update(post);
		await _db.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var post = await _db.Posts.FindAsync(id);
		if (post != null)
		{
			post.IsDeleted = true;
			post.UpdatedAt = DateTime.Now;
			await _db.SaveChangesAsync();
		}
	}

	public Task SaveChangesAsync() => _db.SaveChangesAsync();
}