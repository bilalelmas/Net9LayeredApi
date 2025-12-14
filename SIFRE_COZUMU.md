# 🔐 SQL Server Şifre Sorunu Çözümü

## Sorun
"Login failed for user 'sa'" hatası alıyorsunuz.

## Neden
SQL Server container'ı oluştururken kullandığınız şifre ile `appsettings.Development.json` dosyasındaki şifre eşleşmiyor.

## Çözüm Seçenekleri

### Seçenek 1: Container'ı Yeni Şifreyle Yeniden Oluşturun (ÖNERİLEN)

1. **Mevcut container'ı durdurun ve silin:**
   ```bash
   docker stop sqlserver
   docker rm sqlserver
   ```

2. **Yeni container oluşturun (şifre: `YourStrong@Passw0rd`):**
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
      -p 1433:1433 --name sqlserver \
      -d mcr.microsoft.com/mssql/server:2022-latest
   ```

3. **Container'ın başlamasını bekleyin (10-15 saniye):**
   ```bash
   docker ps | grep sqlserver
   ```

4. **Projeyi yeniden başlatın:**
   ```bash
   cd src/Net9LayeredApi.API
   dotnet run
   ```

### Seçenek 2: appsettings.Development.json'daki Şifreyi Güncelleyin

Eğer container'ı farklı bir şifreyle oluşturduysanız:

1. **Container'ı oluştururken kullandığınız şifreyi bulun:**
   ```bash
   docker inspect sqlserver | grep SA_PASSWORD
   ```

2. **appsettings.Development.json dosyasını düzenleyin:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=Net9LayeredApiDb;User Id=sa;Password=GERÇEK_ŞİFRENİZ;TrustServerCertificate=true;"
     }
   }
   ```

3. **Projeyi yeniden başlatın**

## Şifre Gereksinimleri

SQL Server şifresi şu gereksinimleri karşılamalı:
- En az 8 karakter
- Büyük harf içermeli
- Küçük harf içermeli
- Rakam içermeli
- Özel karakter içermeli (@, !, #, vb.)

**Örnek güçlü şifreler:**
- `YourStrong@Passw0rd`
- `MyP@ssw0rd123`
- `Test123!@#`

## Kontrol Komutları

### Container Durumu
```bash
docker ps | grep sqlserver
```

### Container Logları
```bash
docker logs sqlserver
```

### Connection String Test
```bash
# appsettings.Development.json'daki şifreyi kullanarak test edin
cat src/Net9LayeredApi.API/appsettings.Development.json | grep Password
```

## Hızlı Çözüm (Tüm Adımlar)

```bash
# 1. Container'ı durdur ve sil
docker stop sqlserver && docker rm sqlserver

# 2. Yeni container oluştur
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# 3. 15 saniye bekle
sleep 15

# 4. Container durumunu kontrol et
docker ps | grep sqlserver

# 5. Projeyi başlat
cd src/Net9LayeredApi.API
dotnet run
```

## Başarı Kontrolü

Proje başlatıldığında şu mesajları görmelisiniz:
```
info: Program[0]
      Veritabanı oluşturuluyor...
info: Program[0]
      Veritabanı oluşturuldu: True
```

Eğer hala "Login failed" hatası alıyorsanız:
1. Container'ın tamamen başladığından emin olun (15-20 saniye bekleyin)
2. Şifrenin doğru olduğundan emin olun
3. `appsettings.Development.json` dosyasının doğru okunduğundan emin olun

