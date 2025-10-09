using UniBlog.API.Models;

namespace UniBlog.API.Services.Interfaces;

public interface ILogService
{
    Task LogActionAsync(int? userId, string action, string description);
    Task<IEnumerable<Log>> GetAllLogsAsync();
    Task<IEnumerable<Log>> GetLogsByUserIdAsync(int userId);
}


