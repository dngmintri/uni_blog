using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;

namespace frontend.Services;

public interface IAdminService
{
    Task<List<AdminUserInfo>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(int userId, AdminUserInfo userInfo);
    Task<List<PostInfo>> GetAllPostsAsync();
    Task<bool> UpdatePostAsync(int postId, PostInfo postInfo);
    Task<bool> DeletePostAsync(int postId);
    Task<AdminStats> GetAdminStatsAsync();
}

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

public class AdminUserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PostInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AdminStats
{
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int ActiveUsers { get; set; }
    public int PublishedPosts { get; set; }
    public int PendingPosts { get; set; }
    public DateTime LastUpdated { get; set; }
}