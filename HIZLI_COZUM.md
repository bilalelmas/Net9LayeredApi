# 🚀 Hızlı Çözüm - 500 Hatası

## Sorun
API endpoint'leri `500 Internal Server Error` veriyor.

## Neden
`appsettings.Development.json` dosyasında SQL Server şifresi placeholder olarak duruyor:
```json
"Password=YOUR_PASSWORD_HERE;"
```

## Çözüm

### Adım 1: appsettings.Development.json Dosyasını Düzenleyin

Dosya yolu: `src/Net9LayeredApi.API/appsettings.Development.json`

**Şu satırı bulun:**
```json
"Password=YOUR_PASSWORD_HERE;"
```

**Gerçek SQL Server şifrenizle değiştirin:**
```json
"Password=YourStrong@Passw0rd;"
```

**Tam örnek:**
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

**Önemli:** `YourStrong@Passw0rd` kısmını Docker container'ını oluştururken kullandığınız şifreyle değiştirin!

### Adım 2: Projeyi Yeniden Başlatın

1. Çalışan projeyi durdurun (Ctrl+C)
2. Tekrar başlatın:
   ```bash
   cd src/Net9LayeredApi.API
   dotnet run
   ```

### Adım 3: Test Edin

```bash
curl http://localhost:5002/api/users
```

**Beklenen:** `{"Success":true,"Message":"...","Data":[],"Errors":null}`

## SQL Server Şifresini Hatırlamıyorsanız

Eğer SQL Server container'ını oluştururken kullandığınız şifreyi hatırlamıyorsanız:

1. Yeni bir container oluşturun:
   ```bash
   docker stop sqlserver
   docker rm sqlserver
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyNewPassword123!" \
      -p 1433:1433 --name sqlserver \
      -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. `appsettings.Development.json` dosyasında şifreyi güncelleyin:
   ```json
   "Password=MyNewPassword123!;"
   ```

## Başarı Kontrolü

Şifreyi güncelledikten ve projeyi yeniden başlattıktan sonra:

```bash
./test-api.sh
```

Tüm testler geçmeli! ✅

