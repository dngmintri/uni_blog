# 🔐 Hướng dẫn Setup Secrets & Environment Variables

## 📋 Tổng quan

Project UniBlog sử dụng 2 phương pháp để quản lý thông tin nhạy cảm:
- **User Secrets** - Cho môi trường Development (local machine)
- **Environment Variables** - Cho môi trường Production/Staging

---

## 🛠️ 1. DEVELOPMENT (User Secrets)

### **User Secrets đã được khởi tạo!** ✅

UserSecretsId: `7f489797-d6d3-4e9f-bac4-8494428e5629`

### **Xem secrets hiện tại:**
```bash
cd Backend/UniBlog.API
dotnet user-secrets list
```

### **Thêm/Cập nhật secrets:**

#### Database Connection String
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=uni_blog;User=root;Password=YOUR_PASSWORD"
```

#### JWT Secret Key
```bash
dotnet user-secrets set "JwtSettings:SecretKey" "YourSuperSecretKey_MinLength32Chars"
```

### **Xóa một secret:**
```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

### **Xóa tất cả secrets:**
```bash
dotnet user-secrets clear
```

### **Vị trí lưu trữ:**
- **Windows:** `%APPDATA%\Microsoft\UserSecrets\7f489797-d6d3-4e9f-bac4-8494428e5629\secrets.json`
- **macOS/Linux:** `~/.microsoft/usersecrets/7f489797-d6d3-4e9f-bac4-8494428e5629/secrets.json`

---

## 🚀 2. PRODUCTION (Environment Variables)

### **Cách 1: Set trực tiếp trong OS**

#### Windows (PowerShell)
```powershell
# Permanent (System level)
[System.Environment]::SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Server=prod-server;Database=uni_blog;...", "Machine")
[System.Environment]::SetEnvironmentVariable("JwtSettings__SecretKey", "ProductionSecretKey123...", "Machine")

# Temporary (Current session)
$env:ConnectionStrings__DefaultConnection = "Server=prod-server;..."
$env:JwtSettings__SecretKey = "ProductionSecretKey123..."
```

#### Linux/macOS
```bash
# Thêm vào ~/.bashrc hoặc ~/.zshrc
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=uni_blog;..."
export JwtSettings__SecretKey="ProductionSecretKey123..."

# Reload
source ~/.bashrc
```

### **Cách 2: Docker Environment Variables**

Trong `docker-compose.yml`:
```yaml
version: '3.8'
services:
  api:
    image: uniblog-api
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=uni_blog;User=root;Password=${DB_PASSWORD}
      - JwtSettings__SecretKey=${JWT_SECRET}
    env_file:
      - .env
```

Trong `.env`:
```env
DB_PASSWORD=secure_password
JWT_SECRET=super_secret_key_32_chars_min
```

### **Cách 3: Azure App Service**

```bash
# Azure CLI
az webapp config appsettings set \
  --resource-group myResourceGroup \
  --name myAppName \
  --settings \
    ConnectionStrings__DefaultConnection="Server=azure-db..." \
    JwtSettings__SecretKey="AzureSecretKey..."
```

### **Cách 4: AWS Elastic Beanstalk**

Trong `.ebextensions/environment.config`:
```yaml
option_settings:
  - namespace: aws:elasticbeanstalk:application:environment
    option_name: ConnectionStrings__DefaultConnection
    value: "Server=rds-instance..."
  - namespace: aws:elasticbeanstalk:application:environment
    option_name: JwtSettings__SecretKey
    value: "AwsSecretKey..."
```

---

## ⚙️ 3. CÁCH HOẠT ĐỘNG

### **Configuration Priority (High → Low):**
```
1. Command-line arguments
2. Environment Variables
3. User Secrets (Development only)
4. appsettings.{Environment}.json
5. appsettings.json
```

### **Naming Convention:**
```
JSON Path:              Environment Variable:
ConnectionStrings       ConnectionStrings__DefaultConnection
└─DefaultConnection     (double underscore __)

JwtSettings             JwtSettings__SecretKey
└─SecretKey             JwtSettings__Issuer
└─Issuer                JwtSettings__Audience
└─Audience
```

---

## 🔑 4. SINH JWT SECRET KEY AN TOÀN

### **PowerShell:**
```powershell
# Generate random 256-bit key
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
[Convert]::ToBase64String($bytes)
```

### **Linux/macOS:**
```bash
# Generate random 256-bit key
openssl rand -base64 32
```

### **C#:**
```csharp
using System.Security.Cryptography;
var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
Console.WriteLine(key);
```

### **Online (KHÔNG khuyến nghị cho production):**
- https://generate-random.org/api-key-generator
- https://randomkeygen.com/

---

## ✅ 5. CHECKLIST BẢO MẬT

### **Development:**
- [x] User Secrets đã được init
- [x] Secrets đã được set (ConnectionString, JWT)
- [x] appsettings.json KHÔNG chứa sensitive data
- [x] .gitignore đã ignore appsettings.Development.json

### **Production:**
- [ ] Environment Variables đã được set
- [ ] JWT SecretKey đủ mạnh (>= 32 chars, random)
- [ ] Database password phức tạp
- [ ] HTTPS được enable
- [ ] CORS origins được giới hạn
- [ ] Secrets KHÔNG được commit vào Git

---

## 🚨 6. TROUBLESHOOTING

### **Lỗi: "JwtSettings:SecretKey is null"**
```bash
# Kiểm tra User Secrets
dotnet user-secrets list

# Nếu rỗng, set lại
dotnet user-secrets set "JwtSettings:SecretKey" "YourSecretKey..."
```

### **Lỗi: "Unable to connect to database"**
```bash
# Kiểm tra ConnectionString
dotnet user-secrets list

# Test connection
mysql -h localhost -P 3306 -u root -p uni_blog
```

### **Secrets không load**
```csharp
// Trong Program.cs, kiểm tra:
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
```

---

## 📚 7. TÀI LIỆU THAM KHẢO

- [Safe storage of app secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Environment Variables in .NET](https://learn.microsoft.com/en-us/dotnet/api/system.environment.getenvironmentvariable)

---

**Lưu ý quan trọng:**
- ⚠️ KHÔNG BAO GIỜ commit file `secrets.json` vào Git
- ⚠️ KHÔNG BAO GIỜ hardcode sensitive data trong code
- ⚠️ KHÔNG BAO GIỜ share secrets qua email/chat
- ✅ SỬ DỤNG password manager cho team (1Password, LastPass)
- ✅ ROTATE secrets định kỳ (mỗi 90 ngày)

