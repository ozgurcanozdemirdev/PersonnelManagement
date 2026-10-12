using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public class GirisKayitDAL
  {
    public int GirisYap(int personelId, int? kaydedenId = null)
    {
      DateTime now = VeritabaniZamani(DateTime.Now);
      int yeniId;
      using (var tx = SqlDBConnection.GetConnection().BeginTransaction())
      {
        // Personel kilidi, farklı terminallerdeki giriş/çıkış işlemlerini sıraya alır.
        PersoneliKilitle(personelId, tx, true);
        TimeSpan bas, bit;
        int mola;
        string ad;
        using (var cmd = new SqlCommand(@"SELECT v.BaslangicSaati,v.BitisSaati,v.MolaDakika,v.VardiyaAdi
          FROM dbo.Personel p JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId
          WHERE p.PersonelId=@Pid AND v.AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Pid", personelId);
          using (var r = cmd.ExecuteReader())
          {
            if (!r.Read())
              throw new InvalidOperationException("Aktif vardiya bulunamadı.");
            bas = (TimeSpan)r[0];
            bit = (TimeSpan)r[1];
            mola = Convert.ToInt32(r[2]);
            ad = r[3].ToString();
          }
        }

        var plan = VardiyaPlani(now, bas, bit);
        if (mola < 0 || mola >= (plan.bitis - plan.baslangic).TotalMinutes)
          throw new InvalidOperationException("Mola süresi vardiya süresinden kısa olmalıdır.");
        using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.GirisKayit WITH (UPDLOCK,HOLDLOCK)
          WHERE PersonelId=@Pid AND (KayitTarihi=@Tarih OR CikisSaati IS NULL)", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Pid", personelId);
          cmd.Parameters.AddWithValue("@Tarih", plan.baslangic.Date);
          if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            throw new InvalidOperationException("Bu vardiya günü için giriş veya henüz kapatılmamış bir kayıt mevcut.");
        }

        // Vardiya planının kopyası tutulur; sonraki düzenlemeler bu kaydın hesabını değiştirmez.
        using (var cmd = new SqlCommand(@"INSERT dbo.GirisKayit
          (PersonelId,KayitTarihi,GirisSaati,GirisZamani,GecGeldiMi,KaydedenId,
           PlanlananGirisZamani,PlanlananCikisZamani,PlanlananMolaDakika,VardiyaAdiKayit)
          VALUES (@Pid,@Tarih,@Saat,@Now,@Gec,@Kullanici,@Bas,@Bit,@Mola,@Ad);
          SELECT CAST(SCOPE_IDENTITY() AS int);", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Pid", personelId);
          cmd.Parameters.AddWithValue("@Tarih", plan.baslangic.Date);
          cmd.Parameters.Add("@Saat", SqlDbType.Time).Value = now.TimeOfDay;
          cmd.Parameters.AddWithValue("@Now", now);
          cmd.Parameters.AddWithValue("@Gec", now > plan.baslangic.AddMinutes(5));
          cmd.Parameters.AddWithValue("@Kullanici", (object)kaydedenId ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Bas", plan.baslangic);
          cmd.Parameters.AddWithValue("@Bit", plan.bitis);
          cmd.Parameters.AddWithValue("@Mola", mola);
          cmd.Parameters.AddWithValue("@Ad", ad);
          yeniId = Convert.ToInt32(cmd.ExecuteScalar());
        }

        SistemLogger.Log("Giris", "GirisKayit", yeniId, $"PersonelId:{personelId} - {now:dd.MM.yyyy HH:mm}", transaction: tx);
        tx.Commit();
      }

      return yeniId;
    }

    public bool CikisYap(int personelId, int? kaydedenId = null)
    {
      DateTime now = VeritabaniZamani(DateTime.Now);
      int id;
      using (var tx = SqlDBConnection.GetConnection().BeginTransaction())
      {
        PersoneliKilitle(personelId, tx, false);
        DateTime giris, bas, bit;
        int mola;
        // Açık kayıt tarihle sınırlandırılmaz; gece yarısı önceki günün girişini kapatmayı engellemez.
        using (var cmd = new SqlCommand(@"SELECT TOP (1) GirisKayitId,GirisZamani,PlanlananGirisZamani,
          PlanlananCikisZamani,PlanlananMolaDakika FROM dbo.GirisKayit WITH (UPDLOCK,HOLDLOCK)
          WHERE PersonelId=@Pid AND CikisSaati IS NULL ORDER BY GirisZamani DESC,GirisKayitId DESC", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Pid", personelId);
          using (var r = cmd.ExecuteReader())
          {
            if (!r.Read())
              throw new InvalidOperationException("Bu personelin açık giriş kaydı bulunamadı.");
            if (r.IsDBNull(1) || r.IsDBNull(2) || r.IsDBNull(3) || r.IsDBNull(4))
              throw new InvalidOperationException("Giriş kaydının zaman/plan bilgileri eksik. Veritabanı güncellemesini kontrol edin.");
            id = r.GetInt32(0);
            giris = r.GetDateTime(1);
            bas = r.GetDateTime(2);
            bit = r.GetDateTime(3);
            mola = r.GetInt32(4);
          }
        }

        var sure = CalismaSuresi(giris, now, bas, bit, mola);
        using (var cmd = new SqlCommand(@"UPDATE dbo.GirisKayit SET CikisSaati=@Saat,CikisZamani=@Now,
          CalismaDakika=@Calisma,MesaiDakika=@Mesai,ErkenCiktiMi=@Erken,KaydedenId=ISNULL(KaydedenId,@Kullanici)
          WHERE GirisKayitId=@Id AND CikisSaati IS NULL", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.Add("@Saat", SqlDbType.Time).Value = now.TimeOfDay;
          cmd.Parameters.AddWithValue("@Now", now);
          cmd.Parameters.AddWithValue("@Calisma", sure.calismaDakika);
          cmd.Parameters.AddWithValue("@Mesai", sure.mesaiDakika);
          cmd.Parameters.AddWithValue("@Erken", now < bit.AddMinutes(-5));
          cmd.Parameters.AddWithValue("@Id", id);
          cmd.Parameters.AddWithValue("@Kullanici", (object)kaydedenId ?? DBNull.Value);
          if (cmd.ExecuteNonQuery() != 1)
            throw new InvalidOperationException("Giriş kaydı başka bir işlem tarafından kapatılmış.");
        }

        SistemLogger.Log("Cikis", "GirisKayit", id, $"PersonelId:{personelId} - {now:dd.MM.yyyy HH:mm}", transaction: tx);
        tx.Commit();
      }

      return true;
    }

    // SQL datetime milisaniyeyi yuvarlar. İki kayıtta da aynı hassasiyet
    // kullanılır; hemen yapılan çıkış, yuvarlanan girişten önce görünmez.
    public static DateTime VeritabaniZamani(DateTime zaman) => new System.Data.SqlTypes.SqlDateTime(zaman).Value;

    public static (DateTime baslangic, DateTime bitis) VardiyaPlani(DateTime now, TimeSpan bas, TimeSpan bit)
    {
      bool gece = bit <= bas;
      DateTime tarih = now.Date;
      if (gece && now.TimeOfDay < bit)
        tarih = tarih.AddDays(-1);
      return (tarih.Add(bas), tarih.Add(bit).AddDays(gece ? 1 : 0));
    }

    public static (int calismaDakika, int mesaiDakika) CalismaSuresi(DateTime giris, DateTime cikis, DateTime bas, DateTime bit, int mola)
    {
      if (cikis < giris || bit <= bas || mola < 0 || mola >= (bit - bas).TotalMinutes)
        throw new InvalidOperationException("Çalışma zamanı veya vardiya mola bilgileri geçersiz.");
      int calisma = Math.Max(0, checked((int)(cikis - giris).TotalMinutes) - mola);
      int normal = checked((int)(bit - bas).TotalMinutes) - mola;
      return (calisma, Math.Max(0, calisma - normal));
    }

    private void PersoneliKilitle(int id, SqlTransaction tx, bool aktif)
    {
      using (var cmd = new SqlCommand("SELECT AktifMi,IseGirisTarihi FROM dbo.Personel WITH (UPDLOCK,HOLDLOCK) WHERE PersonelId=@Id", SqlDBConnection.GetConnection(), tx))
      {
        cmd.Parameters.AddWithValue("@Id", id);
        using (var r = cmd.ExecuteReader())
          if (!r.Read() || (aktif && (!r.GetBoolean(0) || r.GetDateTime(1).Date > DateTime.Today)))
            throw new InvalidOperationException("Çalışmaya başlamış aktif personel bulunamadı.");
      }
    }

    public bool AcikKayitVarMi(int id)
    {
      using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.GirisKayit WHERE PersonelId=@Pid AND CikisSaati IS NULL", SqlDBConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@Pid", id);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
      }
    }

    public bool BugunkuGirisVarMi(int id)
    {
      using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId
        JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId WHERE g.PersonelId=@Pid AND g.KayitTarihi=
        CASE WHEN v.BitisSaati<=v.BaslangicSaati AND CAST(@Now AS time)<v.BitisSaati
        THEN DATEADD(day,-1,CAST(@Now AS date)) ELSE CAST(@Now AS date) END", SqlDBConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@Pid", id);
        cmd.Parameters.AddWithValue("@Now", DateTime.Now);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
      }
    }

    public bool BugunkuCikisVarMi(int id) => BugunkuGirisVarMi(id) && !AcikKayitVarMi(id);
    public List<GirisKayit> BugunkuKayitlar() => KayitlariGetir(DateTime.Today, DateTime.Today, null, true);
    public List<GirisKayit> TarihAraligindaGetir(DateTime baslangic, DateTime bitis, int? personelId = null) => KayitlariGetir(baslangic.Date, bitis.Date, personelId, false);
    private List<GirisKayit> KayitlariGetir(DateTime bas, DateTime bit, int? id, bool aciklar)
    {
      if (bit.Date < bas.Date)
        throw new ArgumentException("Tarih aralığı geçersiz.");
      var liste = new List<GirisKayit>();
      string sql = "SELECT * FROM dbo.vwGunlukOzet WHERE (KayitTarihi BETWEEN @Bas AND @Bit" + (aciklar ? " OR CikisSaati IS NULL" : "") + ")" + (id.HasValue ? " AND PersonelId=@Pid" : "") + " ORDER BY KayitTarihi DESC,AdSoyad";
      using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@Bas", bas);
        cmd.Parameters.AddWithValue("@Bit", bit);
        if (id.HasValue)
          cmd.Parameters.AddWithValue("@Pid", id.Value);
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(new GirisKayit { GirisKayitId = Convert.ToInt32(r["GirisKayitId"]), PersonelId = Convert.ToInt32(r["PersonelId"]), AdSoyad = r["AdSoyad"].ToString(), DepartmanAdi = r["DepartmanAdi"].ToString(), VardiyaAdi = r["VardiyaAdi"].ToString(), KayitTarihi = Convert.ToDateTime(r["KayitTarihi"]), GirisSaati = r["GirisSaati"] == DBNull.Value ? (TimeSpan? )null : (TimeSpan)r["GirisSaati"], CikisSaati = r["CikisSaati"] == DBNull.Value ? (TimeSpan? )null : (TimeSpan)r["CikisSaati"], GirisZamani = r["GirisZamani"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["GirisZamani"]), CikisZamani = r["CikisZamani"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["CikisZamani"]), CalismaDakika = r["CalismaDakika"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["CalismaDakika"]), MesaiDakika = r["MesaiDakika"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["MesaiDakika"]), GecGeldiMi = Convert.ToBoolean(r["GecGeldiMi"]), ErkenCiktiMi = Convert.ToBoolean(r["ErkenCiktiMi"]), Aciklama = r["Aciklama"] == DBNull.Value ? null : r["Aciklama"].ToString() });
      }

      return liste;
    }
  }
}
