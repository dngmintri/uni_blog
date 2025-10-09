using UniBlog.API.Models;

namespace UniBlog.API.Repositories.Interfaces;

public interface ILogRepository
{
    Task<IEnumerable<Log>> GetAllAsync();
    Task<IEnumerable<Log>> GetByUserIdAsync(int userId);
    Task<Log> CreateAsync(Log log);
}

