# Veritabanı kurulumu

Her işlem `English` ve `Turkish` klasörlerinde ayrı bir dosya olarak bulunur. İngilizce şema `PersonnelManagementDB`, Türkçe şema `PersonelYonetimiDB` adını kullanır. Uygulama İngilizce şemaya bağlanır. Türkçe sürüm, Türkçe tablo ve sütun adlarıyla bağımsız bir örnektir.

| İşlem | Her iki klasördeki dosya |
| --- | --- |
| Yalnız şema | `DatabaseSetup.sql` |
| Şema ve yapay örnek kayıtlar | `DatabaseSetupWithDemo.sql` |
| Önceden kurulmuş boş şemaya örnek kayıt ekleme | `DatabaseDemoData.sql` |
| Personeli doğrulama; rapor nesnelerini ve indeksleri yenileme | `DatabaseUpdate.sql` |
| Eski Türkçe veritabanını yeni, boş şemaya aktarma | `DatabaseMigration.sql` |
| Sınırlı SQL çalışma hesabı oluşturma | `DatabaseApplicationAccount.sql` |
| Demo verisiz ilk yönetici hesabını oluşturma | `InitializeAdmin.ps1` |

İki kurulum alternatifinden yalnız birini çalıştırın. Kurulum, mevcut veritabanını silmez ve aynı adla yeniden kurmayı reddeder. Demo ve aktarım dosyaları dolu bir hedefe yazmayı reddeder. Bir SQL hatası alırsanız sonraki dosyaya geçmeyin.

Mevcut kayıtları taşımak için önce kaynak veritabanının yedeğini alın ve uygulamayı kapatın. Aktarım scriptindeki `@LegacyDatabase` değerini kaynak adına göre düzenleyin (varsayılan `PersonelTakipV090Db`). Sonra `DatabaseSetup.sql` ve `DatabaseMigration.sql` dosyalarını sırayla çalıştırın. Kaynak veritabanı silinmez; kayıt kimlikleri ve mevcut giriş bilgileri korunur. V0.9 kayıtlarının saklanmış vardiya planları ve izin iptal geçmişi korunur. Planı eksik kayıtlarda alanlar mevcut vardiya tanımından oluşturulur. Yeni girişlerde plan, giriş anında saklanır.

Güncelleme dosyasından önce SSMS içinde doğru dildeki veritabanını seçin. Bu dosya 11 rakamlı kimlik ve doğum tarihi kurallarını doğrular; kuralları, personel adı/aktif vardiya indekslerini ve rapor nesnelerini günceller. Geçersiz mevcut kayıt varsa güncelleme durur ve geri alınır; kayıtlar kendiliğinden düzeltilmez. Önce yedek alın ve uygulamayı kapatın; eski Türkçe şemayı İngilizceye çevirmek için aktarım dosyasını kullanın.

Demo verilerindeki girişler `admin / Admin123!`, `manager / Manager123!`, `employee / Employee123!` biçimindedir. Bunlar gerçek kayıtları taşırken kullanılan şifreler değildir.

Uygulamayı SQL yönetici hesabıyla çalıştırmayın. Ayrı bir hesap için uygulama tablolarına `SELECT`, `INSERT`, `UPDATE`; `AuditLog` tablosuna yalnız `INSERT`; `EmployeeDetails`, `DailyAttendanceSummary`, `AnnualLeaveSummary` ve `GetAbsences` nesnelerine `SELECT` yetkisi verin. `DELETE`, şema değişikliği, `sysadmin` ve `db_owner` yetkileri gerekmez.

Giriş ekranındaki **Bağlantı Ayarları** üzerinden sunucu, veritabanı ve sınırlı hesap bilgilerini girin. Yerel ayar Windows kullanıcısına bağlı olarak şifrelenir. Ortam değişkeni üzerinden bir bağlantı ayarlanmışsa öncelik bu ayardadır.

Rapor indeksleri, gereken alanları indeks içinde tutarak tabloya tekrarlanan erişimleri azaltır. Güncelleme, aynı personel ve tarih için tek giriş kuralını korur. Yeniden çalıştırıldığında mevcut indeksler yeterliyse onları tekrar oluşturmaz.

