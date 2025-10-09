using UniBlog.Client.Models;

namespace UniBlog.Client.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> UpdateUserAsync(int id, UserDto userDto);
}


