
using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public class GirisKayitDAL
  {
    public int GirisYap(int personelId, int? kaydedenId = null)
    {
      if (BugunkuGirisVarMi(personelId))
        throw new InvalidOperationException("Bu personel bugün zaten giriş yapmış.");

      bool gecGeldiMi = GecGeldiMiHesapla(personelId, DateTime.Now.TimeOfDay);

      string sql = @"
                INSERT INTO GirisKayit
                    (PersonelId, KayitTarihi, GirisSaati, GirisZamani, GecGeldiMi, KaydedenId)
                VALUES
                    (@PersonelId, @KayitTarihi, @GirisSaati, @GirisZamani, @GecGeldiMi, @KaydedenId);
                SELECT SCOPE_IDENTITY();";

      var now = DateTime.Now;
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@PersonelId", personelId);
      cmd.Parameters.AddWithValue("@KayitTarihi", now.Date);
      cmd.Parameters.AddWithValue("@GirisSaati", now.ToString("HH:mm:ss"));
      cmd.Parameters.AddWithValue("@GirisZamani", now);
      cmd.Parameters.AddWithValue("@GecGeldiMi", gecGeldiMi);
      cmd.Parameters.AddWithValue("@KaydedenId", (object)kaydedenId ?? DBNull.Value);

      int yeniId = Convert.ToInt32(cmd.ExecuteScalar());
      SistemLogger.Log("Giris", "GirisKayit", yeniId,
          $"PersonelId:{personelId} - {now:dd.MM.yyyy HH:mm}");
      return yeniId;
    }

    public bool CikisYap(int personelId, int? kaydedenId = null)
    {
      int? kayitId = BugunkuAcikKayitId(personelId);
      if (!kayitId.HasValue)
        throw new InvalidOperationException("Bu personelin açık bir giriş kaydı bulunamadı.");

      var now = DateTime.Now;
      var sure = CalismaSuresiHesapla(personelId, kayitId.Value, now);
      bool erkenCiktiMi = ErkenCiktiMiHesapla(personelId, now.TimeOfDay);

      string sql = @"
                UPDATE GirisKayit SET
                    CikisSaati   = @CikisSaati,
                    CikisZamani  = @CikisZamani,
                    CalismaDakika= @CalismaDakika,
                    MesaiDakika  = @MesaiDakika,
                    ErkenCiktiMi = @ErkenCiktiMi,
                    KaydedenId   = ISNULL(KaydedenId, @KaydedenId)
                WHERE GirisKayitId = @KayitId";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@CikisSaati", now.ToString("HH:mm:ss"));
      cmd.Parameters.AddWithValue("@CikisZamani", now);
      cmd.Parameters.AddWithValue("@CalismaDakika", sure.calismaDakika);
      cmd.Parameters.AddWithValue("@MesaiDakika", sure.mesaiDakika);
      cmd.Parameters.AddWithValue("@ErkenCiktiMi", erkenCiktiMi);
      cmd.Parameters.AddWithValue("@KaydedenId", (object)kaydedenId ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@KayitId", kayitId.Value);

      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok)
        SistemLogger.Log("Cikis", "GirisKayit", kayitId.Value,
            $"PersonelId:{personelId} - {now:dd.MM.yyyy HH:mm}");
      return ok;
    }

    public List<GirisKayit> BugunkuKayitlar()
        => TarihAraligindaGetir(DateTime.Today, DateTime.Today);

    public List<GirisKayit> TarihAraligindaGetir(
        DateTime baslangic, DateTime bitis, int? personelId = null)
    {
      var liste = new List<GirisKayit>();
      string sql = @"
                SELECT * FROM vwGunlukOzet
                WHERE  KayitTarihi BETWEEN @Bas AND @Bit
                " + (personelId.HasValue ? "AND PersonelId = @Pid " : "") +
          "ORDER BY KayitTarihi DESC, AdSoyad";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Bas", baslangic.Date);
      cmd.Parameters.AddWithValue("@Bit", bitis.Date);
      if (personelId.HasValue)
        cmd.Parameters.AddWithValue("@Pid", personelId.Value);

      using (var r = cmd.ExecuteReader())
        while (r.Read())
          liste.Add(MapGirisKayit(r));
      return liste;
    }

    public bool BugunkuGirisVarMi(int personelId)
    {
      string sql = @"
                SELECT COUNT(1) FROM GirisKayit
                WHERE PersonelId = @Pid AND KayitTarihi = @Bugun";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Pid", personelId);
      cmd.Parameters.AddWithValue("@Bugun", DateTime.Today);
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public bool BugunkuCikisVarMi(int personelId)
    {
      string sql = @"
                SELECT COUNT(1) FROM GirisKayit
                WHERE PersonelId  = @Pid
                  AND KayitTarihi = @Bugun
                  AND CikisSaati  IS NOT NULL";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Pid", personelId);
      cmd.Parameters.AddWithValue("@Bugun", DateTime.Today);
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    // ── Yardımcılar ───────────────────────────────────────
    private int? BugunkuAcikKayitId(int personelId)
    {
      string sql = @"
                SELECT TOP 1 GirisKayitId FROM GirisKayit
                WHERE PersonelId  = @Pid
                  AND KayitTarihi = @Bugun
                  AND CikisSaati  IS NULL";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Pid", personelId);
      cmd.Parameters.AddWithValue("@Bugun", DateTime.Today);
      var res = cmd.ExecuteScalar();
      return res == null || res == DBNull.Value ? (int?)null : Convert.ToInt32(res);
    }

    private bool GecGeldiMiHesapla(int personelId, TimeSpan girisSaati)
    {
      string sql = @"
                SELECT v.BaslangicSaati FROM Vardiya v
                INNER JOIN Personel p ON p.VardiyaId = v.VardiyaId
                WHERE p.PersonelId = @Pid";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Pid", personelId);
      var res = cmd.ExecuteScalar();
      if (res == null || res == DBNull.Value) return false;
      var planlanan = (TimeSpan)res;
      return girisSaati > planlanan.Add(TimeSpan.FromMinutes(5));
    }

    private bool ErkenCiktiMiHesapla(int personelId, TimeSpan cikisSaati)
    {
      string sql = @"
                SELECT v.BitisSaati FROM Vardiya v
                INNER JOIN Personel p ON p.VardiyaId = v.VardiyaId
                WHERE p.PersonelId = @Pid";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Pid", personelId);
      var res = cmd.ExecuteScalar();
      if (res == null || res == DBNull.Value) return false;
      var planlanan = (TimeSpan)res;
      return cikisSaati < planlanan.Subtract(TimeSpan.FromMinutes(5));
    }

    private (int calismaDakika, int mesaiDakika) CalismaSuresiHesapla(
        int personelId, int kayitId, DateTime cikisZamani)
    {
      string sql = "SELECT GirisZamani FROM GirisKayit WHERE GirisKayitId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", kayitId);
      var girisObj = cmd.ExecuteScalar();
      if (girisObj == null || girisObj == DBNull.Value) return (0, 0);

      var giris = Convert.ToDateTime(girisObj);
      int toplamDakika = (int)(cikisZamani - giris).TotalMinutes;

      string sqlMola = @"
                SELECT v.MolaDakika FROM Vardiya v
                INNER JOIN Personel p ON p.VardiyaId = v.VardiyaId
                WHERE p.PersonelId = @Pid";
      var cmdMola = new SqlCommand(sqlMola, SqlDBConnection.GetConnection());
      cmdMola.Parameters.AddWithValue("@Pid", personelId);
      int molaDakika = Convert.ToInt32(cmdMola.ExecuteScalar());

      int calismaDakika = Math.Max(0, toplamDakika - molaDakika);

      string sqlVardiya = @"
                SELECT v.BaslangicSaati, v.BitisSaati FROM Vardiya v
                INNER JOIN Personel p ON p.VardiyaId = v.VardiyaId
                WHERE p.PersonelId = @Pid";
      var cmdV = new SqlCommand(sqlVardiya, SqlDBConnection.GetConnection());
      cmdV.Parameters.AddWithValue("@Pid", personelId);
      using (var r = cmdV.ExecuteReader())
      {
        if (r.Read())
        {
          var bas = (TimeSpan)r["BaslangicSaati"];
          var bit = (TimeSpan)r["BitisSaati"];
          int normalDakika = (int)(bit - bas).TotalMinutes - molaDakika;
          if (normalDakika < 0) normalDakika += 24 * 60;
          int mesaiDakika = Math.Max(0, calismaDakika - normalDakika);
          return (calismaDakika, mesaiDakika);
        }
      }
      return (calismaDakika, 0);
    }

    private GirisKayit MapGirisKayit(SqlDataReader r) => new GirisKayit
    {
      GirisKayitId = Convert.ToInt32(r["GirisKayitId"]),
      PersonelId = Convert.ToInt32(r["PersonelId"]),
      AdSoyad = r["AdSoyad"].ToString(),
      DepartmanAdi = r["DepartmanAdi"].ToString(),
      VardiyaAdi = r["VardiyaAdi"].ToString(),
      KayitTarihi = Convert.ToDateTime(r["KayitTarihi"]),
      GirisSaati = r["GirisSaati"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["GirisSaati"],
      CikisSaati = r["CikisSaati"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["CikisSaati"],
      GirisZamani = r["GirisZamani"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["GirisZamani"]),
      CikisZamani = r["CikisZamani"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["CikisZamani"]),
      CalismaDakika = r["CalismaDakika"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["CalismaDakika"]),
      MesaiDakika = r["MesaiDakika"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MesaiDakika"]),
      GecGeldiMi = Convert.ToBoolean(r["GecGeldiMi"]),
      ErkenCiktiMi = Convert.ToBoolean(r["ErkenCiktiMi"])
    };
  }
}