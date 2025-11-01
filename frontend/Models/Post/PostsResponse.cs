namespace frontend.Models;
public class PostsResponse
{
    public int Total { get; set; }
    public List<Post> Items { get; set; } = new();
}