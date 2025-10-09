namespace UniBlog.API.Models;

public class Log
{
    public int LogId { get; set; }
    public int? UserId { get; set; }
    public string? Action { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public User? User { get; set; }
}

