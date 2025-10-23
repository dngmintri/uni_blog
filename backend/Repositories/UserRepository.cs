using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class UserRepository : IUserRepository
{
	private readonly BlogDbContext _db;
	public UserRepository(BlogDbContext db) => _db = db;

	public Task<bool> ExistsByUsernameOrEmailAsync(string username, string email) =>
		_db.Users.AnyAsync(u => u.Username == username || u.Email == email);

	public Task<User?> GetByUsernameAsync(string username) =>
		_db.Users.FirstOrDefaultAsync(u => u.Username == username);

	public Task<User?> GetByEmailAsync(string email) =>
		_db.Users.FirstOrDefaultAsync(u => u.Email == email);

	public Task<User?> GetByIdAsync(int id) =>
		_db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);

	public Task<User?> GetByIdForUpdateAsync(int id) =>
		_db.Users.FirstOrDefaultAsync(u => u.UserId == id);

	public async Task<IEnumerable<User>> GetAllAsync() =>
		await _db.Users.AsNoTracking().ToListAsync();

	public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

	public async Task UpdateAsync(User user)
	{
		_db.Users.Update(user);
		await _db.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var user = await _db.Users.FindAsync(id);
		if (user != null)
		{
			_db.Users.Remove(user);
			await _db.SaveChangesAsync();
		}
	}

	public Task SaveChangesAsync() => _db.SaveChangesAsync();
}