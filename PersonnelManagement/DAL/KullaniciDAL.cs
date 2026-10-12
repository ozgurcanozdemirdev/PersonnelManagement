using PersonnelManagement.DAL;
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
      string sql = @"
                SELECT KullaniciId, KullaniciAdi, Ad, Soyad, Email, Rol, AktifMi, SonGirisTarihi
                FROM   Kullanici
                WHERE  KullaniciAdi = @KullaniciAdi
                  AND  Sifre        = @Sifre
                  AND  AktifMi      = 1";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
      cmd.Parameters.AddWithValue("@Sifre", sifre);

      using (var reader = cmd.ExecuteReader())
      {
        if (!reader.Read()) return null;
        var kullanici = MapKullanici(reader);
        reader.Close();

        SonGirisGuncelle(kullanici.KullaniciId);
        SistemLogger.SetKullanici(kullanici.KullaniciId);
        SistemLogger.Log("Login", "Kullanici", kullanici.KullaniciId, "Başarılı giriş");

        return kullanici;
      }
    }

    private void SonGirisGuncelle(int kullaniciId)
    {
      string sql = "UPDATE Kullanici SET SonGirisTarihi = GETDATE() WHERE KullaniciId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", kullaniciId);
      cmd.ExecuteNonQuery();
    }

    public List<Kullanici> TumunuGetir()
    {
      var liste = new List<Kullanici>();
      string sql = "SELECT * FROM Kullanici ORDER BY Ad, Soyad";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());

      using (var reader = cmd.ExecuteReader())
        while (reader.Read())
          liste.Add(MapKullanici(reader));

      return liste;
    }

    public bool Kaydet(Kullanici k, string sifre)
    {
      string sql = @"
                INSERT INTO Kullanici (KullaniciAdi, Sifre, Ad, Soyad, Email, Rol)
                VALUES (@KullaniciAdi, @Sifre, @Ad, @Soyad, @Email, @Rol)";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@KullaniciAdi", k.KullaniciAdi);
      cmd.Parameters.AddWithValue("@Sifre", sifre);
      cmd.Parameters.AddWithValue("@Ad", k.Ad);
      cmd.Parameters.AddWithValue("@Soyad", k.Soyad);
      cmd.Parameters.AddWithValue("@Email", (object)k.Email ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Rol", k.Rol);

      bool basarili = cmd.ExecuteNonQuery() > 0;
      if (basarili) SistemLogger.Log("Insert", "Kullanici", null, k.KullaniciAdi);
      return basarili;
    }

    public bool SifreDegistir(int kullaniciId, string yeniSifre)
    {
      string sql = "UPDATE Kullanici SET Sifre = @Sifre WHERE KullaniciId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Sifre", yeniSifre);
      cmd.Parameters.AddWithValue("@Id", kullaniciId);
      return cmd.ExecuteNonQuery() > 0;
    }

    private Kullanici MapKullanici(SqlDataReader r) => new Kullanici
    {
      KullaniciId = Convert.ToInt32(r["KullaniciId"]),
      KullaniciAdi = r["KullaniciAdi"].ToString(),
      Ad = r["Ad"].ToString(),
      Soyad = r["Soyad"].ToString(),
      Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
      Rol = r["Rol"].ToString(),
      AktifMi = Convert.ToBoolean(r["AktifMi"]),
      SonGirisTarihi = r["SonGirisTarihi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["SonGirisTarihi"])
    };
  }
}