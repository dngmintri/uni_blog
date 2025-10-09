using System.Net.Http.Json;
using UniBlog.Client.Models;
using UniBlog.Client.Services.Interfaces;

namespace UniBlog.Client.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<UserDto>>("/api/users") 
                ?? new List<UserDto>();
        }
        catch
        {
            return new List<UserDto>();
        }
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}", userDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}

