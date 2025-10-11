using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;
public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình enum UserRole
        modelBuilder.Entity<User>()
            .Property(user => user.Role)
            .HasConversion<string>();
    }
}