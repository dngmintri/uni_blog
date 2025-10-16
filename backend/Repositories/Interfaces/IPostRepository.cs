using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces;

public interface IPostRepository
{
	Task<(IEnumerable<Post> items, int total)> GetPagedAsync(int page, int pageSize, bool? published);
	Task<Post?> GetByIdWithUserAsync(int id);
	Task<Post?> GetByIdAsync(int id);
	Task AddAsync(Post post);
	Task SaveChangesAsync();
}