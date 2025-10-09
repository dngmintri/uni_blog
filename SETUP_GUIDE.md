# 📖 Hướng dẫn Cài đặt và Chạy UniBlog

Tài liệu này hướng dẫn chi tiết từng bước để cài đặt và chạy ứng dụng UniBlog.

## 📋 Yêu cầu

Trước khi bắt đầu, đảm bảo bạn đã cài đặt:

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ [MySQL 8.0+](https://dev.mysql.com/downloads/) hoặc [MariaDB 10.5+](https://mariadb.org/download/)
- ✅ IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), hoặc [JetBrains Rider](https://www.jetbrains.com/rider/)
- ✅ Git (optional)

## 🔧 Bước 1: Chuẩn bị Database

### 1.1. Khởi động MySQL/MariaDB

#### Windows
```cmd
# Nếu dùng XAMPP
C:\xampp\mysql\bin\mysql.exe -u root -p

# Nếu cài MySQL standalone
mysql -u root -p
```

#### macOS/Linux
```bash
mysql -u root -p
```

### 1.2. Tạo Database

Trong MySQL console:

```sql
CREATE DATABASE uni_blog CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 1.3. Import Schema

#### Option 1: Từ file SQL
```bash
# Exit MySQL console trước
exit

# Import file
mysql -u root -p uni_blog < uni_blog.sql
```

#### Option 2: Copy-paste SQL
```sql
# Copy toàn bộ nội dung file uni_blog.sql
# Paste vào MySQL console và chạy
```

### 1.4. Verify Database

```sql
USE uni_blog;
SHOW TABLES;
```

Bạn sẽ thấy 4 tables: `users`, `posts`, `comments`, `logs`

## 🎯 Bước 2: Cấu hình Backend

### 2.1. Navigate to Backend folder

```bash
cd Backend/UniBlog.API
```

### 2.2. Cấu hình Connection String

Mở file `appsettings.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=uni_blog;User=root;Password=YOUR_PASSWORD;"
  }
}
```

**Lưu ý:** 
- Thay `YOUR_PASSWORD` bằng password MySQL của bạn
- Nếu không có password, để trống: `Password=;`
- Port mặc định MySQL: `3306`

### 2.3. Cấu hình JWT Settings (Optional)

Trong production, thay đổi `SecretKey`:

```json
{
  "JwtSettings": {
    "SecretKey": "ThayDoiKeyNayTrongProduction123456789",
    "Issuer": "UniBlogAPI",
    "Audience": "UniBlogClient",
    "ExpirationMinutes": 60
  }
}
```

### 2.4. Restore NuGet Packages

```bash
dotnet restore
```

### 2.5. Build Project

```bash
dotnet build
```

### 2.6. Chạy Backend API

```bash
dotnet run
```

Hoặc với watch mode (tự động reload):

```bash
dotnet watch run
```

**Kết quả:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7100
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5100
```

### 2.7. Test API

Mở browser và truy cập:
```
https://localhost:7100/swagger
```

Bạn sẽ thấy Swagger UI với tất cả API endpoints.

## 🎨 Bước 3: Cấu hình Frontend

### 3.1. Mở Terminal/Command mới

**Lưu ý:** Giữ Backend chạy, mở terminal mới cho Frontend

### 3.2. Navigate to Frontend folder

```bash
cd Frontend/UniBlog.Client
```

### 3.3. Kiểm tra API URL

Mở `Program.cs` và verify:

```csharp
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7100") 
});
```

URL phải khớp với Backend API URL.

### 3.4. Restore NuGet Packages

```bash
dotnet restore
```

### 3.5. Build Project

```bash
dotnet build
```

### 3.6. Chạy Frontend

```bash
dotnet run
```

Hoặc với watch mode:

```bash
dotnet watch run
```

**Kết quả:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 3.7. Mở Application

Mở browser và truy cập:
```
https://localhost:5001
```

## ✅ Bước 4: Test Application

### 4.1. Đăng ký tài khoản

1. Click **"Đăng ký"** ở góc trên bên phải
2. Điền form:
   - Username: `testuser`
   - Email: `test@example.com`
   - Full Name: `Test User`
   - Password: `Test123456`
   - Confirm Password: `Test123456`
   - Gender: `Nam`
3. Click **"Đăng ký"**
4. Sẽ redirect về trang Login

### 4.2. Đăng nhập

1. Username: `testuser`
2. Password: `Test123456`
3. Click **"Đăng nhập"**

### 4.3. Tạo bài viết đầu tiên

1. Click **"Tạo bài viết"** trên navbar
2. Điền:
   - Tiêu đề: `Bài viết đầu tiên`
   - Nội dung: `Đây là bài viết đầu tiên của tôi trên UniBlog!`
   - Image URL (optional): Để trống hoặc thêm URL hình
   - Xuất bản ngay: Checked
