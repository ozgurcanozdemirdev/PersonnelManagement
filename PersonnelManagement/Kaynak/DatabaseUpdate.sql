-- Mevcut veritabanını SSMS açılır listesinden seçin; dosyanın tamamını çalıştırın.
-- Bir hata oluşursa transaction geri alınır; hiçbir kayıt otomatik silinmez.
IF OBJECT_ID(N'dbo.Personel', N'U') IS NULL OR OBJECT_ID(N'dbo.Kullanici', N'U') IS NULL
  OR COL_LENGTH(N'dbo.GirisKayit', N'KayitTarihi') IS NULL
  THROW 51030, N'Bu dosya mevcut PersonelTakipV090Db şeması içindir. SSMS içinde doğru veritabanını seçin.', 1;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    EXEC sys.sp_executesql N'-- SSMS''te güncellenecek mevcut veritabanını seçerek dosyanın tamamını çalıştırın.
-- İşlem kayıt silmez. Mükerrer girişler varsa önce inceleme gerektirir.
SET XACT_ABORT ON;';
    EXEC sys.sp_executesql N'IF OBJECT_ID(N''dbo.Personel'',N''U'') IS NULL OR OBJECT_ID(N''dbo.GirisKayit'',N''U'') IS NULL
    THROW 51000,N''Önce doğru personel veritabanını seçin.'',1;
IF EXISTS(SELECT 1 FROM dbo.GirisKayit GROUP BY PersonelId,KayitTarihi HAVING COUNT(*)>1)
    THROW 51001,N''Aynı personel ve gün için mükerrer girişler mevcut. Hiçbir kayıt silinmedi; önce mükerrerleri inceleyin.'',1;
IF EXISTS(SELECT 1 FROM dbo.GirisKayit WHERE CikisSaati IS NULL GROUP BY PersonelId HAVING COUNT(*)>1)
    THROW 51002,N''Aynı personelin birden fazla açık girişi mevcut. Hiçbir kayıt silinmedi; önce açık kayıtları inceleyin.'',1;
IF COL_LENGTH(N''dbo.GirisKayit'',N''PlanlananGirisZamani'') IS NULL
    ALTER TABLE dbo.GirisKayit ADD PlanlananGirisZamani datetime NULL,PlanlananCikisZamani datetime NULL,
        PlanlananMolaDakika int NULL,VardiyaAdiKayit nvarchar(100) NULL;
IF COL_LENGTH(N''dbo.Izin'',N''IptalEdenKullaniciId'') IS NULL
    ALTER TABLE dbo.Izin ADD IptalEdenKullaniciId int NULL,IptalTarihi datetime NULL;';
    EXEC sys.sp_executesql N'-- Eski kayıtlarda geçmiş plan bilinmediğinden güncelleme anındaki vardiya kullanılır.
-- Yeni girişlerin planı uygulama tarafından kayıt anında saklanır.
UPDATE g SET
    PlanlananGirisZamani=DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.BaslangicSaati),CAST(g.KayitTarihi AS datetime)),
    PlanlananCikisZamani=DATEADD(day,CASE WHEN v.BitisSaati<=v.BaslangicSaati THEN 1 ELSE 0 END,
        DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.BitisSaati),CAST(g.KayitTarihi AS datetime))),
    PlanlananMolaDakika=v.MolaDakika,VardiyaAdiKayit=v.VardiyaAdi
FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId
WHERE g.PlanlananGirisZamani IS NULL;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N''dbo.GirisKayit'') AND name=N''UX_GirisKayit_PersonelGun'')
    CREATE UNIQUE INDEX UX_GirisKayit_PersonelGun ON dbo.GirisKayit(PersonelId,KayitTarihi);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N''dbo.GirisKayit'') AND name=N''UX_GirisKayit_Acik'')
    CREATE UNIQUE INDEX UX_GirisKayit_Acik ON dbo.GirisKayit(PersonelId) WHERE CikisSaati IS NULL;
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name=N''FK_Izin_IptalEden'')
    ALTER TABLE dbo.Izin ADD CONSTRAINT FK_Izin_IptalEden FOREIGN KEY(IptalEdenKullaniciId) REFERENCES dbo.Kullanici(KullaniciId);';
    EXEC sys.sp_executesql N'CREATE OR ALTER VIEW dbo.vwGunlukOzet AS
SELECT g.GirisKayitId,g.KayitTarihi,p.PersonelId,p.Ad+'' ''+p.Soyad AS AdSoyad,d.DepartmanAdi,
    COALESCE(g.VardiyaAdiKayit,v.VardiyaAdi) AS VardiyaAdi,
    CAST(g.PlanlananGirisZamani AS time) AS PlanlananGiris,CAST(g.PlanlananCikisZamani AS time) AS PlanlananCikis,
    g.GirisSaati,g.CikisSaati,g.GirisZamani,g.CikisZamani,g.CalismaDakika,g.MesaiDakika,g.GecGeldiMi,g.ErkenCiktiMi,g.Aciklama
FROM dbo.GirisKayit g JOIN dbo.Personel p ON p.PersonelId=g.PersonelId
JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId JOIN dbo.Vardiya v ON v.VardiyaId=p.VardiyaId;';
    EXEC sys.sp_executesql N'CREATE OR ALTER VIEW dbo.vwIzinOzet AS
SELECT p.PersonelId,p.Ad+'' ''+p.Soyad AS AdSoyad,p.YillikIzinHakki,
    ISNULL(x.Kullanilan,0) AS KullanilanYillikIzin,p.YillikIzinHakki-ISNULL(x.Kullanilan,0) AS KalanYillikIzin
