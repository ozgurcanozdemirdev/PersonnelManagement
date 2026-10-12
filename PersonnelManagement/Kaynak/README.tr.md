# Veritabanı kurulumu

- Yeni kurulum: SSMS'te `DatabaseSetup.sql` dosyasının tamamını çalıştır. `PersonelTakipV090Db` varsa kurulum durur; veri silinmez. `SqlScript.txt` aynı kurulumun kopyasıdır; yalnızca birini çalıştır.
- Eski V0.9.0 kurulumu: önce yedek al, SSMS veritabanı listesinden `PersonelTakipV090Db` seç ve `DatabaseUpdate.sql` dosyasının tamamını çalıştır. Güncelleme tekrar uygulanabilir; geçersiz mevcut veri varsa geri alınır.
- SQL Authentication bağlantısını ana klasördeki `tools/ConfigureConnection.ps1` ile tanımla. Kaynak dosyalarında gerçek bağlantı bilgisi bulunmaz.

Güncelleme, girişteki vardiya planını saklayan alanları, ayrı izin iptal durumunu ve veri bütünlüğü kontrollerini ekler. Eski girişlerde geçmiş plan bilinmediğinden güncelleme anındaki vardiya kullanılır; yeni girişler kendi planını saklar.

Farklı bir test kurulumunda `PERSONNELMANAGEMENT_V090_CONNECTION_STRING` yalnız süreç bağlantısını değiştirebilir. V0.9.0 yeni kurulum için ayrı `PersonelTakipV090Db` kullanır; önceki sürümün veritabanını otomatik kopyalamaz veya güncellemez.

Tam kurulum: [ana README](../../README.tr.md).
