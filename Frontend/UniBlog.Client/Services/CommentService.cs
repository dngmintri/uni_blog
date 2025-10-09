using System.Net.Http.Json;
using UniBlog.Client.Models;
using UniBlog.Client.Services.Interfaces;

namespace UniBlog.Client.Services;

public class CommentService : ICommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CommentDto>>($"/api/comments/post/{postId}") 
                ?? new List<CommentDto>();
        }
        catch
        {
            return new List<CommentDto>();
        }
    }

    public async Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/comments", createCommentDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CommentDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/comments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}


