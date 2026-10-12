-- Kayıtları silmeden rapor nesnelerini ve rapor indekslerini yeniler.
-- SSMS içinde uygun veritabanını seçin. Kayıt silmeden rapor nesnelerini günceller.
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.Personel',N'U') IS NULL THROW 51000,N'Beklenmeyen veritabanı şeması.',1;
BEGIN TRY
BEGIN TRANSACTION;
-- Personel kurallarını mevcut veride doğrular; geçersiz kayıtları otomatik değiştirmez.
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Personel') AND name=N'CK_Personel_NationalIdDigits')
    ALTER TABLE dbo.Personel WITH CHECK ADD CONSTRAINT CK_Personel_NationalIdDigits CHECK (LEN(TcKimlikNo)=11 AND TcKimlikNo COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Personel') AND name=N'CK_Personel_BirthDate')
    ALTER TABLE dbo.Personel WITH CHECK ADD CONSTRAINT CK_Personel_BirthDate CHECK (DogumTarihi IS NULL OR (DogumTarihi <= CONVERT(date,GETDATE()) AND DogumTarihi <= IseGirisTarihi));
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Personel') AND name=N'IX_Personel_NameLookup')
    CREATE INDEX IX_Personel_NameLookup ON dbo.Personel(Ad,Soyad,PersonelId) INCLUDE (AktifMi);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Personel') AND name=N'IX_Personel_ActiveShift')
    CREATE INDEX IX_Personel_ActiveShift ON dbo.Personel(VardiyaId) INCLUDE (PersonelId) WHERE AktifMi=1;

-- Eski hesap kayitlarini koruyarak IK yoneticisi rolunu ekler.
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Kullanici') AND name=N'CK_Kullanici_Rol' AND definition LIKE N'%''Yonetici''%')
BEGIN
    IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Kullanici') AND name=N'CK_Kullanici_Rol')
        ALTER TABLE dbo.Kullanici DROP CONSTRAINT CK_Kullanici_Rol;
    ALTER TABLE dbo.Kullanici WITH CHECK ADD CONSTRAINT CK_Kullanici_Rol CHECK (Rol IN (N'Admin',N'Yonetici',N'Personel',N'Kullanici'));
END;


IF NOT EXISTS(SELECT 1 FROM sys.indexes i WHERE i.object_id=OBJECT_ID(N'dbo.GirisKayit') AND i.name=N'UX_GirisKayit_EmployeeDate' AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'GirisZamani') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'CalismaDakika') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'MesaiDakika') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'GecGeldiMi') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'ErkenCiktiMi'))
BEGIN
    IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.GirisKayit') AND name=N'UX_GirisKayit_EmployeeDate')
        CREATE UNIQUE INDEX UX_GirisKayit_EmployeeDate ON dbo.GirisKayit(PersonelId,KayitTarihi) INCLUDE (GirisZamani,CalismaDakika,MesaiDakika,GecGeldiMi,ErkenCiktiMi) WITH (DROP_EXISTING=ON);
    ELSE
        CREATE UNIQUE INDEX UX_GirisKayit_EmployeeDate ON dbo.GirisKayit(PersonelId,KayitTarihi) INCLUDE (GirisZamani,CalismaDakika,MesaiDakika,GecGeldiMi,ErkenCiktiMi);
END;

IF NOT EXISTS(SELECT 1 FROM sys.indexes i WHERE i.object_id=OBJECT_ID(N'dbo.GirisKayit') AND i.name=N'IX_GirisKayit_Tarih' AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'PersonelId') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'CalismaDakika') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'MesaiDakika') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'GecGeldiMi') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.GirisKayit') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'ErkenCiktiMi'))
BEGIN
    IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.GirisKayit') AND name=N'IX_GirisKayit_Tarih')
        CREATE INDEX IX_GirisKayit_Tarih ON dbo.GirisKayit(KayitTarihi) INCLUDE (PersonelId,CalismaDakika,MesaiDakika,GecGeldiMi,ErkenCiktiMi) WITH (DROP_EXISTING=ON);
    ELSE
        CREATE INDEX IX_GirisKayit_Tarih ON dbo.GirisKayit(KayitTarihi) INCLUDE (PersonelId,CalismaDakika,MesaiDakika,GecGeldiMi,ErkenCiktiMi);
END;

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
