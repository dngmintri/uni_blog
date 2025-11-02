using Backend.DTOs.Common;
using Backend.DTOs.Posts;

namespace Backend.Services.Interfaces;

public interface IPostService
{
	Task<PagedResult<PostDto>> GetPagedAsync(int page, int pageSize);
	Task<PostDto?> GetByIdAsync(int id);
	Task<PostDto> CreateAsync(int userId, CreatePostRequest req);
	Task<bool> UpdateAsync(int id, int currentUserId, bool isAdmin, UpdatePostRequest req);
	Task<bool> SoftDeleteAsync(int id, int currentUserId, bool isAdmin);
	Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId);
}