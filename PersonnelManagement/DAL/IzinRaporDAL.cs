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
            liste.Add(new IzinTuru { IzinTuruId = Convert.ToInt32(r["IzinTuruId"]), TuruAdi = r["TuruAdi"].ToString(), Ucretli = Convert.ToBoolean(r["Ucretli"]), AktifMi = Convert.ToBoolean(r["AktifMi"]) });
        return liste;
      }
    }

    public class IzinDal
    {
      public List<Izin> PersonelIzinleri(int personelId, int? yil = null)
      {
        DateTime? bas = yil.HasValue ? (DateTime? )new DateTime(yil.Value, 1, 1) : null;
        return TarihAraligindaGetir(bas, bas?.AddYears(1).AddDays(-1), personelId);
      }

      public List<Izin> BekleyenIzinler() => TarihAraligindaGetir(null, null, null, "Beklemede");
      public List<Izin> TarihAraligindaGetir(DateTime? bas, DateTime? bit, int? personelId = null, string durum = null)
      {
        if (bas.HasValue && bit.HasValue && bit.Value.Date < bas.Value.Date)
          throw new ArgumentException("Tarih aralığı geçersiz.");
        var liste = new List<Izin>();
        string sql = @"SELECT i.*,p.Ad+' '+p.Soyad AS PersonelAdSoyad,it.TuruAdi AS IzinTuruAdi
          FROM dbo.Izin i JOIN dbo.Personel p ON p.PersonelId=i.PersonelId
          JOIN dbo.IzinTuru it ON it.IzinTuruId=i.IzinTuruId WHERE 1=1" + (bas.HasValue ? " AND i.BitisTarihi>=@Bas" : "") + (bit.HasValue ? " AND i.BaslangicTarihi<=@Bit" : "") + (personelId.HasValue ? " AND i.PersonelId=@Pid" : "") + (durum != null ? " AND i.Durum=@Durum" : "") + " ORDER BY i.BaslangicTarihi DESC,i.IzinId DESC";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
        {
          if (bas.HasValue)
            cmd.Parameters.AddWithValue("@Bas", bas.Value.Date);
          if (bit.HasValue)
            cmd.Parameters.AddWithValue("@Bit", bit.Value.Date);
          if (personelId.HasValue)
            cmd.Parameters.AddWithValue("@Pid", personelId.Value);
          if (durum != null)
            cmd.Parameters.AddWithValue("@Durum", durum);
          using (var r = cmd.ExecuteReader())
            while (r.Read())
              liste.Add(MapIzin(r));
        }

        return liste;
      }

      public int Kaydet(Izin izin)
      {
        if (izin == null || izin.BitisTarihi.Date < izin.BaslangicTarihi.Date)
          throw new ArgumentException("İzin tarih aralığı geçersiz.");
        if ((izin.Aciklama ?? "").Length > 500)
          throw new ArgumentException("Açıklama en fazla 500 karakter olabilir.");
        izin.BaslangicTarihi = izin.BaslangicTarihi.Date;
        izin.BitisTarihi = izin.BitisTarihi.Date;
        izin.ToplamGun = (izin.BitisTarihi - izin.BaslangicTarihi).Days + 1;
        int id;
        using (var tx = SqlDBConnection.GetConnection().BeginTransaction())
        {
          PersoneliKilitle(izin.PersonelId, tx, true);
          CakismaDogrula(izin, tx, 0, true);
          HakDogrula(izin, tx);
          using (var cmd = new SqlCommand(@"INSERT dbo.Izin
            (PersonelId,IzinTuruId,BaslangicTarihi,BitisTarihi,ToplamGun,Aciklama)
            VALUES (@Pid,@Tur,@Bas,@Bit,@Gun,@Aciklama);SELECT CAST(SCOPE_IDENTITY() AS int);", SqlDBConnection.GetConnection(), tx))
          {
            cmd.Parameters.AddWithValue("@Pid", izin.PersonelId);
            cmd.Parameters.AddWithValue("@Tur", izin.IzinTuruId);
            cmd.Parameters.AddWithValue("@Bas", izin.BaslangicTarihi);
            cmd.Parameters.AddWithValue("@Bit", izin.BitisTarihi);
            cmd.Parameters.AddWithValue("@Gun", izin.ToplamGun);
            cmd.Parameters.AddWithValue("@Aciklama", (object)izin.Aciklama ?? DBNull.Value);
            id = Convert.ToInt32(cmd.ExecuteScalar());
          }

          SistemLogger.Log("Insert", "Izin", id, $"PersonelId:{izin.PersonelId}", transaction: tx);
          tx.Commit();
        }

        return id;
      }

      public bool Onayla(int izinId, int onayanKullaniciId) => DurumDegistir(izinId, onayanKullaniciId, "Onaylı", null);
      public bool Reddet(int izinId, int onayanKullaniciId, string redNedeni)
      {
        if (string.IsNullOrWhiteSpace(redNedeni) || redNedeni.Trim().Length > 500)
          throw new ArgumentException("Red nedeni zorunludur ve en fazla 500 karakter olabilir.");
        return DurumDegistir(izinId, onayanKullaniciId, "Reddedildi", redNedeni.Trim());
      }

      public bool Iptal(int izinId, int islemiYapanId) => DurumDegistir(izinId, islemiYapanId, "İptal Edildi", null);
      private bool DurumDegistir(int id, int kullaniciId, string hedef, string neden)
      {
        using (var tx = SqlDBConnection.GetConnection().BeginTransaction())
        {
          IslemYetkisi.AdminDogrula(kullaniciId, tx);
          int personelId;
          using (var cmd = new SqlCommand("SELECT PersonelId FROM dbo.Izin WHERE IzinId=@Id", SqlDBConnection.GetConnection(), tx))
          {
            cmd.Parameters.AddWithValue("@Id", id);
            object value = cmd.ExecuteScalar();
            if (value == null)
              return false;
            personelId = Convert.ToInt32(value);
          }

          // Personel kilidi, farklı taleplerin hak/çakışma kontrolünü de sıraya alır.
          PersoneliKilitle(personelId, tx, hedef == "Onaylı");
          Izin izin;
          string mevcut;
          using (var cmd = new SqlCommand("SELECT * FROM dbo.Izin WITH (UPDLOCK,HOLDLOCK) WHERE IzinId=@Id", SqlDBConnection.GetConnection(), tx))
          {
            cmd.Parameters.AddWithValue("@Id", id);
            using (var r = cmd.ExecuteReader())
            {
              if (!r.Read())
                return false;
              if (Convert.ToInt32(r["PersonelId"]) != personelId)
                throw new InvalidOperationException("İzin kaydı değişmiş; listeyi yenileyin.");
              mevcut = r["Durum"].ToString();
              izin = new Izin
              {
                PersonelId = personelId,
                IzinTuruId = Convert.ToInt32(r["IzinTuruId"]),
                BaslangicTarihi = Convert.ToDateTime(r["BaslangicTarihi"]),
                BitisTarihi = Convert.ToDateTime(r["BitisTarihi"])
              };
            }
          }

          bool uygun = hedef == "İptal Edildi" ? (mevcut == "Beklemede" || mevcut == "Onaylı") : mevcut == "Beklemede";
          if (!uygun)
            throw new InvalidOperationException("İzin durumu bu işleme uygun değil. Listeyi yenileyin.");
          if (hedef == "Onaylı")
          {
            CakismaDogrula(izin, tx, id, false);
            HakDogrula(izin, tx);
          }

          string sql = hedef == "İptal Edildi" ? "UPDATE dbo.Izin SET Durum=@Durum,IptalEdenKullaniciId=@Kullanici,IptalTarihi=GETDATE() WHERE IzinId=@Id AND Durum=@Mevcut" : "UPDATE dbo.Izin SET Durum=@Durum,OnayanKullaniciId=@Kullanici,OnayTarihi=GETDATE(),RedNedeni=@Neden WHERE IzinId=@Id AND Durum=@Mevcut";
          using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
          {
            cmd.Parameters.AddWithValue("@Durum", hedef);
            cmd.Parameters.AddWithValue("@Kullanici", kullaniciId);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Mevcut", mevcut);
            if (hedef != "İptal Edildi")
              cmd.Parameters.AddWithValue("@Neden", (object)neden ?? DBNull.Value);
            if (cmd.ExecuteNonQuery() != 1)
              throw new InvalidOperationException("İzin durumu değişmiş; listeyi yenileyin.");
          }

          SistemLogger.Log("Update", "Izin", id, hedef, transaction: tx);
          tx.Commit();
        }

        return true;
      }

      private void PersoneliKilitle(int id, SqlTransaction tx, bool aktif)
      {
        using (var cmd = new SqlCommand("SELECT AktifMi FROM dbo.Personel WITH (UPDLOCK,HOLDLOCK) WHERE PersonelId=@Id", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", id);
          object value = cmd.ExecuteScalar();
          if (value == null || (aktif && !Convert.ToBoolean(value)))
            throw new InvalidOperationException("Aktif personel bulunamadı.");
        }
      }

      private void CakismaDogrula(Izin izin, SqlTransaction tx, int haric, bool bekleyenler)
      {
        using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.Izin WITH (UPDLOCK,HOLDLOCK)
          WHERE PersonelId=@Pid AND IzinId<>@Id AND (Durum=N'Onaylı'" + (bekleyenler ? " OR Durum=N'Beklemede'" : "") + ") AND BitisTarihi>=@Bas AND BaslangicTarihi<=@Bit", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Pid", izin.PersonelId);
          cmd.Parameters.AddWithValue("@Id", haric);
          cmd.Parameters.AddWithValue("@Bas", izin.BaslangicTarihi);
          cmd.Parameters.AddWithValue("@Bit", izin.BitisTarihi);
          if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            throw new InvalidOperationException("Bu tarihlerde çakışan bir izin mevcut.");
        }
      }

      private void HakDogrula(Izin izin, SqlTransaction tx)
      {
        string tur;
        using (var cmd = new SqlCommand("SELECT TuruAdi FROM dbo.IzinTuru WHERE IzinTuruId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", izin.IzinTuruId);
          tur = cmd.ExecuteScalar() as string;
        }

        if (tur == null)
          throw new InvalidOperationException("Aktif izin türü bulunamadı.");
        if (tur != "Yıllık İzin")
          return;
        // Yıl sınırını geçen talebin her yıl içindeki günleri ayrı kontrol edilir; devir eklenmez.
        for (int yil = izin.BaslangicTarihi.Year; yil <= izin.BitisTarihi.Year; yil++)
        {
          DateTime bas = new DateTime(yil, 1, 1), bit = new DateTime(yil, 12, 31);
          DateTime ilk = izin.BaslangicTarihi > bas ? izin.BaslangicTarihi : bas, son = izin.BitisTarihi < bit ? izin.BitisTarihi : bit;
          int istenen = (son - ilk).Days + 1;
          if (KalanYillikIzin(izin.PersonelId, yil, tx) < istenen)
            throw new InvalidOperationException($"{yil} yılı için kalan yıllık izin hakkı yetersiz.");
        }
      }

      public int KalanYillikIzin(int personelId) => KalanYillikIzin(personelId, DateTime.Today.Year, null);
      public int KalanYillikIzin(int personelId, int yil) => KalanYillikIzin(personelId, yil, null);
      private int KalanYillikIzin(int personelId, int yil, SqlTransaction tx)
      {
        using (var cmd = new SqlCommand(@"SELECT p.YillikIzinHakki-ISNULL((SELECT SUM(DATEDIFF(day,
            CASE WHEN i.BaslangicTarihi<@Bas THEN @Bas ELSE i.BaslangicTarihi END,
            CASE WHEN i.BitisTarihi>@Bit THEN @Bit ELSE i.BitisTarihi END)+1)
          FROM dbo.Izin i JOIN dbo.IzinTuru it ON it.IzinTuruId=i.IzinTuruId
          WHERE i.PersonelId=p.PersonelId AND i.Durum=N'Onaylı' AND it.TuruAdi=N'Yıllık İzin'
            AND i.BitisTarihi>=@Bas AND i.BaslangicTarihi<=@Bit),0)
          FROM dbo.Personel p WHERE p.PersonelId=@Pid", SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Bas", new DateTime(yil, 1, 1));
          cmd.Parameters.AddWithValue("@Bit", new DateTime(yil, 12, 31));
          cmd.Parameters.AddWithValue("@Pid", personelId);
          object value = cmd.ExecuteScalar();
          return value == null ? 0 : Convert.ToInt32(value);
        }
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
        OnayanKullaniciId = r["OnayanKullaniciId"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["OnayanKullaniciId"]),
        OnayTarihi = r["OnayTarihi"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["OnayTarihi"]),
        OlusturmaTarihi = Convert.ToDateTime(r["OlusturmaTarihi"])
      };
    }

    public class RaporDal
    {
      public List<PersonelRapor> PersonelOzetRaporu(DateTime baslangic, DateTime bitis, int? personelId = null)
      {
        if (bitis.Date < baslangic.Date)
          throw new ArgumentException("Tarih aralığı geçersiz.");
        var liste = new List<PersonelRapor>();
        string sql = @"SELECT p.PersonelId,p.Ad+' '+p.Soyad AS AdSoyad,d.DepartmanAdi,
          COUNT(g.GirisKayitId) AS ToplamCalismaGun,ISNULL(SUM(g.CalismaDakika),0) AS ToplamCalismaDakika,
          ISNULL(SUM(g.MesaiDakika),0) AS ToplamMesaiDakika,ISNULL(SUM(CAST(g.GecGeldiMi AS int)),0) AS GecGelmeSayisi,
          ISNULL(SUM(CAST(g.ErkenCiktiMi AS int)),0) AS ErkenCikisSayisi,
          (SELECT COUNT(*) FROM dbo.fnDevamsizlik(@Bas,@Bit,@Now) dv WHERE dv.PersonelId=p.PersonelId) AS ToplamDevamsizlik,
          (SELECT ISNULL(SUM(DATEDIFF(day,CASE WHEN iz.BaslangicTarihi<@Bas THEN @Bas ELSE iz.BaslangicTarihi END,
            CASE WHEN iz.BitisTarihi>@Bit THEN @Bit ELSE iz.BitisTarihi END)+1),0) FROM dbo.Izin iz
            WHERE iz.PersonelId=p.PersonelId AND iz.Durum=N'Onaylı' AND iz.BitisTarihi>=@Bas AND iz.BaslangicTarihi<=@Bit) AS ToplamIzinGun
          FROM dbo.Personel p JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId
          LEFT JOIN dbo.GirisKayit g ON g.PersonelId=p.PersonelId AND g.KayitTarihi BETWEEN @Bas AND @Bit
          WHERE p.IseGirisTarihi<=@Bit AND (p.IstenCikisTarihi IS NULL OR p.IstenCikisTarihi>=@Bas)" + (personelId.HasValue ? " AND p.PersonelId=@Pid" : "") + " GROUP BY p.PersonelId,p.Ad,p.Soyad,d.DepartmanAdi ORDER BY p.Ad,p.Soyad OPTION (MAXRECURSION 0)";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
        {
          cmd.Parameters.AddWithValue("@Bas", baslangic.Date);
          cmd.Parameters.AddWithValue("@Bit", bitis.Date);
          cmd.Parameters.AddWithValue("@Now", DateTime.Now);
          if (personelId.HasValue)
            cmd.Parameters.AddWithValue("@Pid", personelId.Value);
          using (var r = cmd.ExecuteReader())
            while (r.Read())
              liste.Add(new PersonelRapor { PersonelId = Convert.ToInt32(r["PersonelId"]), AdSoyad = r["AdSoyad"].ToString(), DepartmanAdi = r["DepartmanAdi"].ToString(), ToplamCalismaGun = Convert.ToInt32(r["ToplamCalismaGun"]), ToplamCalismaDakika = Convert.ToInt32(r["ToplamCalismaDakika"]), ToplamMesaiDakika = Convert.ToInt32(r["ToplamMesaiDakika"]), ToplamDevamsizlik = Convert.ToInt32(r["ToplamDevamsizlik"]), ToplamIzinGun = Convert.ToInt32(r["ToplamIzinGun"]), GecGelmeSayisi = Convert.ToInt32(r["GecGelmeSayisi"]), ErkenCikisSayisi = Convert.ToInt32(r["ErkenCikisSayisi"]) });
        }

        return liste;
      }

      public System.Data.DataTable DevamsizlikRaporu(DateTime baslangic, DateTime bitis, int? personelId = null)
      {
        if (bitis.Date < baslangic.Date)
          throw new ArgumentException("Tarih aralığı geçersiz.");
        // Ana menü ve dönem raporu aynı gün/izin/vardiya bitişi kuralını kullanır.
        string sql = "SELECT * FROM dbo.fnDevamsizlik(@Bas,@Bit,@Now)" + (personelId.HasValue ? " WHERE PersonelId=@Pid" : "") + " ORDER BY DevamsizlikTarihi DESC,PersonelAdSoyad OPTION (MAXRECURSION 0)";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
        {
          cmd.Parameters.AddWithValue("@Bas", baslangic.Date);
          cmd.Parameters.AddWithValue("@Bit", bitis.Date);
          cmd.Parameters.AddWithValue("@Now", DateTime.Now);
          if (personelId.HasValue)
            cmd.Parameters.AddWithValue("@Pid", personelId.Value);
          var table = new System.Data.DataTable();
          using (var adapter = new SqlDataAdapter(cmd))
            adapter.Fill(table);
          return table;
        }
      }

      public (int toplamPersonel, int bugunGiren, int bugunIzinli, int bugunDevamsiz) BugunOzet()
      {
        using (var cmd = new SqlCommand(@"SELECT
          (SELECT COUNT(*) FROM dbo.Personel WHERE AktifMi=1 AND IseGirisTarihi<=@Bugun),
          (SELECT COUNT(DISTINCT g.PersonelId) FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId
            WHERE p.AktifMi=1 AND g.KayitTarihi=@Bugun),
          (SELECT COUNT(DISTINCT i.PersonelId) FROM dbo.Izin i JOIN dbo.Personel p ON p.PersonelId=i.PersonelId
            WHERE p.AktifMi=1 AND p.IseGirisTarihi<=@Bugun AND i.Durum=N'Onaylı' AND @Bugun BETWEEN i.BaslangicTarihi AND i.BitisTarihi),
          (SELECT COUNT(DISTINCT dv.PersonelId) FROM dbo.fnDevamsizlik(@Bugun,@Bugun,@Now) dv
            JOIN dbo.Personel p ON p.PersonelId=dv.PersonelId WHERE p.AktifMi=1) OPTION (MAXRECURSION 0)", SqlDBConnection.GetConnection()))
        {
          cmd.Parameters.AddWithValue("@Bugun", DateTime.Today);
          cmd.Parameters.AddWithValue("@Now", DateTime.Now);
          using (var r = cmd.ExecuteReader())
            if (r.Read())
              return (Convert.ToInt32(r[0]), Convert.ToInt32(r[1]), Convert.ToInt32(r[2]), Convert.ToInt32(r[3]));
        }

        return (0, 0, 0, 0);
      }
    }
  }
}
