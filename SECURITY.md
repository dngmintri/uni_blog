# 🔒 Security Guidelines - UniBlog Project

## ✅ ĐÃ HOÀN THÀNH

### 1. **User Secrets Setup** ✅
- User Secrets đã được khởi tạo
- Connection String đã được lưu an toàn
- JWT Secret Key đã được lưu an toàn
- `appsettings.json` đã được clean (không còn sensitive data)

### 2. **Git Security** ✅
- `.gitignore` đã được cập nhật
- Sensitive files sẽ KHÔNG được commit
- Environment files (.env) đã được ignore

---

## 🎯 CÁCH SỬ DỤNG

### **Development (Bạn đang dùng):**

Secrets đã được lưu vào **User Secrets** - an toàn và không commit vào Git!

**Xem secrets hiện tại:**
```bash
cd Backend/UniBlog.API
dotnet user-secrets list
```

**Kết quả:**
```
JwtSettings:SecretKey = YourSuperSecretKeyForJWTTokenGeneration12345678
ConnectionStrings:DefaultConnection = Server=localhost;Port=3306;Database=uni_blog;User=root;Password=;
```

**Chỉnh sửa password database của bạn:**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=uni_blog;User=root;Password=YOUR_REAL_PASSWORD"
```

### **Chạy Backend:**
```bash
cd Backend/UniBlog.API
dotnet run --launch-profile https
```

Backend sẽ tự động load secrets từ User Secrets - KHÔNG CẦN làm gì thêm!

---

## 📋 CHO PRODUCTION

Xem file `Backend/SECRETS_SETUP.md` để biết cách setup cho:
- Docker
- Azure
- AWS
- Linux servers
- Environment Variables

---

## 🚨 LƯU Ý BẢO MẬT

### **KHÔNG BAO GIỜ:**
- ❌ Commit secrets vào Git
- ❌ Hardcode passwords trong code
- ❌ Share secrets qua email/chat
- ❌ Screenshot secrets và post lên mạng
- ❌ Dùng password yếu cho production

### **NÊN LÀM:**
- ✅ Dùng User Secrets cho local development
- ✅ Dùng Environment Variables cho production
- ✅ Generate strong JWT keys (>= 32 chars)
- ✅ Rotate secrets định kỳ
- ✅ Review code trước khi commit

---

## 🔑 GENERATE SECURE JWT KEY

### PowerShell (Windows):
```powershell
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
$key = [Convert]::ToBase64String($bytes)
echo $key

# Set vào User Secrets
dotnet user-secrets set "JwtSettings:SecretKey" "$key"
```

### Bash (Linux/macOS):
```bash
# Generate key
key=$(openssl rand -base64 32)
echo $key

# Set vào User Secrets
dotnet user-secrets set "JwtSettings:SecretKey" "$key"
```

---

## 📂 FILE STRUCTURE

```
uni_blog/
├── Backend/
│   ├── UniBlog.API/
│   │   ├── appsettings.json          ← KHÔNG chứa secrets
│   │   └── UniBlog.API.csproj        ← Có UserSecretsId
│   ├── SECRETS_SETUP.md              ← Hướng dẫn chi tiết
│   └── env.example.txt               ← Template cho .env
├── .gitignore                        ← Đã ignore sensitive files
└── SECURITY.md                       ← File này
```

---

## ✅ CHECKLIST TRƯỚC KHI DEPLOY

- [ ] Database password đã đổi khỏi default
- [ ] JWT SecretKey >= 32 characters, random
- [ ] CORS origins chỉ allow domain thật
- [ ] HTTPS enabled
- [ ] appsettings.json KHÔNG chứa secrets
- [ ] Environment variables đã set đúng
- [ ] Logs không in ra sensitive data
- [ ] API rate limiting enabled (TODO)
- [ ] Account lockout enabled (TODO)

---

## 🆘 TROUBLESHOOTING

### "Configuration binding error - JwtSettings:SecretKey is null"

**Nguyên nhân:** User Secrets chưa được set

**Giải pháp:**
```bash
cd Backend/UniBlog.API
dotnet user-secrets set "JwtSettings:SecretKey" "YourSecretKey32CharsMinimum!"
```

### "Unable to connect to database"

**Nguyên nhân:** Connection string chưa đúng

**Giải pháp:**
```bash
# Check current value
dotnet user-secrets list

# Update với password đúng
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=uni_blog;User=root;Password=YOUR_PASSWORD"
```

---

## 📞 LIÊN HỆ

Nếu phát hiện security issue, ĐỪNG tạo public issue trên GitHub.
Hãy liên hệ trực tiếp với maintainer.

---

**Cập nhật lần cuối:** 2025-10-09
**Status:** ✅ Secured with User Secrets


