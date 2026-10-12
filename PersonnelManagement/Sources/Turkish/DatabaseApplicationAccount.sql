-- SQL kurulum yöneticisiyle çalıştırın. Benzersiz bir hesap adı ve yerel parola belirleyin.
USE [PersonelYonetimiDB];
GO
DECLARE @LoginName sysname=N'PersonelYonetimiApp';
DECLARE @SqlPassword nvarchar(128)=NULL;
IF @SqlPassword IS NULL OR LEN(@SqlPassword)<12 THROW 51000,N'Set a private SQL password of at least 12 characters / En az 12 karakterlik özel SQL parolası belirleyin.',1;
IF SUSER_ID(@LoginName) IS NOT NULL OR USER_ID(@LoginName) IS NOT NULL THROW 51000,N'Account already exists; no account was changed / Hesap mevcut; değiştirilmedi.',1;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @Sql nvarchar(max)=N'CREATE LOGIN '+QUOTENAME(@LoginName)+N' WITH PASSWORD='+QUOTENAME(@SqlPassword,'''')+N',CHECK_POLICY=ON,CHECK_EXPIRATION=OFF;CREATE USER '+QUOTENAME(@LoginName)+N' FOR LOGIN '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Departman] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Pozisyon] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Vardiya] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Kullanici] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Personel] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[IzinTuru] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[GirisKayit] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Izin] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Devamsizlik] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT INSERT ON OBJECT::dbo.[SistemLog] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[PersonelDetay] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[GunlukGirisOzeti] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[YillikIzinOzeti] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[DevamsizliklariGetir] TO '+QUOTENAME(@LoginName)+N';';
 EXEC sys.sp_executesql @Sql;
 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
