using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface ICommentRepository
{
	Task<bool> PostExistsAsync(int postId);
	Task<IEnumerable<Comment>> GetByPostAsync(int postId);
	Task AddAsync(Comment comment);
	Task SaveChangesAsync();
}