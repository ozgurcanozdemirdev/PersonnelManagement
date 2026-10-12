-- Run as a SQL setup administrator. Choose a unique login and set its password locally.
USE [PersonnelManagementDB];
GO
DECLARE @LoginName sysname=N'PersonnelManagementApp';
DECLARE @SqlPassword nvarchar(128)=NULL;
IF @SqlPassword IS NULL OR LEN(@SqlPassword)<12 THROW 51000,N'Set a private SQL password of at least 12 characters / En az 12 karakterlik özel SQL parolası belirleyin.',1;
IF SUSER_ID(@LoginName) IS NOT NULL OR USER_ID(@LoginName) IS NOT NULL THROW 51000,N'Account already exists; no account was changed / Hesap mevcut; değiştirilmedi.',1;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @Sql nvarchar(max)=N'CREATE LOGIN '+QUOTENAME(@LoginName)+N' WITH PASSWORD='+QUOTENAME(@SqlPassword,'''')+N',CHECK_POLICY=ON,CHECK_EXPIRATION=OFF;CREATE USER '+QUOTENAME(@LoginName)+N' FOR LOGIN '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Department] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Position] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Shift] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[UserAccount] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[Employee] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[LeaveType] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[AttendanceRecord] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[LeaveRequest] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.[AbsenceRecord] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT INSERT ON OBJECT::dbo.[AuditLog] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[EmployeeDetails] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[DailyAttendanceSummary] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[AnnualLeaveSummary] TO '+QUOTENAME(@LoginName)+N';';
 SET @Sql+=N'GRANT SELECT ON OBJECT::dbo.[GetAbsences] TO '+QUOTENAME(@LoginName)+N';';
 EXEC sys.sp_executesql @Sql;
 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
