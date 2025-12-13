#!/bin/bash

# Net9 Layered API Test Script
# Bu script API'nin temel fonksiyonlarını test eder

API_URL="http://localhost:5002"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "🧪 Net9 Layered API Test Başlatılıyor..."
echo ""

# Test 1: Health Check
echo "1️⃣ Health Check Testi..."
response=$(curl -s -w "\n%{http_code}" "$API_URL/ping")
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | head -n1)

if [ "$http_code" == "200" ] && [ "$body" == "pong" ]; then
    echo -e "${GREEN}✅ Health Check başarılı: $body${NC}"
else
    echo -e "${RED}❌ Health Check başarısız: HTTP $http_code, Body: $body${NC}"
    exit 1
fi
echo ""

# Test 2: Swagger UI Kontrolü
echo "2️⃣ Swagger UI Kontrolü..."
swagger_code=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/swagger/index.html" || echo "000")
if [ "$swagger_code" == "200" ]; then
    echo -e "${GREEN}✅ Swagger UI erişilebilir${NC}"
else
    echo -e "${YELLOW}⚠️  Swagger UI kontrol edilemedi (HTTP $swagger_code)${NC}"
fi
echo ""

# Test 3: User Oluşturma
echo "3️⃣ User Oluşturma Testi..."
user_response=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser_'$(date +%s)'",
    "email": "test_'$(date +%s)'@example.com",
    "password": "Test123!",
    "role": "User"
  }')
user_http_code=$(echo "$user_response" | tail -n1)
user_body=$(echo "$user_response" | head -n1)

if [ "$user_http_code" == "201" ]; then
    echo -e "${GREEN}✅ User başarıyla oluşturuldu${NC}"
    # User ID'yi çıkar (basit JSON parse)
    USER_ID=$(echo "$user_body" | grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4)
    echo "   User ID: $USER_ID"
else
    echo -e "${RED}❌ User oluşturma başarısız: HTTP $user_http_code${NC}"
    echo "   Response: $user_body"
    exit 1
fi
echo ""

# Test 4: User Listeleme
echo "4️⃣ User Listeleme Testi..."
users_response=$(curl -s -w "\n%{http_code}" "$API_URL/api/users")
users_http_code=$(echo "$users_response" | tail -n1)

if [ "$users_http_code" == "200" ]; then
    echo -e "${GREEN}✅ User listesi başarıyla alındı${NC}"
else
    echo -e "${RED}❌ User listesi alınamadı: HTTP $users_http_code${NC}"
fi
echo ""

# Test 5: User Get (ID ile)
if [ ! -z "$USER_ID" ]; then
    echo "5️⃣ User Get (ID ile) Testi..."
    user_get_response=$(curl -s -w "\n%{http_code}" "$API_URL/api/users/$USER_ID")
    user_get_http_code=$(echo "$user_get_response" | tail -n1)
    
    if [ "$user_get_http_code" == "200" ]; then
        echo -e "${GREEN}✅ User başarıyla getirildi${NC}"
    else
        echo -e "${RED}❌ User getirilemedi: HTTP $user_get_http_code${NC}"
    fi
    echo ""
fi

# Test 6: Duplicate Email Testi
echo "6️⃣ Duplicate Email Testi..."
duplicate_response=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser_duplicate",
    "email": "test_'$(date +%s)'@example.com",
    "password": "Test123!",
    "role": "User"
  }')
# Aynı email ile tekrar dene
duplicate_response2=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser_duplicate2",
    "email": "test_'$(date +%s)'@example.com",
    "password": "Test123!",
    "role": "User"
  }')
duplicate_http_code=$(echo "$duplicate_response2" | tail -n1)

if [ "$duplicate_http_code" == "400" ]; then
    echo -e "${GREEN}✅ Duplicate email kontrolü çalışıyor${NC}"
else
    echo -e "${YELLOW}⚠️  Duplicate email kontrolü beklenen sonucu vermedi: HTTP $duplicate_http_code${NC}"
fi
echo ""

# Test 7: Product Oluşturma (User ID gerekli)
if [ ! -z "$USER_ID" ]; then
    echo "7️⃣ Product Oluşturma Testi..."
    product_response=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/products" \
      -H "Content-Type: application/json" \
      -d '{
        "userId": "'$USER_ID'",
        "name": "Test Ürün",
        "description": "Bu bir test ürünüdür",
        "price": 99.99,
        "stock": 10
      }')
    product_http_code=$(echo "$product_response" | tail -n1)
    
    if [ "$product_http_code" == "201" ]; then
        echo -e "${GREEN}✅ Product başarıyla oluşturuldu${NC}"
        PRODUCT_ID=$(echo "$product_response" | grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4)
        echo "   Product ID: $PRODUCT_ID"
    else
        echo -e "${RED}❌ Product oluşturma başarısız: HTTP $product_http_code${NC}"
        echo "   Response: $(echo "$product_response" | head -n1)"
    fi
    echo ""
fi

# Test 8: Product Listeleme
echo "8️⃣ Product Listeleme Testi..."
products_response=$(curl -s -w "\n%{http_code}" "$API_URL/api/products")
products_http_code=$(echo "$products_response" | tail -n1)

if [ "$products_http_code" == "200" ]; then
    echo -e "${GREEN}✅ Product listesi başarıyla alındı${NC}"
else
    echo -e "${RED}❌ Product listesi alınamadı: HTTP $products_http_code${NC}"
fi
echo ""

# Test 9: Invalid Rating Testi
if [ ! -z "$USER_ID" ] && [ ! -z "$PRODUCT_ID" ]; then
    echo "9️⃣ Invalid Rating Testi..."
    invalid_rating_response=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/reviews" \
      -H "Content-Type: application/json" \
      -d '{
        "userId": "'$USER_ID'",
        "productId": "'$PRODUCT_ID'",
        "rating": 10,
        "comment": "Geçersiz rating testi"
      }')
    invalid_rating_http_code=$(echo "$invalid_rating_response" | tail -n1)
    
    if [ "$invalid_rating_http_code" == "400" ]; then
        echo -e "${GREEN}✅ Invalid rating kontrolü çalışıyor${NC}"
    else
        echo -e "${YELLOW}⚠️  Invalid rating kontrolü beklenen sonucu vermedi: HTTP $invalid_rating_http_code${NC}"
    fi
    echo ""
fi

# Test 10: Not Found Testi
echo "🔟 Not Found Testi..."
notfound_response=$(curl -s -w "\n%{http_code}" "$API_URL/api/users/00000000-0000-0000-0000-000000000000")
notfound_http_code=$(echo "$notfound_response" | tail -n1)

if [ "$notfound_http_code" == "404" ]; then
    echo -e "${GREEN}✅ Not Found kontrolü çalışıyor${NC}"
else
    echo -e "${YELLOW}⚠️  Not Found kontrolü beklenen sonucu vermedi: HTTP $notfound_http_code${NC}"
fi
echo ""

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${GREEN}✅ Temel testler tamamlandı!${NC}"
echo ""
echo "📝 Detaylı test için:"
echo "   - Swagger UI: $API_URL"
echo "   - Test Rehberi: TEST_REHBERI.md"
echo ""

