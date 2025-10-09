using UniBlog.API.DTOs.User;
using UniBlog.API.Repositories.Interfaces;
using UniBlog.API.Services.Interfaces;

namespace UniBlog.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender?.ToString(),
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            UserId = u.UserId,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            DateOfBirth = u.DateOfBirth,
            Gender = u.Gender?.ToString(),
            Role = u.Role.ToString(),
            CreatedAt = u.CreatedAt,
            LastLogin = u.LastLogin
        });
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        user.Email = userDto.Email;
        user.FullName = userDto.FullName;
        user.DateOfBirth = userDto.DateOfBirth;
        user.Gender = userDto.Gender;

        await _userRepository.UpdateAsync(user);
        return await GetUserByIdAsync(id);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var exists = await _userRepository.ExistsAsync(id);
        if (!exists) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }
}

