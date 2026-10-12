# PersonnelManagement 0.9.0

[English](README.md)

Personel, departman/pozisyon, vardiya, giriş-çıkış, izin ve raporlama işlemleri için C# WinForms uygulaması. Arayüz Türkçedir; namespace ve assembly adı `PersonnelManagement`.

## Gereksinimler

- Windows, .NET Framework 4.8.1.
- .NET masaüstü iş yükü ve 4.8.1 targeting pack kurulu Visual Studio veya Build Tools.
- SQL Server 2022 veya sonrası, SSMS ve etkin SQL Server Authentication.
- Kurulum araçları için Windows PowerShell 5.1.

## Kurulum

1. SSMS'e SQL Server Authentication ile bağlan.
2. Yeni kurulum için `PersonnelManagement/Kaynak/DatabaseSetup.sql` dosyasının tamamını çalıştır. `PersonelTakipV090Db` oluşturulur; bu adla veritabanı varsa kurulum durur. Mevcut veriler silinmez. Zaten çalışan yerel kurulumunda scripti yeniden çalıştırman gerekmez.
3. Bu sürümün ana klasöründe bağlantıyı tanımla:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\ConfigureConnection.ps1 -Server "localhost\SQLEXPRESS" -Database "PersonelTakipV090Db"
```

Sunucu adını kendi SQL instance adına göre değiştir. SQL kullanıcı adı ve şifresi istenir. Doğrulanan bağlantı, `%LOCALAPPDATA%\PersonnelManagement\V0.9.0\connection.dat` dosyasına bu Windows kullanıcısına özel şifreli olarak kaydedilir; proje dosyalarına yazılmaz. Başka bilgisayar veya Windows kullanıcısı için aracı yeniden çalıştır.

Şifreli bağlantı etkin kullanılır. Yerel geliştirme sunucunda kendinden imzalı sertifika varsa `-TrustServerCertificate` ekleyebilirsin. Güvenilen sertifikası olan sunucularda sertifika doğrulamasını açık bırak.

`PERSONNELMANAGEMENT_V090_CONNECTION_STRING` ortam değişkeni yalnız ilgili uygulama sürecinin bağlantısını değiştirebilir ve yerel dosyadan önce okunur. Kaynak dosyalarındaki bağlantı varsayılanları boştur. Gerçek şifreleri GitHub'a gönderme.

Kurulum scriptindeki örnek uygulama hesaplarının giriş bilgileri ilgili INSERT satırlarının yanında açıklanır. Bunlar SQL bağlantı bilgileri değildir. Gerçek personel verileriyle kullanmadan önce değiştir. Uygulama şifreleri bu sürümün mevcut saklama biçiminde tutulur.

## Derleme

Slnx destekleyen Visual Studio ile `PersonnelManagement.slnx` dosyasını aç. Diğer sürümlerde `PersonnelManagement/PersonnelManagement.csproj` doğrudan açılabilir. Derle ve çalıştır. Developer PowerShell ile:

```powershell
MSBuild.exe .\PersonnelManagement\PersonnelManagement.csproj /t:Rebuild /p:Configuration=Release
```

Harici NuGet paketi gerekmiyor. `.vs`, `bin` ve `obj` Git tarafından hariç tutulur.

## Testler

[tests/README.tr.md](tests/README.tr.md) test kapsamını açıklar. Yalnız test süreci için `PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING` tanımla; yerel SQL hesabının geçici veritabanı oluşturma ve silme izni olmalı. Ardından:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tests\Verify.ps1
```

Testler GUID adlı geçici veritabanları kullanır ve sonunda kaldırır. Normal personel veritabanında çalışmaz. Fiziksel yazıcı ve bütün manuel ekran akışları bu kontrollerin kapsamına girmez.

## Sürüm kapsamı

V0.9.0 giriş-çıkış, izin doğrulama ve raporlama düzeltmeleri ile regresyon testlerini içerir. Kullanıcı girişi, rol yönetimi ve veritabanı loglaması etkindir. Arayüz ve mevcut kod adları Türkçedir; uygulama şifreleri bu sürümün mevcut saklama biçiminde tutulur.

Değişiklikler [CHANGELOG.md](CHANGELOG.md) dosyasındadır. Lisans: [MIT](LICENSE).
