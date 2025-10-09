# 🎨 UniBlog Frontend - Blazor WebAssembly

Frontend của ứng dụng UniBlog được xây dựng với Blazor WebAssembly, một framework SPA hiện đại của Microsoft.

## 📁 Cấu trúc Project

```
UniBlog.Client/
├── Models/                   # Data Models
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── RegisterRequest.cs
│   ├── UserDto.cs
│   ├── PostDto.cs
│   ├── CreatePostDto.cs
│   ├── CommentDto.cs
│   └── CreateCommentDto.cs
│
├── Pages/                    # Razor Pages/Components
│   ├── Index.razor          # Trang chủ
│   ├── Login.razor          # Đăng nhập
│   ├── Register.razor       # Đăng ký
│   ├── PostDetail.razor     # Chi tiết bài viết
│   ├── CreatePost.razor     # Tạo bài viết
│   └── Profile.razor        # Hồ sơ người dùng
│
├── Services/                 # API Services
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IPostService.cs
│   │   ├── ICommentService.cs
│   │   └── IUserService.cs
│   ├── AuthService.cs
│   ├── PostService.cs
│   ├── CommentService.cs
│   ├── UserService.cs
│   └── CustomAuthStateProvider.cs
│
├── Shared/                   # Shared Components
│   ├── MainLayout.razor     # Layout chính
│   ├── NavMenu.razor        # Navigation menu
│   └── RedirectToLogin.razor
│
├── wwwroot/                  # Static Files
│   ├── css/
│   │   └── app.css          # Custom styles
│   └── index.html           # Entry HTML
│
├── _Imports.razor            # Global imports
├── App.razor                 # Root component
├── Program.cs                # Application entry point
└── UniBlog.Client.csproj     # Project file
```

## 🚀 Cài đặt và Chạy

### 1. Prerequisites
- .NET 8.0 SDK
- Backend API đang chạy tại `https://localhost:7100`

### 2. Cấu hình API Endpoint

Trong `Program.cs`, cập nhật URL của Backend API:

```csharp
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7100") 
});
```

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Chạy ứng dụng

```bash
dotnet run
```

Hoặc với watch mode (tự động reload khi code thay đổi):

```bash
dotnet watch run
```

Ứng dụng sẽ chạy tại:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## 🎨 Tính năng

### Trang công khai (Không cần đăng nhập)
- ✅ Trang chủ - Xem danh sách bài viết
- ✅ Đăng nhập
- ✅ Đăng ký tài khoản

### Trang yêu cầu đăng nhập
- ✅ Chi tiết bài viết & Bình luận
- ✅ Tạo bài viết mới
- ✅ Chỉnh sửa/Xóa bài viết của mình
- ✅ Xóa comment của mình
- ✅ Xem và chỉnh sửa profile

## 🔐 Authentication

### Local Storage

JWT Token được lưu trong browser's LocalStorage với key `authToken`.

### Protected Routes

Các trang yêu cầu authentication sử dụng attribute:
```csharp
@attribute [Authorize]
```

Người dùng chưa đăng nhập sẽ tự động redirect về trang `/login`.

### Custom Auth State Provider

`CustomAuthStateProvider.cs` quản lý:
- Parse JWT token
- Extract claims (UserId, Username, Role)
- Set HTTP Authorization header
- Notify authentication state changes

## 📦 NuGet Packages

- `Microsoft.AspNetCore.Components.WebAssembly` - Blazor WASM framework
- `Microsoft.AspNetCore.Components.WebAssembly.DevServer` - Development server
- `Microsoft.Extensions.Http` - HTTP client factory
- `Blazored.LocalStorage` - Local storage management
- `Microsoft.AspNetCore.Components.Authorization` - Authorization support

## 🎨 Styling

### CSS Architecture

File `wwwroot/css/app.css` sử dụng:
- CSS Variables cho theming
- Modern CSS với Flexbox/Grid
- Responsive design
- Component-based styling
- Smooth transitions & animations

### CSS Variables

```css
:root {
    --primary-color: #4f46e5;
    --secondary-color: #06b6d4;
    --danger-color: #ef4444;
    --success-color: #10b981;
    --text-primary: #1f2937;
    --bg-primary: #ffffff;
    /* ... */
}
```

