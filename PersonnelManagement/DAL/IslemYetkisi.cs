using System;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public static class IslemYetkisi
  {
    public static void AdminDogrula(int id, SqlTransaction transaction = null)
    {
      // Oturum önbelleği yerine güncel SQL kaydı, açık ekranlardaki rol değişimini de yakalar.
      using (var cmd = new SqlCommand("SELECT Rol,AktifMi FROM dbo.Kullanici WITH (HOLDLOCK) WHERE KullaniciId=@Id", SqlDBConnection.GetConnection(), transaction))
      {
        cmd.Parameters.AddWithValue("@Id", id);
        using (var r = cmd.ExecuteReader())
          if (!r.Read() || !Convert.ToBoolean(r[1]) || !string.Equals(r[0].ToString().Trim(), "Admin", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Bu işlem için aktif admin yetkisi gerekiyor.");
      }
    }
  }
}
