using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces;

public interface IPostRepository
{
	Task<(IEnumerable<Post> items, int total)> GetPagedAsync(int page, int pageSize, bool? published);
	Task<Post?> GetByIdWithUserAsync(int id);
	Task<Post?> GetByIdAsync(int id);
	Task<IEnumerable<Post>> GetAllAsync();
	Task AddAsync(Post post);
	Task UpdateAsync(Post post);
	Task DeleteAsync(int id);
	Task SaveChangesAsync();
	// Add method:
	Task<IEnumerable<Post>> GetByUserIdAsync(int userId);
}