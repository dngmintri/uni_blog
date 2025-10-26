namespace frontend.Models;

public class AdminStats
{
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int ActiveUsers { get; set; }
    public int PublishedPosts { get; set; }
    public int PendingPosts { get; set; }
    public DateTime LastUpdated { get; set; }
}