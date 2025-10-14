using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kết nối database MySQL
var connectDB = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseMySql(
        connectDB,
        ServerVersion.AutoDetect(connectDB)
    ));

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("Wasm", p => p
    .WithOrigins("https://localhost:7172", "http://localhost:5173")
    .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Wasm");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

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