using frontend.Models;
using frontend.Services;
using System.Text;
using System.Text.Json;

namespace frontend.Services;

public class CommentService : BaseAuthenticatedService
{
    public CommentService(HttpClient httpClient, ITokenManagerService tokenManager) 
        : base(httpClient, tokenManager)
    {
    }

    public async Task<List<Comment>> GetCommentsByPostAsync(int postId)
    {
        try
        {
            var response = await ExecuteAuthenticatedRequestAsync<List<Comment>>(() =>
                _httpClient.GetAsync($"api/posts/{postId}/comments"));
            return response ?? new List<Comment>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting comments: {ex.Message}");
            return new List<Comment>();
        }
    }

    public async Task<Comment?> CreateCommentAsync(int postId, CreateCommentRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await ExecuteAuthenticatedRequestAsync<Comment>(() =>
                _httpClient.PostAsync($"api/posts/{postId}/comments", content));
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating comment: {ex.Message}");
            return null;
        }
    }
}
