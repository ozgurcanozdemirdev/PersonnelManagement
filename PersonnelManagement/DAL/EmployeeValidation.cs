using PersonnelManagement.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace PersonnelManagement.DAL
{
  internal static class EmployeeValidation
  {
    internal static void Validate(Personel personel)
    {
      if (personel == null)
        throw new ArgumentNullException(nameof(personel));
      personel.Ad = (personel.Ad ?? "").Trim();
      personel.Soyad = (personel.Soyad ?? "").Trim();
      personel.TcKimlikNo = (personel.TcKimlikNo ?? "").Trim();
      if (personel.Ad.Length == 0 || personel.Ad.Length > 50 || personel.Soyad.Length == 0 || personel.Soyad.Length > 50)
        throw new ArgumentException("Ad ve soyad zorunludur; her biri en fazla 50 karakter olabilir.");
      if (personel.TcKimlikNo.Length != 11 || personel.TcKimlikNo.Any(c => c < '0' || c > '9'))
        throw new ArgumentException("TC Kimlik No 11 rakamdan oluşmalıdır.");
      CheckLength(personel.Cinsiyet, 10, "Cinsiyet");
      CheckLength(personel.Telefon, 20, "Telefon");
      CheckLength(personel.Email, 100, "Email");
      CheckLength(personel.Adres, 500, "Adres");
      CheckLength(personel.FotoPath, 500, "Fotoğraf yolu");
      if (personel.YillikIzinHakki < 0)
        throw new ArgumentException("Yıllık izin hakkı negatif olamaz.");
      if (personel.IseGirisTarihi.Date < new DateTime(1753, 1, 1))
        throw new ArgumentException("İşe giriş tarihi SQL tarih aralığının dışında.");
      if (personel.DogumTarihi.HasValue && personel.DogumTarihi.Value.Date < new DateTime(1753, 1, 1))
        throw new ArgumentException("Doğum tarihi desteklenen tarih aralığının dışında.");
      if (personel.DogumTarihi.HasValue && (personel.DogumTarihi.Value.Date > DateTime.Today || personel.DogumTarihi.Value.Date > personel.IseGirisTarihi.Date))
        throw new ArgumentException("Doğum tarihi bugünden veya işe giriş tarihinden sonra olamaz.");
    }

    internal static void ValidateAssignment(Personel personel, SqlTransaction transaction)
    {
      // Seçim formdan sonra pasife alınmış olabilir. Aktif kayıtlar aynı
      // transaction içinde kilitlenir ve pozisyonun departmanı doğrulanır.
      using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.Departman d WITH (HOLDLOCK)
        JOIN dbo.Pozisyon p WITH (HOLDLOCK) ON p.DepartmanId=d.DepartmanId
        JOIN dbo.Vardiya v WITH (HOLDLOCK) ON v.VardiyaId=@Vardiya
        WHERE d.DepartmanId=@Departman AND p.PozisyonId=@Pozisyon
          AND d.AktifMi=1 AND p.AktifMi=1 AND v.AktifMi=1", SqlDBConnection.GetConnection(), transaction))
      {
        cmd.Parameters.AddWithValue("@Departman", personel.DepartmanId);
        cmd.Parameters.AddWithValue("@Pozisyon", personel.PozisyonId);
        cmd.Parameters.AddWithValue("@Vardiya", personel.VardiyaId);
        if (Convert.ToInt32(cmd.ExecuteScalar()) != 1)
          throw new InvalidOperationException("Aktif departman, ona bağlı pozisyon ve aktif vardiya seçilmelidir.");
      }
    }

    internal static void ValidateShift(Vardiya vardiya)
    {
      if (vardiya == null)
        throw new ArgumentNullException(nameof(vardiya));
      if (string.IsNullOrWhiteSpace(vardiya.VardiyaAdi) || vardiya.VardiyaAdi.Trim().Length > 100)
        throw new ArgumentException("Vardiya adı zorunludur ve en fazla 100 karakter olabilir.");
      if (vardiya.BaslangicSaati < TimeSpan.Zero || vardiya.BaslangicSaati >= TimeSpan.FromDays(1) || vardiya.BitisSaati < TimeSpan.Zero || vardiya.BitisSaati >= TimeSpan.FromDays(1) || vardiya.BaslangicSaati == vardiya.BitisSaati)
        throw new ArgumentException("Vardiya başlangıç ve bitiş saatleri geçerli ve farklı olmalıdır.");
      TimeSpan sure = vardiya.BitisSaati - vardiya.BaslangicSaati;
      if (sure < TimeSpan.Zero)
        sure += TimeSpan.FromDays(1);
      if (vardiya.MolaDakika < 0 || vardiya.MolaDakika >= sure.TotalMinutes)
        throw new ArgumentException("Mola süresi vardiya süresinden kısa olmalıdır.");
      if (!(vardiya.Pazartesi || vardiya.Sali || vardiya.Carsamba || vardiya.Persembe || vardiya.Cuma || vardiya.Cumartesi || vardiya.Pazar))
        throw new ArgumentException("En az bir çalışma günü seçilmelidir.");
    }

    private static void CheckLength(string value, int max, string field)
    {
      if ((value ?? "").Length > max)
        throw new ArgumentException(field + " en fazla " + max + " karakter olabilir.");
    }
  }
}
