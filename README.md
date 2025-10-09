# 📝 UniBlog - Nền tảng Blog cho Sinh viên

UniBlog là một nền tảng blog được xây dựng với **ASP.NET Web API** (Backend) và **Blazor WebAssembly** (Frontend), được thiết kế đặc biệt cho cộng đồng sinh viên để chia sẻ kiến thức và kết nối với nhau.

## 🏗️ Kiến trúc Hệ thống

```
uni_blog/
├── Backend/                      # ASP.NET Web API
│   └── UniBlog.API/
│       ├── Controllers/          # API Controllers
│       ├── Data/                 # Database Context
│       ├── DTOs/                 # Data Transfer Objects
│       ├── Models/               # Domain Models
│       ├── Repositories/         # Data Access Layer
│       ├── Services/             # Business Logic Layer
│       └── Program.cs            # Application Entry Point
│
├── Frontend/                     # Blazor WASM
│   └── UniBlog.Client/
│       ├── Models/               # Client Models
│       ├── Pages/                # Razor Pages/Components
│       ├── Services/             # API Services
│       ├── Shared/               # Shared Components
│       └── wwwroot/              # Static Files
│
└── uni_blog.sql                  # Database Schema
```

## 🚀 Công nghệ Sử dụng

### Backend
- **ASP.NET Core 8.0** - Web API Framework
- **Entity Framework Core** - ORM
- **MySQL/MariaDB** - Database
- **JWT Authentication** - Bảo mật
- **BCrypt.Net** - Mã hóa mật khẩu
- **AutoMapper** - Object Mapping
- **Swagger/OpenAPI** - API Documentation

### Frontend
- **Blazor WebAssembly** - SPA Framework
- **Blazored.LocalStorage** - Local Storage Management
- **HttpClient** - API Communication
- **CSS3** - Styling
- **Authorization** - Route Protection

## 📋 Yêu cầu Hệ thống

- **.NET 8.0 SDK** hoặc cao hơn
- **MySQL 8.0+** hoặc **MariaDB 10.5+**
- **Visual Studio 2022** hoặc **VS Code** hoặc **Rider**
- **Git** (optional)

## 🔧 Cài đặt và Chạy

### 1. Cấu hình Database

```bash
# Tạo database
mysql -u root -p

# Import schema
mysql -u root -p uni_blog < uni_blog.sql
```

Hoặc tạo database mới:
```sql
CREATE DATABASE uni_blog CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 2. Cấu hình Backend

**Bước 1:** Di chuyển vào thư mục Backend
```bash
cd Backend/UniBlog.API
```

**Bước 2:** Cập nhật Connection String trong `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=uni_blog;User=root;Password=your_password;"
  }
}
```

**Bước 3:** Restore packages
```bash
dotnet restore
```

**Bước 4:** Build project
```bash
dotnet build
```

**Bước 5:** Chạy API
```bash
dotnet run
```

API sẽ chạy tại:
- HTTPS: `https://localhost:7100`
- HTTP: `http://localhost:5100`
- Swagger UI: `https://localhost:7100/swagger`

### 3. Cấu hình Frontend

**Bước 1:** Di chuyển vào thư mục Frontend
```bash
cd Frontend/UniBlog.Client
```

**Bước 2:** Cập nhật API URL trong `Program.cs` (nếu cần)
```csharp
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7100") 
});
```

**Bước 3:** Restore packages
```bash
dotnet restore
```

**Bước 4:** Build project
```bash
dotnet build
```

**Bước 5:** Chạy ứng dụng
```bash
dotnet run
```

Hoặc sử dụng watch mode để tự động reload:
```bash
dotnet watch run
```

Frontend sẽ chạy tại:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## 📚 Cấu trúc API Endpoints

### Authentication
```
POST   /api/auth/login      - Đăng nhập
POST   /api/auth/register   - Đăng ký tài khoản
```

### Users
```
GET    /api/users           - Lấy danh sách users (Admin only)
GET    /api/users/{id}      - Lấy thông tin user
GET    /api/users/me        - Lấy thông tin user hiện tại
PUT    /api/users/{id}      - Cập nhật thông tin user
DELETE /api/users/{id}      - Xóa user (Admin only)
```

