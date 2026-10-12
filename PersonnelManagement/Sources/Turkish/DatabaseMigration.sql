-- Boş hedefe aktarım yapar. Aşağıdaki kaynak adını düzenleyin; kaynak korunur.
USE [PersonelYonetimiDB];
GO
SET XACT_ABORT ON;
DECLARE @LegacyDatabase sysname=N'PersonelTakipV090Db';
IF DB_ID(@LegacyDatabase) IS NULL THROW 51000,N'Legacy database is missing / Kaynak veritabanı bulunamadı.',1;
IF @LegacyDatabase=DB_NAME() THROW 51000,N'Source and target must differ / Kaynak ve hedef farklı olmalıdır.',1;
DECLARE @History nvarchar(max)=N'';
IF COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'PlanlananGirisZamani') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'PlanlananCikisZamani') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'PlanlananMolaDakika') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'VardiyaAdiKayit') IS NOT NULL
 SET @History+=N'UPDATE g SET PlanlananGirisZamani=s.PlanlananGirisZamani,PlanlananCikisZamani=s.PlanlananCikisZamani,PlanlananMolaDakika=s.PlanlananMolaDakika,VardiyaAdiKayit=s.VardiyaAdiKayit FROM dbo.GirisKayit g JOIN [PersonelTakipDB].dbo.GirisKayit s ON s.GirisKayitId=g.GirisKayitId;
';
IF COL_LENGTH(@LegacyDatabase+N'.dbo.Izin',N'IptalEdenKullaniciId') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.Izin',N'IptalTarihi') IS NOT NULL
 SET @History+=N'UPDATE i SET IptalEdenKullaniciId=s.IptalEdenKullaniciId,IptalTarihi=s.IptalTarihi FROM dbo.Izin i JOIN [PersonelTakipDB].dbo.Izin s ON s.IzinId=i.IzinId;
