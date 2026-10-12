# Regresyon testleri

Yerel SQL Server bağlantısını PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING ortam değişkenine tanımlayıp Verify.ps1 dosyasını çalıştır. Bağlantı geçici test veritabanı oluşturma/silme yetkisine sahip olmalıdır.

Testler GUID adlarıyla geçici veritabanı oluşturur; mevcut personel veritabanını kullanmaz. Geçici veritabanı test sonunda kaldırılır. Hata halinde SCRATCH_DATABASE_REMOVED satırı ve çıkış kodu kontrol edilmelidir.

Giriş-çıkış, gece vardiyası, izin çakışması ve hakları, eşzamanlı işlemler, log hatasında geri alma, form yükleme, fotoğraf kaynakları, raporlar, XLSX/CSV ve yazdırma sayfalaması doğrulanır. Yazdırma, bitmap üzerindeki sayfa çizimiyle test edilir; fiziksel yazıcıya çıktı gönderilmez.

Eski şemanın güncellenmesi de ayrı bir geçici veritabanında test edilir. Mükerrer kayıtlar ve geçersiz mevcut veriler güncellemeyi durdurmalıdır; başarısız güncellemenin eklediği alanlar geri alınır. Başarılı ve tekrarlanan güncellemelerde on tablonun kayıt sayıları korunur.