### Posts
```
GET    /api/posts                    - Lấy danh sách bài viết
GET    /api/posts/{id}               - Lấy chi tiết bài viết
GET    /api/posts/user/{userId}      - Lấy bài viết của user
POST   /api/posts                    - Tạo bài viết mới (Auth required)
PUT    /api/posts/{id}               - Cập nhật bài viết (Auth required)
DELETE /api/posts/{id}               - Xóa bài viết (Auth required)
```

### Comments
```
GET    /api/comments/post/{postId}   - Lấy comments của bài viết
POST   /api/comments                 - Tạo comment mới (Auth required)
DELETE /api/comments/{id}            - Xóa comment (Auth required)
```

### Logs
```
GET    /api/logs                     - Lấy tất cả logs (Admin only)
GET    /api/logs/user/{userId}       - Lấy logs của user (Admin only)
```

## 🔐 Authentication & Authorization

### JWT Token
- Token được lưu trong **LocalStorage** của browser
- Thời gian sống mặc định: **60 phút**
- Token tự động gửi kèm trong Header `Authorization: Bearer {token}`

### Roles
- **User**: Người dùng thông thường
  - Tạo, sửa, xóa bài viết của mình
  - Tạo, xóa comment của mình
  - Xem profile
  
- **Admin**: Quản trị viên
  - Tất cả quyền của User
  - Xóa bất kỳ bài viết, comment nào
  - Xem logs hệ thống
  - Quản lý users

## 🎨 Tính năng Chính

### Người dùng
- ✅ Đăng ký/Đăng nhập
- ✅ Quản lý profile cá nhân
- ✅ Xem danh sách bài viết
- ✅ Xem chi tiết bài viết
- ✅ Tạo bài viết mới
- ✅ Chỉnh sửa/xóa bài viết của mình
- ✅ Bình luận trên bài viết
- ✅ Xóa comment của mình
- ✅ Theo dõi lượt xem

### Admin
- ✅ Tất cả quyền của User
- ✅ Xem danh sách users
- ✅ Xóa users
- ✅ Xóa bất kỳ bài viết/comment nào
- ✅ Xem system logs

## 🗄️ Database Schema

### users
- `user_id` (PK)
- `username` (unique)
- `password_hash`
- `email` (unique)
- `full_name`
- `date_of_birth`
- `gender` (Nam/Nữ/Khác)
- `role` (User/Admin)
- `created_at`
- `last_login`

### posts
- `post_id` (PK)
- `user_id` (FK)
- `title`
- `content`
- `image_url`
- `created_at`
- `updated_at`
- `views`
- `is_published`

### comments
- `comment_id` (PK)
- `post_id` (FK)
- `user_id` (FK)
- `content`
- `created_at`
- `is_deleted`

### logs
- `log_id` (PK)
- `user_id` (FK)
- `action`
- `description`
- `created_at`

## 🔨 Development

### Chạy Backend ở chế độ Development
```bash
cd Backend/UniBlog.API
dotnet watch run
```

### Chạy Frontend ở chế độ Development
```bash
cd Frontend/UniBlog.Client
dotnet watch run
```

### Xem API Documentation
Truy cập Swagger UI sau khi chạy Backend:
```
https://localhost:7100/swagger
```

## 📦 Build cho Production

### Build Backend
```bash
cd Backend/UniBlog.API
dotnet publish -c Release -o ./publish
```

### Build Frontend
```bash
cd Frontend/UniBlog.Client
dotnet publish -c Release -o ./publish
```

## 🐛 Troubleshooting

### Lỗi kết nối Database
- Kiểm tra MySQL/MariaDB đã chạy chưa
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo database `uni_blog` đã được tạo

### Lỗi CORS
- Kiểm tra URL Frontend trong CORS configuration ở `Program.cs` Backend
- Mặc định: `https://localhost:5001` và `http://localhost:5000`

### Lỗi JWT Token
- Kiểm tra `SecretKey` trong `appsettings.json` phải đủ dài (>= 32 ký tự)
- Xóa LocalStorage và đăng nhập lại

### Frontend không kết nối được API
- Kiểm tra Backend đã chạy chưa
- Kiểm tra `BaseAddress` trong `Program.cs` Frontend
- Kiểm tra HTTPS certificate

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

Nếu có bất kỳ câu hỏi nào, vui lòng tạo issue trên GitHub.

---

**Chúc bạn code vui vẻ! 🚀**
