using System.Text;
using System.Text.Json;
using frontend.Models;

namespace frontend.Services;

public class AdminService : BaseAuthenticatedService, IAdminService
{
    public AdminService(HttpClient httpClient, ITokenManagerService tokenManager) 
        : base(httpClient, tokenManager) { }

    public async Task<List<AdminUserInfo>> GetAllUsersAsync()
    {
        var result = await ExecuteAuthenticatedRequestAsync<List<AdminUserInfo>>(() => 
            _httpClient.GetAsync("api/admin/users"));
        return result ?? new List<AdminUserInfo>();
    }

    public async Task<bool> UpdateUserAsync(int userId, AdminUserInfo userInfo)
    {
        var updateRequest = new
        {
            FullName = userInfo.FullName,
            Email = userInfo.Email,
            DateOfBirth = userInfo.DateOfBirth,
            Gender = userInfo.Gender,
            Role = userInfo.Role,
            IsActive = userInfo.IsActive
        };

        var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        Console.WriteLine($"AdminService: Sending update request for user {userId}: {json}");
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteAuthenticatedRequestWithContentAsync(updateRequest, () => 
            _httpClient.PutAsync($"api/admin/users/{userId}", content));
    }


    public async Task<List<PostInfo>> GetAllPostsAsync()
    {
        var result = await ExecuteAuthenticatedRequestAsync<List<PostInfo>>(() => 
            _httpClient.GetAsync("api/admin/posts"));
        return result ?? new List<PostInfo>();
    }

    public async Task<bool> UpdatePostAsync(int postId, PostInfo postInfo)
    {
        var updateRequest = new
        {
            Title = postInfo.Title,
            Content = postInfo.Content,
            ImageUrl = postInfo.ImageUrl
        };
        
        var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteAuthenticatedRequestWithContentAsync(updateRequest, () => 
            _httpClient.PutAsync($"api/admin/posts/{postId}", content));
    }

    public async Task<bool> DeletePostAsync(int postId)
    {
        return await ExecuteAuthenticatedRequestAsync(() => 
            _httpClient.DeleteAsync($"api/admin/posts/{postId}"));
    }

    public async Task<AdminStats> GetAdminStatsAsync()
    {
        var result = await ExecuteAuthenticatedRequestAsync<AdminStats>(() => 
            _httpClient.GetAsync("api/admin/stats"));
        return result ?? new AdminStats();
    }
}