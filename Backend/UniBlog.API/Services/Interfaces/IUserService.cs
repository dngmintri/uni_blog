using UniBlog.API.DTOs.User;

namespace UniBlog.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> UpdateUserAsync(int id, UserDto userDto);
    Task<bool> DeleteUserAsync(int id);
}


