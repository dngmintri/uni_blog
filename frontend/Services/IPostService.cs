public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task<bool> CreatePostAsync(PostCreateRequest post);
    // Task<bool> UpdatePostAsync(Post post);
    // Task<bool> DeletePostAsync(int postId);
}