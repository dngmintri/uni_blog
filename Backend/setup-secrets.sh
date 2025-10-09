#!/bin/bash

# ===================================================================
# UniBlog - User Secrets Setup Script (Bash)
# ===================================================================
# Script này giúp bạn setup User Secrets một cách dễ dàng

echo ""
echo "🔐 UniBlog - User Secrets Setup"
echo ""

PROJECT_PATH="UniBlog.API"

# Change to project directory
cd "$PROJECT_PATH" || exit

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Check if User Secrets is initialized
echo -e "${YELLOW}📋 Checking User Secrets status...${NC}"
if ! dotnet user-secrets list &> /dev/null; then
    echo -e "${RED}❌ User Secrets chưa được khởi tạo!${NC}"
    echo -e "${YELLOW}   Đang khởi tạo...${NC}"
    dotnet user-secrets init
    echo -e "${GREEN}✅ Đã khởi tạo User Secrets!${NC}"
else
    echo -e "${GREEN}✅ User Secrets đã được khởi tạo!${NC}"
fi

echo ""
echo -e "${CYAN}📝 Current Secrets:${NC}"
dotnet user-secrets list

# Menu
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "   Bạn muốn làm gì?"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "   [1] Set Database Connection String"
echo "   [2] Set JWT Secret Key"
echo "   [3] Generate new JWT Secret Key"
echo "   [4] List all secrets"
echo "   [5] Clear all secrets"
echo "   [6] Exit"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
read -p "Chọn (1-6): " choice

case $choice in
    1)
        echo ""
        echo -e "${CYAN}📊 Setup Database Connection String${NC}"
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        
        read -p "Database Server [localhost]: " server
        server=${server:-localhost}
        
        read -p "Port [3306]: " port
        port=${port:-3306}
        
        read -p "Database Name [uni_blog]: " database
        database=${database:-uni_blog}
        
        read -p "Username [root]: " user
        user=${user:-root}
        
        read -sp "Password: " password
        echo ""
        
        connection_string="Server=$server;Port=$port;Database=$database;User=$user;Password=$password"
        
        dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$connection_string"
        
        if [ $? -eq 0 ]; then
            echo -e "\n${GREEN}✅ Connection String đã được lưu!${NC}"
        else
            echo -e "\n${RED}❌ Có lỗi xảy ra!${NC}"
        fi
        ;;
    
    2)
        echo ""
        echo -e "${CYAN}🔑 Setup JWT Secret Key${NC}"
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        echo -e "${YELLOW}⚠️  Secret Key phải có ít nhất 32 ký tự!${NC}"
        
        read -sp "Nhập JWT Secret Key: " secret_key
        echo ""
        
        if [ ${#secret_key} -lt 32 ]; then
            echo -e "\n${RED}❌ Secret Key phải có ít nhất 32 ký tự!${NC}"
            echo -e "${YELLOW}   Sử dụng option [3] để generate tự động.${NC}"
        else
            dotnet user-secrets set "JwtSettings:SecretKey" "$secret_key"
            
            if [ $? -eq 0 ]; then
                echo -e "\n${GREEN}✅ JWT Secret Key đã được lưu!${NC}"
            else
                echo -e "\n${RED}❌ Có lỗi xảy ra!${NC}"
            fi
        fi
        ;;
    
    3)
        echo ""
        echo -e "${CYAN}🎲 Generating random JWT Secret Key...${NC}"
        
        # Generate 256-bit random key using openssl
        generated_key=$(openssl rand -base64 32)
        
        echo ""
        echo -e "${YELLOW}Generated Key:${NC}"
        echo -e "${GREEN}$generated_key${NC}"
        
        read -p "$(echo -e "\nSave this key to User Secrets? (y/n): ")" confirm
        
        if [ "$confirm" = "y" ] || [ "$confirm" = "Y" ]; then
            dotnet user-secrets set "JwtSettings:SecretKey" "$generated_key"
            
            if [ $? -eq 0 ]; then
                echo -e "\n${GREEN}✅ JWT Secret Key đã được lưu!${NC}"
            else
                echo -e "\n${RED}❌ Có lỗi xảy ra!${NC}"
            fi
        else
            echo -e "\n${YELLOW}⚠️  Key không được lưu. Bạn có thể copy và dùng sau.${NC}"
        fi
        ;;
    
    4)
        echo ""
        echo -e "${CYAN}📋 All Secrets:${NC}"
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        dotnet user-secrets list
        ;;
    
    5)
        echo ""
        echo -e "${RED}⚠️  WARNING: Bạn sắp xóa TẤT CẢ secrets!${NC}"
        read -p "Bạn có chắc chắn? (yes/no): " confirm
        
        if [ "$confirm" = "yes" ]; then
            dotnet user-secrets clear
            
            if [ $? -eq 0 ]; then
                echo -e "\n${GREEN}✅ Đã xóa tất cả secrets!${NC}"
            else
                echo -e "\n${RED}❌ Có lỗi xảy ra!${NC}"
            fi
        else
            echo -e "\n${GREEN}✅ Đã hủy thao tác.${NC}"
        fi
        ;;
    
    6)
        echo ""
        echo -e "${CYAN}👋 Bye!${NC}"
        cd .. || exit
        exit 0
        ;;
    
    *)
        echo ""
        echo -e "${RED}❌ Lựa chọn không hợp lệ!${NC}"
        ;;
esac

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${YELLOW}💡 Tip: Chạy 'dotnet user-secrets list' để xem secrets${NC}"
echo -e "${YELLOW}📖 Đọc file SECRETS_SETUP.md để biết thêm chi tiết${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Return to original directory
cd .. || exit

