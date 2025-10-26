using System.Text.Json;

namespace frontend.Services;

public abstract class BaseAuthenticatedService
{
    protected readonly HttpClient _httpClient;
    protected readonly ITokenManagerService _tokenManager;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected BaseAuthenticatedService(HttpClient httpClient, ITokenManagerService tokenManager)
    {
        _httpClient = httpClient;
        _tokenManager = tokenManager;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    protected async Task<bool> EnsureTokenIsSetAsync()
    {
        return await _tokenManager.EnsureTokenIsSetAsync(_httpClient);
    }

    protected async Task<T?> ExecuteAuthenticatedRequestAsync<T>(Func<Task<HttpResponseMessage>> requestFunc)
    {
        if (!await EnsureTokenIsSetAsync())
        {
            Console.WriteLine("BaseAuthenticatedService: No valid token available");
            return default(T);
        }

        try
        {
            var response = await requestFunc();
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"BaseAuthenticatedService: Request successful, response length: {content.Length}");
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("BaseAuthenticatedService: Unauthorized response - clearing token");
                await _tokenManager.ClearTokenAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"BaseAuthenticatedService: Request failed with status {response.StatusCode}: {errorContent}");
            }
            
            return default(T);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BaseAuthenticatedService: Request exception: {ex.Message}");
            return default(T);
        }
    }

    protected async Task<bool> ExecuteAuthenticatedRequestAsync(Func<Task<HttpResponseMessage>> requestFunc)
    {
        if (!await EnsureTokenIsSetAsync())
        {
            Console.WriteLine("BaseAuthenticatedService: No valid token available");
            return false;
        }

        try
        {
            var response = await requestFunc();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("BaseAuthenticatedService: Request successful");
                return true;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("BaseAuthenticatedService: Unauthorized response - clearing token");
                await _tokenManager.ClearTokenAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"BaseAuthenticatedService: Request failed with status {response.StatusCode}: {errorContent}");
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BaseAuthenticatedService: Request exception: {ex.Message}");
            return false;
        }
    }

    protected async Task<bool> ExecuteAuthenticatedRequestWithContentAsync<T>(T requestData, Func<Task<HttpResponseMessage>> requestFunc)
    {
        if (!await EnsureTokenIsSetAsync())
        {
            Console.WriteLine("BaseAuthenticatedService: No valid token available");
            return false;
        }

        try
        {
            var response = await requestFunc();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("BaseAuthenticatedService: Request with content successful");
                return true;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("BaseAuthenticatedService: Unauthorized response - clearing token");
                await _tokenManager.ClearTokenAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"BaseAuthenticatedService: Request failed with status {response.StatusCode}: {errorContent}");
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BaseAuthenticatedService: Request exception: {ex.Message}");
            return false;
        }
    }
}
