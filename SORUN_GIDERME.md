# 🔧 Localhost Sorun Giderme Rehberi

## Sorun: Localhost çalışmıyor

### Tespit Edilen Sorunlar

1. **Port 5000 kullanımda** - Başka bir process port 5000'i kullanıyor
2. **Docker çalışmıyor** - SQL Server container'ı çalışmıyor

## Çözüm Adımları

### 1. Port 5000'i Kullanan Process'i Durdurma

**Seçenek A: Process'i bul ve durdur**
```bash
# Port 5000'i kullanan process'i bul
lsof -ti:5000

# Process'i durdur (PID'yi yukarıdaki komuttan alın)
kill -9 635
```

**Seçenek B: Farklı port kullan**
`launchSettings.json` dosyasında portu değiştirin:
```json
"applicationUrl": "http://localhost:5002"
```

### 2. Docker'ı Başlatma

**Adım 1: Docker Desktop'ı açın**
- macOS: Applications klasöründen Docker.app'i çalıştırın
- Terminal'de kontrol edin:
  ```bash
  docker --version
  ```

**Adım 2: SQL Server Container'ını Başlatın**
```bash
# Container'ı başlat (eğer varsa)
docker start sqlserver

# Veya yeni container oluştur
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

**Adım 3: Container'ın çalıştığını kontrol edin**
```bash
docker ps | grep sqlserver
```

### 3. Connection String Kontrolü

`appsettings.Development.json` dosyasının var olduğunu ve doğru olduğunu kontrol edin:

```bash
cat src/Net9LayeredApi.API/appsettings.Development.json
```

Eğer yoksa oluşturun:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=Net9LayeredApiDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 4. Projeyi Çalıştırma

```bash
cd src/Net9LayeredApi.API
dotnet run
```

**Beklenen Çıktı:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 5. Test

Yeni bir terminal'de:
```bash
curl http://localhost:5000/ping
```

Beklenen: `pong`

## Hızlı Çözüm Komutları

```bash
# 1. Port 5000'i kullanan process'i durdur
kill -9 $(lsof -ti:5000)

# 2. Docker'ı başlat (eğer yüklüyse)
open -a Docker

# 3. SQL Server container'ını başlat
docker start sqlserver || docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# 4. Projeyi çalıştır
cd src/Net9LayeredApi.API && dotnet run
```

## Alternatif: Farklı Port Kullan

Eğer port 5000'i kullanamıyorsanız:

1. `launchSettings.json` dosyasını düzenleyin:
```json
"applicationUrl": "http://localhost:5002"
```

2. Projeyi çalıştırın:
```bash
cd src/Net9LayeredApi.API
dotnet run
```

3. Test edin:
```bash
curl http://localhost:5002/ping
```

## Yaygın Hatalar ve Çözümleri

### Hata: "A network-related or instance-specific error occurred"
**Çözüm:** SQL Server container'ının çalıştığını kontrol edin: `docker ps | grep sqlserver`

### Hata: "Port 5000 is already in use"
**Çözüm:** Port'u kullanan process'i durdurun veya farklı port kullanın

### Hata: "Cannot connect to Docker daemon"
**Çözüm:** Docker Desktop'ı başlatın: `open -a Docker`

### Hata: "appsettings.Development.json not found"
**Çözüm:** Dosyayı oluşturun (yukarıdaki örnek JSON'u kullanın)