### Responsive Design

Tất cả components đều responsive và hoạt động tốt trên:
- 📱 Mobile (320px+)
- 📱 Tablet (768px+)
- 💻 Desktop (1024px+)

## 🔧 Services

### AuthService

```csharp
Task<bool> LoginAsync(LoginRequest request);
Task<bool> RegisterAsync(RegisterRequest request);
Task LogoutAsync();
Task<UserDto?> GetCurrentUserAsync();
```

### PostService

```csharp
Task<List<PostDto>> GetPostsAsync(bool publishedOnly = true);
Task<PostDto?> GetPostByIdAsync(int id);
Task<PostDto?> CreatePostAsync(CreatePostDto createPostDto);
Task<PostDto?> UpdatePostAsync(int id, CreatePostDto updatePostDto);
Task<bool> DeletePostAsync(int id);
```

### CommentService

```csharp
Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId);
Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto);
Task<bool> DeleteCommentAsync(int id);
```

## 📱 Components

### Pages

#### Index.razor
- Hiển thị danh sách bài viết
- Pagination (có thể mở rộng)
- Click vào card để xem chi tiết

#### Login.razor
- Form đăng nhập với validation
- Error handling
- Auto redirect sau khi login thành công

#### Register.razor
- Form đăng ký với validation
- Password confirmation
- Success message và redirect

#### PostDetail.razor
- Hiển thị full content của bài viết
- View counter tự động tăng
- Comment section
- Form thêm comment mới

#### CreatePost.razor
- Form tạo bài viết mới
- Rich text editor (có thể mở rộng)
- Image URL support
- Publish/Draft option

#### Profile.razor
- Thông tin user
- Danh sách bài viết của user
- Statistics

### Shared Components

#### NavMenu.razor
- Responsive navigation
- Dynamic menu based on auth state
- User dropdown (có thể mở rộng)

#### MainLayout.razor
- Layout wrapper cho tất cả pages
- Navbar + Content area
- Footer (có thể thêm)

## 🛠️ Development Tips

### Hot Reload

Sử dụng `dotnet watch run` để tự động reload khi code thay đổi.

### Browser DevTools

- F12 để mở DevTools
- Check Network tab để debug API calls
- Check Console cho errors
- Application tab để xem LocalStorage

### Common Issues

#### CORS Error
- Đảm bảo Backend API đã cấu hình CORS đúng
- Check Frontend URL trong Backend CORS policy

#### 401 Unauthorized
- Token có thể đã expired
- Clear LocalStorage và login lại
- Check token expiration time

#### API Connection Failed
- Đảm bảo Backend đã chạy
- Check `BaseAddress` trong Program.cs
- Verify HTTPS certificate

## 🚀 Build for Production

### Publish

```bash
dotnet publish -c Release -o ./publish
```

### Optimization

Blazor WASM tự động:
- Minify JavaScript
- Compress assemblies (using Brotli)
- Remove unused code (trimming)
- Optimize for size

### Deploy

Files trong `publish/wwwroot` có thể deploy lên:
- GitHub Pages
- Netlify
- Vercel
- Azure Static Web Apps
- AWS S3 + CloudFront
- Bất kỳ static file hosting nào

### Environment Configuration

Tạo `wwwroot/appsettings.json` cho production:

```json
{
  "ApiBaseUrl": "https://your-production-api.com"
}
```

Và load trong Program.cs:

```csharp
var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var config = await http.GetFromJsonAsync<Dictionary<string, string>>("appsettings.json");
var apiUrl = config["ApiBaseUrl"];

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
```

## 🎯 Future Enhancements

### Tính năng có thể thêm
- 📝 Rich text editor (TinyMCE, Quill)
- 🖼️ Image upload
- 🔍 Search & filter posts
- 📄 Pagination
- 👍 Like/Reaction system
- 🔔 Notifications
- 💬 Real-time comments (SignalR)
- 🌙 Dark mode
- 🌍 Internationalization (i18n)
- 📱 Progressive Web App (PWA)

### Performance Optimization
- Virtual scrolling cho long lists
- Lazy loading cho images
- Component caching
- Service Worker
- CDN cho static assets

---

**Happy Coding! 🚀**

