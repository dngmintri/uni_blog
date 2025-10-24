using System.Text;
using System.Text.Json;
using Blazorise;
using Blazored.LocalStorage;

namespace frontend.Services;

public class UserService : BaseAuthenticatedService, IUserService
{
    public UserService(HttpClient httpClient, ITokenManagerService tokenManager) 
        : base(httpClient, tokenManager) { }

    public async Task<AuthResponse?> GetUserProfileAsync()
    {
        return await ExecuteAuthenticatedRequestAsync<AuthResponse>(() => 
            _httpClient.GetAsync("api/users/profile"));
    }

    public async Task<bool> UpdateProfileAsync(UserInfo userInfo)
    {
        var updateRequest = new
        {
            FullName = userInfo.FullName,
            Email = userInfo.Email,
            DateOfBirth = userInfo.DateOfBirth?.ToString("yyyy-MM-dd"),
            Gender = userInfo.Gender
        };
        
        var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        Console.WriteLine($"UserService: Sending update request: {json}");
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteAuthenticatedRequestWithContentAsync(updateRequest, () => 
            _httpClient.PutAsync("api/users/profile", content));
    }

    public async Task<bool> ChangePasswordAsync(frontend.Pages.User.Profile.PasswordChange passwordChange)
    {
        var changePasswordRequest = new
        {
            CurrentPassword = passwordChange.CurrentPassword,
            NewPassword = passwordChange.NewPassword
        };
        
        var json = JsonSerializer.Serialize(changePasswordRequest, _jsonOptions);
        Console.WriteLine($"UserService: Sending password change request");
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteAuthenticatedRequestWithContentAsync(changePasswordRequest, () => 
            _httpClient.PutAsync("api/users/change-password", content));
    }

    public async Task<bool> UploadAvatarAsync(IFileEntry file)
    {
        if (!await EnsureTokenIsSetAsync())
            return false;

        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB max
            content.Add(new StreamContent(fileStream), "file", file.Name);

            var response = await _httpClient.PostAsync("api/users/upload-avatar", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UserService: Upload avatar error: {ex.Message}");
            return false;
        }
    }
}