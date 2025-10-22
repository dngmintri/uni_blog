using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;

namespace frontend.Services;

public interface IAdminService
{
    Task<List<AdminUserInfo>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(int userId, AdminUserInfo userInfo);
    Task<bool> DeleteUserAsync(int userId);
    Task<List<PostInfo>> GetAllPostsAsync();
    Task<bool> UpdatePostAsync(int postId, PostInfo postInfo);
    Task<bool> DeletePostAsync(int postId);
    Task<AdminStats> GetAdminStatsAsync();
}

public class AdminService : IAdminService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILocalStorageService _localStorage;

    public AdminService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<AdminUserInfo>> GetAllUsersAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return new List<AdminUserInfo>();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/admin/users");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<AdminUserInfo>>(content, _jsonOptions) ?? new List<AdminUserInfo>();
            }

            return new List<AdminUserInfo>();
        }
        catch
        {
            return new List<AdminUserInfo>();
        }
    }

    public async Task<bool> UpdateUserAsync(int userId, AdminUserInfo userInfo)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Tạo object phù hợp với backend AdminUserUpdateRequest
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

            var response = await _httpClient.PutAsync($"api/admin/users/{userId}", content);
            Console.WriteLine($"AdminService: Update response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"AdminService: Error response: {errorContent}");
            }
            
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/admin/users/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<PostInfo>> GetAllPostsAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return new List<PostInfo>();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/admin/posts");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PostInfo>>(content, _jsonOptions) ?? new List<PostInfo>();
            }

            return new List<PostInfo>();
        }
        catch
        {
            return new List<PostInfo>();
        }
    }

    public async Task<bool> UpdatePostAsync(int postId, PostInfo postInfo)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(postInfo, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/admin/posts/{postId}", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePostAsync(int postId)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/admin/posts/{postId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<AdminStats> GetAdminStatsAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return new AdminStats();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/admin/stats");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AdminStats>(content, _jsonOptions) ?? new AdminStats();
            }

            return new AdminStats();
        }
        catch
        {
            return new AdminStats();
        }
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
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = true;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
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
