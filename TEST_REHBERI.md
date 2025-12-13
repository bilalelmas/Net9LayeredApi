# 🧪 Test Rehberi

Bu rehber, Net9 Layered API projesinin çalıştığını ve tüm özelliklerin doğru çalıştığını test etmek için adım adım talimatlar içerir.

## 📋 Test Adımları

### 1. Proje Build Kontrolü

```bash
cd /Users/bilalelmas/GitHub/Net9LayeredApi
dotnet build
```

**Beklenen Sonuç:** `0 Hata` mesajı görünmeli.

### 2. SQL Server Kontrolü

```bash
# Docker container'ın çalıştığını kontrol et
docker ps | grep sqlserver

# Veya container'ı başlat (eğer çalışmıyorsa)
docker start sqlserver
```

**Beklenen Sonuç:** `sqlserver` container'ı `Up` durumunda olmalı.

### 3. Connection String Kontrolü

`appsettings.Development.json` dosyasının var olduğunu ve connection string'in doğru olduğunu kontrol edin:

```bash
cat src/Net9LayeredApi.API/appsettings.Development.json
```

### 4. Projeyi Çalıştırma

```bash
cd src/Net9LayeredApi.API
dotnet run
```

**Beklenen Sonuç:** 
- `Now listening on: http://localhost:5000` mesajı görünmeli
- `Veritabanı oluşturuldu: True` mesajı görünmeli

### 5. Health Check Testi

Yeni bir terminal açın ve:

```bash
curl http://localhost:5000/ping
```

**Beklenen Sonuç:** `pong` dönmeli.

### 6. Swagger UI Testi

Tarayıcıda şu adrese gidin:
```
http://localhost:5000
```

**Beklenen Sonuç:** Swagger UI sayfası açılmalı ve tüm endpoint'ler görünmeli.

### 7. API Endpoint Testleri

#### 7.1. User CRUD Testleri

**Kullanıcı Oluştur:**
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "User"
  }'
```

**Beklenen Sonuç:** `201 Created` ve kullanıcı bilgileri (şifre hash'lenmiş olarak)

**Tüm Kullanıcıları Listele:**
```bash
curl http://localhost:5000/api/users
```

**Beklenen Sonuç:** `200 OK` ve kullanıcı listesi

**Kullanıcı Güncelle:**
```bash
# Önce oluşturulan kullanıcının ID'sini alın, sonra:
curl -X PUT http://localhost:5000/api/users/{USER_ID} \
  -H "Content-Type: application/json" \
  -d '{
    "username": "updateduser",
    "email": "updated@example.com",
    "password": "NewPass123!",
    "role": "User"
  }'
```

#### 7.2. Product CRUD Testleri

**Ürün Oluştur:**
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "{USER_ID}",
    "name": "Test Ürün",
    "description": "Bu bir test ürünüdür",
    "price": 99.99,
    "stock": 10
  }'
```

**Beklenen Sonuç:** `201 Created` ve ürün bilgileri

**Tüm Ürünleri Listele:**
```bash
curl http://localhost:5000/api/products
```

#### 7.3. Review CRUD Testleri

**Yorum Oluştur:**
```bash
curl -X POST http://localhost:5000/api/reviews \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "{USER_ID}",
    "productId": "{PRODUCT_ID}",
    "rating": 5,
    "comment": "Harika bir ürün!"
  }'
```

**Beklenen Sonuç:** `201 Created` ve yorum bilgileri

**Ürüne Ait Yorumları Listele:**
```bash
curl http://localhost:5000/api/products/{PRODUCT_ID}/reviews
```

#### 7.4. Order CRUD Testleri

**Sipariş Oluştur:**
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "{USER_ID}",
    "items": [
      {
        "productId": "{PRODUCT_ID}",
        "quantity": 2,
        "unitPrice": 99.99
      }
    ]
  }'
```

**Beklenen Sonuç:** `201 Created` ve sipariş bilgileri (totalPrice otomatik hesaplanmış)

### 8. Hata Senaryoları Testleri

#### 8.1. Duplicate Email Testi
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser2",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "User"
  }'
```

**Beklenen Sonuç:** `400 Bad Request` ve "Email veya kullanıcı adı zaten kullanılıyor." mesajı

#### 8.2. Geçersiz Rating Testi
```bash
curl -X POST http://localhost:5000/api/reviews \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "{USER_ID}",
    "productId": "{PRODUCT_ID}",
    "rating": 10,
    "comment": "Geçersiz rating"
  }'
```

**Beklenen Sonuç:** `400 Bad Request` ve "Rating 1 ile 5 arasında olmalıdır." mesajı

#### 8.3. Olmayan Kayıt Testi
```bash
curl http://localhost:5000/api/users/00000000-0000-0000-0000-000000000000
```

**Beklenen Sonuç:** `404 Not Found` ve "Kullanıcı bulunamadı." mesajı

### 9. Veritabanı Kontrolü

SQL Server'a bağlanıp tabloların oluşturulduğunu kontrol edin:

```bash
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YOUR_PASSWORD" \
  -Q "SELECT name FROM sys.tables"
```

**Beklenen Sonuç:** Users, Products, Reviews, Orders, OrderItems tabloları listelenmeli

### 10. Log Kontrolü

Proje çalışırken terminal'de şu log mesajlarını görmelisiniz:
- ✅ `Veritabanı oluşturuluyor...`
- ✅ `Veritabanı oluşturuldu: True`
- ✅ `Now listening on: http://localhost:5000`

## ✅ Başarı Kriterleri

Proje başarıyla çalışıyorsa:

1. ✅ Build hatasız tamamlanır
2. ✅ SQL Server container çalışıyor
3. ✅ `/ping` endpoint'i `pong` döner
4. ✅ Swagger UI erişilebilir
5. ✅ Tüm CRUD operasyonları çalışır
6. ✅ Hata mesajları doğru HTTP status kodları ile döner
7. ✅ Veritabanı tabloları oluşturulmuş
8. ✅ CreatedAt/UpdatedAt otomatik set ediliyor
9. ✅ Password hash'leniyor (response'da görünmemeli)
10. ✅ Standart ApiResponse formatı kullanılıyor

## 🐛 Sorun Giderme

### SQL Server bağlantı hatası
- Docker container'ın çalıştığını kontrol edin: `docker ps`
- Connection string'i kontrol edin: `appsettings.Development.json`

### Build hatası
- .NET 9 SDK'nın yüklü olduğunu kontrol edin: `dotnet --version`
- Paketleri restore edin: `dotnet restore`

### Port kullanımda hatası
- Farklı bir port kullanın veya kullanan process'i durdurun
- `launchSettings.json` dosyasında port ayarını değiştirin

## 📝 Test Senaryoları Özeti

| Test Senaryosu | Endpoint | Beklenen Sonuç |
|----------------|----------|----------------|
| Health Check | GET /ping | 200 OK, "pong" |
| User Create | POST /api/users | 201 Created |
| User List | GET /api/users | 200 OK, Array |
| User Get | GET /api/users/{id} | 200 OK, User Object |
| User Update | PUT /api/users/{id} | 200 OK |
| User Delete | DELETE /api/users/{id} | 200 OK |
| Duplicate Email | POST /api/users | 400 Bad Request |
| Invalid Rating | POST /api/reviews | 400 Bad Request |
| Not Found | GET /api/users/{invalid-id} | 404 Not Found |

