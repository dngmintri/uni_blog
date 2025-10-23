using System.Text;
using System.Text.Json;
using Blazorise;
using Blazored.LocalStorage;

namespace frontend.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILocalStorageService _localStorage;

    public UserService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<AuthResponse?> GetUserProfileAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/users/profile");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateProfileAsync(UserInfo userInfo)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            Console.WriteLine($"UserService token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("UserService: No token found");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine("UserService: Authorization header set");

            // Tạo object theo format backend expect
            var updateRequest = new
            {
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                DateOfBirth = userInfo.DateOfBirth?.ToString("yyyy-MM-dd"),
                Gender = userInfo.Gender
            };
            
            var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
            Console.WriteLine($"UserService sending JSON: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("api/users/profile", content);
            Console.WriteLine($"UserService response status: {response.StatusCode}");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"UserService response content: {responseContent}");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"UserService error response: {responseContent}");
            }
            else
            {
                Console.WriteLine($"UserService success response: {responseContent}");
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UserService exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(frontend.Pages.User.Profile.PasswordChange passwordChange)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Tạo object theo format backend expect (chỉ có CurrentPassword và NewPassword)
            var changePasswordRequest = new
            {
                CurrentPassword = passwordChange.CurrentPassword,
                NewPassword = passwordChange.NewPassword
            };
            
            var json = JsonSerializer.Serialize(changePasswordRequest, _jsonOptions);
            Console.WriteLine($"UserService ChangePassword sending JSON: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("api/users/change-password", content);
            Console.WriteLine($"UserService ChangePassword response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"UserService ChangePassword error response: {errorContent}");
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UserService ChangePassword exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UploadAvatarAsync(IFileEntry file)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB max
            content.Add(new StreamContent(fileStream), "file", file.Name);

            var response = await _httpClient.PostAsync("api/users/upload-avatar", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
