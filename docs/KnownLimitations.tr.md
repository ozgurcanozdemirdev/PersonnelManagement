# Bilinen sınırlar

Uygulama İK personeli içindir. Çalışanların yalnız kendi bilgilerini gördüğü portal, mevzuata göre izin hakkı hesabı, bordro, işe alım ve işten ayrılma iş akışları içermez.

Departman ve pozisyon veri erişim sınıfları vardır; ayrı yönetim ekranları yoktur. Başlangıç tanımları kurulum yöneticisi tarafından eklenebilir; demo kurulum örnekleri sağlar. Parola sıfırlama Admin yetkili veri erişim işlemi olarak vardır. Parola sıfırlama, hesap aktiflik yönetimi ve kendi parolasını değiştirme ekranları yoktur.

Eski User/Kullanici rolleri aktarımda kayıt geçmişi için korunur. Giriş yapabilmeleri için Admin desteklenen Admin/Manager/Employee rollerinden birini atamalıdır. Saklanmış planı olmayan eski giriş kayıtları mevcut vardiya tanımından doldurulur; asıl tarihsel tanım geri üretilemez.

Asenkron okuma personel arama ve rapor yükleme sırasında ekranı kullanılabilir tutar ve iptal destekler. Personel listesinde sunucu tarafında sayfalama yoktur. Küçük seçim sorgusu daha az veri taşır; süreler bilgisayara ve verilere bağlıdır.

Türkçe şema bağımsız SQL örneğidir; uygulama İngilizce şema kullanır. SQL hesabının izinleri ile uygulama kullanıcılarının rolleri farklıdır. Paylaşılan SQL hesabıyla doğrudan erişimde kişi bazlı İK yetkilendirmesi uygulanmaz.

Testler yapay veritabanlarını ve bellekte yazdırma sayfasını doğrular. Fiziksel yazıcı çıktısı, tüm ekran/DPI ayarları ve canlı kurulum davranışı ayrıca kontrol edilmelidir.
