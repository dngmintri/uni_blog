# ===================================================================
# UniBlog - User Secrets Setup Script (PowerShell)
# ===================================================================
# Script này giúp bạn setup User Secrets một cách dễ dàng

Write-Host "`n🔐 UniBlog - User Secrets Setup`n" -ForegroundColor Cyan

$projectPath = "UniBlog.API"

# Change to project directory
Set-Location $projectPath

# Check if User Secrets is initialized
Write-Host "📋 Checking User Secrets status..." -ForegroundColor Yellow
$secretsList = dotnet user-secrets list 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ User Secrets chưa được khởi tạo!" -ForegroundColor Red
    Write-Host "   Đang khởi tạo..." -ForegroundColor Yellow
    dotnet user-secrets init
    Write-Host "✅ Đã khởi tạo User Secrets!" -ForegroundColor Green
}
else {
    Write-Host "✅ User Secrets đã được khởi tạo!" -ForegroundColor Green
}

Write-Host "`n📝 Current Secrets:" -ForegroundColor Cyan
dotnet user-secrets list

# Menu
Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "   Bạn muốn làm gì?" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "   [1] Set Database Connection String" -ForegroundColor White
Write-Host "   [2] Set JWT Secret Key" -ForegroundColor White
Write-Host "   [3] Generate new JWT Secret Key" -ForegroundColor White
Write-Host "   [4] List all secrets" -ForegroundColor White
Write-Host "   [5] Clear all secrets" -ForegroundColor White
Write-Host "   [6] Exit" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Gray

$choice = Read-Host "Chọn (1-6)"

switch ($choice) {
    "1" {
        Write-Host "`n📊 Setup Database Connection String" -ForegroundColor Cyan
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
        
        $server = Read-Host "Database Server [localhost]"
        if ([string]::IsNullOrWhiteSpace($server)) { $server = "localhost" }
        
        $port = Read-Host "Port [3306]"
        if ([string]::IsNullOrWhiteSpace($port)) { $port = "3306" }
        
        $database = Read-Host "Database Name [uni_blog]"
        if ([string]::IsNullOrWhiteSpace($database)) { $database = "uni_blog" }
        
        $user = Read-Host "Username [root]"
        if ([string]::IsNullOrWhiteSpace($user)) { $user = "root" }
        
        $password = Read-Host "Password" -AsSecureString
        $passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
            [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
        )
        
        $connectionString = "Server=$server;Port=$port;Database=$database;User=$user;Password=$passwordPlain"
        
        dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "`n✅ Connection String đã được lưu!" -ForegroundColor Green
        }
        else {
            Write-Host "`n❌ Có lỗi xảy ra!" -ForegroundColor Red
        }
    }
    
    "2" {
        Write-Host "`n🔑 Setup JWT Secret Key" -ForegroundColor Cyan
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
        Write-Host "⚠️  Secret Key phải có ít nhất 32 ký tự!" -ForegroundColor Yellow
        
        $secretKey = Read-Host "Nhập JWT Secret Key" -AsSecureString
        $secretKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
            [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secretKey)
        )
        
        if ($secretKeyPlain.Length -lt 32) {
            Write-Host "`n❌ Secret Key phải có ít nhất 32 ký tự!" -ForegroundColor Red
            Write-Host "   Sử dụng option [3] để generate tự động." -ForegroundColor Yellow
        }
        else {
            dotnet user-secrets set "JwtSettings:SecretKey" $secretKeyPlain
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "`n✅ JWT Secret Key đã được lưu!" -ForegroundColor Green
            }
            else {
                Write-Host "`n❌ Có lỗi xảy ra!" -ForegroundColor Red
            }
        }
    }
    
    "3" {
        Write-Host "`n🎲 Generating random JWT Secret Key..." -ForegroundColor Cyan
        
        # Generate 256-bit random key
        $bytes = New-Object byte[] 32
        [System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
        $generatedKey = [Convert]::ToBase64String($bytes)
        
        Write-Host "`nGenerated Key:" -ForegroundColor Yellow
        Write-Host $generatedKey -ForegroundColor Green
        
        $confirm = Read-Host "`nSave this key to User Secrets? (y/n)"
        
        if ($confirm -eq "y" -or $confirm -eq "Y") {
            dotnet user-secrets set "JwtSettings:SecretKey" $generatedKey
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "`n✅ JWT Secret Key đã được lưu!" -ForegroundColor Green
            }
            else {
                Write-Host "`n❌ Có lỗi xảy ra!" -ForegroundColor Red
            }
        }
        else {
            Write-Host "`n⚠️  Key không được lưu. Bạn có thể copy và dùng sau." -ForegroundColor Yellow
        }
    }
    
    "4" {
        Write-Host "`n📋 All Secrets:" -ForegroundColor Cyan
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
        dotnet user-secrets list
    }
    
    "5" {
        Write-Host "`n⚠️  WARNING: Bạn sắp xóa TẤT CẢ secrets!" -ForegroundColor Red
        $confirm = Read-Host "Bạn có chắc chắn? (yes/no)"
        
        if ($confirm -eq "yes") {
            dotnet user-secrets clear
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "`n✅ Đã xóa tất cả secrets!" -ForegroundColor Green
            }
            else {
                Write-Host "`n❌ Có lỗi xảy ra!" -ForegroundColor Red
            }
        }
        else {
            Write-Host "`n✅ Đã hủy thao tác." -ForegroundColor Green
        }
    }
    
    "6" {
        Write-Host "`n👋 Bye!" -ForegroundColor Cyan
        exit
    }
    
    default {
        Write-Host "`n❌ Lựa chọn không hợp lệ!" -ForegroundColor Red
    }
}

Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "💡 Tip: Chạy 'dotnet user-secrets list' để xem secrets" -ForegroundColor Yellow
Write-Host "📖 Đọc file SECRETS_SETUP.md để biết thêm chi tiết" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Gray

# Return to original directory
Set-Location ..

