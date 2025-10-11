using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kết nối database MySQL
var connectDB = builder.Configuration.GetConnectionString("DefaultConnection"); // Lấy chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseMySql(
        connectDB,
        ServerVersion.AutoDetect(connectDB)
    ));

var app = builder.Build();

app.MapGet("/", () => "HELLO WORLD");
// Test connect
app.MapGet("/dbchecking", async (BlogDbContext blogDbContext) =>
{
    try
    {
        await blogDbContext.Database.CanConnectAsync();
        return Results.Ok("Da ket noi den database");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Loi ket noi database. {ex.Message}");
    }
});

app.Run();