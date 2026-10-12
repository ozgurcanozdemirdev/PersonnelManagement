-- Uygun ve boş veritabanında çalıştırın. Yalnız yapay örnek veri içerir.
-- Örnek giriş: admin / Admin123!, manager / Manager123!, employee / Employee123!
USE [PersonelYonetimiDB];
GO
SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.Personel) OR EXISTS(SELECT 1 FROM dbo.Kullanici) OR EXISTS(SELECT 1 FROM dbo.Departman) OR EXISTS(SELECT 1 FROM dbo.Vardiya) OR EXISTS(SELECT 1 FROM dbo.IzinTuru)
    THROW 51000,N'Örnek veriler için boş veritabanı gerekir.',1;
INSERT dbo.Departman(DepartmanAdi,Aciklama) VALUES(N'Operasyon',N'Yapay örnek departman');
DECLARE @DepartmanId int=SCOPE_IDENTITY();
INSERT dbo.Pozisyon(DepartmanId,PozisyonAdi) VALUES(@DepartmanId,N'Uzman');
DECLARE @PozisyonId int=SCOPE_IDENTITY();
INSERT dbo.Vardiya(VardiyaAdi,BaslangicSaati,BitisSaati,MolaDakika) VALUES(N'Gündüz vardiyası','08:00','17:00',60);
DECLARE @VardiyaId int=SCOPE_IDENTITY();
INSERT dbo.Kullanici(KullaniciAdi,Sifre,Ad,Soyad,Rol) VALUES
(N'admin',N'PBKDF2$600000$qb39d3GEjthcLBeRnUC16Q==$HJ0OEYpRNeruTwuVP+Aaqc6UBffA8bo+7hwnRm1JtT4=',N'Demo',N'Yönetici',N'Admin'),
(N'manager',N'PBKDF2$600000$SisSaX1gy4tx4inAqPh7sg==$h91hWE53X3CTZMY3bmgEdAIaKHZ81C5TfDMCdfNusqA=',N'Demo',N'IK Yoneticisi',N'Yonetici'),
(N'employee',N'PBKDF2$600000$ITfUowhAdPQbkeFeN3yThA==$pETUXstbKYPm/z+cUr+IibUzQdmllVTxxHn2NLfLorI=',N'Demo',N'IK Personeli',N'Personel');
INSERT dbo.IzinTuru(TuruAdi,Ucretli,YillikHaktanDuser) VALUES
(N'Yıllık İzin',1,1),(N'Hastalık İzni',1,0),(N'Mazeret İzni',1,0),(N'Ücretsiz İzin',0,0);
INSERT dbo.Personel(TcKimlikNo,Ad,Soyad,DepartmanId,PozisyonId,VardiyaId,IseGirisTarihi,YillikIzinHakki)
VALUES('00000000001',N'Demo',N'Personel',@DepartmanId,@PozisyonId,@VardiyaId,DATEADD(year,-1,CAST(GETDATE() AS date)),14);
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;
GO
