using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Repositories;
using Backend.Repositories.Interfaces;
using Backend.Services;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

//==================================================
// KẾT NỐI DATABASE (MySQL)
//==================================================
var connectDB = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseMySql(
        connectDB,
        ServerVersion.AutoDetect(connectDB)
    ));

//==================================================
// CẤU HÌNH DỊCH VỤ CƠ BẢN
//==================================================
builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddEndpointsApiExplorer();

//==================================================
// ĐĂNG KÝ DEPENDENCY INJECTION (Repositories & Services)
//==================================================
// Repository
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// JWT Token Service
builder.Services.AddSingleton<JwtTokenService>();

//==================================================
// CẤU HÌNH SWAGGER (API Documentation)
//==================================================
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "UniBlog API", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    o.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

//==================================================
// CẤU HÌNH CORS (Cho phép frontend gọi API)
//==================================================
builder.Services.AddCors(o => o.AddPolicy("Wasm", p => p
    .WithOrigins("http://localhost:5000", "http://localhost:5001", "https://localhost:7172", "https://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

//==================================================
// CẤU HÌNH JWT AUTHENTICATION
//==================================================
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // tránh delay thời gian hết hạn token
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

//==================================================
// MIDDLEWARE PIPELINE
//==================================================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Wasm");
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//==================================================
// TEST ROUTE & ENDPOINTS PHỤ
//==================================================
// Test API mặc định
app.MapGet("/", () => "HELLO WORLD");

// Kiểm tra kết nối database
app.MapGet("/dbchecking", async (BlogDbContext blogDbContext) =>
{
    try
    {
        await blogDbContext.Database.CanConnectAsync();
        return Results.Ok("Đã kết nối đến database thành công!");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Lỗi kết nối database: {ex.Message}");
    }
});

app.Run();
