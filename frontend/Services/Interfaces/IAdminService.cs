using frontend.Models;
namespace frontend.Services;

public interface IAdminService
{
    Task<List<AdminUserInfo>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(int userId, AdminUserInfo userInfo);
    Task<List<PostInfo>> GetAllPostsAsync();
    Task<bool> UpdatePostAsync(int postId, PostInfo postInfo);
    Task<bool> DeletePostAsync(int postId);
    Task<AdminStats> GetAdminStatsAsync();
}