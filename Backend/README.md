# 🔧 UniBlog Backend - ASP.NET Web API

Backend API cho ứng dụng UniBlog được xây dựng với ASP.NET Core 8.0, Entity Framework Core và MySQL.

## 📁 Cấu trúc Project

```
UniBlog.API/
├── Controllers/              # API Controllers
│   ├── AuthController.cs     # Authentication endpoints
│   ├── UsersController.cs    # User management
│   ├── PostsController.cs    # Blog posts management
│   ├── CommentsController.cs # Comments management
│   └── LogsController.cs     # System logs
│
├── Data/                     # Database Context
│   └── ApplicationDbContext.cs
│
├── DTOs/                     # Data Transfer Objects
│   ├── Auth/                 # Authentication DTOs
│   ├── User/                 # User DTOs
│   ├── Post/                 # Post DTOs
│   └── Comment/              # Comment DTOs
│
├── Models/                   # Domain Models
│   ├── User.cs
│   ├── Post.cs
│   ├── Comment.cs
│   └── Log.cs
│
├── Repositories/             # Data Access Layer
│   ├── Interfaces/
│   └── Implementations/
│
├── Services/                 # Business Logic Layer
│   ├── Interfaces/
│   └── Implementations/
│
├── Properties/
│   └── launchSettings.json   # Launch configuration
│
├── appsettings.json          # Configuration
├── appsettings.Development.json
├── Program.cs                # Application entry point
└── UniBlog.API.csproj        # Project file
```

## 🚀 Cài đặt

### 1. Prerequisites
- .NET 8.0 SDK
- MySQL 8.0+ hoặc MariaDB 10.5+

### 2. Cấu hình Database

Cập nhật connection string trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=uni_blog;User=root;Password=your_password;"
  }
}
```

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Chạy ứng dụng

```bash
dotnet run
```

Hoặc với watch mode:

```bash
dotnet watch run
```

API sẽ chạy tại:
- HTTPS: https://localhost:7100
- HTTP: http://localhost:5100
- Swagger: https://localhost:7100/swagger

## 📚 API Endpoints

### Authentication

#### POST /api/auth/login
Đăng nhập

**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response:**
```json
{
  "token": "string",
  "username": "string",
  "role": "string",
  "expiration": "2024-01-01T00:00:00"
}
```

#### POST /api/auth/register
Đăng ký tài khoản mới

**Request:**
```json
{
  "username": "string",
  "password": "string",
  "email": "string",
  "fullName": "string",
  "dateOfBirth": "2000-01-01",
  "gender": "Nam|Nữ|Khác"
}
```

### Posts

#### GET /api/posts?publishedOnly=true
Lấy danh sách bài viết

#### GET /api/posts/{id}
Lấy chi tiết bài viết (tự động tăng views)

#### POST /api/posts
Tạo bài viết mới (yêu cầu authentication)

**Request:**
```json
{
  "title": "string",
  "content": "string",
  "imageUrl": "string",
  "isPublished": true
}
```

#### PUT /api/posts/{id}
Cập nhật bài viết (yêu cầu authentication)

#### DELETE /api/posts/{id}
Xóa bài viết (yêu cầu authentication)

### Comments

#### GET /api/comments/post/{postId}
Lấy comments của bài viết

#### POST /api/comments
Tạo comment mới (yêu cầu authentication)

**Request:**
```json
{
  "postId": 1,
  "content": "string"
}
```

#### DELETE /api/comments/{id}
Xóa comment (yêu cầu authentication)

## 🔐 Authentication & Security

### JWT Configuration

File `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration12345678",
    "Issuer": "UniBlogAPI",
    "Audience": "UniBlogClient",
    "ExpirationMinutes": 60
  }
}
```

**Lưu ý:** Thay đổi `SecretKey` trong production!

### Authorization

Sử dụng Bearer Token trong header:
```
Authorization: Bearer {your-jwt-token}
```

### Roles
- **User**: Người dùng thông thường
- **Admin**: Quản trị viên

## 🗄️ Database

### Entity Framework Migrations

Nếu cần tạo migration mới:

```bash
# Add migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

**Lưu ý:** Project này sử dụng MySQL schema có sẵn (`uni_blog.sql`), nên không cần chạy migrations.

## 📦 NuGet Packages

- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `Microsoft.EntityFrameworkCore` - ORM
- `Pomelo.EntityFrameworkCore.MySql` - MySQL provider
- `BCrypt.Net-Next` - Password hashing
- `AutoMapper.Extensions.Microsoft.DependencyInjection` - Object mapping
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI

## 🔧 Configuration

### CORS

Mặc định cho phép request từ:
- https://localhost:5001
- http://localhost:5000

Để thêm origin khác, cập nhật trong `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("your-frontend-url")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
```

## 🧪 Testing API

### Sử dụng Swagger UI

1. Chạy API
2. Mở browser: https://localhost:7100/swagger
3. Test các endpoints trực tiếp trên UI

### Sử dụng cURL

```bash
# Login
curl -X POST https://localhost:7100/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'

# Get posts
curl https://localhost:7100/api/posts

# Create post (with authentication)
curl -X POST https://localhost:7100/api/posts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"title":"Test","content":"Content","isPublished":true}'
```

## 🐛 Debugging

### Enable detailed errors

Trong `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### Database Connection Issues

Kiểm tra:
1. MySQL/MariaDB service đã chạy
2. Connection string đúng
3. Database `uni_blog` đã được tạo
4. User có quyền truy cập database

## 📝 Best Practices

1. **Không commit** file `appsettings.json` chứa thông tin nhạy cảm
2. Sử dụng **User Secrets** cho development
3. Sử dụng **Environment Variables** cho production
4. Luôn validate input từ client
5. Implement proper error handling
6. Log các actions quan trọng

## 🚀 Deployment

### Publish

```bash
dotnet publish -c Release -o ./publish
```

### Environment Variables (Production)

Đặt các biến môi trường sau:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="your-production-connection-string"
JwtSettings__SecretKey="your-production-secret-key"
```

---

**Happy Coding! 🚀**

