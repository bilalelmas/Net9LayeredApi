# ✅ Başarı Kontrol Rehberi

Projenin başarıyla çalıştığını kontrol etmek için aşağıdaki adımları takip edin.

## 🎯 Hızlı Kontrol (1 dakika)

### 1. Health Check ✅
```bash
curl http://localhost:5002/ping
```
**Beklenen:** `pong` ✅

### 2. Swagger UI Kontrolü
Tarayıcıda şu adreslere gidin:
- `http://localhost:5002` (Swagger UI - root'ta olmalı)
- `http://localhost:5002/swagger` (alternatif)

**Beklenen:** Swagger UI sayfası açılmalı ve tüm endpoint'ler görünmeli

### 3. API Endpoint Testi
```bash
curl http://localhost:5002/api/users
```
**Beklenen:** `{"Success":true,"Message":"...","Data":[],"Errors":null}` veya boş array

## 📊 Detaylı Test (5 dakika)

### Otomatik Test Scripti
```bash
cd /Users/bilalelmas/GitHub/Net9LayeredApi
./test-api.sh
```

Bu script şunları test eder:
- ✅ Health check
- ✅ Swagger UI erişilebilirliği
- ✅ User CRUD operasyonları
- ✅ Product CRUD operasyonları
- ✅ Hata senaryoları

## ✅ Başarı Kriterleri

Proje başarıyla çalışıyorsa:

1. ✅ **Health Check:** `/ping` endpoint'i `pong` döner
2. ✅ **Swagger UI:** Tarayıcıda Swagger UI sayfası açılır
3. ✅ **API Endpoints:** Tüm endpoint'ler çalışır (200, 201, 404 gibi doğru status kodları)
4. ✅ **Veritabanı:** SQL Server bağlantısı başarılı
5. ✅ **Hata Yönetimi:** Hatalar doğru HTTP status kodları ile döner

## 🔍 Sorun Tespiti

### Swagger UI 404 Hatası
**Çözüm:** `Program.cs`'de `RoutePrefix` ayarını kontrol edin veya `/swagger` adresini deneyin

### API 500 Hatası
**Olası Nedenler:**
1. SQL Server container'ı çalışmıyor
2. Connection string yanlış
3. Veritabanı oluşturulmamış

**Kontrol:**
```bash
# SQL Server container kontrolü
docker ps | grep sqlserver

# Connection string kontrolü
cat src/Net9LayeredApi.API/appsettings.Development.json
```

### Veritabanı Bağlantı Hatası
**Çözüm:**
1. Docker Desktop'ın çalıştığını kontrol edin
2. SQL Server container'ını başlatın: `docker start sqlserver`
3. Connection string'deki şifreyi kontrol edin

## 📝 Test Senaryoları

### Senaryo 1: User Oluşturma
```bash
curl -X POST http://localhost:5002/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "User"
  }'
```
**Beklenen:** `201 Created` ve user bilgileri

### Senaryo 2: User Listeleme
```bash
curl http://localhost:5002/api/users
```
**Beklenen:** `200 OK` ve user listesi

### Senaryo 3: Duplicate Email Testi
```bash
# Aynı email ile tekrar oluştur
curl -X POST http://localhost:5002/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser2",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "User"
  }'
```
**Beklenen:** `400 Bad Request` ve hata mesajı

## 🎉 Başarılı Test Sonucu

Tüm testler geçtiyse:
- ✅ Proje çalışıyor
- ✅ API endpoint'leri çalışıyor
- ✅ Veritabanı bağlantısı başarılı
- ✅ Hata yönetimi çalışıyor
- ✅ Swagger dokümantasyonu erişilebilir

**Tebrikler! Proje başarıyla çalışıyor! 🎉**

