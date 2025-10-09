using Microsoft.EntityFrameworkCore;
using UniBlog.API.Data;
using UniBlog.API.Models;
using UniBlog.API.Repositories.Interfaces;

namespace UniBlog.API.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ApplicationDbContext _context;

    public LogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Log>> GetAllAsync()
    {
        return await _context.Logs
            .Include(l => l.User)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetByUserIdAsync(int userId)
    {
        return await _context.Logs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<Log> CreateAsync(Log log)
    {
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }
}


