using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IUserRepository
{
	Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);
	Task<User?> GetByUsernameAsync(string username);
	Task<User?> GetByIdAsync(int id);
	Task AddAsync(User user);
	Task SaveChangesAsync();
}