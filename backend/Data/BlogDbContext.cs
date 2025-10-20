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

        modelBuilder.Entity<User>()
            .Property(user => user.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== SEED DỮ LIỆU =====
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Username = "admin",
                Email = "admin@example.com",
                FullName = "Admin User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = User.UserRoleEnum.Admin,
                CreatedAt = DateTime.Now,
                IsActive = true
            },
            new User
            {
                UserId = 2,
                Username = "john_doe",
                Email = "john@example.com",
                FullName = "John Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRoleEnum.User,
                CreatedAt = DateTime.Now,
                IsActive = true
            },
            new User
            {
                UserId = 3,
                Username = "jane_smith",
                Email = "jane@example.com",
                FullName = "Jane Smith",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = User.UserRoleEnum.User,
                CreatedAt = DateTime.Now,
                IsActive = true
            }
        );

        // Seed Posts
        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                PostId = 1,
                UserId = 1,
                Title = "Welcome to UniBlog",
                Content = "This is the first post on our blog platform. Welcome everyone!",
                ImageUrl = "https://via.placeholder.com/300x200",
                CreatedAt = DateTime.Now,
                Views = 100,
                IsPublished = true,
                IsDeleted = false
            },
            new Post
            {
                PostId = 2,
                UserId = 2,
                Title = "My First Blog Post",
                Content = "Hello everyone! This is my first blog post. I'm excited to share my thoughts here.",
                ImageUrl = "https://via.placeholder.com/300x200",
                CreatedAt = DateTime.Now.AddDays(-1),
                Views = 50,
                IsPublished = true,
                IsDeleted = false
            },
            new Post
            {
                PostId = 3,
                UserId = 3,
                Title = "Tips for Blogging",
                Content = "Here are some tips for writing great blog posts: 1. Be authentic 2. Write regularly 3. Engage with readers",
                ImageUrl = "https://via.placeholder.com/300x200",
                CreatedAt = DateTime.Now.AddDays(-2),
                Views = 75,
                IsPublished = true,
                IsDeleted = false
            }
        );

        // Seed Comments
        modelBuilder.Entity<Comment>().HasData(
            new Comment
            {
                CommentId = 1,
                PostId = 1,
                UserId = 2,
                Content = "Great post! Looking forward to more content.",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            },
            new Comment
            {
                CommentId = 2,
                PostId = 1,
                UserId = 3,
                Content = "Thanks for creating this platform!",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            },
            new Comment
            {
                CommentId = 3,
                PostId = 2,
                UserId = 1,
                Content = "Nice first post, John!",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            }
        );
    }
}