using Blazorise;
using frontend.Models;

namespace frontend.Services;
public interface IUserService
{
    Task<AuthResponse?> GetUserProfileAsync();
    Task<bool> UpdateProfileAsync(UserInfo userInfo);
    Task<bool> ChangePasswordAsync(PasswordChange passwordChange);
    Task<bool> UploadAvatarAsync(IFileEntry file);
}