3. Click **"Tạo bài viết"**

### 4.4. Xem và Comment

1. Click vào bài viết vừa tạo
2. Scroll xuống phần comment
3. Viết comment và click **"Gửi bình luận"**

### 4.5. Xem Profile

1. Click **"Hồ sơ"** trên navbar
2. Xem thông tin cá nhân và danh sách bài viết

## 🐛 Troubleshooting

### Lỗi: "Unable to connect to database"

**Nguyên nhân:**
- MySQL/MariaDB chưa chạy
- Connection string sai

**Giải pháp:**
```bash
# Windows (XAMPP)
Mở XAMPP Control Panel -> Start MySQL

# macOS
brew services start mysql

# Linux
sudo systemctl start mysql
```

Verify:
```bash
mysql -u root -p
# Nếu connect được -> MySQL đang chạy
```

### Lỗi: "CORS policy"

**Nguyên nhân:**
- Frontend URL không được cấu hình trong Backend CORS

**Giải pháp:**
Trong `Backend/UniBlog.API/Program.cs`, check:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
```

### Lỗi: "401 Unauthorized"

**Nguyên nhân:**
- Token expired
- Chưa đăng nhập

**Giải pháp:**
1. Mở DevTools (F12)
2. Application tab -> Local Storage
3. Xóa `authToken`
4. Đăng nhập lại

### Lỗi: "SSL certificate problem"

**Nguyên nhân:**
- Development certificate chưa được trust

**Giải pháp:**
```bash
dotnet dev-certs https --trust
```

### Frontend không kết nối được Backend

**Check list:**
1. ✅ Backend đang chạy? (`https://localhost:7100`)
2. ✅ URL trong Frontend `Program.cs` đúng?
3. ✅ CORS được cấu hình?
4. ✅ Firewall không block?

## 📝 Tạo Admin User (Optional)

### Option 1: Qua MySQL

```sql
USE uni_blog;

INSERT INTO users (username, password_hash, email, full_name, role, created_at)
VALUES (
    'admin',
    '$2a$11$YourHashedPasswordHere',  -- Use BCrypt to hash
    'admin@uniblog.com',
    'Administrator',
    'Admin',
    NOW()
);
```

### Option 2: Đăng ký rồi update role

1. Đăng ký user bình thường qua UI
2. Update role trong database:

```sql
UPDATE users 
SET role = 'Admin' 
WHERE username = 'yourUsername';
```

## 🚀 Chạy cả 2 projects cùng lúc

### Visual Studio 2022

1. Right-click Solution
2. Properties
3. Startup Project
4. Select **"Multiple startup projects"**
5. Set both projects to **"Start"**
6. Click OK
7. Press F5

### VS Code

Tạo file `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Run Backend",
      "type": "shell",
      "command": "dotnet run --project Backend/UniBlog.API",
      "isBackground": true
    },
    {
      "label": "Run Frontend",
      "type": "shell",
      "command": "dotnet run --project Frontend/UniBlog.Client",
      "isBackground": true
    },
    {
      "label": "Run Both",
      "dependsOn": ["Run Backend", "Run Frontend"]
    }
  ]
}
```

Chạy: `Ctrl+Shift+P` -> `Tasks: Run Task` -> `Run Both`

### Terminal/Command Line

**PowerShell (Windows):**
```powershell
# Terminal 1
cd Backend\UniBlog.API
dotnet watch run

# Terminal 2  
cd Frontend\UniBlog.Client
dotnet watch run
```

**Bash (macOS/Linux):**
```bash
# Terminal 1
cd Backend/UniBlog.API
dotnet watch run

# Terminal 2
cd Frontend/UniBlog.Client
dotnet watch run
```

## 📚 Next Steps

Sau khi setup thành công:

1. 📖 Đọc [README.md](README.md) để hiểu kiến trúc
2. 🔧 Đọc [Backend/README.md](Backend/README.md) để tìm hiểu API
3. 🎨 Đọc [Frontend/README.md](Frontend/README.md) để tìm hiểu UI
4. 🚀 Bắt đầu customize và phát triển thêm tính năng!

## 💡 Tips

- Sử dụng `dotnet watch run` để tự động reload khi code thay đổi
- Mở Swagger UI để test API dễ dàng
- Sử dụng Browser DevTools để debug Frontend
- Check Console log để tìm lỗi
- Đọc error messages cẩn thận

---

**Chúc bạn thành công! 🎉**

Nếu gặp vấn đề, hãy tạo issue trên GitHub hoặc liên hệ support.


