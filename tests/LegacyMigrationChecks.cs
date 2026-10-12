using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

internal static partial class RegressionChecks
{
    private static int LegacyScalar(SqlConnection connection, string sql)
    {
        using (var command = new SqlCommand(sql, connection))
            return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void LegacyMigrationChecks(string[] args)
    {
        string name = "PersonnelManagementFix_" + Guid.NewGuid().ToString("N");
        if (!Regex.IsMatch(name, "^PersonnelManagementFix_[a-f0-9]{32}$")
            || name.Equals(server.InitialCatalog, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Legacy scratch database safety guard failed.");
        try
        {
            using (var master = Connect("master"))
                RunBatches("CREATE DATABASE [" + name + "] COLLATE Turkish_CI_AS;", master);
            using (var legacy = Connect(name))
            {
                RunBatches(File.ReadAllText(args[3]), legacy);
                RunBatches(@"
INSERT dbo.Departman(DepartmanAdi) VALUES(N'Synthetic legacy department');
INSERT dbo.Pozisyon(DepartmanId,PozisyonAdi) VALUES(1,N'Synthetic legacy position');
INSERT dbo.Vardiya(VardiyaAdi,BaslangicSaati,BitisSaati,MolaDakika)
    VALUES(N'Synthetic legacy overnight','22:00','06:00',30);
INSERT dbo.Personel(TcKimlikNo,Ad,Soyad,DepartmanId,PozisyonId,VardiyaId,IseGirisTarihi)
    VALUES('87777777770',N'Synthetic',N'Legacy',1,1,1,'20200101');
INSERT dbo.Kullanici(KullaniciAdi,Sifre,Ad,Soyad,Rol)
    VALUES(N'legacy_fixture',N'FixtureOnly123!',N'Synthetic',N'Legacy',N'Admin');
INSERT dbo.IzinTuru(TuruAdi) VALUES(N'Yıllık İzin');
INSERT dbo.Izin(PersonelId,IzinTuruId,BaslangicTarihi,BitisTarihi,ToplamGun)
    VALUES(1,1,'20260105','20260106',2);
INSERT dbo.GirisKayit(PersonelId,KayitTarihi,GirisSaati,CikisSaati,GirisZamani,CikisZamani,CalismaDakika)
    VALUES(1,'20261005','22:00','06:00','2026-10-05T22:00:00','2026-10-06T06:00:00',450);
INSERT dbo.Devamsizlik(PersonelId,DevamsizlikTarihi,Mazeret) VALUES(1,'20261001',N'Synthetic reason');
INSERT dbo.SistemLog(KullaniciId,Islem,Aciklama) VALUES(1,N'Legacy',N'Synthetic log preserved');
INSERT dbo.GirisKayit(PersonelId,KayitTarihi,GirisSaati,GirisZamani)
    VALUES(1,'20261005','22:00','2026-10-05T22:00:00');", legacy);

                string update = File.ReadAllText(Path.Combine(Path.GetDirectoryName(args[1]), "DatabaseUpdate.sql"));
                bool duplicateStopped = false;
                try { RunBatches(update, legacy); }
                catch (SqlException ex) { duplicateStopped = ex.Number == 51001; }
                Observe("legacy_duplicate_stops_migration", duplicateStopped, "duplicate legacy attendance stops the schema update");
                Observe("legacy_duplicate_rows_preserved", LegacyScalar(legacy, "SELECT COUNT(*) FROM dbo.GirisKayit") == 2,
                    "rejected update did not delete duplicate records");
                Observe("legacy_duplicate_schema_unchanged", LegacyScalar(legacy,
                    "SELECT CASE WHEN COL_LENGTH('dbo.GirisKayit','PlanlananGirisZamani') IS NULL THEN 1 ELSE 0 END") == 1,
                    "duplicate check runs before adding columns");

                // Yalnız test için eklenen mükerrer satır kaldırılır; kurulum scripti veri silmez.
                RunBatches("DELETE dbo.GirisKayit WHERE GirisKayitId=2; UPDATE dbo.Personel SET YillikIzinHakki=-1;", legacy);
                bool invalidStopped = false;
                try { RunBatches(update, legacy); }
                catch (SqlException ex) { invalidStopped = ex.Number == 547; }
                Observe("legacy_invalid_data_stops_migration", invalidStopped, "invalid legacy data stops checked constraints");
                Observe("legacy_partial_ddl_rolled_back", LegacyScalar(legacy,
                    "SELECT CASE WHEN COL_LENGTH('dbo.GirisKayit','PlanlananGirisZamani') IS NULL THEN 1 ELSE 0 END") == 1,
                    "failed constraint validation rolls back preceding schema changes");
                Observe("legacy_invalid_row_preserved", LegacyScalar(legacy,
                    "SELECT COUNT(*) FROM dbo.Personel WHERE YillikIzinHakki=-1") == 1,
                    "invalid values are preserved");
                RunBatches("UPDATE dbo.Personel SET YillikIzinHakki=14;", legacy);

                string[] tables = { "Departman", "Pozisyon", "Vardiya", "Personel", "Kullanici", "GirisKayit", "Devamsizlik", "IzinTuru", "Izin", "SistemLog" };
                int[] before = tables.Select(t => LegacyScalar(legacy, "SELECT COUNT(*) FROM dbo.[" + t + "]")).ToArray();
                RunBatches(update, legacy);
                Observe("legacy_all_tables_preserved", before.SequenceEqual(tables.Select(t => LegacyScalar(legacy, "SELECT COUNT(*) FROM dbo.[" + t + "]"))),
                    "all ten old tables retain their records after update");
                Observe("legacy_overnight_plan", LegacyScalar(legacy, @"SELECT COUNT(*) FROM dbo.GirisKayit
                    WHERE GirisKayitId=1 AND PlanlananGirisZamani='2026-10-05T22:00:00'
                    AND PlanlananCikisZamani='2026-10-06T06:00:00' AND PlanlananMolaDakika=30
                    AND GirisZamani='2026-10-05T22:00:00' AND CikisZamani='2026-10-06T06:00:00'
                    AND CalismaDakika=450") == 1, "old overnight plan backfilled without changing actual attendance");
                Observe("legacy_cancel_columns", LegacyScalar(legacy,
                    "SELECT CASE WHEN COL_LENGTH('dbo.Izin','IptalEdenKullaniciId') IS NOT NULL AND COL_LENGTH('dbo.Izin','IptalTarihi') IS NOT NULL THEN 1 ELSE 0 END") == 1,
                    "cancellation metadata available on old schema");
                RunBatches(update, legacy);
                Observe("legacy_update_repeat", before.SequenceEqual(tables.Select(t => LegacyScalar(legacy, "SELECT COUNT(*) FROM dbo.[" + t + "]"))),
                    "repeated migration retains every old table count");
            }
        }
        finally
        {
            SqlConnection.ClearAllPools();
            using (var master = Connect("master"))
                RunBatches("IF DB_ID(N'" + name + "') IS NOT NULL BEGIN ALTER DATABASE [" + name
                    + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + name + "]; END", master);
            Console.WriteLine("LEGACY_SCRATCH_DATABASE_REMOVED=true");
        }
    }
}
