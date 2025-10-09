# 🔐 Setup Môi Trường ĐƠN GIẢN với .env

## 📝 File đã tạo sẵn

File `.env.development` đã có sẵn trong `Backend/UniBlog.API/`

## ⚙️ Cách sử dụng

### 1. Mở file `.env.development`

```bash
cd Backend/UniBlog.API
notepad .env.development    # Windows
nano .env.development       # Linux/macOS
```

### 2. Chỉnh sửa password database

```env
# Database Configuration
DB_SERVER=localhost
DB_PORT=3306
DB_NAME=uni_blog
DB_USER=root
DB_PASSWORD=YOUR_MYSQL_PASSWORD_HERE    # ← Sửa dòng này

# JWT Configuration  
JWT_SECRET_KEY=YourSuperSecretKeyForJWTTokenGeneration12345678
JWT_ISSUER=UniBlogAPI
JWT_AUDIENCE=UniBlogClient
JWT_EXPIRATION_MINUTES=60
```

### 3. Chạy Backend

```bash
cd Backend/UniBlog.API
dotnet run --launch-profile https
```

**XO NG!** Backend sẽ tự động đọc file `.env.development` ✅

---

## 🔑 Sinh JWT Secret Key mới (Optional)

### PowerShell:
```powershell
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
[Convert]::ToBase64String($bytes)
```

### Bash:
```bash
openssl rand -base64 32
```

Copy kết quả và thay vào `JWT_SECRET_KEY` trong file `.env.development`

---

## ⚠️ Lưu ý

- ✅ File `.env.development` đã được git ignore - an toàn!
- ✅ KHÔNG commit file này lên Git
- ✅ Chỉ cần sửa password một lần duy nhất
- ✅ Không cần User Secrets, không cần phức tạp!

---

## 🚀 Production

Cho production, tạo file `.env.production` với format giống hệt.

---

**Đơn giản vậy thôi!** 😊

