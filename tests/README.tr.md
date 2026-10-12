# Regresyon kontrolleri

Windows üzerinde Visual Studio MSBuild, .NET Framework 4.8.1 targeting pack ve SQL Server Authentication kabul eden yerel SQL Server gerekir. Harici test framework'ü gerekmez.

Test yöneticisi SQL hesabı geçici veritabanı ve SQL login oluşturup kaldırabilmelidir. Test edilen uygulama, tablo yetkileri olan ayrı bir geçici SQL hesabı kullanır; bu hesap `sysadmin` veya `db_owner` değildir. Geliştirme instance'ını kullanın.

`PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING` değişkenini mevcut işlemde yerel gizli ayarlarınızdan doldurun. Şifreleri kaynak dosyasına yazmayın veya commit etmeyin. Ardından depo kökünden çalıştırın:

```powershell
.\tests\Verify.ps1 -Configuration Release
```

İsteğe bağlı `-MSBuildPath`, MSBuild konumunu belirtir. Script uygulamayı ve test programını derler. Test programı yalnız GUID adlı `PersonnelManagementTest_...` ve `PersonelYonetimiTest_...` veritabanları oluşturur, sentetik kayıtları yükler, iki şemayı ve tekrarlanabilir güncellemeleri doğrular. Geçici veritabanları ve login `finally` içinde kaldırılır. Yapılandırılmış uygulama veritabanı ve bağlantı dosyası değiştirilmez.

Kontroller personel doğrulamasını, kullanılan vardiyanın pasife alınmasını, denetim hatasında geri almayı, giriş işleminin geri alınmasını, rol sınırlarını, senkron/asenkron sonuç eşitliğini ve kilit bekleyen SQL okumasının iptalini kapsar. Performans çıktısı eklenen 10.000 sentetik personelde tam detay sorgusunu ID/ad/soyad seçim sorgusuyla karşılaştırır; beş çalıştırmanın ortanca süresini ve SqlClient'ın aldığı bayt miktarını verir. Süre makineye göre değişir; kontrol sabit hız oranı yerine seçim listesinin korunmasını ve veri aktarımının azalmasını doğrular.

Başarı için çıkış kodu 0, `CHECKS_PASSED=...`, `LIVE_DATABASE_WRITES=0` ve `SCRATCH_RESOURCES_REMOVED=true` gerekir. Hata halinde `TEST_ERROR`, `CLEANUP_ERROR` ve `LOGIN_CLEANUP_ERROR` satırlarını inceleyin. Çıktı şifre içermez; test kimlikleri ve ölçümler içerir.

İlk yönetici scriptlerinin iki dilde hesap/denetim kaydını birlikte oluşturması, şifre hashinin doğrulanması ve dolu tabloyu reddetmesi test edilir. Eşzamanlı vardiya testinde atama kilidi sürerken pasife alma başlatılır; vardiyanın aktif kaldığı doğrulanır. SQL kuralları `char(11)` tarafından boşlukla tamamlanan kısa kimlikleri de reddeder.

Ek kontroller sürüme özel şifreli bağlantı önceliği ve değiştirme, SQL Authentication yapılandırması, anında çıkış, yıllık izin bakiyesi, XLSX/CSV içeriği, uzun metinli yazdırma sayfaları, iki dilde demo scriptleri, sınırlı SQL hesabı ve eski/V0.9 aktarımında kimlik/parola/geçmiş korunmasını kapsar. Yazdırma testleri fiziksel yazıcıya iş göndermeden bitmap üzerinde çalışır.

`CHECKS_PASSED` rol kontrollerini de içerir; `ROLE_CHECKS` bu alt grubun sayısını ayrıca gösterir.
