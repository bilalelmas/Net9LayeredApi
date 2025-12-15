# Net9 Layered API

.NET 9 REST API projesi - Katmanlı mimari, Minimal API, EF Core + SQL Server, DTO yapısı, global exception handling, standart API response ve Swagger dokümantasyonu içeren modern bir e-ticaret API'si.

## 🏗️ Mimari

Proje 4 katmanlı mimari (Clean Architecture) ile geliştirilmiştir:

```
┌─────────────────────────────────────────┐
│           API Layer                     │
│  (Minimal API, Middleware, Endpoints)   │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│        Application Layer                │
│  (DTOs, Services, Business Logic)      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│         Domain Layer                    │
│  (Entities, Domain Models)              │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Infrastructure Layer               │
│  (EF Core, DbContext, Persistence)     │
└─────────────────────────────────────────┘
```

**Katmanlar:**
- **Domain**: Entity'ler ve domain modelleri
- **Application**: DTOs, Services, Business Logic, AutoMapper
- **Infrastructure**: EF Core, DbContext, Persistence
- **API**: Minimal API endpoints, Middleware, Controllers

## 🚀 Özellikler

- ✅ .NET 9 Minimal API
- ✅ Katmanlı Mimari (Clean Architecture)
- ✅ Entity Framework Core + SQL Server
- ✅ AutoMapper ile DTO mapping
- ✅ Global Exception Handling
- ✅ Standart API Response Formatı
- ✅ Swagger/OpenAPI Dokümantasyonu
- ✅ BCrypt ile Password Hashing
- ✅ CreatedAt/UpdatedAt otomatik yönetimi
- ✅ İlişkisel veritabanı yapısı

## 📋 Gereksinimler

- .NET 9 SDK
- SQL Server (Docker ile çalıştırılabilir)
- Git

## 🔧 Kurulum

### 1. Projeyi Klonlayın

```bash
git clone https://github.com/bilalelmas/Net9LayeredApi.git
cd Net9LayeredApi
```

### 2. SQL Server Kurulumu

Docker ile SQL Server container'ı oluşturun:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YOUR_SA_PASSWORD>" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

**Not:** `<YOUR_SA_PASSWORD>` yerine güçlü bir şifre girin (örn: `YourStrong@Passw0rd`). Bu şifreyi bir sonraki adımda kullanacaksınız.

**Önemli:** Eğer container zaten varsa:
```bash
docker start sqlserver
```

### 3. Connection String Yapılandırması

`src/Net9LayeredApi.API/appsettings.Development.json` dosyasını açın ve aşağıdaki connection string içindeki `<YOUR_SA_PASSWORD>` kısmını, 2. adımda Docker container'ı oluştururken kullandığınız `SA_PASSWORD` ile değiştirin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=Net9LayeredApiDb_Clean;User Id=sa;Password=<YOUR_SA_PASSWORD>;TrustServerCertificate=true;"
  }
}
```

### 4. Projeyi Çalıştırın

```bash
cd src/Net9LayeredApi.API
dotnet restore
dotnet run
```

API `http://localhost:5002` adresinde çalışacaktır.

## 📚 API Endpoints

### Users

- `GET /api/users` - Tüm kullanıcıları listele
- `GET /api/users/{id}` - Kullanıcı detayı
- `POST /api/users` - Yeni kullanıcı oluştur
- `PUT /api/users/{id}` - Kullanıcı güncelle
- `DELETE /api/users/{id}` - Kullanıcı sil

### Products

- `GET /api/products` - Tüm ürünleri listele
- `GET /api/products/{id}` - Ürün detayı
- `POST /api/products` - Yeni ürün oluştur
- `PUT /api/products/{id}` - Ürün güncelle
- `DELETE /api/products/{id}` - Ürün sil

### Reviews

- `GET /api/reviews` - Tüm yorumları listele
- `GET /api/reviews/{id}` - Yorum detayı
- `GET /api/products/{productId}/reviews` - Ürüne ait yorumlar
- `POST /api/reviews` - Yeni yorum oluştur
- `PUT /api/reviews/{id}` - Yorum güncelle
- `DELETE /api/reviews/{id}` - Yorum sil

### Orders

- `GET /api/orders` - Tüm siparişleri listele
- `GET /api/orders/{id}` - Sipariş detayı
- `GET /api/users/{userId}/orders` - Kullanıcıya ait siparişler
- `POST /api/orders` - Yeni sipariş oluştur
- `PUT /api/orders/{id}` - Sipariş güncelle (status)
- `DELETE /api/orders/{id}` - Sipariş sil

## 📖 Swagger Dokümantasyonu

Proje çalıştığında Swagger UI'a `http://localhost:5002/swagger` adresinden erişebilirsiniz.

## 🔒 Güvenlik

- Şifreler BCrypt ile hash'lenir
- Hassas bilgiler (connection strings) `appsettings.Development.json` içinde tutulur ve `.gitignore`'a eklenmiştir
- Global exception handling ile güvenli hata yönetimi

## 📝 Standart API Response Formatı

Tüm endpoint'ler standart `ApiResponse` formatını kullanır:

### Başarılı Response Örneği

**GET /api/users/{id}** (200 OK):
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla getirildi.",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "johndoe",
    "email": "john@example.com",
    "role": "User",
    "createdAt": "2024-12-14T10:30:00Z",
    "updatedAt": "2024-12-14T10:30:00Z"
  }
}
```

**POST /api/users** (201 Created):
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla oluşturuldu.",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "johndoe",
    "email": "john@example.com",
    "role": "User",
    "createdAt": "2024-12-14T10:30:00Z",
    "updatedAt": "2024-12-14T10:30:00Z"
  }
}
```

**DELETE /api/users/{id}** (204 No Content):
```
(Response body yok)
```

### Hata Response Örnekleri

**404 Not Found:**
```json
{
  "success": false,
  "message": "Kullanıcı bulunamadı.",
  "data": null
}
```

**400 Bad Request:**
```json
{
  "success": false,
  "message": "Stok miktarı negatif olamaz.",
  "data": null
}
```

**401 Unauthorized:**
```json
{
  "success": false,
  "message": "Yetkilendirme gerekli.",
  "data": null
}
```

**409 Conflict (Duplicate Email):**
```json
{
  "success": false,
  "message": "Bu email adresi zaten kullanılıyor.",
  "data": null
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "message": "Beklenmeyen bir hata oluştu.",
  "data": null
}
```

## 🛠️ Teknolojiler

- .NET 9
- Entity Framework Core 9.0
- SQL Server
- AutoMapper
- BCrypt.Net-Next
- Swashbuckle.AspNetCore (Swagger)

## 🧪 Test

### Hızlı Test

Projeyi çalıştırdıktan sonra (yeni bir terminal'de):

1. **Health Check:**
   ```bash
   curl http://localhost:5002/ping
   ```
   Beklenen: `pong`

2. **Swagger UI:**
   Tarayıcıda `http://localhost:5002/swagger` adresine gidin

3. **User Oluştur:**
   ```bash
   curl -X POST http://localhost:5002/api/users \
     -H "Content-Type: application/json" \
     -d '{"username":"testuser","email":"test@example.com","password":"Test123!","role":"User"}'
   ```

4. **User Listele:**
   ```bash
   curl http://localhost:5002/api/users
   ```

Tüm endpoint'leri test etmek için Swagger UI'ı kullanabilir veya curl/Postman gibi araçlarla test edebilirsiniz.

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.
