using frontend.Models;

public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task<PagedResult<Post>> GetPagedPostsAsync(int page = 1, int pageSize = 10);
    Task<bool> CreatePostAsync(PostCreateRequest post);
    // Task<bool> UpdatePostAsync(Post post);
    // Task<bool> DeletePostAsync(int postId);
}