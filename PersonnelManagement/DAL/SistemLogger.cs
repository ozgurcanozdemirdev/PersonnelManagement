using PersonnelManagement.DAL;
using System;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public static class SistemLogger
  {

    private static int? _aktifKullaniciId = null;

    public static void SetKullanici(int kullaniciId)
        => _aktifKullaniciId = kullaniciId;

    public static void Log(string islem, string tabloAdi = null, int? etkilenenId = null, string aciklama = null)
    {
      try
      {
        string sql = @" INSERT INTO SistemLog (KullaniciId, Islem, TabloAdi, EtkilenenId, Aciklama) VALUES (@KullaniciId, @Islem, @TabloAdi, @EtkilenenId,@Aciklama)";

        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@KullaniciId", (object)_aktifKullaniciId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Islem", islem);
        cmd.Parameters.AddWithValue("@TabloAdi", (object)tabloAdi ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EtkilenenId", (object)etkilenenId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Aciklama", (object)aciklama ?? DBNull.Value);
        cmd.ExecuteNonQuery();
      }
      catch { }
    }
  }
}


