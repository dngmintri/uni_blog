using System.Net.Http.Json;

public class PostService : IPostService
{
    private readonly HttpClient _http;
    private readonly IUploadService _uploadService;
    public PostService(HttpClient http,IUploadService uploadService)
    {
        _http = http;
        _uploadService = uploadService;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<PostsResponse>("api/posts");
            return response?.Items ?? new List<Post>();
        }
        catch
        {
            return new List<Post>();
        }
    }

    public async Task<bool> CreatePostAsync(PostCreateRequest post)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/posts", post);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}