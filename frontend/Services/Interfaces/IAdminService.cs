using frontend.Models;
namespace frontend.Services;

public interface IAdminService
{
    Task<List<AdminUserInfo>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(int userId, AdminUserInfo userInfo);
    Task<List<PostInfo>> GetAllPostsAsync();
    Task<bool> DeletePostAsync(int postId);
    Task<AdminStats> GetAdminStatsAsync();
}