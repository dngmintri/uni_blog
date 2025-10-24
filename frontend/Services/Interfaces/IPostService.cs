public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
}