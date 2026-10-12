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
      string sql = sadeceAktif
          ? "SELECT * FROM Departman WHERE AktifMi = 1 ORDER BY DepartmanAdi"
          : "SELECT * FROM Departman ORDER BY DepartmanAdi";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          liste.Add(new Departman
          {
            DepartmanId = Convert.ToInt32(r["DepartmanId"]),
            DepartmanAdi = r["DepartmanAdi"].ToString(),
            Aciklama = r["Aciklama"] == DBNull.Value ? null : r["Aciklama"].ToString(),
            AktifMi = Convert.ToBoolean(r["AktifMi"]),
            OlusturmaTarihi = Convert.ToDateTime(r["OlusturmaTarihi"])
          });
      return liste;
    }

    public bool Kaydet(Departman d)
    {
      string sql = "INSERT INTO Departman (DepartmanAdi, Aciklama) VALUES (@Adi, @Aciklama)";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Adi", d.DepartmanAdi);
      cmd.Parameters.AddWithValue("@Aciklama", (object)d.Aciklama ?? DBNull.Value);
      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok) SistemLogger.Log("Insert", "Departman", null, d.DepartmanAdi);
      return ok;
    }

    public bool Guncelle(Departman d)
    {
      string sql = "UPDATE Departman SET DepartmanAdi = @Adi, Aciklama = @Aciklama WHERE DepartmanId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Adi", d.DepartmanAdi);
      cmd.Parameters.AddWithValue("@Aciklama", (object)d.Aciklama ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Id", d.DepartmanId);
      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok) SistemLogger.Log("Update", "Departman", d.DepartmanId, d.DepartmanAdi);
      return ok;
    }

    public bool Sil(int departmanId)
    {
      string sql = "UPDATE Departman SET AktifMi = 0 WHERE DepartmanId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", departmanId);
      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok) SistemLogger.Log("Delete", "Departman", departmanId);
      return ok;
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
      string sql = departmanId.HasValue
          ? "SELECT * FROM Pozisyon WHERE AktifMi = 1 AND DepartmanId = @DepId ORDER BY PozisyonAdi"
          : "SELECT * FROM Pozisyon WHERE AktifMi = 1 ORDER BY PozisyonAdi";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      if (departmanId.HasValue)
        cmd.Parameters.AddWithValue("@DepId", departmanId.Value);

      using (var r = cmd.ExecuteReader())
        while (r.Read())
          liste.Add(new Pozisyon
          {
            PozisyonId = Convert.ToInt32(r["PozisyonId"]),
            DepartmanId = Convert.ToInt32(r["DepartmanId"]),
            PozisyonAdi = r["PozisyonAdi"].ToString(),
            Aciklama = r["Aciklama"] == DBNull.Value ? null : r["Aciklama"].ToString(),
            AktifMi = Convert.ToBoolean(r["AktifMi"])
          });
      return liste;
    }

    public bool Kaydet(Pozisyon p)
    {
      string sql = "INSERT INTO Pozisyon (DepartmanId, PozisyonAdi, Aciklama) VALUES (@DepId, @Adi, @Aciklama)";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@DepId", p.DepartmanId);
      cmd.Parameters.AddWithValue("@Adi", p.PozisyonAdi);
      cmd.Parameters.AddWithValue("@Aciklama", (object)p.Aciklama ?? DBNull.Value);
      return cmd.ExecuteNonQuery() > 0;
    }

    public bool Guncelle(Pozisyon p)
    {
      string sql = "UPDATE Pozisyon SET PozisyonAdi = @Adi, Aciklama = @Aciklama WHERE PozisyonId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Adi", p.PozisyonAdi);
      cmd.Parameters.AddWithValue("@Aciklama", (object)p.Aciklama ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Id", p.PozisyonId);
      return cmd.ExecuteNonQuery() > 0;
    }

    public bool Sil(int pozisyonId)
    {
      string sql = "UPDATE Pozisyon SET AktifMi = 0 WHERE PozisyonId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", pozisyonId);
      return cmd.ExecuteNonQuery() > 0;
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
      string sql = sadeceAktif
          ? "SELECT * FROM Vardiya WHERE AktifMi = 1 ORDER BY VardiyaAdi"
          : "SELECT * FROM Vardiya ORDER BY VardiyaAdi";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          liste.Add(MapVardiya(r));
      return liste;
    }

    public Vardiya IdIleGetir(int vardiyaId)
    {
      string sql = "SELECT * FROM Vardiya WHERE VardiyaId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", vardiyaId);
      using (var r = cmd.ExecuteReader())
        return r.Read() ? MapVardiya(r) : null;
    }

    public int Kaydet(Vardiya v)
    {
      string sql = @"
                INSERT INTO Vardiya
                    (VardiyaAdi, BaslangicSaati, BitisSaati, MolaDakika,
                     Pazartesi, Sali, Carsamba, Persembe, Cuma, Cumartesi, Pazar)
                VALUES
                    (@Adi, @Baslangic, @Bitis, @Mola,
                     @Pzt, @Sal, @Car, @Per, @Cum, @Cmt, @Paz);
                SELECT SCOPE_IDENTITY();";

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      SetVardiyaParams(cmd, v);
      int yeniId = Convert.ToInt32(cmd.ExecuteScalar());
      SistemLogger.Log("Insert", "Vardiya", yeniId, v.VardiyaAdi);
      return yeniId;
    }

    public bool Guncelle(Vardiya v)
    {
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

      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      SetVardiyaParams(cmd, v);
      cmd.Parameters.AddWithValue("@Id", v.VardiyaId);
      bool ok = cmd.ExecuteNonQuery() > 0;
      if (ok) SistemLogger.Log("Update", "Vardiya", v.VardiyaId, v.VardiyaAdi);
      return ok;
    }

    public bool Sil(int vardiyaId)
    {
      string sql = "UPDATE Vardiya SET AktifMi = 0 WHERE VardiyaId = @Id";
      var cmd = new SqlCommand(sql, SqlDBConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", vardiyaId);
      return cmd.ExecuteNonQuery() > 0;
    }

    private void SetVardiyaParams(SqlCommand cmd, Vardiya v)
    {
      cmd.Parameters.AddWithValue("@Adi", v.VardiyaAdi);
      cmd.Parameters.AddWithValue("@Baslangic", v.BaslangicSaati.ToString(@"hh\:mm"));
      cmd.Parameters.AddWithValue("@Bitis", v.BitisSaati.ToString(@"hh\:mm"));
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
