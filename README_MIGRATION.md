# Hướng dẫn cập nhật database

Để cập nhật database sau khi đã bỏ `IsPublished` và `Views` khỏi model Post, bạn cần chạy lệnh sau:

## Cách 1: Sử dụng EF Core CLI

Mở terminal trong thư mục backend và chạy:

```bash
cd backend
dotnet ef database update
```

## Cách 2: Nếu lệnh trên không chạy được

Vì có ký tự đặc biệt trong tên thư mục, bạn có thể:

1. Mở PowerShell hoặc Terminal
2. Navigate đến thư mục backend:
   ```powershell
   cd "F:\CNLTHĐ\uni_blog\backend"
   ```
3. Chạy lệnh:
   ```powershell
   dotnet ef database update
   ```

## Lưu ý

- Lệnh này sẽ xóa các cột `IsPublished` và `Views` khỏi bảng `posts` trong database
- Tất cả code đã được cập nhật để không sử dụng 2 cột này nữa
- Không có dữ liệu nào bị mất vì các cột này chưa được sử dụng trong production

