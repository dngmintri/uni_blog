using Blazorise;

namespace frontend.Services
{
    public interface IUserService
    {
        Task<AuthResponse?> GetUserProfileAsync();
        Task<bool> UpdateProfileAsync(UserInfo userInfo);
        Task<bool> ChangePasswordAsync(frontend.Pages.User.Profile.PasswordChange passwordChange);
        Task<bool> UploadAvatarAsync(IFileEntry file);
    }

    public class UserInfo
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    public class PasswordChange
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
