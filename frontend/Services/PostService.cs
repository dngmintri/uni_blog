using System.Net.Http.Json;


public class PostService : IPostService
{
    private readonly HttpClient _http;
    public PostService(HttpClient http)
    {
        _http = http;
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
}