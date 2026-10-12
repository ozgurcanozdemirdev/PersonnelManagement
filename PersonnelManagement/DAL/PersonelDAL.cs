using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace PersonnelManagement.DAL
{

  public class PersonelDal
  {
    public List<Personel> TumunuGetir(bool sadeceAktif = true)
    {
      var liste = new List<Personel>();
      string sql = sadeceAktif
          ? "SELECT * FROM vwPersonelDetay WHERE AktifMi = 1 ORDER BY Ad, Soyad"
          : "SELECT * FROM vwPersonelDetay ORDER BY Ad, Soyad";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          liste.Add(MapPersonel(r));
      return liste;
    }

    public Personel IdIleGetir(int personelId)
    {
      string sql = "SELECT * FROM vwPersonelDetay WHERE PersonelId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", personelId);
      using (var r = cmd.ExecuteReader())
        return r.Read() ? MapPersonel(r) : null;
    }

    public List<Personel> Ara(string aramaMetni)
    {
      var liste = new List<Personel>();
      string sql = @"
                SELECT * FROM vwPersonelDetay
                WHERE  AktifMi = 1
                  AND (Ad                  LIKE @Arama
                    OR Soyad               LIKE @Arama
                    OR TcKimlikNo          LIKE @Arama
                    OR (Ad + ' ' + Soyad)  LIKE @Arama)
                ORDER BY Ad, Soyad";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Arama", $"%{aramaMetni}%");
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          liste.Add(MapPersonel(r));
      return liste;
    }

    public int Kaydet(Personel p)
    {
      string sql = @"
                INSERT INTO Personel
                    (TcKimlikNo, Ad, Soyad, Cinsiyet, DogumTarihi,
                     Telefon, Email, Adres, FotoPath,
                     DepartmanId, PozisyonId, VardiyaId,
                     IseGirisTarihi, YillikIzinHakki)
                VALUES
                    (@Tc, @Ad, @Soyad, @Cinsiyet, @Dogum,
                     @Tel, @Email, @Adres, @Foto,
                     @DepId, @PozId, @VarId,
                     @IseGiris, @IzinHakki);
                SELECT SCOPE_IDENTITY();";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      SetPersonelParams(cmd, p);
      int yeniId = Convert.ToInt32(cmd.ExecuteScalar());
      SistemLogger.Log("Insert", "Personel", yeniId, p.AdSoyad);
      return yeniId;
    }

    public bool Guncelle(Personel p)
    {
      string sql = @"
                UPDATE Personel SET
                    TcKimlikNo       = @Tc,
                    Ad               = @Ad,
                    Soyad            = @Soyad,
                    Cinsiyet         = @Cinsiyet,
                    DogumTarihi      = @Dogum,
                    Telefon          = @Tel,
                    Email            = @Email,
                    Adres            = @Adres,
                    FotoPath         = @Foto,
                    DepartmanId      = @DepId,
                    PozisyonId       = @PozId,
                    VardiyaId        = @VarId,
                    IseGirisTarihi   = @IseGiris,
                    YillikIzinHakki  = @IzinHakki,
                    GuncellemeTarihi = GETDATE()
                WHERE PersonelId = @Id";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      SetPersonelParams(cmd, p);
      cmd.Parameters.AddWithValue("@Id", p.PersonelId);
      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok) SistemLogger.Log("Update", "Personel", p.PersonelId, p.AdSoyad);
      return ok;
    }

    public bool Sil(int personelId, DateTime istenCikisTarihi)
    {
      string sql = @"
                UPDATE Personel SET
                    AktifMi          = 0,
                    IstenCikisTarihi = @Cikis,
                    GuncellemeTarihi = GETDATE()
                WHERE PersonelId = @Id";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Cikis", istenCikisTarihi);
      cmd.Parameters.AddWithValue("@Id", personelId);
      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok) SistemLogger.Log("Delete", "Personel", personelId, "Pasife alındı");
      return ok;
    }

    public bool TcKimlikMevcutMu(string tcKimlik, int haricPersonelId = 0)
    {
      string sql = @"
                SELECT COUNT(1) FROM Personel
                WHERE TcKimlikNo = @Tc AND PersonelId <> @HaricId";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Tc", tcKimlik);
      cmd.Parameters.AddWithValue("@HaricId", haricPersonelId);
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    private void SetPersonelParams(SqlCommand cmd, Personel p)
    {
      cmd.Parameters.AddWithValue("@Tc", p.TcKimlikNo);
      cmd.Parameters.AddWithValue("@Ad", p.Ad);
      cmd.Parameters.AddWithValue("@Soyad", p.Soyad);
      cmd.Parameters.AddWithValue("@Cinsiyet", (object)p.Cinsiyet ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Dogum", (object)p.DogumTarihi ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Tel", (object)p.Telefon ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Email", (object)p.Email ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Adres", (object)p.Adres ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Foto", (object)p.FotoPath ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@DepId", p.DepartmanId);
      cmd.Parameters.AddWithValue("@PozId", p.PozisyonId);
      cmd.Parameters.AddWithValue("@VarId", p.VardiyaId);
      cmd.Parameters.AddWithValue("@IseGiris", p.IseGirisTarihi);
      cmd.Parameters.AddWithValue("@IzinHakki", p.YillikIzinHakki);
    }

    private Personel MapPersonel(SqlDataReader r) => new Personel
    {
      PersonelId = Convert.ToInt32(r["PersonelId"]),
      TcKimlikNo = r["TcKimlikNo"].ToString(),
      Ad = r["Ad"].ToString(),
      Soyad = r["Soyad"].ToString(),
      Cinsiyet = r["Cinsiyet"] == DBNull.Value ? null : r["Cinsiyet"].ToString(),
      DogumTarihi = r["DogumTarihi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["DogumTarihi"]),
      Telefon = r["Telefon"] == DBNull.Value ? null : r["Telefon"].ToString(),
      Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
      Adres = r["Adres"] == DBNull.Value ? null : r["Adres"].ToString(),
      FotoPath = r["FotoPath"] == DBNull.Value ? null : r["FotoPath"].ToString(),
      DepartmanId = Convert.ToInt32(r["DepartmanId"]),
      DepartmanAdi = r["DepartmanAdi"].ToString(),
      PozisyonId = Convert.ToInt32(r["PozisyonId"]),
      PozisyonAdi = r["PozisyonAdi"].ToString(),
      VardiyaId = Convert.ToInt32(r["VardiyaId"]),
      VardiyaAdi = r["VardiyaAdi"].ToString(),
      BaslangicSaati = (TimeSpan)r["BaslangicSaati"],
      BitisSaati = (TimeSpan)r["BitisSaati"],
      MolaDakika = Convert.ToInt32(r["MolaDakika"]),
      IseGirisTarihi = Convert.ToDateTime(r["IseGirisTarihi"]),
      IstenCikisTarihi = r["IstenCikisTarihi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["IstenCikisTarihi"]),
      YillikIzinHakki = Convert.ToInt32(r["YillikIzinHakki"]),
      AktifMi = Convert.ToBoolean(r["AktifMi"]),
      OlusturmaTarihi = Convert.ToDateTime(r["OlusturmaTarihi"]),
      GuncellemeTarihi = r["GuncellemeTarihi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["GuncellemeTarihi"])
    };
  }
}