-- Mevcut veritabanını silmeden şemayı kurar. İki kurulum alternatifinden yalnız birini çalıştırın.
SET NOEXEC OFF;
USE [master];
GO
EXEC sys.sp_set_session_context @key=N'PersonnelManagementSetupCreated',@value=0;
IF DB_ID(N'PersonelYonetimiDB') IS NOT NULL
BEGIN
    RAISERROR(N'Veritabanı zaten var. Boş ve farklı bir veritabanı adı kullanın; mevcut kayıtların üzerine yazılmaz.',16,1);
    SET NOEXEC ON;
END
ELSE
BEGIN
    EXEC(N'CREATE DATABASE [PersonelYonetimiDB] COLLATE Turkish_CI_AS;');
    EXEC sys.sp_set_session_context @key=N'PersonnelManagementSetupCreated',@value=1;
END;
GO
USE [PersonelYonetimiDB];
GO
IF ISNULL(CONVERT(int,SESSION_CONTEXT(N'PersonnelManagementSetupCreated')),0)<>1 THROW 51000,N'Kurulum bu veritabanını oluşturmadı.',1;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
CREATE TABLE dbo.Departman (
    [DepartmanId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Departman PRIMARY KEY,
    [DepartmanAdi] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [Aciklama] [nvarchar](255) COLLATE Turkish_CI_AS NULL,
    [AktifMi] [bit] NOT NULL CONSTRAINT DF_Departman_AktifMi DEFAULT ((1)),
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_Departman_OlusturmaTarihi DEFAULT (getdate())
);

CREATE TABLE dbo.Pozisyon (
    [PozisyonId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pozisyon PRIMARY KEY,
    [DepartmanId] [int] NOT NULL,
    [PozisyonAdi] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [Aciklama] [nvarchar](255) COLLATE Turkish_CI_AS NULL,
    [AktifMi] [bit] NOT NULL CONSTRAINT DF_Pozisyon_AktifMi DEFAULT ((1))
);

CREATE TABLE dbo.Vardiya (
    [VardiyaId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Vardiya PRIMARY KEY,
    [VardiyaAdi] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [BaslangicSaati] [time](0) NOT NULL,
    [BitisSaati] [time](0) NOT NULL,
    [MolaDakika] [int] NOT NULL CONSTRAINT DF_Vardiya_MolaDakika DEFAULT ((0)),
    [Pazartesi] [bit] NOT NULL CONSTRAINT DF_Vardiya_Pazartesi DEFAULT ((1)),
    [Sali] [bit] NOT NULL CONSTRAINT DF_Vardiya_Sali DEFAULT ((1)),
    [Carsamba] [bit] NOT NULL CONSTRAINT DF_Vardiya_Carsamba DEFAULT ((1)),
    [Persembe] [bit] NOT NULL CONSTRAINT DF_Vardiya_Persembe DEFAULT ((1)),
    [Cuma] [bit] NOT NULL CONSTRAINT DF_Vardiya_Cuma DEFAULT ((1)),
    [Cumartesi] [bit] NOT NULL CONSTRAINT DF_Vardiya_Cumartesi DEFAULT ((0)),
    [Pazar] [bit] NOT NULL CONSTRAINT DF_Vardiya_Pazar DEFAULT ((0)),
    [AktifMi] [bit] NOT NULL CONSTRAINT DF_Vardiya_AktifMi DEFAULT ((1)),
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_Vardiya_OlusturmaTarihi DEFAULT (getdate())
);

CREATE TABLE dbo.Personel (
    [PersonelId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Personel PRIMARY KEY,
    [TcKimlikNo] [char](11) COLLATE Turkish_CI_AS NOT NULL,
    [Ad] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Soyad] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Cinsiyet] [nvarchar](10) COLLATE Turkish_CI_AS NULL,
    [DogumTarihi] [date] NULL,
    [Telefon] [nvarchar](20) COLLATE Turkish_CI_AS NULL,
    [Email] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
    [Adres] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [FotoPath] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [DepartmanId] [int] NOT NULL,
    [PozisyonId] [int] NOT NULL,
    [VardiyaId] [int] NOT NULL,
    [IseGirisTarihi] [date] NOT NULL,
    [IstenCikisTarihi] [date] NULL,
    [YillikIzinHakki] [int] NOT NULL CONSTRAINT DF_Personel_YillikIzinHakki DEFAULT ((14)),
    [AktifMi] [bit] NOT NULL CONSTRAINT DF_Personel_AktifMi DEFAULT ((1)),
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_Personel_OlusturmaTarihi DEFAULT (getdate()),
    [GuncellemeTarihi] [datetime] NULL
);

CREATE TABLE dbo.GirisKayit (
    [GirisKayitId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_GirisKayit PRIMARY KEY,
    [PersonelId] [int] NOT NULL,
    [KayitTarihi] [date] NOT NULL,
    [GirisSaati] [time](0) NULL,
    [CikisSaati] [time](0) NULL,
    [GirisZamani] [datetime] NULL,
    [CikisZamani] [datetime] NULL,
    [CalismaDakika] [int] NULL,
    [MesaiDakika] [int] NULL CONSTRAINT DF_GirisKayit_MesaiDakika DEFAULT ((0)),
    [GecGeldiMi] [bit] NOT NULL CONSTRAINT DF_GirisKayit_GecGeldiMi DEFAULT ((0)),
    [ErkenCiktiMi] [bit] NOT NULL CONSTRAINT DF_GirisKayit_ErkenCiktiMi DEFAULT ((0)),
    [Aciklama] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [KaydedenKullaniciId] [int] NULL,
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_GirisKayit_OlusturmaTarihi DEFAULT (getdate()),
    [PlanlananGirisZamani] datetime NULL,
    [PlanlananCikisZamani] datetime NULL,
    [PlanlananMolaDakika] int NULL,
    [VardiyaAdiKayit] nvarchar(100) NULL
);

CREATE TABLE dbo.IzinTuru (
    [IzinTuruId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_IzinTuru PRIMARY KEY,
    [TuruAdi] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [Ucretli] [bit] NOT NULL CONSTRAINT DF_IzinTuru_Ucretli DEFAULT ((1)),
    [AktifMi] [bit] NOT NULL CONSTRAINT DF_IzinTuru_AktifMi DEFAULT ((1)),
    [YillikHaktanDuser] bit NOT NULL CONSTRAINT DF_IzinTuru_YillikHaktanDuser DEFAULT (0)
);

CREATE TABLE dbo.Izin (
    [IzinId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Izin PRIMARY KEY,
    [PersonelId] [int] NOT NULL,
    [IzinTuruId] [int] NOT NULL,
    [BaslangicTarihi] [date] NOT NULL,
    [BitisTarihi] [date] NOT NULL,
    [ToplamGun] [int] NOT NULL,
    [Aciklama] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [Durum] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL CONSTRAINT DF_Izin_Durum DEFAULT ('Pending'),
    [OnaylayanKullaniciId] [int] NULL,
    [OnayTarihi] [datetime] NULL,
    [RedNedeni] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_Izin_OlusturmaTarihi DEFAULT (getdate()),
    [IptalEdenKullaniciId] int NULL,
    [IptalTarihi] datetime NULL
);

CREATE TABLE dbo.Kullanici (
    [KullaniciId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kullanici PRIMARY KEY,
    [KullaniciAdi] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Sifre] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [Ad] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Soyad] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Email] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
    [Rol] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL CONSTRAINT DF_Kullanici_Rol DEFAULT ('Personel'),
    [AktifMi] [bit] NOT NULL CONSTRAINT DF_Kullanici_AktifMi DEFAULT ((1)),
    [SonGirisTarihi] [datetime] NULL,
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_Kullanici_OlusturmaTarihi DEFAULT (getdate())
);

CREATE TABLE dbo.SistemLog (
    [SistemLogId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_SistemLog PRIMARY KEY,
    [KullaniciId] [int] NULL,
    [Islem] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [TabloAdi] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
    [EtkilenenId] [int] NULL,
    [Aciklama] [nvarchar](1000) COLLATE Turkish_CI_AS NULL,
    [LogTarihi] [datetime] NOT NULL CONSTRAINT DF_SistemLog_LogTarihi DEFAULT (getdate())
);

CREATE TABLE dbo.Devamsizlik (
    [DevamsizlikId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Devamsizlik PRIMARY KEY,
    [PersonelId] [int] NOT NULL,
    [DevamsizlikTarihi] [date] NOT NULL,
    [Aciklama] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [Mazeret] [nvarchar](200) COLLATE Turkish_CI_AS NULL,
    [OlusturmaTarihi] [datetime] NOT NULL CONSTRAINT DF_Devamsizlik_OlusturmaTarihi DEFAULT (getdate())
);

ALTER TABLE dbo.Kullanici ADD CONSTRAINT UQ_Kullanici_KullaniciAdi UNIQUE (KullaniciAdi);
ALTER TABLE dbo.Kullanici ADD CONSTRAINT CK_Kullanici_Rol CHECK (Rol IN (N'Admin',N'Yonetici',N'Personel',N'Kullanici'));
ALTER TABLE dbo.Personel ADD CONSTRAINT UQ_Personel_TcKimlikNo UNIQUE (TcKimlikNo);
ALTER TABLE dbo.Personel ADD CONSTRAINT CK_Personel_NationalIdDigits CHECK (LEN(TcKimlikNo)=11 AND TcKimlikNo COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%');
ALTER TABLE dbo.Personel ADD CONSTRAINT CK_Personel_BirthDate CHECK (DogumTarihi IS NULL OR (DogumTarihi <= CONVERT(date,GETDATE()) AND DogumTarihi <= IseGirisTarihi));
CREATE INDEX IX_Personel_NameLookup ON dbo.Personel(Ad,Soyad,PersonelId) INCLUDE (AktifMi);
CREATE INDEX IX_Personel_ActiveShift ON dbo.Personel(VardiyaId) INCLUDE (PersonelId) WHERE AktifMi=1;

ALTER TABLE dbo.Personel ADD CONSTRAINT CK_Personel_YillikIzinHakki CHECK (YillikIzinHakki >= 0);
ALTER TABLE dbo.Personel ADD CONSTRAINT CK_Personel_EmploymentDates CHECK (IstenCikisTarihi IS NULL OR IstenCikisTarihi >= IseGirisTarihi);
ALTER TABLE dbo.Pozisyon ADD CONSTRAINT UQ_Pozisyon_Departman UNIQUE (PozisyonId,DepartmanId);
ALTER TABLE dbo.Pozisyon ADD CONSTRAINT FK_Pozisyon_Departman FOREIGN KEY (DepartmanId) REFERENCES dbo.Departman(DepartmanId);
ALTER TABLE dbo.Personel ADD CONSTRAINT FK_Personel_Departman FOREIGN KEY (DepartmanId) REFERENCES dbo.Departman(DepartmanId);
ALTER TABLE dbo.Personel ADD CONSTRAINT FK_Personel_PositionDepartment FOREIGN KEY (PozisyonId,DepartmanId) REFERENCES dbo.Pozisyon(PozisyonId,DepartmanId);
ALTER TABLE dbo.Personel ADD CONSTRAINT FK_Personel_Vardiya FOREIGN KEY (VardiyaId) REFERENCES dbo.Vardiya(VardiyaId);
ALTER TABLE dbo.GirisKayit ADD CONSTRAINT FK_GirisKayit_Personel FOREIGN KEY (PersonelId) REFERENCES dbo.Personel(PersonelId);
ALTER TABLE dbo.GirisKayit ADD CONSTRAINT FK_GirisKayit_RecordedBy FOREIGN KEY (KaydedenKullaniciId) REFERENCES dbo.Kullanici(KullaniciId);
ALTER TABLE dbo.Izin ADD CONSTRAINT FK_Izin_Personel FOREIGN KEY (PersonelId) REFERENCES dbo.Personel(PersonelId);
ALTER TABLE dbo.Izin ADD CONSTRAINT FK_Izin_Type FOREIGN KEY (IzinTuruId) REFERENCES dbo.IzinTuru(IzinTuruId);
ALTER TABLE dbo.Izin ADD CONSTRAINT FK_Izin_ReviewedBy FOREIGN KEY (OnaylayanKullaniciId) REFERENCES dbo.Kullanici(KullaniciId);
ALTER TABLE dbo.Izin ADD CONSTRAINT FK_Izin_CancelledBy FOREIGN KEY (IptalEdenKullaniciId) REFERENCES dbo.Kullanici(KullaniciId);
ALTER TABLE dbo.Devamsizlik ADD CONSTRAINT FK_Devamsizlik_Personel FOREIGN KEY (PersonelId) REFERENCES dbo.Personel(PersonelId);
ALTER TABLE dbo.SistemLog ADD CONSTRAINT FK_SistemLog_User FOREIGN KEY (KullaniciId) REFERENCES dbo.Kullanici(KullaniciId);
ALTER TABLE dbo.Vardiya ADD CONSTRAINT CK_Vardiya_MolaDakika CHECK (MolaDakika >= 0 AND MolaDakika < CASE WHEN BitisSaati > BaslangicSaati THEN DATEDIFF(minute,BaslangicSaati,BitisSaati) ELSE DATEDIFF(minute,BaslangicSaati,BitisSaati)+1440 END);
ALTER TABLE dbo.Izin ADD CONSTRAINT CK_Izin_Tarihler CHECK (BitisTarihi >= BaslangicTarihi AND ToplamGun = DATEDIFF(day,BaslangicTarihi,BitisTarihi)+1);
ALTER TABLE dbo.Izin ADD CONSTRAINT CK_Izin_Durum CHECK (Durum IN (N'Beklemede',N'Onaylı',N'Reddedildi',N'İptal Edildi'));
ALTER TABLE dbo.GirisKayit ADD CONSTRAINT CK_GirisKayit_Duration CHECK ((CalismaDakika IS NULL OR CalismaDakika >= 0) AND (MesaiDakika IS NULL OR MesaiDakika >= 0) AND (CikisZamani IS NULL OR GirisZamani IS NULL OR CikisZamani >= GirisZamani));
CREATE UNIQUE INDEX UX_GirisKayit_EmployeeDate ON dbo.GirisKayit(PersonelId,KayitTarihi) INCLUDE (GirisZamani,CalismaDakika,MesaiDakika,GecGeldiMi,ErkenCiktiMi);
CREATE UNIQUE INDEX UX_GirisKayit_Open ON dbo.GirisKayit(PersonelId) WHERE CikisSaati IS NULL;
CREATE UNIQUE INDEX UX_Devamsizlik_EmployeeDate ON dbo.Devamsizlik(PersonelId,DevamsizlikTarihi);
CREATE INDEX IX_Izin_EmployeeDates ON dbo.Izin(PersonelId,BaslangicTarihi,BitisTarihi) INCLUDE (Durum,IzinTuruId);
CREATE INDEX IX_GirisKayit_Tarih ON dbo.GirisKayit(KayitTarihi) INCLUDE (PersonelId,CalismaDakika,MesaiDakika,GecGeldiMi,ErkenCiktiMi);

EXEC sys.sp_executesql N'CREATE OR ALTER VIEW PersonelDetay AS
    SELECT
        p.PersonelId,
        p.TcKimlikNo,
        p.Ad,
        p.Soyad,
        p.Ad + '' '' + p.Soyad  AS AdSoyad,
        p.Cinsiyet,
        p.DogumTarihi,
        p.Telefon,
        p.Email,
        p.Adres,
        p.FotoPath,
        p.DepartmanId,
        d.DepartmanAdi,
        p.PozisyonId,
        pz.PozisyonAdi,
        p.VardiyaId,
        v.VardiyaAdi,
        v.BaslangicSaati,
        v.BitisSaati,
        v.MolaDakika,
        p.IseGirisTarihi,
        p.IstenCikisTarihi,
        p.YillikIzinHakki,
        p.AktifMi,
        p.OlusturmaTarihi,
        p.GuncellemeTarihi
    FROM       Personel  p
    INNER JOIN Departman d  ON p.DepartmanId = d.DepartmanId
    INNER JOIN Pozisyon  pz ON p.PozisyonId  = pz.PozisyonId
    INNER JOIN Vardiya   v  ON p.VardiyaId   = v.VardiyaId;
';
EXEC sys.sp_executesql N'CREATE OR ALTER VIEW dbo.GunlukGirisOzeti AS
SELECT g.GirisKayitId,g.KayitTarihi,p.PersonelId,p.Ad+'' ''+p.Soyad AS AdSoyad,d.DepartmanAdi,
    COALESCE(g.VardiyaAdiKayit,v.VardiyaAdi) AS VardiyaAdi,
    CAST(g.PlanlananGirisZamani AS time) AS PlanlananGiris,CAST(g.PlanlananCikisZamani AS time) AS PlanlananCikis,
    g.GirisSaati,g.CikisSaati,g.GirisZamani,g.CikisZamani,g.CalismaDakika,g.MesaiDakika,g.GecGeldiMi,g.ErkenCiktiMi,g.Aciklama
FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId
JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId;';
EXEC sys.sp_executesql N'CREATE OR ALTER VIEW dbo.YillikIzinOzeti AS
SELECT p.PersonelId,p.Ad+'' ''+p.Soyad AS AdSoyad,p.YillikIzinHakki,
    ISNULL(x.Kullanilan,0) AS KullanilanYillikIzin,p.YillikIzinHakki-ISNULL(x.Kullanilan,0) AS KalanYillikIzin
FROM dbo.Personel p OUTER APPLY
(
    SELECT SUM(DATEDIFF(day,
        CASE WHEN i.BaslangicTarihi<DATEFROMPARTS(YEAR(GETDATE()),1,1) THEN DATEFROMPARTS(YEAR(GETDATE()),1,1) ELSE i.BaslangicTarihi END,
        CASE WHEN i.BitisTarihi>DATEFROMPARTS(YEAR(GETDATE()),12,31) THEN DATEFROMPARTS(YEAR(GETDATE()),12,31) ELSE i.BitisTarihi END)+1) AS Kullanilan
    FROM dbo.Izin i JOIN dbo.IzinTuru it ON it.IzinTuruId=i.IzinTuruId
    WHERE i.PersonelId=p.PersonelId AND i.Durum=N''Onaylı'' AND it.YillikHaktanDuser=1
      AND i.BitisTarihi>=DATEFROMPARTS(YEAR(GETDATE()),1,1) AND i.BaslangicTarihi<=DATEFROMPARTS(YEAR(GETDATE()),12,31)
) x;';
EXEC sys.sp_executesql N'CREATE OR ALTER FUNCTION dbo.DevamsizliklariGetir(@Start date,@End date,@Simdi datetime)
RETURNS TABLE AS RETURN
(
    WITH Tarihler AS
    (
        SELECT @Start AS Tarih WHERE @Start<=@End
        UNION ALL SELECT DATEADD(day,1,Tarih) FROM Tarihler WHERE Tarih<@End
    ), Beklenen AS
    (
        SELECT p.PersonelId,p.Ad+'' ''+p.Soyad AS PersonelAdSoyad,d.DepartmanAdi,t.Tarih,
            DATEADD(day,CASE WHEN v.BitisSaati<=v.BaslangicSaati THEN 1 ELSE 0 END,
                DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.BitisSaati),CAST(t.Tarih AS datetime))) AS PlanlananBitis
        FROM Tarihler t JOIN dbo.Personel p ON p.IseGirisTarihi<=t.Tarih
            AND (p.IstenCikisTarihi IS NULL OR p.IstenCikisTarihi>=t.Tarih)
            AND (p.AktifMi=1 OR p.IstenCikisTarihi IS NOT NULL)
        JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId
        WHERE CASE ((DATEDIFF(day,CONVERT(date,''19000101''),t.Tarih)%7)+7)%7
            WHEN 0 THEN v.Pazartesi WHEN 1 THEN v.Sali WHEN 2 THEN v.Carsamba WHEN 3 THEN v.Persembe
            WHEN 4 THEN v.Cuma WHEN 5 THEN v.Cumartesi ELSE v.Pazar END=1
    ), Otomatik AS
    (
        SELECT b.* FROM Beklenen b WHERE b.PlanlananBitis<=@Simdi
        AND NOT EXISTS(SELECT 1 FROM dbo.GirisKayit g WHERE g.PersonelId=b.PersonelId AND g.KayitTarihi=b.Tarih AND g.GirisZamani IS NOT NULL)
        AND NOT EXISTS(SELECT 1 FROM dbo.Izin i WHERE i.PersonelId=b.PersonelId AND i.Durum=N''Onaylı'' AND b.Tarih BETWEEN i.BaslangicTarihi AND i.BitisTarihi)
    )
    SELECT dv.DevamsizlikId,dv.PersonelId,p.Ad+'' ''+p.Soyad AS PersonelAdSoyad,d.DepartmanAdi,
        dv.DevamsizlikTarihi,dv.Mazeret,dv.Aciklama
    FROM dbo.Devamsizlik dv JOIN dbo.Personel p ON p.PersonelId=dv.PersonelId JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId
    WHERE dv.DevamsizlikTarihi BETWEEN @Start AND @End
    UNION ALL
    SELECT CAST(NULL AS int),o.PersonelId,o.PersonelAdSoyad,o.DepartmanAdi,o.Tarih,
        CAST(NULL AS nvarchar(500)),CAST(N''Vardiya tamamlandı; giriş veya onaylı izin bulunamadı.'' AS nvarchar(500))
    FROM Otomatik o WHERE NOT EXISTS(SELECT 1 FROM dbo.Devamsizlik dv WHERE dv.PersonelId=o.PersonelId AND dv.DevamsizlikTarihi=o.Tarih)
);';
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
