using UniBlog.API.Models;
using UniBlog.API.Repositories.Interfaces;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogActionAsync(int? userId, string action, string description)
    {
        var log = new Log
        {
            UserId = userId,
            Action = action,
            Description = description,
            CreatedAt = DateTime.Now
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task<IEnumerable<Log>> GetAllLogsAsync()
    {
        return await _logRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Log>> GetLogsByUserIdAsync(int userId)
    {
        return await _logRepository.GetByUserIdAsync(userId);
    }
}


