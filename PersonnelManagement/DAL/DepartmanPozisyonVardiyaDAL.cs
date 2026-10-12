using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public class DepartmanDAL
  {
    public List<Departman> TumunuGetir(bool sadeceAktif = true)
    {
      var liste = new List<Departman>();
      string sql = sadeceAktif ? "SELECT * FROM Departman WHERE AktifMi = 1 ORDER BY DepartmanAdi" : "SELECT * FROM Departman ORDER BY DepartmanAdi";
      using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
      {
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(new Departman { DepartmanId = Convert.ToInt32(r["DepartmanId"]), DepartmanAdi = r["DepartmanAdi"].ToString(), Aciklama = r["Aciklama"] == DBNull.Value ? null : r["Aciklama"].ToString(), AktifMi = Convert.ToBoolean(r["AktifMi"]), OlusturmaTarihi = Convert.ToDateTime(r["OlusturmaTarihi"]) });
        return liste;
      }
    }

    public bool Kaydet(Departman d)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        if (d == null || string.IsNullOrWhiteSpace(d.DepartmanAdi) || d.DepartmanAdi.Trim().Length > 100 || (d.Aciklama ?? "").Length > 255)
          throw new ArgumentException("Departman adı zorunludur; ad 100, açıklama 255 karakteri aşamaz.");
        string sql = "INSERT INTO Departman (DepartmanAdi, Aciklama) VALUES (@Adi, @Aciklama)";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Adi", d.DepartmanAdi);
          cmd.Parameters.AddWithValue("@Aciklama", (object)d.Aciklama ?? DBNull.Value);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Insert", "Departman", null, d.DepartmanAdi, transaction: tx);
          return ok;
        }
      });
    }

    public bool Guncelle(Departman d)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        if (d == null || string.IsNullOrWhiteSpace(d.DepartmanAdi) || d.DepartmanAdi.Trim().Length > 100 || (d.Aciklama ?? "").Length > 255)
          throw new ArgumentException("Departman adı zorunludur; ad 100, açıklama 255 karakteri aşamaz.");
        string sql = "UPDATE Departman SET DepartmanAdi = @Adi, Aciklama = @Aciklama WHERE DepartmanId = @Id";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Adi", d.DepartmanAdi);
          cmd.Parameters.AddWithValue("@Aciklama", (object)d.Aciklama ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Id", d.DepartmanId);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Update", "Departman", d.DepartmanId, d.DepartmanAdi, transaction: tx);
          return ok;
        }
      });
    }

    public bool Sil(int departmanId)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        // Atama ile pasife alma aynı kaydı kilitler; eşzamanlı işlem tutarsızlık oluşturmaz.
        using (var check = new SqlCommand("SELECT DepartmanId FROM dbo.Departman WITH (XLOCK,HOLDLOCK) WHERE DepartmanId=@Id", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", departmanId);
          if (check.ExecuteScalar() == null)
            return false;
        }

        using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.Personel WHERE DepartmanId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", departmanId);
          if (Convert.ToInt32(check.ExecuteScalar()) != 0)
            throw new InvalidOperationException("Aktif personelin kullandığı departman pasife alınamaz.");
        }

        string sql = "UPDATE Departman SET AktifMi = 0 WHERE DepartmanId = @Id";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", departmanId);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Delete", "Departman", departmanId, transaction: tx);
          return ok;
        }
      });
    }
  }

  // ════════════════════════════════════════════════
  //  PozisyonDal
  // ════════════════════════════════════════════════
  public class PozisyonDal
  {
    public List<Pozisyon> TumunuGetir(int? departmanId = null)
    {
      var liste = new List<Pozisyon>();
      string sql = departmanId.HasValue ? "SELECT * FROM Pozisyon WHERE AktifMi = 1 AND DepartmanId = @DepId ORDER BY PozisyonAdi" : "SELECT * FROM Pozisyon WHERE AktifMi = 1 ORDER BY PozisyonAdi";
      using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
      {
        if (departmanId.HasValue)
          cmd.Parameters.AddWithValue("@DepId", departmanId.Value);
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(new Pozisyon { PozisyonId = Convert.ToInt32(r["PozisyonId"]), DepartmanId = Convert.ToInt32(r["DepartmanId"]), PozisyonAdi = r["PozisyonAdi"].ToString(), Aciklama = r["Aciklama"] == DBNull.Value ? null : r["Aciklama"].ToString(), AktifMi = Convert.ToBoolean(r["AktifMi"]) });
        return liste;
      }
    }

    public bool Kaydet(Pozisyon p)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        if (p == null || string.IsNullOrWhiteSpace(p.PozisyonAdi) || p.PozisyonAdi.Trim().Length > 100 || (p.Aciklama ?? "").Length > 255)
          throw new ArgumentException("Pozisyon adı zorunludur; ad 100, açıklama 255 karakteri aşamaz.");
        using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.Departman WITH (HOLDLOCK) WHERE DepartmanId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", p.DepartmanId);
          if (Convert.ToInt32(check.ExecuteScalar()) != 1)
            throw new InvalidOperationException("Aktif departman seçilmelidir.");
        }

        string sql = "INSERT INTO Pozisyon (DepartmanId, PozisyonAdi, Aciklama) VALUES (@DepId, @Adi, @Aciklama)";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@DepId", p.DepartmanId);
          cmd.Parameters.AddWithValue("@Adi", p.PozisyonAdi);
          cmd.Parameters.AddWithValue("@Aciklama", (object)p.Aciklama ?? DBNull.Value);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Insert", "Pozisyon", p.PozisyonId == 0 ? (int? )null : p.PozisyonId, p.PozisyonAdi, transaction: tx);
          return ok;
        }
      });
    }

    public bool Guncelle(Pozisyon p)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        if (p == null || string.IsNullOrWhiteSpace(p.PozisyonAdi) || p.PozisyonAdi.Trim().Length > 100 || (p.Aciklama ?? "").Length > 255)
          throw new ArgumentException("Pozisyon adı zorunludur; ad 100, açıklama 255 karakteri aşamaz.");
        using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.Departman WITH (HOLDLOCK) WHERE DepartmanId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", p.DepartmanId);
          if (Convert.ToInt32(check.ExecuteScalar()) != 1)
            throw new InvalidOperationException("Aktif departman seçilmelidir.");
        }

        string sql = "UPDATE Pozisyon SET PozisyonAdi = @Adi, Aciklama = @Aciklama WHERE PozisyonId = @Id";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Adi", p.PozisyonAdi);
          cmd.Parameters.AddWithValue("@Aciklama", (object)p.Aciklama ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Id", p.PozisyonId);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Update", "Pozisyon", p.PozisyonId == 0 ? (int? )null : p.PozisyonId, p.PozisyonAdi, transaction: tx);
          return ok;
        }
      });
    }

    public bool Sil(int pozisyonId)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        // Atama ile pasife alma aynı kaydı kilitler; eşzamanlı işlem tutarsızlık oluşturmaz.
        using (var check = new SqlCommand("SELECT PozisyonId FROM dbo.Pozisyon WITH (XLOCK,HOLDLOCK) WHERE PozisyonId=@Id", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", pozisyonId);
          if (check.ExecuteScalar() == null)
            return false;
        }

        using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.Personel WHERE PozisyonId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", pozisyonId);
          if (Convert.ToInt32(check.ExecuteScalar()) != 0)
            throw new InvalidOperationException("Aktif personelin kullandığı pozisyon pasife alınamaz.");
        }

        string sql = "UPDATE Pozisyon SET AktifMi = 0 WHERE PozisyonId = @Id";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", pozisyonId);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Delete", "Pozisyon", pozisyonId, transaction: tx);
          return ok;
        }
      });
    }
  }

  // ════════════════════════════════════════════════
  //  VardiyaDal
  // ════════════════════════════════════════════════
  public class VardiyaDal
  {
    public List<Vardiya> TumunuGetir(bool sadeceAktif = true)
    {
      var liste = new List<Vardiya>();
      string sql = sadeceAktif ? "SELECT * FROM Vardiya WHERE AktifMi = 1 ORDER BY VardiyaAdi" : "SELECT * FROM Vardiya ORDER BY VardiyaAdi";
      using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
      {
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            liste.Add(MapVardiya(r));
        return liste;
      }
    }

    public Vardiya IdIleGetir(int vardiyaId)
    {
      string sql = "SELECT * FROM Vardiya WHERE VardiyaId = @Id";
      using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@Id", vardiyaId);
        using (var r = cmd.ExecuteReader())
          return r.Read() ? MapVardiya(r) : null;
      }
    }

    public int Kaydet(Vardiya v)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        EmployeeValidation.ValidateShift(v);
        string sql = @"
                INSERT INTO Vardiya
                    (VardiyaAdi, BaslangicSaati, BitisSaati, MolaDakika,
                     Pazartesi, Sali, Carsamba, Persembe, Cuma, Cumartesi, Pazar)
                VALUES
                    (@Adi, @Baslangic, @Bitis, @Mola,
                     @Pzt, @Sal, @Car, @Per, @Cum, @Cmt, @Paz);
                SELECT SCOPE_IDENTITY();";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          SetVardiyaParams(cmd, v);
          int yeniId = Convert.ToInt32(cmd.ExecuteScalar());
          SistemLogger.Log("Insert", "Vardiya", yeniId, v.VardiyaAdi, transaction: tx);
          return yeniId;
        }
      });
    }

    public bool Guncelle(Vardiya v)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        EmployeeValidation.ValidateShift(v);
        string sql = @"
                UPDATE Vardiya SET
                    VardiyaAdi     = @Adi,
                    BaslangicSaati = @Baslangic,
                    BitisSaati     = @Bitis,
                    MolaDakika     = @Mola,
                    Pazartesi      = @Pzt,
                    Sali           = @Sal,
                    Carsamba       = @Car,
                    Persembe       = @Per,
                    Cuma           = @Cum,
                    Cumartesi      = @Cmt,
                    Pazar          = @Paz
                WHERE VardiyaId = @Id";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          SetVardiyaParams(cmd, v);
          cmd.Parameters.AddWithValue("@Id", v.VardiyaId);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Update", "Vardiya", v.VardiyaId, v.VardiyaAdi, transaction: tx);
          return ok;
        }
      });
    }

    public bool Sil(int vardiyaId)
    {
      return DatabaseTransaction.Execute(tx =>
      {
        // Atama ile pasife alma aynı kaydı kilitler; eşzamanlı işlem tutarsızlık oluşturmaz.
        using (var check = new SqlCommand("SELECT VardiyaId FROM dbo.Vardiya WITH (XLOCK,HOLDLOCK) WHERE VardiyaId=@Id", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", vardiyaId);
          if (check.ExecuteScalar() == null)
            return false;
        }

        using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.Personel WHERE VardiyaId=@Id AND AktifMi=1", SqlDBConnection.GetConnection(), tx))
        {
          check.Parameters.AddWithValue("@Id", vardiyaId);
          if (Convert.ToInt32(check.ExecuteScalar()) != 0)
            throw new InvalidOperationException("Aktif personelin kullandığı vardiya pasife alınamaz.");
        }

        string sql = "UPDATE Vardiya SET AktifMi = 0 WHERE VardiyaId = @Id";
        using (var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", vardiyaId);
          bool ok = cmd.ExecuteNonQuery() > 0;
          if (ok)
            SistemLogger.Log("Delete", "Vardiya", vardiyaId, transaction: tx);
          return ok;
        }
      });
    }

    private void SetVardiyaParams(SqlCommand cmd, Vardiya v)
    {
      cmd.Parameters.AddWithValue("@Adi", v.VardiyaAdi);
      cmd.Parameters.Add("@Baslangic", System.Data.SqlDbType.Time).Value = v.BaslangicSaati;
      cmd.Parameters.Add("@Bitis", System.Data.SqlDbType.Time).Value = v.BitisSaati;
      cmd.Parameters.AddWithValue("@Mola", v.MolaDakika);
      cmd.Parameters.AddWithValue("@Pzt", v.Pazartesi);
      cmd.Parameters.AddWithValue("@Sal", v.Sali);
      cmd.Parameters.AddWithValue("@Car", v.Carsamba);
      cmd.Parameters.AddWithValue("@Per", v.Persembe);
      cmd.Parameters.AddWithValue("@Cum", v.Cuma);
      cmd.Parameters.AddWithValue("@Cmt", v.Cumartesi);
      cmd.Parameters.AddWithValue("@Paz", v.Pazar);
    }

    private Vardiya MapVardiya(SqlDataReader r) => new Vardiya
    {
      VardiyaId = Convert.ToInt32(r["VardiyaId"]),
      VardiyaAdi = r["VardiyaAdi"].ToString(),
      BaslangicSaati = (TimeSpan)r["BaslangicSaati"],
      BitisSaati = (TimeSpan)r["BitisSaati"],
      MolaDakika = Convert.ToInt32(r["MolaDakika"]),
      Pazartesi = Convert.ToBoolean(r["Pazartesi"]),
      Sali = Convert.ToBoolean(r["Sali"]),
      Carsamba = Convert.ToBoolean(r["Carsamba"]),
      Persembe = Convert.ToBoolean(r["Persembe"]),
      Cuma = Convert.ToBoolean(r["Cuma"]),
      Cumartesi = Convert.ToBoolean(r["Cumartesi"]),
      Pazar = Convert.ToBoolean(r["Pazar"]),
      AktifMi = Convert.ToBoolean(r["AktifMi"])
    };
  }
}
