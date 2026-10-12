using System;
using System.Data;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public static class SistemLogger
  {
    private static int? _aktifKullaniciId;
    public static void SetKullanici(int kullaniciId) => _aktifKullaniciId = kullaniciId;
    public static void ClearKullanici() => _aktifKullaniciId = null;
    public static void Log(string islem, string tabloAdi = null, int? etkilenenId = null, string aciklama = null, SqlTransaction transaction = null, int? kullaniciId = null)
    {
      // Log hatası çağırana ulaşır; varsa veri değişikliği aynı transaction ile geri alınır.
      using (var cmd = new SqlCommand(@"INSERT INTO dbo.SistemLog
        (KullaniciId, Islem, TabloAdi, EtkilenenId, Aciklama)
        VALUES (@KullaniciId, @Islem, @TabloAdi, @EtkilenenId, @Aciklama)", SqlDBConnection.GetConnection(), transaction))
      {
        cmd.Parameters.Add("@KullaniciId", SqlDbType.Int).Value = (object)(kullaniciId ?? _aktifKullaniciId) ?? DBNull.Value;
        cmd.Parameters.AddWithValue("@Islem", islem);
        cmd.Parameters.AddWithValue("@TabloAdi", (object)tabloAdi ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EtkilenenId", (object)etkilenenId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Aciklama", (object)aciklama ?? DBNull.Value);
        cmd.ExecuteNonQuery();
      }
    }
  }
}
