using System.Net.Http.Json;
using UniBlog.Client.Models;
using UniBlog.Client.Services.Interfaces;

namespace UniBlog.Client.Services;

public class PostService : IPostService
{
    private readonly HttpClient _httpClient;

    public PostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PostDto>> GetPostsAsync(bool publishedOnly = true)
    {
        try
        {
            var url = publishedOnly ? "/api/posts?publishedOnly=true" : "/api/posts";
            return await _httpClient.GetFromJsonAsync<List<PostDto>>(url) ?? new List<PostDto>();
        }
        catch
        {
            return new List<PostDto>();
        }
    }

    public async Task<PostDto?> GetPostByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PostDto>($"/api/posts/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<PostDto>> GetPostsByUserIdAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PostDto>>($"/api/posts/user/{userId}") 
                ?? new List<PostDto>();
        }
        catch
        {
            return new List<PostDto>();
        }
    }

    public async Task<PostDto?> CreatePostAsync(CreatePostDto createPostDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/posts", createPostDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PostDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<PostDto?> UpdatePostAsync(int id, CreatePostDto updatePostDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/posts/{id}", updatePostDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PostDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/posts/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

