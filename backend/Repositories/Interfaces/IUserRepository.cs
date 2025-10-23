using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IUserRepository
{
	Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);
	Task<User?> GetByUsernameAsync(string username);
	Task<User?> GetByEmailAsync(string email);
	Task<User?> GetByIdAsync(int id);
	Task<User?> GetByIdForUpdateAsync(int id);
	Task<IEnumerable<User>> GetAllAsync();
	Task AddAsync(User user);
	Task UpdateAsync(User user);
	Task DeleteAsync(int id);
	Task SaveChangesAsync();
}