-- PersonnelManagement 0.9.0 - Yeni kurulum
-- Var olan veritabani silinmez. Mevcut kurulum icin DatabaseUpdate.sql kullanilir.
-- Dosyanin tamami tek batch olarak calistirilir.
USE [master];
SET NOCOUNT ON;
IF DB_ID(N'PersonelTakipV090Db') IS NOT NULL
 THROW 51000, N'PersonelTakipV090Db zaten mevcut. Veriler korunuyor; DatabaseUpdate.sql dosyasini kullanin.', 1;
EXEC(N'CREATE DATABASE [PersonelTakipV090Db] COLLATE Turkish_CI_AS;');
EXEC sys.sp_executesql N'USE [PersonelTakipV090Db];
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
EXEC sys.sp_executesql N''-- ============================================================
-- 1. Departman
-- ============================================================
CREATE TABLE Departman
(
    DepartmanId     INT           IDENTITY(1,1) PRIMARY KEY,
    DepartmanAdi    NVARCHAR(100) NOT NULL,
    Aciklama        NVARCHAR(255) NULL,
    AktifMi         BIT           NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME      NOT NULL DEFAULT GETDATE()
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 2. Pozisyon
-- ============================================================
CREATE TABLE Pozisyon
(
    PozisyonId      INT           IDENTITY(1,1) PRIMARY KEY,
    DepartmanId     INT           NOT NULL,
    PozisyonAdi     NVARCHAR(100) NOT NULL,
    Aciklama        NVARCHAR(255) NULL,
    AktifMi         BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FkPozisyonDepartman FOREIGN KEY (DepartmanId)
        REFERENCES Departman(DepartmanId)
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 3. Vardiya
-- ============================================================
CREATE TABLE Vardiya
(
    VardiyaId        INT           IDENTITY(1,1) PRIMARY KEY,
    VardiyaAdi       NVARCHAR(100) NOT NULL,
    BaslangicSaati   TIME(0)       NOT NULL,
    BitisSaati       TIME(0)       NOT NULL,
    MolaDakika       INT           NOT NULL DEFAULT 0,
    Pazartesi        BIT           NOT NULL DEFAULT 1,
    Sali             BIT           NOT NULL DEFAULT 1,
    Carsamba         BIT           NOT NULL DEFAULT 1,
    Persembe         BIT           NOT NULL DEFAULT 1,
    Cuma             BIT           NOT NULL DEFAULT 1,
    Cumartesi        BIT           NOT NULL DEFAULT 0,
    Pazar            BIT           NOT NULL DEFAULT 0,
    AktifMi          BIT           NOT NULL DEFAULT 1,
    OlusturmaTarihi  DATETIME      NOT NULL DEFAULT GETDATE()
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 4. Personel
-- ============================================================
CREATE TABLE Personel
(
    PersonelId        INT            IDENTITY(1,1) PRIMARY KEY,
    TcKimlikNo        CHAR(11)       NOT NULL UNIQUE,
    Ad                NVARCHAR(50)   NOT NULL,
    Soyad             NVARCHAR(50)   NOT NULL,
    Cinsiyet          NVARCHAR(10)   NULL,
    DogumTarihi       DATE           NULL,
    Telefon           NVARCHAR(20)   NULL,
    Email             NVARCHAR(100)  NULL,
    Adres             NVARCHAR(500)  NULL,
    FotoPath          NVARCHAR(500)  NULL,
    DepartmanId       INT            NOT NULL,
    PozisyonId        INT            NOT NULL,
    VardiyaId         INT            NOT NULL,
    IseGirisTarihi    DATE           NOT NULL,
    IstenCikisTarihi  DATE           NULL,
    YillikIzinHakki   INT            NOT NULL DEFAULT 14,
    AktifMi           BIT            NOT NULL DEFAULT 1,
    OlusturmaTarihi   DATETIME       NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi  DATETIME       NULL,
    CONSTRAINT FkPersonelDepartman FOREIGN KEY (DepartmanId)
        REFERENCES Departman(DepartmanId),
    CONSTRAINT FkPersonelPozisyon  FOREIGN KEY (PozisyonId)
        REFERENCES Pozisyon(PozisyonId),
    CONSTRAINT FkPersonelVardiya   FOREIGN KEY (VardiyaId)
        REFERENCES Vardiya(VardiyaId)
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 5. Kullanici
-- ============================================================
CREATE TABLE Kullanici
(
    KullaniciId     INT           IDENTITY(1,1) PRIMARY KEY,
    KullaniciAdi    NVARCHAR(50)  NOT NULL UNIQUE,
    Sifre           NVARCHAR(100) NOT NULL,
    Ad              NVARCHAR(50)  NOT NULL,
    Soyad           NVARCHAR(50)  NOT NULL,
    Email           NVARCHAR(100) NULL,
    Rol             NVARCHAR(20)  NOT NULL DEFAULT ''''Kullanici'''',
    AktifMi         BIT           NOT NULL DEFAULT 1,
    SonGirisTarihi  DATETIME      NULL,
    OlusturmaTarihi DATETIME      NOT NULL DEFAULT GETDATE()
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 6. GirisKayit
-- ============================================================
CREATE TABLE GirisKayit
(
    GirisKayitId     INT           IDENTITY(1,1) PRIMARY KEY,
    PersonelId       INT           NOT NULL,
    KayitTarihi      DATE          NOT NULL,
    GirisSaati       TIME(0)       NULL,
    CikisSaati       TIME(0)       NULL,
    GirisZamani      DATETIME      NULL,
    CikisZamani      DATETIME      NULL,
    CalismaDakika    INT           NULL,
    MesaiDakika      INT           NULL DEFAULT 0,
    GecGeldiMi       BIT           NOT NULL DEFAULT 0,
    ErkenCiktiMi     BIT           NOT NULL DEFAULT 0,
    Aciklama         NVARCHAR(500) NULL,
    KaydedenId       INT           NULL,
    OlusturmaTarihi  DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FkGirisKayitPersonel  FOREIGN KEY (PersonelId)
        REFERENCES Personel(PersonelId),
    CONSTRAINT FkGirisKayitKullanici FOREIGN KEY (KaydedenId)
        REFERENCES Kullanici(KullaniciId)
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 7. Devamsizlik
-- ============================================================
CREATE TABLE Devamsizlik
(
    DevamsizlikId     INT           IDENTITY(1,1) PRIMARY KEY,
    PersonelId        INT           NOT NULL,
    DevamsizlikTarihi DATE          NOT NULL,
    Aciklama          NVARCHAR(500) NULL,
    Mazeret           NVARCHAR(200) NULL,
    OlusturmaTarihi   DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FkDevamsizlikPersonel FOREIGN KEY (PersonelId)
        REFERENCES Personel(PersonelId)
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 8. IzinTuru
-- ============================================================
CREATE TABLE IzinTuru
(
    IzinTuruId  INT           IDENTITY(1,1) PRIMARY KEY,
    TuruAdi     NVARCHAR(100) NOT NULL,
    Ucretli     BIT           NOT NULL DEFAULT 1,
    AktifMi     BIT           NOT NULL DEFAULT 1
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 9. Izin
-- ============================================================
CREATE TABLE Izin
(
    IzinId           INT           IDENTITY(1,1) PRIMARY KEY,
    PersonelId       INT           NOT NULL,
    IzinTuruId       INT           NOT NULL,
    BaslangicTarihi  DATE          NOT NULL,
    BitisTarihi      DATE          NOT NULL,
    ToplamGun        INT           NOT NULL,
    Aciklama         NVARCHAR(500) NULL,
    Durum            NVARCHAR(20)  NOT NULL DEFAULT ''''Beklemede'''',
    OnayanKullaniciId INT          NULL,
    OnayTarihi       DATETIME      NULL,
    RedNedeni        NVARCHAR(500) NULL,
    OlusturmaTarihi  DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FkIzinPersonel  FOREIGN KEY (PersonelId)
        REFERENCES Personel(PersonelId),
    CONSTRAINT FkIzinIzinTuru  FOREIGN KEY (IzinTuruId)
        REFERENCES IzinTuru(IzinTuruId),
    CONSTRAINT FkIzinKullanici FOREIGN KEY (OnayanKullaniciId)
        REFERENCES Kullanici(KullaniciId)
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- 10. SistemLog
-- ============================================================
CREATE TABLE SistemLog
(
    SistemLogId  INT            IDENTITY(1,1) PRIMARY KEY,
    KullaniciId  INT            NULL,
    Islem        NVARCHAR(100)  NOT NULL,
    TabloAdi     NVARCHAR(100)  NULL,
    EtkilenenId  INT            NULL,
    Aciklama     NVARCHAR(1000) NULL,
    LogTarihi    DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FkSistemLogKullanici FOREIGN KEY (KullaniciId)
        REFERENCES Kullanici(KullaniciId)
);'';
EXEC sys.sp_executesql N''-- ============================================================
-- INDEXLER
-- ============================================================
CREATE INDEX IxGirisKayitPersonelTarih ON GirisKayit (PersonelId, KayitTarihi);
CREATE INDEX IxGirisKayitTarih         ON GirisKayit (KayitTarihi);
CREATE INDEX IxIzinPersonelId          ON Izin        (PersonelId, Durum);
CREATE INDEX IxPersonelTcKimlik        ON Personel    (TcKimlikNo);'';
EXEC sys.sp_executesql N''-- ============================================================
-- VIEW''''LAR
-- ============================================================

-- vwPersonelDetay
CREATE VIEW vwPersonelDetay AS
    SELECT
        p.PersonelId,
        p.TcKimlikNo,
        p.Ad,
        p.Soyad,
        p.Ad + '''' '''' + p.Soyad  AS AdSoyad,
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
    INNER JOIN Vardiya   v  ON p.VardiyaId   = v.VardiyaId;'';
EXEC sys.sp_executesql N''-- vwGunlukOzet
CREATE VIEW vwGunlukOzet AS
    SELECT
        g.GirisKayitId,
        g.KayitTarihi,
        p.PersonelId,
        p.Ad + '''' '''' + p.Soyad  AS AdSoyad,
        d.DepartmanAdi,
        v.VardiyaAdi,
        v.BaslangicSaati       AS PlanlananGiris,
        v.BitisSaati           AS PlanlananCikis,
        g.GirisSaati,
        g.CikisSaati,
        g.GirisZamani,
        g.CikisZamani,
        g.CalismaDakika,
        g.MesaiDakika,
        g.GecGeldiMi,
        g.ErkenCiktiMi,
        g.Aciklama
    FROM       GirisKayit g
    INNER JOIN Personel   p ON g.PersonelId  = p.PersonelId
    INNER JOIN Departman  d ON p.DepartmanId = d.DepartmanId
    INNER JOIN Vardiya    v ON p.VardiyaId   = v.VardiyaId;'';
EXEC sys.sp_executesql N''-- vwIzinOzet
CREATE VIEW vwIzinOzet AS
    SELECT
        p.PersonelId,
        p.Ad + '''' '''' + p.Soyad  AS AdSoyad,
        p.YillikIzinHakki,
        ISNULL(SUM(
            CASE WHEN it.TuruAdi = N''''Yıllık İzin''''
                  AND i.Durum   = ''''Onaylı''''
                  AND YEAR(i.BaslangicTarihi) = YEAR(GETDATE())
                 THEN i.ToplamGun ELSE 0 END), 0) AS KullanilanYillikIzin,
        p.YillikIzinHakki -
        ISNULL(SUM(
            CASE WHEN it.TuruAdi = N''''Yıllık İzin''''
                  AND i.Durum   = ''''Onaylı''''
                  AND YEAR(i.BaslangicTarihi) = YEAR(GETDATE())
                 THEN i.ToplamGun ELSE 0 END), 0) AS KalanYillikIzin
    FROM       Personel  p
    LEFT JOIN  Izin      i  ON p.PersonelId  = i.PersonelId
    LEFT JOIN  IzinTuru  it ON i.IzinTuruId  = it.IzinTuruId
    WHERE p.AktifMi = 1
    GROUP BY p.PersonelId, p.Ad, p.Soyad, p.YillikIzinHakki;'';
EXEC sys.sp_executesql N''-- ============================================================
-- ÖRNEK VERİLER
-- ============================================================

INSERT INTO Departman (DepartmanAdi, Aciklama) VALUES
(N''''Bilgi Teknolojileri'''', N''''IT departmanı''''),
(N''''İnsan Kaynakları'''',    N''''IK departmanı''''),
(N''''Muhasebe'''',            N''''Muhasebe departmanı''''),
(N''''Üretim'''',              N''''Üretim departmanı''''),
(N''''Satış'''',               N''''Satış departmanı'''');'';
EXEC sys.sp_executesql N''INSERT INTO Pozisyon (DepartmanId, PozisyonAdi) VALUES
(1, N''''Yazılım Geliştirici''''),
(1, N''''Sistem Yöneticisi''''),
(2, N''''IK Uzmanı''''),
(2, N''''IK Müdürü''''),
(3, N''''Muhasebe Uzmanı''''),
(4, N''''Üretim Operatörü''''),
(5, N''''Satış Temsilcisi'''');'';
EXEC sys.sp_executesql N''INSERT INTO Vardiya (VardiyaAdi, BaslangicSaati, BitisSaati, MolaDakika, Cumartesi, Pazar) VALUES
(N''''Sabah Vardiyası'''', ''''08:00'''', ''''17:00'''', 60, 0, 0),
(N''''Akşam Vardiyası'''', ''''16:00'''', ''''00:00'''', 30, 0, 0),
(N''''Gece Vardiyası'''',  ''''00:00'''', ''''08:00'''', 30, 1, 1),
(N''''Esnek Vardiya'''',   ''''09:00'''', ''''18:00'''', 60, 0, 0);'';
EXEC sys.sp_executesql N''INSERT INTO IzinTuru (TuruAdi, Ucretli) VALUES
(N''''Yıllık İzin'''',   1),
(N''''Mazeret İzni'''',  1),
(N''''Hastalık İzni'''', 1),
(N''''Ücretsiz İzin'''', 0),
(N''''Doğum İzni'''',    1),
(N''''Ölüm İzni'''',     1);'';
EXEC sys.sp_executesql N''-- Admin: admin / Admin123
INSERT INTO Kullanici (KullaniciAdi, Sifre, Ad, Soyad, Email, Rol)
VALUES (''''admin'''', ''''Admin123'''', N''''Sistem'''', N''''Yöneticisi'''', ''''admin@sirket.com'''', ''''Admin'''');'';
EXEC sys.sp_executesql N''PRINT ''''====================================================='''';
PRINT '''' PersonelTakipV090Db başarıyla oluşturuldu!'''';
PRINT '''' Admin: admin / Admin123'''';
PRINT ''''====================================================='''';'';
EXEC sys.sp_executesql N''-- Mevcut veritabanını SSMS açılır listesinden seçin; dosyanın tamamını çalıştırın.
-- Bir hata oluşursa transaction geri alınır; hiçbir kayıt otomatik silinmez.
IF OBJECT_ID(N''''dbo.Personel'''', N''''U'''') IS NULL OR OBJECT_ID(N''''dbo.Kullanici'''', N''''U'''') IS NULL
  OR COL_LENGTH(N''''dbo.GirisKayit'''', N''''KayitTarihi'''') IS NULL
  THROW 51030, N''''Bu dosya mevcut PersonelTakipV090Db şeması içindir. SSMS içinde doğru veritabanını seçin.'''', 1;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    EXEC sys.sp_executesql N''''-- SSMS''''''''te güncellenecek mevcut veritabanını seçerek dosyanın tamamını çalıştırın.
-- İşlem kayıt silmez. Mükerrer girişler varsa önce inceleme gerektirir.
SET XACT_ABORT ON;'''';
    EXEC sys.sp_executesql N''''IF OBJECT_ID(N''''''''dbo.Personel'''''''',N''''''''U'''''''') IS NULL OR OBJECT_ID(N''''''''dbo.GirisKayit'''''''',N''''''''U'''''''') IS NULL
    THROW 51000,N''''''''Önce doğru personel veritabanını seçin.'''''''',1;
IF EXISTS(SELECT 1 FROM dbo.GirisKayit GROUP BY PersonelId,KayitTarihi HAVING COUNT(*)>1)
    THROW 51001,N''''''''Aynı personel ve gün için mükerrer girişler mevcut. Hiçbir kayıt silinmedi; önce mükerrerleri inceleyin.'''''''',1;
IF EXISTS(SELECT 1 FROM dbo.GirisKayit WHERE CikisSaati IS NULL GROUP BY PersonelId HAVING COUNT(*)>1)
    THROW 51002,N''''''''Aynı personelin birden fazla açık girişi mevcut. Hiçbir kayıt silinmedi; önce açık kayıtları inceleyin.'''''''',1;
IF COL_LENGTH(N''''''''dbo.GirisKayit'''''''',N''''''''PlanlananGirisZamani'''''''') IS NULL
    ALTER TABLE dbo.GirisKayit ADD PlanlananGirisZamani datetime NULL,PlanlananCikisZamani datetime NULL,
        PlanlananMolaDakika int NULL,VardiyaAdiKayit nvarchar(100) NULL;
IF COL_LENGTH(N''''''''dbo.Izin'''''''',N''''''''IptalEdenKullaniciId'''''''') IS NULL
    ALTER TABLE dbo.Izin ADD IptalEdenKullaniciId int NULL,IptalTarihi datetime NULL;'''';
    EXEC sys.sp_executesql N''''-- Eski kayıtlarda geçmiş plan bilinmediğinden güncelleme anındaki vardiya kullanılır.
-- Yeni girişlerin planı uygulama tarafından kayıt anında saklanır.
UPDATE g SET
    PlanlananGirisZamani=DATEADD(second,DATEDIFF(second,CAST(''''''''00:00'''''''' AS time),v.BaslangicSaati),CAST(g.KayitTarihi AS datetime)),
    PlanlananCikisZamani=DATEADD(day,CASE WHEN v.BitisSaati<=v.BaslangicSaati THEN 1 ELSE 0 END,
        DATEADD(second,DATEDIFF(second,CAST(''''''''00:00'''''''' AS time),v.BitisSaati),CAST(g.KayitTarihi AS datetime))),
    PlanlananMolaDakika=v.MolaDakika,VardiyaAdiKayit=v.VardiyaAdi
FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId
WHERE g.PlanlananGirisZamani IS NULL;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N''''''''dbo.GirisKayit'''''''') AND name=N''''''''UX_GirisKayit_PersonelGun'''''''')
    CREATE UNIQUE INDEX UX_GirisKayit_PersonelGun ON dbo.GirisKayit(PersonelId,KayitTarihi);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N''''''''dbo.GirisKayit'''''''') AND name=N''''''''UX_GirisKayit_Acik'''''''')
    CREATE UNIQUE INDEX UX_GirisKayit_Acik ON dbo.GirisKayit(PersonelId) WHERE CikisSaati IS NULL;
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name=N''''''''FK_Izin_IptalEden'''''''')
    ALTER TABLE dbo.Izin ADD CONSTRAINT FK_Izin_IptalEden FOREIGN KEY(IptalEdenKullaniciId) REFERENCES dbo.Kullanici(KullaniciId);'''';
    EXEC sys.sp_executesql N''''CREATE OR ALTER VIEW dbo.vwGunlukOzet AS
SELECT g.GirisKayitId,g.KayitTarihi,p.PersonelId,p.Ad+'''''''' ''''''''+p.Soyad AS AdSoyad,d.DepartmanAdi,
    COALESCE(g.VardiyaAdiKayit,v.VardiyaAdi) AS VardiyaAdi,
    CAST(g.PlanlananGirisZamani AS time) AS PlanlananGiris,CAST(g.PlanlananCikisZamani AS time) AS PlanlananCikis,
    g.GirisSaati,g.CikisSaati,g.GirisZamani,g.CikisZamani,g.CalismaDakika,g.MesaiDakika,g.GecGeldiMi,g.ErkenCiktiMi,g.Aciklama
FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId
JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId;'''';
    EXEC sys.sp_executesql N''''CREATE OR ALTER VIEW dbo.vwIzinOzet AS
SELECT p.PersonelId,p.Ad+'''''''' ''''''''+p.Soyad AS AdSoyad,p.YillikIzinHakki,
    ISNULL(x.Kullanilan,0) AS KullanilanYillikIzin,p.YillikIzinHakki-ISNULL(x.Kullanilan,0) AS KalanYillikIzin
FROM dbo.Personel p OUTER APPLY
(
    SELECT SUM(DATEDIFF(day,
        CASE WHEN i.BaslangicTarihi<DATEFROMPARTS(YEAR(GETDATE()),1,1) THEN DATEFROMPARTS(YEAR(GETDATE()),1,1) ELSE i.BaslangicTarihi END,
        CASE WHEN i.BitisTarihi>DATEFROMPARTS(YEAR(GETDATE()),12,31) THEN DATEFROMPARTS(YEAR(GETDATE()),12,31) ELSE i.BitisTarihi END)+1) AS Kullanilan
    FROM dbo.Izin i JOIN dbo.IzinTuru it ON it.IzinTuruId=i.IzinTuruId
    WHERE i.PersonelId=p.PersonelId AND i.Durum=N''''''''Onaylı'''''''' AND it.TuruAdi=N''''''''Yıllık İzin''''''''
      AND i.BitisTarihi>=DATEFROMPARTS(YEAR(GETDATE()),1,1) AND i.BaslangicTarihi<=DATEFROMPARTS(YEAR(GETDATE()),12,31)
) x;'''';
    EXEC sys.sp_executesql N''''CREATE OR ALTER FUNCTION dbo.fnDevamsizlik(@Bas date,@Bit date,@Simdi datetime)
RETURNS TABLE AS RETURN
(
    WITH Tarihler AS
    (
        SELECT @Bas AS Tarih WHERE @Bas<=@Bit
        UNION ALL SELECT DATEADD(day,1,Tarih) FROM Tarihler WHERE Tarih<@Bit
    ), Beklenen AS
    (
        SELECT p.PersonelId,p.Ad+'''''''' ''''''''+p.Soyad AS PersonelAdSoyad,d.DepartmanAdi,t.Tarih,
            DATEADD(day,CASE WHEN v.BitisSaati<=v.BaslangicSaati THEN 1 ELSE 0 END,
                DATEADD(second,DATEDIFF(second,CAST(''''''''00:00'''''''' AS time),v.BitisSaati),CAST(t.Tarih AS datetime))) AS PlanlananBitis
        FROM Tarihler t JOIN dbo.Personel p ON p.IseGirisTarihi<=t.Tarih
            AND (p.IstenCikisTarihi IS NULL OR p.IstenCikisTarihi>=t.Tarih)
            AND (p.AktifMi=1 OR p.IstenCikisTarihi IS NOT NULL)
        JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId
        WHERE CASE ((DATEDIFF(day,CONVERT(date,''''''''19000101''''''''),t.Tarih)%7)+7)%7
            WHEN 0 THEN v.Pazartesi WHEN 1 THEN v.Sali WHEN 2 THEN v.Carsamba WHEN 3 THEN v.Persembe
            WHEN 4 THEN v.Cuma WHEN 5 THEN v.Cumartesi ELSE v.Pazar END=1
    ), Otomatik AS
    (
        SELECT b.* FROM Beklenen b WHERE b.PlanlananBitis<=@Simdi
        AND NOT EXISTS(SELECT 1 FROM dbo.GirisKayit g WHERE g.PersonelId=b.PersonelId AND g.KayitTarihi=b.Tarih AND g.GirisZamani IS NOT NULL)
        AND NOT EXISTS(SELECT 1 FROM dbo.Izin i WHERE i.PersonelId=b.PersonelId AND i.Durum=N''''''''Onaylı'''''''' AND b.Tarih BETWEEN i.BaslangicTarihi AND i.BitisTarihi)
    )
    -- Manuel açıklamalar korunur; aynı personel/gün otomatik hesapta ikinci kez sayılmaz.
    SELECT dv.DevamsizlikId,dv.PersonelId,p.Ad+'''''''' ''''''''+p.Soyad AS PersonelAdSoyad,d.DepartmanAdi,
        dv.DevamsizlikTarihi,dv.Mazeret,dv.Aciklama
    FROM dbo.Devamsizlik dv JOIN dbo.Personel p ON p.PersonelId=dv.PersonelId JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId
    WHERE dv.DevamsizlikTarihi BETWEEN @Bas AND @Bit
    UNION ALL
    SELECT CAST(NULL AS int),o.PersonelId,o.PersonelAdSoyad,o.DepartmanAdi,o.Tarih,
        CAST(NULL AS nvarchar(500)),CAST(N''''''''Vardiya tamamlandı; giriş veya onaylı izin bulunamadı.'''''''' AS nvarchar(500))
    FROM Otomatik o WHERE NOT EXISTS(SELECT 1 FROM dbo.Devamsizlik dv WHERE dv.PersonelId=o.PersonelId AND dv.DevamsizlikTarihi=o.Tarih)
);'''';

    -- Geçersiz mevcut veri varsa tüm güncelleme durur; kayıtlar otomatik düzeltilmez veya silinmez.
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N''''CK_Personel_BilgilerV090'''')
      ALTER TABLE dbo.Personel WITH CHECK ADD CONSTRAINT CK_Personel_BilgilerV090 CHECK
      (LEN(LTRIM(RTRIM(Ad)))>0 AND LEN(LTRIM(RTRIM(Soyad)))>0 AND LEN(TcKimlikNo)=11
       AND TcKimlikNo COLLATE Latin1_General_100_BIN2 NOT LIKE ''''%[^0-9]%'''' AND YillikIzinHakki>=0
       AND (DogumTarihi IS NULL OR DogumTarihi<=IseGirisTarihi)
       AND (IstenCikisTarihi IS NULL OR IstenCikisTarihi>=IseGirisTarihi));
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N''''CK_Vardiya_SureV090'''')
      ALTER TABLE dbo.Vardiya WITH CHECK ADD CONSTRAINT CK_Vardiya_SureV090 CHECK
      (LEN(LTRIM(RTRIM(VardiyaAdi)))>0 AND BaslangicSaati<>BitisSaati AND MolaDakika>=0
       AND MolaDakika*60 < (DATEDIFF(second,BaslangicSaati,BitisSaati)+86400)%86400
       AND (CONVERT(int,Pazartesi)+CONVERT(int,Sali)+CONVERT(int,Carsamba)+CONVERT(int,Persembe)
            +CONVERT(int,Cuma)+CONVERT(int,Cumartesi)+CONVERT(int,Pazar))>0);
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N''''CK_Izin_TarihlerV090'''')
      ALTER TABLE dbo.Izin WITH CHECK ADD CONSTRAINT CK_Izin_TarihlerV090 CHECK
      (BitisTarihi>=BaslangicTarihi AND ToplamGun=DATEDIFF(day,BaslangicTarihi,BitisTarihi)+1
       AND Durum IN(N''''Beklemede'''',N''''Onaylı'''',N''''Reddedildi'''',N''''İptal Edildi''''));
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N''''CK_GirisKayit_SureV090'''')
      ALTER TABLE dbo.GirisKayit WITH CHECK ADD CONSTRAINT CK_GirisKayit_SureV090 CHECK
      ((CikisZamani IS NULL OR (GirisZamani IS NOT NULL AND CikisZamani>=GirisZamani))
       AND (CalismaDakika IS NULL OR CalismaDakika>=0) AND (MesaiDakika IS NULL OR MesaiDakika>=0));
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N''''dbo.Pozisyon'''') AND name=N''''UX_Pozisyon_DepartmanV090'''')
      CREATE UNIQUE INDEX UX_Pozisyon_DepartmanV090 ON dbo.Pozisyon(PozisyonId,DepartmanId);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name=N''''FK_Personel_PozisyonDepartmanV090'''')
      ALTER TABLE dbo.Personel WITH CHECK ADD CONSTRAINT FK_Personel_PozisyonDepartmanV090
      FOREIGN KEY(PozisyonId,DepartmanId) REFERENCES dbo.Pozisyon(PozisyonId,DepartmanId);
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;'';
 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;';
