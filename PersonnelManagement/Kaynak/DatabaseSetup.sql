-- PersonnelManagement V0.8.0: yeni kurulum; mevcut veritabanı silinmez.
-- Bu dosya ile DatabaseSetup.sql aynı kurulumu içerir; yalnızca birini çalıştırın.

USE [master];
GO
SET NOEXEC OFF;
GO

EXEC sys.sp_set_session_context @key = N'PersonnelManagementSetupCreated', @value = 0;
GO

IF DB_ID(N'PersonelTakipDb') IS NULL
BEGIN
    BEGIN TRY
        EXEC(N'CREATE DATABASE [PersonelTakipDb] COLLATE Turkish_CI_AS;');
        EXEC sys.sp_set_session_context @key = N'PersonnelManagementSetupCreated', @value = 1;
    END TRY
    BEGIN CATCH
        DECLARE @Hata nvarchar(2048) = ERROR_MESSAGE();
        RAISERROR(N'Veritabanı oluşturulamadı: %s', 16, 1, @Hata);
        SET NOEXEC ON;
    END CATCH;
END;
GO

USE [PersonelTakipDb];
GO
IF ISNULL(CAST(SESSION_CONTEXT(N'PersonnelManagementSetupCreated') AS int), 0) <> 1
BEGIN
    RAISERROR(N'PersonelTakipDb zaten var. Kurulum için boş ve farklı bir veritabanı adı kullanın.', 16, 1);
    SET NOEXEC ON;
END;
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO

-- ============================================================
-- 1. Departman
-- ============================================================
CREATE TABLE Departman
(
    DepartmanId     INT           IDENTITY(1,1) PRIMARY KEY,
    DepartmanAdi    NVARCHAR(100) NOT NULL,
    Aciklama        NVARCHAR(255) NULL,
    AktifMi         BIT           NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
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
);
GO

-- ============================================================
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
);
GO

-- ============================================================
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
);
GO

-- ============================================================
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
    Rol             NVARCHAR(20)  NOT NULL DEFAULT 'Kullanici',
    AktifMi         BIT           NOT NULL DEFAULT 1,
    SonGirisTarihi  DATETIME      NULL,
    OlusturmaTarihi DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
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
);
GO

-- ============================================================
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
);
GO

-- ============================================================
-- 8. IzinTuru
-- ============================================================
CREATE TABLE IzinTuru
(
    IzinTuruId  INT           IDENTITY(1,1) PRIMARY KEY,
    TuruAdi     NVARCHAR(100) NOT NULL,
    Ucretli     BIT           NOT NULL DEFAULT 1,
    AktifMi     BIT           NOT NULL DEFAULT 1
);
GO

-- ============================================================
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
    Durum            NVARCHAR(20)  NOT NULL DEFAULT 'Beklemede',
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
);
GO

-- ============================================================
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
);
GO

-- ============================================================
-- INDEXLER
-- ============================================================
CREATE INDEX IxGirisKayitPersonelTarih ON GirisKayit (PersonelId, KayitTarihi);
CREATE INDEX IxGirisKayitTarih         ON GirisKayit (KayitTarihi);
CREATE INDEX IxIzinPersonelId          ON Izin        (PersonelId, Durum);
CREATE INDEX IxPersonelTcKimlik        ON Personel    (TcKimlikNo);
GO

-- ============================================================
-- VIEW'LAR
-- ============================================================

-- vwPersonelDetay
CREATE VIEW vwPersonelDetay AS
    SELECT
        p.PersonelId,
        p.TcKimlikNo,
        p.Ad,
        p.Soyad,
        p.Ad + ' ' + p.Soyad  AS AdSoyad,
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
GO

-- vwGunlukOzet
CREATE VIEW vwGunlukOzet AS
    SELECT
        g.GirisKayitId,
        g.KayitTarihi,
        p.PersonelId,
        p.Ad + ' ' + p.Soyad  AS AdSoyad,
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
    INNER JOIN Vardiya    v ON p.VardiyaId   = v.VardiyaId;
GO

-- vwIzinOzet
CREATE VIEW vwIzinOzet AS
    SELECT
        p.PersonelId,
        p.Ad + ' ' + p.Soyad  AS AdSoyad,
        p.YillikIzinHakki,
        ISNULL(SUM(
            CASE WHEN it.TuruAdi = N'Yıllık İzin'
                  AND i.Durum   = 'Onaylı'
                  AND YEAR(i.BaslangicTarihi) = YEAR(GETDATE())
                 THEN i.ToplamGun ELSE 0 END), 0) AS KullanilanYillikIzin,
        p.YillikIzinHakki -
        ISNULL(SUM(
            CASE WHEN it.TuruAdi = N'Yıllık İzin'
                  AND i.Durum   = 'Onaylı'
                  AND YEAR(i.BaslangicTarihi) = YEAR(GETDATE())
                 THEN i.ToplamGun ELSE 0 END), 0) AS KalanYillikIzin
    FROM       Personel  p
    LEFT JOIN  Izin      i  ON p.PersonelId  = i.PersonelId
    LEFT JOIN  IzinTuru  it ON i.IzinTuruId  = it.IzinTuruId
    WHERE p.AktifMi = 1
    GROUP BY p.PersonelId, p.Ad, p.Soyad, p.YillikIzinHakki;
GO

-- ============================================================
-- ÖRNEK VERİLER
-- ============================================================

INSERT INTO Departman (DepartmanAdi, Aciklama) VALUES
(N'Bilgi Teknolojileri', N'IT departmanı'),
(N'İnsan Kaynakları',    N'IK departmanı'),
(N'Muhasebe',            N'Muhasebe departmanı'),
(N'Üretim',              N'Üretim departmanı'),
(N'Satış',               N'Satış departmanı');
GO

INSERT INTO Pozisyon (DepartmanId, PozisyonAdi) VALUES
(1, N'Yazılım Geliştirici'),
(1, N'Sistem Yöneticisi'),
(2, N'IK Uzmanı'),
(2, N'IK Müdürü'),
(3, N'Muhasebe Uzmanı'),
(4, N'Üretim Operatörü'),
(5, N'Satış Temsilcisi');
GO

INSERT INTO Vardiya (VardiyaAdi, BaslangicSaati, BitisSaati, MolaDakika, Cumartesi, Pazar) VALUES
(N'Sabah Vardiyası', '08:00', '17:00', 60, 0, 0),
(N'Akşam Vardiyası', '16:00', '00:00', 30, 0, 0),
(N'Gece Vardiyası',  '00:00', '08:00', 30, 1, 1),
(N'Esnek Vardiya',   '09:00', '18:00', 60, 0, 0);
GO

INSERT INTO IzinTuru (TuruAdi, Ucretli) VALUES
(N'Yıllık İzin',   1),
(N'Mazeret İzni',  1),
(N'Hastalık İzni', 1),
(N'Ücretsiz İzin', 0),
(N'Doğum İzni',    1),
(N'Ölüm İzni',     1);
GO

-- Admin: admin / Admin123
INSERT INTO Kullanici (KullaniciAdi, Sifre, Ad, Soyad, Email, Rol)
VALUES ('admin', 'Admin123', N'Sistem', N'Yöneticisi', 'admin@sirket.com', 'Admin');
GO

PRINT '=====================================================';
PRINT ' PersonelTakipDb başarıyla oluşturuldu!';
PRINT ' Admin: admin / Admin123';
PRINT '=====================================================';
GO
IF @@TRANCOUNT > 0 COMMIT TRANSACTION;
GO
SET NOEXEC OFF;
GO
