using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PersonnelManagement.Tests
{
  internal static partial class RegressionTests
  {
    private static readonly List<string> _additionalLogins = new List<string>();

    private static int Scalar(SqlConnection connection, string sql)
    {
      using (var command = new SqlCommand(sql, connection)) return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void SqlText(SqlConnection connection, string text)
    {
      foreach (string batch in Regex.Split(text, @"^[ \t]*GO[ \t]*(?:--[^\r\n]*)?\r?$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        if (!string.IsNullOrWhiteSpace(batch)) Execute(connection, batch);
    }

    private static void MigrationChecks(string source)
    {
      foreach (bool snapshot in new[] { false, true })
      {
        string legacy = "PersonnelManagementTest_" + Guid.NewGuid().ToString("N");
        _databases.Add(legacy);
        using (var master = Connect("master")) Execute(master, "CREATE DATABASE [" + legacy + "] COLLATE Turkish_CI_AS");
        using (var old = Connect(legacy))
        {
          string fixture = Path.GetFullPath(Path.Combine(source, "..", "..", "tests", "PersonnelManagement.Tests", "LegacySchemaFixture.sql"));
          SqlText(old, File.ReadAllText(fixture));
          Execute(old, @"INSERT dbo.Departman(DepartmanAdi) VALUES(N'Synthetic department');
            INSERT dbo.Pozisyon(DepartmanId,PozisyonAdi) VALUES(1,N'Synthetic position');
            INSERT dbo.Vardiya(VardiyaAdi,BaslangicSaati,BitisSaati,MolaDakika,Pazartesi,Sali,Carsamba,Persembe,Cuma,Cumartesi,Pazar) VALUES(N'Current shift','08:00','17:00',60,1,1,1,1,1,0,0);
            INSERT dbo.Kullanici(KullaniciAdi,Sifre,Ad,Soyad,Rol) VALUES(N'synthetic_admin',N'Synthetic123!',N'Synthetic',N'Admin',N'Admin'),(N'synthetic_staff',N'Synthetic123!',N'Synthetic',N'Staff',N'Calisan');
            INSERT dbo.Personel(TcKimlikNo,Ad,Soyad,DepartmanId,PozisyonId,VardiyaId,IseGirisTarihi) VALUES('86000000001',N'Synthetic',N'Migration',1,1,1,'20200101');
            INSERT dbo.IzinTuru(TuruAdi) VALUES(N'Yıllık İzin');
            INSERT dbo.GirisKayit(PersonelId,KayitTarihi,GirisSaati,CikisSaati,GirisZamani,CikisZamani,KaydedenId) VALUES(1,'20260105','09:00','18:00','20260105 09:00','20260105 18:00',1);
            INSERT dbo.Izin(PersonelId,IzinTuruId,BaslangicTarihi,BitisTarihi,ToplamGun,Durum) VALUES(1,1,'20260106','20260107',2,N'İptal Edildi');
            INSERT dbo.SistemLog(KullaniciId,Islem,TabloAdi,EtkilenenId,Aciklama) VALUES(1,N'Insert',N'Personel',1,N'Synthetic history');");
          if (snapshot)
          {
            Execute(old, "ALTER TABLE dbo.GirisKayit ADD PlanlananGirisZamani datetime NULL,PlanlananCikisZamani datetime NULL,PlanlananMolaDakika int NULL,VardiyaAdiKayit nvarchar(100) NULL;ALTER TABLE dbo.Izin ADD IptalEdenKullaniciId int NULL,IptalTarihi datetime NULL;");
            Execute(old, "UPDATE dbo.GirisKayit SET PlanlananGirisZamani='20260105 09:00',PlanlananCikisZamani='20260105 18:00',PlanlananMolaDakika=45,VardiyaAdiKayit=N'Historical shift';UPDATE dbo.Izin SET IptalEdenKullaniciId=1,IptalTarihi='20260104';");
          }
        }
        foreach (string language in new[] { "English", "Turkish" })
        {
          bool english = language == "English";
          string name = (english ? "PersonnelManagementTest_" : "PersonelYonetimiTest_") + Guid.NewGuid().ToString("N");
          _databases.Add(name);
          string original = english ? "PersonnelManagementDB" : "PersonelYonetimiDB";
          using (var master = Connect("master")) Script(master, Path.Combine(source, language, "DatabaseSetup.sql"), original, name);
          string migration = File.ReadAllText(Path.Combine(source, language, "DatabaseMigration.sql")).Replace(original, name).Replace("N'PersonelTakipV090Db'", "N'" + legacy + "'");
          using (var target = Connect(name))
          {
            SqlText(target, migration);
            string label = language + (snapshot ? "_v090" : "_legacy");
            Check("migration_ids_" + label, Scalar(target, english ? "SELECT COUNT(*) FROM dbo.Employee WHERE EmployeeId=1 AND NationalId='86000000001'" : "SELECT COUNT(*) FROM dbo.Personel WHERE PersonelId=1 AND TcKimlikNo='86000000001'") == 1);
            Check("migration_roles_" + label, Scalar(target, english ? "SELECT COUNT(*) FROM dbo.UserAccount WHERE Role=N'Employee'" : "SELECT COUNT(*) FROM dbo.Kullanici WHERE Rol=N'Personel'") == 1);
            Check("migration_password_preserved_" + label, Scalar(target, english ? "SELECT COUNT(*) FROM dbo.UserAccount WHERE PasswordHash=N'Synthetic123!'" : "SELECT COUNT(*) FROM dbo.Kullanici WHERE Sifre=N'Synthetic123!'") == 2);
            string timeColumn = english ? "PlannedCheckInAt" : "PlanlananGirisZamani";
            string breakColumn = english ? "PlannedBreakMinutes" : "PlanlananMolaDakika";
            string table = english ? "AttendanceRecord" : "GirisKayit";
            Check("migration_shift_history_" + label, Scalar(target, "SELECT COUNT(*) FROM dbo." + table + " WHERE CAST(" + timeColumn + " AS time)='" + (snapshot ? "09:00" : "08:00") + "' AND " + breakColumn + "=" + (snapshot ? 45 : 60)) == 1);
            if (snapshot) Check("migration_cancel_history_" + label, Scalar(target, english ? "SELECT COUNT(*) FROM dbo.LeaveRequest WHERE CancelledByUserId=1 AND CancelledAt='20260104'" : "SELECT COUNT(*) FROM dbo.Izin WHERE IptalEdenKullaniciId=1 AND IptalTarihi='20260104'") == 1);
            Check("migration_refuses_populated_target_" + label, Rejected(() => SqlText(target, migration)));
            Check("migration_rejection_preserves_target_" + label, Scalar(target, "SELECT COUNT(*) FROM dbo." + (english ? "Employee" : "Personel")) == 1);
          }
        }
        using (var old = Connect(legacy))
          Check("migration_source_retained_" + snapshot, Scalar(old, "SELECT COUNT(*) FROM dbo.Personel WHERE PersonelId=1") == 1 && Scalar(old, "SELECT COUNT(*) FROM dbo.Kullanici WHERE Sifre=N'Synthetic123!'") == 2);
      }
    }

    private static void ApplicationAccountChecks(string source, string language, string database)
    {
      bool english = language == "English";
      string login = "PersonnelManagementTest_" + Guid.NewGuid().ToString("N");
      _additionalLogins.Add(login);
      string password = "Aa1!" + Guid.NewGuid().ToString("N");
      string script = File.ReadAllText(Path.Combine(source, language, "DatabaseApplicationAccount.sql"))
        .Replace(english ? "PersonnelManagementDB" : "PersonelYonetimiDB", database)
        .Replace(english ? "N'PersonnelManagementApp'" : "N'PersonelYonetimiApp'", "N'" + login + "'")
        .Replace("@SqlPassword nvarchar(128)=NULL", "@SqlPassword nvarchar(128)=N'" + password + "'");
      using (var setup = Connect(database)) SqlText(setup, script);
      var settings = new SqlConnectionStringBuilder(_server.ConnectionString) { InitialCatalog = database, UserID = login, Password = password, IntegratedSecurity = false };
      using (var app = new SqlConnection(settings.ConnectionString))
      {
        app.Open();
        Check("sql_account_not_elevated_" + language, Scalar(app, "SELECT IS_SRVROLEMEMBER('sysadmin')+IS_MEMBER('db_owner')") == 0);
        Check("sql_account_can_read_personnel_" + language, Scalar(app, "SELECT COUNT(*) FROM dbo." + (english ? "Employee" : "Personel")) > 0);
        Check("sql_account_cannot_delete_personnel_" + language, Rejected(() => Execute(app, "DELETE dbo." + (english ? "Employee" : "Personel") + " WHERE 1=0")));
        Check("sql_account_cannot_update_audit_" + language, Rejected(() => Execute(app, english ? "UPDATE dbo.AuditLog SET Description=N'Changed' WHERE 1=0" : "UPDATE dbo.SistemLog SET Aciklama=N'Changed' WHERE 1=0")));
        Check("sql_account_can_read_details_" + language, Scalar(app, "SELECT COUNT(*) FROM dbo." + (english ? "EmployeeDetails" : "PersonelDetay")) > 0);
      }
      using (var setup = Connect(database)) Check("sql_account_repeat_refused_" + language, Rejected(() => SqlText(setup, script)));
    }
  }
}
