# PersonnelManagement

İnsan kaynakları ekipleri için C# WinForms personel, vardiya, giriş/çıkış, izin ve rapor yönetimi uygulaması. **1.0.0** sürümü SQL Server kullanır; kod ve ana veritabanı adları İngilizcedir. Arayüz Türkçe ve İngilizce arasında değiştirilebilir.

[English](README.md) · [Sürüm geçmişi](CHANGELOG.md) · [MIT lisansı](LICENSE)

## Özellikler

- Departman, pozisyon ve vardiya bağlantılarıyla personel kayıtları.
- Gece vardiyası, mola ve fazla mesaiyi dikkate alan giriş/çıkış işlemleri.
- Her girişte saklanan vardiya planı; sonraki vardiya değişiklikleri geçmiş kaydın hesabını değiştirmez.
- İzin çakışması, onay/red, iptal geçmişi ve yıl bazında yıllık izin bakiyesi.
- Personel, giriş/çıkış, izin ve devamsızlık raporları; XLSX/CSV çıktısı ve çok sayfalı yazdırma.
- Kalıcı TR/EN seçimi; ekran metinleri ve doğrulama mesajları.
- Arayüz ve veri erişiminde Admin, İK Yöneticisi ve İK Çalışanı yetkileri.
- PBKDF2-SHA256 parola saklama, şifreli yerel SQL ayarı ve işlemle birlikte kaydedilen loglar.
- İptal edilebilen asenkron arama/rapor okuması ve seçim listeleri için yalnız kimlik/ad sorguları.

## Gereksinimler

Windows, WinForms ve .NET Framework **4.8.1** hedefleme paketini içeren Visual Studio/MSBuild ve **SQL Server Authentication** kabul eden yerel SQL Server. Çözüm `.slnx` biçimindedir. Harici NuGet paketi gerekmez.

## Demo ile çalıştırma

1. `PersonnelManagement.slnx` çözümünü açıp Release olarak derleyin.
2. SSMS'te [English/DatabaseSetupWithDemo.sql](PersonnelManagement/Sources/English/DatabaseSetupWithDemo.sql) dosyasının tamamını çalıştırın. `PersonnelManagementDB` ve yapay örnek kayıtlar oluşturulur. Mevcut veritabanının üzerine yazılmaz. Ad zaten kullanılıyorsa ayrı bir ad seçip çalıştıracağınız scriptlerdeki bu adı değiştirin. SQL hatasında sonraki adıma geçmeyin.
3. [English/DatabaseApplicationAccount.sql](PersonnelManagement/Sources/English/DatabaseApplicationAccount.sql) dosyasını SQL kurulum yöneticisiyle çalıştırın. Değişkenlerde benzersiz SQL hesap adı ve özel parola belirleyin; parolayı kaynak dosyada yayımlamayın. Oluşan hesap sysadmin/db_owner değildir; personel silme ve log güncelleme izni yoktur.
4. Uygulamayı başlatın. **Bağlantı Ayarları** ekranında sunucu, veritabanı, SQL hesap adı ve SQL parolasını girip kaydedin. **Sunucu sertifikasına güven** seçeneğini o sunucunun sertifikasına güvenmek istediğinizde işaretleyin. Bağlantı şifreleme açık kalır.
5. Aşağıdaki uygulama hesaplarından biriyle giriş yapın. Bunlar SQL bağlantı hesabından ayrıdır.

| Hesap | Parola | Rol |
| --- | --- | --- |
| admin | Admin123! | Sistem Yöneticisi |
| manager | Manager123! | İK Yöneticisi |
| employee | Employee123! | İK Çalışanı |

Boş kurulum için demo dosyası yerine `DatabaseSetup.sql` kullanılır. İlk uygulama admini `tools/CreateAdmin.ps1` ile oluşturulur. Ayrıntılar [veritabanı açıklamalarında](PersonnelManagement/Sources/README.tr.md).

## Yetkiler

Çalışan rolü insan kaynakları personelidir; şirket çalışanlarının yalnız kendi bilgilerini gördüğü bir portal değildir.

| İşlem | Admin | İK Yöneticisi | İK Çalışanı |
| --- | --- | --- | --- |
| Ana menü ve personel kayıtları | Evet | Evet | Evet |
| Personel ekleme/güncelleme ve giriş/çıkış kaydı | Evet | Evet | Evet |
| Yıllık izin hakkını değiştirme veya işten çıkarma | Evet | Evet | Hayır |
| Vardiya görüntüleme ve izin talebi oluşturma | Evet | Evet | Evet |
| Vardiya yönetimi; izin onay/red/iptal | Evet | Evet | Hayır |
| Rapor, dışa aktarma ve yazdırma | Evet | Evet | Hayır |
| Uygulama hesabı oluşturma ve rol atama | Evet | Hayır | Hayır |

İşlemler güncel veritabanı rolünü ve aktiflik durumunu kontrol eder. Son aktif adminin rolü düşürülemez. İK çalışanı mevcut yıllık izin hakkını korur ve yeni kayıtta varsayılan değeri kullanır. Uygulama mevzuata göre izin hakkı hesaplamaz.

## Bağlantı ve testler

SQL ayarı Windows DPAPI ile `%LOCALAPPDATA%\PersonnelManagement\V1.0.0\English\connection.dat` konumuna kaydedilir. Bu ayrı profil, eski Türkçe V1.0 veritabanına ait ayarların yüklenmesini önler. Profil yoksa uygulama dosyasının yanındaki şifreli `connection.dat` okunabilir; bozuk profil olduğunda sessizce başka veritabanına geçilmez.

`PERSONNELMANAGEMENT_V100_CONNECTION_STRING` işlem değişkeni önceliklidir. Kaynak kodda kullanılabilir bağlantı veya özel SQL parolası yoktur. Windows Authentication ve yüksek yetkili çalışma hesapları reddedilir.

Windows PowerShell 5.1 ile yapılandırma:

```powershell
$sqlCredential = Get-Credential -Message 'Sınırlı uygulama SQL hesabı'
.\tools\ConfigureConnection.ps1 -Server 'SUNUCU\INSTANCE' -Credential $sqlCredential
```

Güvendiğiniz imzalı olmayan sertifika için `-TrustServerCertificate` eklenebilir. `-SettingsDirectory` ile uygulama klasöründeki şifreli yedek konuma yazılabilir. Araç bağlantı ve İngilizce şema doğrulanmadan mevcut ayarı değiştirmez.

Yerel geliştirme SQL sunucusu için yalnız mevcut işlemde `PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING` tanımlayıp `.\tests\Verify.ps1 -Configuration Release` çalıştırın. Testler GUID adlı geçici veritabanları ve SQL hesapları oluşturur, sonunda kaldırır. [Test açıklamaları](tests/README.tr.md) ve [bilinen sınırlar](docs/KnownLimitations.tr.md).

`UI` ekranları ve dil desteğini, `DataAccess` parametreli SQL işlemlerini, `Models` veri nesnelerini, `Security` parola/yetki/doğrulamayı, `Logging` işlem loglarını, `Reporting` dışa aktarımı içerir.

Türkçe SQL kopyaları ayrı `PersonelYonetimiDB` veritabanında Türkçe tablo/sütun kullanır. Uygulama arayüz dilinden bağımsız olarak İngilizce şemaya bağlanır. Aktarım scriptleri kayıt kimliklerini, parolaları, V0.9 vardiya planlarını ve iptal geçmişini korur. Eski kayıtta plan yoksa mevcut vardiyadan oluşturulur. Canlı veritabanı otomatik dönüştürülmez.