Uygulama rol kodları: Admin (sistem yöneticisi), Manager (İK yöneticisi), Employee (İK çalışanı). Mevcut kurulumda Manager hesabı eklemeden önce DatabaseUpdate.sql dosyasını çalıştırın. Şema güncellemesi kullanıcı oluşturmaz ve mevcut şifreleri değiştirmez. Bağımsız Türkçe örnekte rol kodları Admin, Yonetici ve Personel olur. Her iki demo scripti üç yapay hesap oluşturur. Yetki tablosu projenin ana README dosyasındadır.

## Demo verisiz ilk yönetici

Uygulamayı Release derleyin ve `English/DatabaseSetup.sql` dosyasını bir kez çalıştırın. Proje kökünde **Windows PowerShell 5.1** kullanın:

```powershell
$sqlCredential = Get-Credential -Message 'SQL Server kurulum hesabı'
$adminPassword = Read-Host 'İlk uygulama yöneticisinin şifresi' -AsSecureString
.\PersonnelManagement\Sources\English\InitializeAdmin.ps1 -Server 'SERVER\INSTANCE' -SqlCredential $sqlCredential -AdminPassword $adminPassword
```

SQL kurulum hesabına `UserAccount` için select/insert ve `AuditLog` için insert gerekir; bu hesap uygulamanın Admin rolü ve çalışma bağlantı hesabından ayrıdır. Şifre 8–50 karakter olmalıdır. Script SQL Server Authentication ve şifreli bağlantı kullanır. Kendinden imzalı sertifikalı, açıkça güvendiğiniz sunucuda `-TrustServerCertificate` ekleyebilirsiniz. Uygulamanın koduyla şifre hashlenir; hesap ve denetim kaydı birlikte yazılır. Kullanıcı tablosu doluysa çalıştırma reddedilir. Bağlantı ayarları değiştirilmez. Bağımsız Türkçe şema için kendi kurulumundan sonra `Turkish/InitializeAdmin.ps1` kullanılır; uygulama İngilizce şemaya bağlanır.

Departman/pozisyon ekranları sonraki aşamada kalır. Boş İngilizce şemada başlangıç tanımları SSMS üzerinden yetkili kurulum hesabıyla eklenebilir:

```sql
BEGIN TRANSACTION;
INSERT dbo.Department(DepartmentName) VALUES(N'Human Resources');
DECLARE @DepartmentId int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.Position(DepartmentId,PositionName) VALUES(@DepartmentId,N'HR Specialist');
COMMIT TRANSACTION;
```

Bağımsız Türkçe örnekte aynı işlem:

```sql
BEGIN TRANSACTION;
INSERT dbo.Departman(DepartmanAdi) VALUES(N'İnsan Kaynakları');
DECLARE @DepartmanId int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.Pozisyon(DepartmanId,PozisyonAdi) VALUES(@DepartmanId,N'İK Uzmanı');
COMMIT TRANSACTION;
```

Uygulamanın vardiya ekranından bir vardiya oluşturup ardından personel ekleyin. Bu isteğe bağlı tanımları mevcut tanımlar üzerinde tekrar çalıştırmayın. SQL kurulum bilgilerini kaynak kodda saklamayın; uygulama için sınırlı SQL bağlantı hesabı ayarlayın.

## V1.0.0 bağlantısı ve V0.9 aktarımı

Uygulama yalnız SQL Server Authentication kullanır. `DatabaseApplicationAccount.sql` sınırlı SQL hesabı oluşturur; değişkenlerde hesap adı ve özel parolayı yerel olarak belirleyin. Mevcut hesabı değiştirmez.

Aktarımda `@LegacyDatabase` değişkenini kaynak adına göre düzenleyin; varsayılan `PersonelTakipV090Db`. V0.9 vardiya planı ve iptal bilgileri korunur. Planı bulunmayan kayıtlar mevcut vardiya tanımından doldurulur. Kaynak veritabanı silinmez.

Bağlantı ekranı ve `tools/ConfigureConnection.ps1`, `%LOCALAPPDATA%\PersonnelManagement\V1.0.0\English\connection.dat` profilini kullanır. Sertifikaya güvenme tercihi açıkça seçilir.
