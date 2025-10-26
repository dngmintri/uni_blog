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
}