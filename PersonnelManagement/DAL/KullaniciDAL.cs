using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public class KullaniciDal
  {
    public Kullanici GirisDogrula(string kullaniciAdi, string sifre)
    {
      SistemLogger.ClearKullanici();
      if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrEmpty(sifre))
        return null;
      Kullanici kullanici = DatabaseTransaction.Execute(tx =>
      {
        Kullanici aday;
        using (var cmd = new SqlCommand(@"SELECT KullaniciId,KullaniciAdi,Ad,Soyad,Email,Rol,AktifMi,SonGirisTarihi,Sifre
          FROM dbo.Kullanici WITH (UPDLOCK,HOLDLOCK) WHERE KullaniciAdi=@Ad AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Ad", kullaniciAdi.Trim());
          using (var reader = cmd.ExecuteReader())
          {
            // SQL'in harf büyüklüğünü önemsemeyen karşılaştırması parolaya uygulanmaz.
            // Bu sürümün mevcut parola formatı korunur; birebir eşleşme aranır.
            if (!reader.Read() || !string.Equals(reader["Sifre"].ToString(), sifre, StringComparison.Ordinal))
              return null;
            aday = MapKullanici(reader);
          }
        }

        using (var cmd = new SqlCommand(@"UPDATE dbo.Kullanici SET SonGirisTarihi=GETDATE()
          OUTPUT INSERTED.SonGirisTarihi WHERE KullaniciId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", aday.KullaniciId);
          object tarih = cmd.ExecuteScalar();
          if (tarih == null)
            throw new InvalidOperationException("Kullanıcı artık aktif değil.");
          aday.SonGirisTarihi = Convert.ToDateTime(tarih);
        }

        SistemLogger.Log("Login", "Kullanici", aday.KullaniciId, "Başarılı giriş", tx, aday.KullaniciId);
        return aday;
      });
      // Başarısız transaction oturum açmış bir kullanıcı bırakmaz.
      if (kullanici != null)
        SistemLogger.SetKullanici(kullanici.KullaniciId);
      return kullanici;
    }

    public List<Kullanici> TumunuGetir()
    {
      var liste = new List<Kullanici>();
      using (var cmd = new SqlCommand("SELECT KullaniciId,KullaniciAdi,Ad,Soyad,Email,Rol,AktifMi,SonGirisTarihi FROM dbo.Kullanici ORDER BY Ad,Soyad", SqlDBConnection.GetConnection()))
      using (var reader = cmd.ExecuteReader())
        while (reader.Read())
          liste.Add(MapKullanici(reader));
      return liste;
    }

    public bool Kaydet(Kullanici kullanici, string sifre)
    {
      if (kullanici == null || string.IsNullOrWhiteSpace(kullanici.KullaniciAdi) || kullanici.KullaniciAdi.Trim().Length > 50 || string.IsNullOrWhiteSpace(kullanici.Ad) || kullanici.Ad.Trim().Length > 50 || string.IsNullOrWhiteSpace(kullanici.Soyad) || kullanici.Soyad.Trim().Length > 50 || string.IsNullOrWhiteSpace(kullanici.Rol) || kullanici.Rol.Length > 20 || (kullanici.Email ?? "").Length > 100)
        throw new ArgumentException("Kullanıcı bilgileri eksik veya alan uzunlukları geçersiz.");
      ValidatePassword(sifre);
      return DatabaseTransaction.Execute(tx =>
      {
        using (var cmd = new SqlCommand(@"INSERT dbo.Kullanici(KullaniciAdi,Sifre,Ad,Soyad,Email,Rol)
          VALUES(@KullaniciAdi,@Sifre,@Ad,@Soyad,@Email,@Rol); SELECT CAST(SCOPE_IDENTITY() AS int);", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@KullaniciAdi", kullanici.KullaniciAdi.Trim());
          cmd.Parameters.AddWithValue("@Sifre", sifre);
          cmd.Parameters.AddWithValue("@Ad", kullanici.Ad.Trim());
          cmd.Parameters.AddWithValue("@Soyad", kullanici.Soyad.Trim());
          cmd.Parameters.AddWithValue("@Email", (object)kullanici.Email ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Rol", kullanici.Rol.Trim());
          int id = Convert.ToInt32(cmd.ExecuteScalar());
          SistemLogger.Log("Insert", "Kullanici", id, kullanici.KullaniciAdi, transaction: tx);
          return true;
        }
      });
    }

    public bool SifreDegistir(int kullaniciId, string yeniSifre, int? islemiYapanId = null)
    {
      ValidatePassword(yeniSifre);
      return DatabaseTransaction.Execute(tx =>
      {
        if (islemiYapanId.HasValue)
          IslemYetkisi.AdminDogrula(islemiYapanId.Value, tx);
        using (var cmd = new SqlCommand("UPDATE dbo.Kullanici SET Sifre=@Sifre WHERE KullaniciId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Sifre", yeniSifre);
          cmd.Parameters.AddWithValue("@Id", kullaniciId);
          bool ok = cmd.ExecuteNonQuery() == 1;
          if (ok)
            SistemLogger.Log("Update", "Kullanici", kullaniciId, "Parola değiştirildi", tx, islemiYapanId);
          return ok;
        }
      });
    }

    private static void ValidatePassword(string sifre)
    {
      if (string.IsNullOrWhiteSpace(sifre) || sifre.Length > 100)
        throw new ArgumentException("Parola boş olamaz ve en fazla 100 karakter olabilir.");
    }

    private Kullanici MapKullanici(SqlDataReader reader) => new Kullanici
    {
      KullaniciId = Convert.ToInt32(reader["KullaniciId"]),
      KullaniciAdi = reader["KullaniciAdi"].ToString(),
      Ad = reader["Ad"].ToString(),
      Soyad = reader["Soyad"].ToString(),
      Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
      Rol = reader["Rol"].ToString().Trim(),
      AktifMi = Convert.ToBoolean(reader["AktifMi"]),
      SonGirisTarihi = reader["SonGirisTarihi"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(reader["SonGirisTarihi"])
    };
  }
}