FROM dbo.Personel p OUTER APPLY
(
    SELECT SUM(DATEDIFF(day,
        CASE WHEN i.BaslangicTarihi<DATEFROMPARTS(YEAR(GETDATE()),1,1) THEN DATEFROMPARTS(YEAR(GETDATE()),1,1) ELSE i.BaslangicTarihi END,
        CASE WHEN i.BitisTarihi>DATEFROMPARTS(YEAR(GETDATE()),12,31) THEN DATEFROMPARTS(YEAR(GETDATE()),12,31) ELSE i.BitisTarihi END)+1) AS Kullanilan
    FROM dbo.Izin i JOIN dbo.IzinTuru it ON it.IzinTuruId=i.IzinTuruId
    WHERE i.PersonelId=p.PersonelId AND i.Durum=N''Onaylı'' AND it.TuruAdi=N''Yıllık İzin''
      AND i.BitisTarihi>=DATEFROMPARTS(YEAR(GETDATE()),1,1) AND i.BaslangicTarihi<=DATEFROMPARTS(YEAR(GETDATE()),12,31)
) x;';
    EXEC sys.sp_executesql N'CREATE OR ALTER FUNCTION dbo.fnDevamsizlik(@Bas date,@Bit date,@Simdi datetime)
RETURNS TABLE AS RETURN
(
    WITH Tarihler AS
    (
        SELECT @Bas AS Tarih WHERE @Bas<=@Bit
        UNION ALL SELECT DATEADD(day,1,Tarih) FROM Tarihler WHERE Tarih<@Bit
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
    -- Manuel açıklamalar korunur; aynı personel/gün otomatik hesapta ikinci kez sayılmaz.
    SELECT dv.DevamsizlikId,dv.PersonelId,p.Ad+'' ''+p.Soyad AS PersonelAdSoyad,d.DepartmanAdi,
        dv.DevamsizlikTarihi,dv.Mazeret,dv.Aciklama
    FROM dbo.Devamsizlik dv JOIN dbo.Personel p ON p.PersonelId=dv.PersonelId JOIN dbo.Departman d ON d.DepartmanId=p.DepartmanId
    WHERE dv.DevamsizlikTarihi BETWEEN @Bas AND @Bit
    UNION ALL
    SELECT CAST(NULL AS int),o.PersonelId,o.PersonelAdSoyad,o.DepartmanAdi,o.Tarih,
        CAST(NULL AS nvarchar(500)),CAST(N''Vardiya tamamlandı; giriş veya onaylı izin bulunamadı.'' AS nvarchar(500))
    FROM Otomatik o WHERE NOT EXISTS(SELECT 1 FROM dbo.Devamsizlik dv WHERE dv.PersonelId=o.PersonelId AND dv.DevamsizlikTarihi=o.Tarih)
);';

    -- Geçersiz mevcut veri varsa tüm güncelleme durur; kayıtlar otomatik düzeltilmez veya silinmez.
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_Personel_BilgilerV090')
      ALTER TABLE dbo.Personel WITH CHECK ADD CONSTRAINT CK_Personel_BilgilerV090 CHECK
      (LEN(LTRIM(RTRIM(Ad)))>0 AND LEN(LTRIM(RTRIM(Soyad)))>0 AND LEN(TcKimlikNo)=11
       AND TcKimlikNo COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%' AND YillikIzinHakki>=0
       AND (DogumTarihi IS NULL OR DogumTarihi<=IseGirisTarihi)
       AND (IstenCikisTarihi IS NULL OR IstenCikisTarihi>=IseGirisTarihi));
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_Vardiya_SureV090')
      ALTER TABLE dbo.Vardiya WITH CHECK ADD CONSTRAINT CK_Vardiya_SureV090 CHECK
      (LEN(LTRIM(RTRIM(VardiyaAdi)))>0 AND BaslangicSaati<>BitisSaati AND MolaDakika>=0
       AND MolaDakika*60 < (DATEDIFF(second,BaslangicSaati,BitisSaati)+86400)%86400
       AND (CONVERT(int,Pazartesi)+CONVERT(int,Sali)+CONVERT(int,Carsamba)+CONVERT(int,Persembe)
            +CONVERT(int,Cuma)+CONVERT(int,Cumartesi)+CONVERT(int,Pazar))>0);
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_Izin_TarihlerV090')
      ALTER TABLE dbo.Izin WITH CHECK ADD CONSTRAINT CK_Izin_TarihlerV090 CHECK
      (BitisTarihi>=BaslangicTarihi AND ToplamGun=DATEDIFF(day,BaslangicTarihi,BitisTarihi)+1
       AND Durum IN(N'Beklemede',N'Onaylı',N'Reddedildi',N'İptal Edildi'));
    IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_GirisKayit_SureV090')
      ALTER TABLE dbo.GirisKayit WITH CHECK ADD CONSTRAINT CK_GirisKayit_SureV090 CHECK
      ((CikisZamani IS NULL OR (GirisZamani IS NOT NULL AND CikisZamani>=GirisZamani))
       AND (CalismaDakika IS NULL OR CalismaDakika>=0) AND (MesaiDakika IS NULL OR MesaiDakika>=0));
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Pozisyon') AND name=N'UX_Pozisyon_DepartmanV090')
      CREATE UNIQUE INDEX UX_Pozisyon_DepartmanV090 ON dbo.Pozisyon(PozisyonId,DepartmanId);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Personel_PozisyonDepartmanV090')
      ALTER TABLE dbo.Personel WITH CHECK ADD CONSTRAINT FK_Personel_PozisyonDepartmanV090
      FOREIGN KEY(PozisyonId,DepartmanId) REFERENCES dbo.Pozisyon(PozisyonId,DepartmanId);
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
