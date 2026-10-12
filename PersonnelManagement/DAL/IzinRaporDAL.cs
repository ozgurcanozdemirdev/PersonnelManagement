using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public class IzinTuruDAL
  {
    public class IzinTuruDal
    {
      public List<IzinTuru> TumunuGetir()
      {
        var liste = new List<IzinTuru>();
        string sql = "SELECT * FROM IzinTuru WHERE AktifMi = 1 ORDER BY TuruAdi";
        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(new IzinTuru
            {
              IzinTuruId = Convert.ToInt32(r["IzinTuruId"]),
              TuruAdi = r["TuruAdi"].ToString(),
              Ucretli = Convert.ToBoolean(r["Ucretli"]),
              AktifMi = Convert.ToBoolean(r["AktifMi"])
            });
        return liste;
      }
    }

    public class IzinDal
    {
      public List<Izin> PersonelIzinleri(int personelId, int? yil = null)
      {
        var liste = new List<Izin>();
        string sql = @"
                SELECT i.*, p.Ad + ' ' + p.Soyad AS PersonelAdSoyad, it.TuruAdi AS IzinTuruAdi
                FROM   Izin     i
                INNER JOIN Personel  p  ON i.PersonelId  = p.PersonelId
                INNER JOIN IzinTuru  it ON i.IzinTuruId  = it.IzinTuruId
                WHERE  i.PersonelId = @Pid
                " + (yil.HasValue ? "AND YEAR(i.BaslangicTarihi) = @Yil " : "") +
            "ORDER BY i.BaslangicTarihi DESC";

        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Pid", personelId);
        if (yil.HasValue) cmd.Parameters.AddWithValue("@Yil", yil.Value);

        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(MapIzin(r));
        return liste;
      }

      public List<Izin> BekleyenIzinler()
      {
        var liste = new List<Izin>();
        string sql = @"
                SELECT i.*, p.Ad + ' ' + p.Soyad AS PersonelAdSoyad, it.TuruAdi AS IzinTuruAdi
                FROM   Izin     i
                INNER JOIN Personel  p  ON i.PersonelId = p.PersonelId
                INNER JOIN IzinTuru  it ON i.IzinTuruId = it.IzinTuruId
                WHERE  i.Durum = 'Beklemede'
                ORDER BY i.OlusturmaTarihi";

        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(MapIzin(r));
        return liste;
      }

      public int Kaydet(Izin izin)
      {
        if (IzinCakisiyorMu(izin.PersonelId, izin.BaslangicTarihi, izin.BitisTarihi))
          throw new InvalidOperationException("Bu tarihler arasında zaten onaylı bir izin kaydı mevcut.");

        izin.ToplamGun = (izin.BitisTarihi - izin.BaslangicTarihi).Days + 1;

        string sql = @"
                INSERT INTO Izin
                    (PersonelId, IzinTuruId, BaslangicTarihi, BitisTarihi, ToplamGun, Aciklama)
                VALUES
                    (@Pid, @TurId, @Bas, @Bit, @Gun, @Aciklama);
                SELECT SCOPE_IDENTITY();";

        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Pid", izin.PersonelId);
        cmd.Parameters.AddWithValue("@TurId", izin.IzinTuruId);
        cmd.Parameters.AddWithValue("@Bas", izin.BaslangicTarihi);
        cmd.Parameters.AddWithValue("@Bit", izin.BitisTarihi);
        cmd.Parameters.AddWithValue("@Gun", izin.ToplamGun);
        cmd.Parameters.AddWithValue("@Aciklama", (object)izin.Aciklama ?? DBNull.Value);

        int yeniId = Convert.ToInt32(cmd.ExecuteScalar());
        SistemLogger.Log("Insert", "Izin", yeniId, $"PersonelId:{izin.PersonelId}");
        return yeniId;
      }

      public bool Onayla(int izinId, int onayanKullaniciId)
      {
        string sql = @"
                UPDATE Izin SET
                    Durum              = 'Onaylı',
                    OnayanKullaniciId  = @OnayanId,
                    OnayTarihi         = GETDATE()
                WHERE IzinId = @Id";
        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@OnayanId", onayanKullaniciId);
        cmd.Parameters.AddWithValue("@Id", izinId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok) SistemLogger.Log("Onay", "Izin", izinId, "Onaylandı");
        return ok;
      }

      public bool Reddet(int izinId, int onayanKullaniciId, string redNedeni)
      {
        string sql = @"
                UPDATE Izin SET
                    Durum              = 'Reddedildi',
                    OnayanKullaniciId  = @OnayanId,
                    OnayTarihi         = GETDATE(),
                    RedNedeni          = @RedNedeni
                WHERE IzinId = @Id";
        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@OnayanId", onayanKullaniciId);
        cmd.Parameters.AddWithValue("@RedNedeni", redNedeni);
        cmd.Parameters.AddWithValue("@Id", izinId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok) SistemLogger.Log("Red", "Izin", izinId, redNedeni);
        return ok;
      }

      public bool Iptal(int izinId)
      {
        string sql = "UPDATE Izin SET Durum = 'Reddedildi' WHERE IzinId = @Id";
        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Id", izinId);
        return cmd.ExecuteNonQuery() > 0;
      }

      public int KalanYillikIzin(int personelId)
      {
        string sql = "SELECT KalanYillikIzin FROM vwIzinOzet WHERE PersonelId = @Pid";
        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Pid", personelId);
        var res = cmd.ExecuteScalar();
        return res == null || res == DBNull.Value ? 0 : Convert.ToInt32(res);
      }

      private bool IzinCakisiyorMu(int personelId, DateTime bas, DateTime bit, int haricIzinId = 0)
      {
        string sql = @"
                SELECT COUNT(1) FROM Izin
                WHERE PersonelId = @Pid
                  AND IzinId     <> @HaricId
                  AND Durum      = 'Onaylı'
                  AND NOT (BitisTarihi < @Bas OR BaslangicTarihi > @Bit)";
        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Pid", personelId);
        cmd.Parameters.AddWithValue("@HaricId", haricIzinId);
        cmd.Parameters.AddWithValue("@Bas", bas);
        cmd.Parameters.AddWithValue("@Bit", bit);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
      }

      private Izin MapIzin(SqlDataReader r) => new Izin
      {
        IzinId = Convert.ToInt32(r["IzinId"]),
        PersonelId = Convert.ToInt32(r["PersonelId"]),
        PersonelAdSoyad = r["PersonelAdSoyad"].ToString(),
        IzinTuruId = Convert.ToInt32(r["IzinTuruId"]),
        IzinTuruAdi = r["IzinTuruAdi"].ToString(),
        BaslangicTarihi = Convert.ToDateTime(r["BaslangicTarihi"]),
        BitisTarihi = Convert.ToDateTime(r["BitisTarihi"]),
        ToplamGun = Convert.ToInt32(r["ToplamGun"]),
        Aciklama = r["Aciklama"] == DBNull.Value ? null : r["Aciklama"].ToString(),
        Durum = r["Durum"].ToString(),
        RedNedeni = r["RedNedeni"] == DBNull.Value ? null : r["RedNedeni"].ToString(),
        OnayanKullaniciId = r["OnayanKullaniciId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["OnayanKullaniciId"]),
        OnayTarihi = r["OnayTarihi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["OnayTarihi"]),
        OlusturmaTarihi = Convert.ToDateTime(r["OlusturmaTarihi"])
      };
    }

    public class RaporDal
    {
      public List<PersonelRapor> PersonelOzetRaporu(
          DateTime baslangic, DateTime bitis, int? personelId = null)
      {
        var liste = new List<PersonelRapor>();
        string sql = @"
                SELECT
                    p.PersonelId,
                    p.Ad + ' ' + p.Soyad                                        AS AdSoyad,
                    d.DepartmanAdi,
                    COUNT(g.GirisKayitId)                                       AS ToplamCalismaGun,
                    ISNULL(SUM(g.CalismaDakika), 0)                             AS ToplamCalismaDakika,
                    ISNULL(SUM(g.MesaiDakika),   0)                             AS ToplamMesaiDakika,
                    ISNULL(SUM(CAST(g.GecGeldiMi   AS INT)), 0)                 AS GecGelmeSayisi,
                    ISNULL(SUM(CAST(g.ErkenCiktiMi AS INT)), 0)                 AS ErkenCikisSayisi,
                    (SELECT COUNT(1) FROM Devamsizlik dv
                     WHERE  dv.PersonelId = p.PersonelId
                       AND  dv.DevamsizlikTarihi BETWEEN @Bas AND @Bit)         AS ToplamDevamsizlik,
                    (SELECT ISNULL(SUM(iz.ToplamGun), 0) FROM Izin iz
                     WHERE  iz.PersonelId = p.PersonelId
                       AND  iz.Durum      = 'Onaylı'
                       AND  iz.BaslangicTarihi BETWEEN @Bas AND @Bit)           AS ToplamIzinGun
                FROM       Personel    p
                INNER JOIN Departman   d ON p.DepartmanId = d.DepartmanId
                LEFT  JOIN GirisKayit  g ON p.PersonelId  = g.PersonelId
                                        AND g.KayitTarihi BETWEEN @Bas AND @Bit
                WHERE p.AktifMi = 1
                " + (personelId.HasValue ? "AND p.PersonelId = @Pid " : "") +
            "GROUP BY p.PersonelId, p.Ad, p.Soyad, d.DepartmanAdi " +
            "ORDER BY p.Ad, p.Soyad";

        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Bas", baslangic.Date);
        cmd.Parameters.AddWithValue("@Bit", bitis.Date);
        if (personelId.HasValue)
          cmd.Parameters.AddWithValue("@Pid", personelId.Value);

        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(new PersonelRapor
            {
              PersonelId = Convert.ToInt32(r["PersonelId"]),
              AdSoyad = r["AdSoyad"].ToString(),
              DepartmanAdi = r["DepartmanAdi"].ToString(),
              ToplamCalismaGun = Convert.ToInt32(r["ToplamCalismaGun"]),
              ToplamCalismaDakika = Convert.ToInt32(r["ToplamCalismaDakika"]),
              ToplamMesaiDakika = Convert.ToInt32(r["ToplamMesaiDakika"]),
              GecGelmeSayisi = Convert.ToInt32(r["GecGelmeSayisi"]),
              ErkenCikisSayisi = Convert.ToInt32(r["ErkenCikisSayisi"]),
              ToplamDevamsizlik = Convert.ToInt32(r["ToplamDevamsizlik"]),
              ToplamIzinGun = Convert.ToInt32(r["ToplamIzinGun"])
            });
        return liste;
      }

      public (int toplamPersonel, int bugunGiren, int bugunIzinli, int bugunDevamsiz) BugunOzet()
      {
        string sql = @"
                SELECT
                    (SELECT COUNT(1) FROM Personel WHERE AktifMi = 1)                  AS ToplamPersonel,
                    (SELECT COUNT(DISTINCT PersonelId) FROM GirisKayit
                     WHERE KayitTarihi = CAST(GETDATE() AS DATE))                      AS BugunGiren,
                    (SELECT COUNT(1) FROM Izin
                     WHERE Durum = 'Onaylı'
                       AND CAST(GETDATE() AS DATE) BETWEEN BaslangicTarihi
                                                       AND BitisTarihi)                AS BugunIzinli,
                    (SELECT COUNT(1) FROM Personel p WHERE p.AktifMi = 1
                       AND p.PersonelId NOT IN (
                           SELECT PersonelId FROM GirisKayit
                           WHERE KayitTarihi = CAST(GETDATE() AS DATE))
                       AND p.PersonelId NOT IN (
                           SELECT PersonelId FROM Izin
                           WHERE Durum = 'Onaylı'
                             AND CAST(GETDATE() AS DATE) BETWEEN BaslangicTarihi
                                                             AND BitisTarihi))         AS BugunDevamsiz";

        var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
        using (var r = cmd.ExecuteReader())
        {
          if (r.Read())
            return (
                Convert.ToInt32(r["ToplamPersonel"]),
                Convert.ToInt32(r["BugunGiren"]),
                Convert.ToInt32(r["BugunIzinli"]),
                Convert.ToInt32(r["BugunDevamsiz"])
            );
        }
        return (0, 0, 0, 0);
      }
    }
  }
}