';
DECLARE @Migration nvarchar(max)=N'BEGIN TRY
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.[Departman]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Pozisyon]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Vardiya]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Kullanici]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Personel]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[IzinTuru]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[GirisKayit]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Izin]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Devamsizlik]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
IF EXISTS(SELECT 1 FROM dbo.[SistemLog]) THROW 51000,N''Aktarım için hedef veritabanı boş olmalıdır.'',1;
SET IDENTITY_INSERT dbo.[Departman] ON;
INSERT dbo.[Departman]([DepartmanId],[DepartmanAdi],[Aciklama],[AktifMi],[OlusturmaTarihi])
SELECT s.[DepartmanId],s.[DepartmanAdi],s.[Aciklama],s.[AktifMi],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Departman] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Departman] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Departman])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Departman]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[Pozisyon] ON;
INSERT dbo.[Pozisyon]([PozisyonId],[DepartmanId],[PozisyonAdi],[Aciklama],[AktifMi])
SELECT s.[PozisyonId],s.[DepartmanId],s.[PozisyonAdi],s.[Aciklama],s.[AktifMi] FROM [PersonelTakipDB].dbo.[Pozisyon] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Pozisyon] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Pozisyon])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Pozisyon]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[Vardiya] ON;
INSERT dbo.[Vardiya]([VardiyaId],[VardiyaAdi],[BaslangicSaati],[BitisSaati],[MolaDakika],[Pazartesi],[Sali],[Carsamba],[Persembe],[Cuma],[Cumartesi],[Pazar],[AktifMi],[OlusturmaTarihi])
SELECT s.[VardiyaId],s.[VardiyaAdi],s.[BaslangicSaati],s.[BitisSaati],s.[MolaDakika],s.[Pazartesi],s.[Sali],s.[Carsamba],s.[Persembe],s.[Cuma],s.[Cumartesi],s.[Pazar],s.[AktifMi],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Vardiya] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Vardiya] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Vardiya])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Vardiya]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[Kullanici] ON;
INSERT dbo.[Kullanici]([KullaniciId],[KullaniciAdi],[Sifre],[Ad],[Soyad],[Email],[Rol],[AktifMi],[SonGirisTarihi],[OlusturmaTarihi])
SELECT s.[KullaniciId],s.[KullaniciAdi],s.[Sifre],s.[Ad],s.[Soyad],s.[Email],CASE s.[Rol] WHEN N''Yonetici'' THEN N''Yonetici'' WHEN N''Yönetici'' THEN N''Yonetici'' WHEN N''Manager'' THEN N''Yonetici'' WHEN N''Calisan'' THEN N''Personel'' WHEN N''Kullanici'' THEN N''Kullanici'' ELSE s.[Rol] END,s.[AktifMi],s.[SonGirisTarihi],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Kullanici] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Kullanici] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Kullanici])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Kullanici]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[Personel] ON;
INSERT dbo.[Personel]([PersonelId],[TcKimlikNo],[Ad],[Soyad],[Cinsiyet],[DogumTarihi],[Telefon],[Email],[Adres],[FotoPath],[DepartmanId],[PozisyonId],[VardiyaId],[IseGirisTarihi],[IstenCikisTarihi],[YillikIzinHakki],[AktifMi],[OlusturmaTarihi],[GuncellemeTarihi])
SELECT s.[PersonelId],s.[TcKimlikNo],s.[Ad],s.[Soyad],s.[Cinsiyet],s.[DogumTarihi],s.[Telefon],s.[Email],s.[Adres],s.[FotoPath],s.[DepartmanId],s.[PozisyonId],s.[VardiyaId],s.[IseGirisTarihi],s.[IstenCikisTarihi],s.[YillikIzinHakki],s.[AktifMi],s.[OlusturmaTarihi],s.[GuncellemeTarihi] FROM [PersonelTakipDB].dbo.[Personel] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Personel] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Personel])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Personel]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[IzinTuru] ON;
INSERT dbo.[IzinTuru]([IzinTuruId],[TuruAdi],[Ucretli],[AktifMi],[YillikHaktanDuser])
SELECT s.[IzinTuruId],CASE s.[TuruAdi] WHEN N''Yıllık İzin'' THEN N''Yıllık İzin'' WHEN N''Hastalık İzni'' THEN N''Hastalık İzni'' WHEN N''Mazeret İzni'' THEN N''Mazeret İzni'' WHEN N''Ücretsiz İzin'' THEN N''Ücretsiz İzin'' ELSE s.[TuruAdi] END,s.[Ucretli],s.[AktifMi],CASE WHEN s.[TuruAdi] IN (N''Yıllık İzin'',N''Yıllık İzin'') THEN 1 ELSE 0 END FROM [PersonelTakipDB].dbo.[IzinTuru] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[IzinTuru] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[IzinTuru])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[IzinTuru]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[GirisKayit] ON;
INSERT dbo.[GirisKayit]([GirisKayitId],[PersonelId],[KayitTarihi],[GirisSaati],[CikisSaati],[GirisZamani],[CikisZamani],[CalismaDakika],[MesaiDakika],[GecGeldiMi],[ErkenCiktiMi],[Aciklama],[KaydedenKullaniciId],[OlusturmaTarihi])
SELECT s.[GirisKayitId],s.[PersonelId],s.[KayitTarihi],s.[GirisSaati],s.[CikisSaati],__CheckInExpression__,__CheckOutExpression__,s.[CalismaDakika],s.[MesaiDakika],s.[GecGeldiMi],s.[ErkenCiktiMi],s.[Aciklama],s.[KaydedenId],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[GirisKayit] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[GirisKayit] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[GirisKayit])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[GirisKayit]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[Izin] ON;
INSERT dbo.[Izin]([IzinId],[PersonelId],[IzinTuruId],[BaslangicTarihi],[BitisTarihi],[ToplamGun],[Aciklama],[Durum],[OnaylayanKullaniciId],[OnayTarihi],[RedNedeni],[OlusturmaTarihi])
SELECT s.[IzinId],s.[PersonelId],s.[IzinTuruId],s.[BaslangicTarihi],s.[BitisTarihi],s.[ToplamGun],s.[Aciklama],CASE s.[Durum] WHEN N''Beklemede'' THEN N''Beklemede'' WHEN N''Onaylı'' THEN N''Onaylı'' WHEN N''Reddedildi'' THEN N''Reddedildi'' WHEN N''İptal Edildi'' THEN N''İptal Edildi'' ELSE s.[Durum] END,s.[OnayanKullaniciId],s.[OnayTarihi],s.[RedNedeni],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Izin] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Izin] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Izin])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Izin]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[Devamsizlik] ON;
INSERT dbo.[Devamsizlik]([DevamsizlikId],[PersonelId],[DevamsizlikTarihi],[Aciklama],[Mazeret],[OlusturmaTarihi])
SELECT s.[DevamsizlikId],s.[PersonelId],s.[DevamsizlikTarihi],s.[Aciklama],s.[Mazeret],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Devamsizlik] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Devamsizlik] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Devamsizlik])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Devamsizlik]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;
SET IDENTITY_INSERT dbo.[SistemLog] ON;
INSERT dbo.[SistemLog]([SistemLogId],[KullaniciId],[Islem],[TabloAdi],[EtkilenenId],[Aciklama],[LogTarihi])
SELECT s.[SistemLogId],s.[KullaniciId],s.[Islem],s.[TabloAdi],s.[EtkilenenId],s.[Aciklama],s.[LogTarihi] FROM [PersonelTakipDB].dbo.[SistemLog] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[SistemLog] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[SistemLog])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[SistemLog]) THROW 51000,N''Aktarım kayıt sayısı doğrulaması başarısız.'',1;

__OptionalHistory__
UPDATE g SET
    GirisZamani=COALESCE(GirisZamani,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),GirisSaati),CAST(KayitTarihi AS datetime))),
    CikisZamani=COALESCE(CikisZamani,CASE WHEN CikisSaati IS NOT NULL THEN DATEADD(day,CASE WHEN CikisSaati<GirisSaati THEN 1 ELSE 0 END,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),CikisSaati),CAST(KayitTarihi AS datetime))) END),
    PlanlananGirisZamani=COALESCE(g.PlanlananGirisZamani,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.BaslangicSaati),CAST(g.KayitTarihi AS datetime))),
    PlanlananCikisZamani=COALESCE(g.PlanlananCikisZamani,DATEADD(day,CASE WHEN v.BitisSaati<=v.BaslangicSaati THEN 1 ELSE 0 END,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.BitisSaati),CAST(g.KayitTarihi AS datetime)))),
    PlanlananMolaDakika=COALESCE(g.PlanlananMolaDakika,v.MolaDakika),VardiyaAdiKayit=COALESCE(g.VardiyaAdiKayit,v.VardiyaAdi)
FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;
';
SET @Migration=REPLACE(@Migration,N'__OptionalHistory__',@History);
SET @Migration=REPLACE(@Migration,N'__CheckInExpression__',CASE WHEN COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'GirisZamani') IS NOT NULL THEN N's.[GirisZamani]' ELSE N'NULL' END);
SET @Migration=REPLACE(@Migration,N'__CheckOutExpression__',CASE WHEN COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'CikisZamani') IS NOT NULL THEN N's.[CikisZamani]' ELSE N'NULL' END);
SET @Migration=REPLACE(@Migration,N'[PersonelTakipDB]',QUOTENAME(@LegacyDatabase));
EXEC sys.sp_executesql @Migration;
GO
