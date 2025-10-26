using System.Net.Http.Json;
using frontend.Services;
using frontend.Models;

public class PostService : BaseAuthenticatedService, IPostService
{
    private readonly IUploadService _uploadService;
    public PostService(HttpClient http, ITokenManagerService tokenManager, IUploadService uploadService) 
        : base(http, tokenManager)
    {
        _uploadService = uploadService;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<PostsResponse>("api/posts");
            return response?.Items ?? new List<Post>();
        }
        catch
        {
            return new List<Post>();
        }
    }

    public async Task<PagedResult<Post>> GetPagedPostsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<Post>>($"api/posts?page={page}&pageSize={pageSize}");
            return response ?? new PagedResult<Post>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting paged posts: {ex.Message}");
            return new PagedResult<Post>();
        }
    }

    public async Task<bool> CreatePostAsync(PostCreateRequest post)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(post);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var success = await ExecuteAuthenticatedRequestAsync(() =>
                _httpClient.PostAsync("api/posts", content));
            return success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating post: {ex.Message}");
            return false;
        }
    }
    public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
    {
        try
        {
            Console.WriteLine($"📡 PostService.GetPostsByUserIdAsync: Calling api/posts/user/{userId}");
            var response = await _httpClient.GetAsync($"api/posts/user/{userId}");
            Console.WriteLine($"📡 PostService.GetPostsByUserIdAsync: Response status = {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📡 PostService.GetPostsByUserIdAsync: Response content length = {content.Length}");
                Console.WriteLine($"📡 PostService.GetPostsByUserIdAsync: Response content preview = {content.Substring(0, Math.Min(200, content.Length))}");
                
                var posts = System.Text.Json.JsonSerializer.Deserialize<List<Post>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine($"📦 PostService.GetPostsByUserIdAsync: Deserialized {posts?.Count ?? 0} posts");
                return posts ?? new List<Post>();
            }
            else
            {
                Console.WriteLine($"❌ PostService.GetPostsByUserIdAsync: Failed with status {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ PostService.GetPostsByUserIdAsync: Error content = {errorContent}");
                return new List<Post>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ PostService.GetPostsByUserIdAsync: Exception = {ex.Message}");
            Console.WriteLine($"❌ PostService.GetPostsByUserIdAsync: Stack trace = {ex.StackTrace}");
            return new List<Post>();
        }
    }
}