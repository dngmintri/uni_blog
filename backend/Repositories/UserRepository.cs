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

	public Task<User?> GetByIdAsync(int id) =>
		_db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);

	public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

	public Task SaveChangesAsync() => _db.SaveChangesAsync();
}