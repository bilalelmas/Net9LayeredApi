# 🧪 Test Rehberi

Bu rehber, Net9 Layered API projesini adım adım test etmeniz için hazırlanmıştır.

💡 **Not:** Tüm `curl` komutlarına `| jq` eklenmiştir. Bu sayede JSON çıktıları daha okunabilir (alt alta formatlanmış) görünecektir. Eğer `jq` yüklü değilse, komutlardan `| jq` kısmını kaldırabilirsiniz (JSON tek satırda görünecek ama yine de çalışacaktır).

## 📋 Ön Hazırlık

### 1. SQL Server Kontrolü

```bash
docker ps --filter "name=sqlserver"
```

Eğer container çalışmıyorsa:
```bash
docker start sqlserver
```

### 2. Projeyi Çalıştırın

**Yeni bir terminal açın:**
```bash
cd /Users/bilalelmas/GitHub/Net9LayeredApi/src/Net9LayeredApi.API
dotnet run
```

✅ API çalıştığında şu mesajı göreceksiniz:
```
Now listening on: http://localhost:5002
```

⚠️ **Önemli:** API çalışırken bu terminali açık tutun!

---

## 🧪 Test Adımları

### Test 1: Health Check ✅

```bash
curl http://localhost:5002/ping
```

**Beklenen Sonuç:**
```
pong
```

---

### Test 2: Swagger UI Kontrolü ✅

Tarayıcıda şu adrese gidin:
```
http://localhost:5002/swagger
```

**Beklenen:** Swagger UI açılmalı ve tüm endpoint'ler görünmeli

---

### Test 3: User Oluşturma (201 Created) ✅

```bash
curl -X POST http://localhost:5002/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "User"
  }' | jq
```

**Beklenen Response:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla oluşturuldu.",
  "data": {
    "id": "...",
    "username": "testuser",
    "email": "test@example.com",
    "role": "User",
    "createdAt": "...",
    "updatedAt": "..."
  }
}
```

**Status Code:** `201 Created`

---

### Test 4: User Listeleme (200 OK) ✅

```bash
curl http://localhost:5002/api/users | jq
```

**Beklenen Response:**
```json
{
  "success": true,
  "message": "Kullanıcılar başarıyla getirildi.",
  "data": [
    {
      "id": "...",
      "username": "testuser",
      "email": "test@example.com",
      "role": "User",
      "createdAt": "...",
      "updatedAt": "..."
    }
  ]
}
```

**Status Code:** `200 OK`

---

### Test 5: Duplicate Email (409 Conflict) ✅

Aynı email ile tekrar user oluştur:

```bash
curl -X POST http://localhost:5002/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser2",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "User"
  }' | jq
```

**Beklenen Response:**
```json
{
  "success": false,
  "message": "Bu email adresi zaten kullanılıyor.",
  "data": null
}
```

**Status Code:** `409 Conflict`

---

### Test 6: 404 Not Found ✅

Var olmayan bir user ID ile sorgu:

```bash
curl http://localhost:5002/api/users/00000000-0000-0000-0000-000000000000 | jq
```

**Beklenen Response:**
```json
{
  "success": false,
  "message": "Kullanıcı bulunamadı.",
  "data": null
}
```

**Status Code:** `404 Not Found`

---

### Test 7: 401 Unauthorized ✅

```bash
curl http://localhost:5002/api/auth/check
```

**Beklenen:** `401 Unauthorized`

---

### Test 8: Product Oluşturma ✅

Önce bir user ID alın (Test 4'ten), sonra:

```bash
curl -X POST http://localhost:5002/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "<USER_ID_BURAYA>",
    "name": "Test Ürün",
    "description": "Test açıklama",
    "price": 99.99,
    "stock": 10
  }' | jq
```

**Beklenen:** `201 Created` ve product bilgileri

---

### Test 9: DELETE (204 No Content) ✅

Bir user ID ile delete işlemi:

```bash
curl -X DELETE http://localhost:5002/api/users/<USER_ID>
```

**Beklenen:** 
- **Status Code:** `204 No Content`
- **Response Body:** Yok (boş)

---

### Test 10: 400 Bad Request ✅

Geçersiz veri ile product oluştur:

```bash
curl -X POST http://localhost:5002/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "<USER_ID>",
    "name": "Test Ürün",
    "description": "Test açıklama",
    "price": -10,
    "stock": 10
  }' | jq
```

**Beklenen:** `400 Bad Request` ve hata mesajı

---

## ✅ Test Kontrol Listesi

- [ ] Health check çalışıyor (pong)
- [ ] Swagger UI açılıyor
- [ ] User oluşturma çalışıyor (201 Created)
- [ ] User listeleme çalışıyor (200 OK)
- [ ] Duplicate email hatası çalışıyor (409 Conflict)
- [ ] 404 Not Found çalışıyor
- [ ] 401 Unauthorized çalışıyor
- [ ] Product oluşturma çalışıyor
- [ ] DELETE çalışıyor (204 No Content)
- [ ] Response formatı doğru (success, message, data)
- [ ] Başarılı response'larda data dolu
- [ ] Hata response'larında data null

---

## 💡 İpuçları

1. **Swagger UI Kullanımı:**
   - `http://localhost:5002/swagger` adresinden tüm endpoint'leri test edebilirsiniz
   - Her endpoint için "Try it out" butonuna tıklayın

2. **Response Format Kontrolü:**
   - Tüm başarılı response'larda `data` alanı dolu olmalı
   - Tüm hata response'larında `data` alanı `null` olmalı

3. **Status Code Kontrolü:**
   - `curl -v` kullanarak HTTP status code'ları görebilirsiniz:
   ```bash
   curl -v http://localhost:5002/api/users
   ```

4. **JSON Formatı:**
   - `jq` kullanarak JSON response'ları formatlayabilirsiniz:
   ```bash
   curl http://localhost:5002/api/users | jq
   ```

---

## 🐛 Sorun Giderme

### API çalışmıyor
- SQL Server container'ının çalıştığından emin olun
- Port 5002'nin kullanılabilir olduğundan emin olun

### Connection String Hatası
- `appsettings.json` veya `appsettings.Development.json` dosyasındaki şifreyi kontrol edin
- SQL Server container'ının çalıştığından emin olun

### Migration Hatası
- Veritabanını sıfırlamak için:
  ```bash
  docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <ŞİFRE> -Q "DROP DATABASE Net9LayeredApiDb; CREATE DATABASE Net9LayeredApiDb;"
  ```

