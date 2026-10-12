-- Copy into an empty target. Set the legacy database name below; the source is retained.
USE [PersonnelManagementDB];
GO
SET XACT_ABORT ON;
DECLARE @LegacyDatabase sysname=N'PersonelTakipV090Db';
IF DB_ID(@LegacyDatabase) IS NULL THROW 51000,N'Legacy database is missing / Kaynak veritabanı bulunamadı.',1;
IF @LegacyDatabase=DB_NAME() THROW 51000,N'Source and target must differ / Kaynak ve hedef farklı olmalıdır.',1;
DECLARE @History nvarchar(max)=N'';
IF COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'PlanlananGirisZamani') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'PlanlananCikisZamani') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'PlanlananMolaDakika') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.GirisKayit',N'VardiyaAdiKayit') IS NOT NULL
 SET @History+=N'UPDATE g SET PlannedCheckInAt=s.PlanlananGirisZamani,PlannedCheckOutAt=s.PlanlananCikisZamani,PlannedBreakMinutes=s.PlanlananMolaDakika,RecordedShiftName=s.VardiyaAdiKayit FROM dbo.AttendanceRecord g JOIN [PersonelTakipDB].dbo.GirisKayit s ON s.GirisKayitId=g.AttendanceId;
';
IF COL_LENGTH(@LegacyDatabase+N'.dbo.Izin',N'IptalEdenKullaniciId') IS NOT NULL AND COL_LENGTH(@LegacyDatabase+N'.dbo.Izin',N'IptalTarihi') IS NOT NULL
 SET @History+=N'UPDATE i SET CancelledByUserId=s.IptalEdenKullaniciId,CancelledAt=s.IptalTarihi FROM dbo.LeaveRequest i JOIN [PersonelTakipDB].dbo.Izin s ON s.IzinId=i.LeaveId;
';
DECLARE @Migration nvarchar(max)=N'BEGIN TRY
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.[Department]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Position]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Shift]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[UserAccount]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[Employee]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[LeaveType]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[AttendanceRecord]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[LeaveRequest]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[AbsenceRecord]) THROW 51000,N''Migration requires an empty target database.'',1;
IF EXISTS(SELECT 1 FROM dbo.[AuditLog]) THROW 51000,N''Migration requires an empty target database.'',1;
SET IDENTITY_INSERT dbo.[Department] ON;
INSERT dbo.[Department]([DepartmentId],[DepartmentName],[Description],[IsActive],[CreatedAt])
SELECT s.[DepartmanId],s.[DepartmanAdi],s.[Aciklama],s.[AktifMi],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Departman] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Department] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Department])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Departman]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[Position] ON;
INSERT dbo.[Position]([PositionId],[DepartmentId],[PositionName],[Description],[IsActive])
SELECT s.[PozisyonId],s.[DepartmanId],s.[PozisyonAdi],s.[Aciklama],s.[AktifMi] FROM [PersonelTakipDB].dbo.[Pozisyon] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Position] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Position])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Pozisyon]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[Shift] ON;
INSERT dbo.[Shift]([ShiftId],[ShiftName],[StartTime],[EndTime],[BreakMinutes],[Monday],[Tuesday],[Wednesday],[Thursday],[Friday],[Saturday],[Sunday],[IsActive],[CreatedAt])
SELECT s.[VardiyaId],s.[VardiyaAdi],s.[BaslangicSaati],s.[BitisSaati],s.[MolaDakika],s.[Pazartesi],s.[Sali],s.[Carsamba],s.[Persembe],s.[Cuma],s.[Cumartesi],s.[Pazar],s.[AktifMi],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Vardiya] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Shift] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Shift])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Vardiya]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[UserAccount] ON;
INSERT dbo.[UserAccount]([UserId],[Username],[PasswordHash],[FirstName],[LastName],[Email],[Role],[IsActive],[LastLoginAt],[CreatedAt])
SELECT s.[KullaniciId],s.[KullaniciAdi],s.[Sifre],s.[Ad],s.[Soyad],s.[Email],CASE s.[Rol] WHEN N''Yonetici'' THEN N''Manager'' WHEN N''Yönetici'' THEN N''Manager'' WHEN N''Manager'' THEN N''Manager'' WHEN N''Calisan'' THEN N''Employee'' WHEN N''Kullanici'' THEN N''User'' ELSE s.[Rol] END,s.[AktifMi],s.[SonGirisTarihi],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Kullanici] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[UserAccount] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[UserAccount])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Kullanici]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[Employee] ON;
INSERT dbo.[Employee]([EmployeeId],[NationalId],[FirstName],[LastName],[Gender],[BirthDate],[Phone],[Email],[Address],[PhotoPath],[DepartmentId],[PositionId],[ShiftId],[EmploymentStartDate],[EmploymentEndDate],[AnnualLeaveAllowance],[IsActive],[CreatedAt],[UpdatedAt])
SELECT s.[PersonelId],s.[TcKimlikNo],s.[Ad],s.[Soyad],CASE s.[Cinsiyet] WHEN N''Erkek'' THEN N''Male'' WHEN N''Kadın'' THEN N''Female'' ELSE s.[Cinsiyet] END,s.[DogumTarihi],s.[Telefon],s.[Email],s.[Adres],s.[FotoPath],s.[DepartmanId],s.[PozisyonId],s.[VardiyaId],s.[IseGirisTarihi],s.[IstenCikisTarihi],s.[YillikIzinHakki],s.[AktifMi],s.[OlusturmaTarihi],s.[GuncellemeTarihi] FROM [PersonelTakipDB].dbo.[Personel] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[Employee] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[Employee])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Personel]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[LeaveType] ON;
INSERT dbo.[LeaveType]([LeaveTypeId],[TypeName],[IsPaid],[IsActive],[AffectsAnnualAllowance])
SELECT s.[IzinTuruId],CASE s.[TuruAdi] WHEN N''Yıllık İzin'' THEN N''Annual Leave'' WHEN N''Hastalık İzni'' THEN N''Sick Leave'' WHEN N''Mazeret İzni'' THEN N''Personal Leave'' WHEN N''Ücretsiz İzin'' THEN N''Unpaid Leave'' ELSE s.[TuruAdi] END,s.[Ucretli],s.[AktifMi],CASE WHEN s.[TuruAdi] IN (N''Yıllık İzin'',N''Annual Leave'') THEN 1 ELSE 0 END FROM [PersonelTakipDB].dbo.[IzinTuru] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[LeaveType] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[LeaveType])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[IzinTuru]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[AttendanceRecord] ON;
INSERT dbo.[AttendanceRecord]([AttendanceId],[EmployeeId],[AttendanceDate],[CheckInTime],[CheckOutTime],[CheckInAt],[CheckOutAt],[WorkMinutes],[OvertimeMinutes],[IsLate],[LeftEarly],[Description],[RecordedByUserId],[CreatedAt])
SELECT s.[GirisKayitId],s.[PersonelId],s.[KayitTarihi],s.[GirisSaati],s.[CikisSaati],__CheckInExpression__,__CheckOutExpression__,s.[CalismaDakika],s.[MesaiDakika],s.[GecGeldiMi],s.[ErkenCiktiMi],s.[Aciklama],s.[KaydedenId],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[GirisKayit] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[AttendanceRecord] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[AttendanceRecord])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[GirisKayit]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[LeaveRequest] ON;
INSERT dbo.[LeaveRequest]([LeaveId],[EmployeeId],[LeaveTypeId],[StartDate],[EndDate],[TotalDays],[Description],[Status],[ReviewedByUserId],[ReviewedAt],[RejectionReason],[CreatedAt])
SELECT s.[IzinId],s.[PersonelId],s.[IzinTuruId],s.[BaslangicTarihi],s.[BitisTarihi],s.[ToplamGun],s.[Aciklama],CASE s.[Durum] WHEN N''Beklemede'' THEN N''Pending'' WHEN N''Onaylı'' THEN N''Approved'' WHEN N''Reddedildi'' THEN N''Rejected'' WHEN N''İptal Edildi'' THEN N''Cancelled'' ELSE s.[Durum] END,s.[OnayanKullaniciId],s.[OnayTarihi],s.[RedNedeni],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Izin] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[LeaveRequest] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[LeaveRequest])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Izin]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[AbsenceRecord] ON;
INSERT dbo.[AbsenceRecord]([AbsenceId],[EmployeeId],[AbsenceDate],[Description],[Excuse],[CreatedAt])
SELECT s.[DevamsizlikId],s.[PersonelId],s.[DevamsizlikTarihi],s.[Aciklama],s.[Mazeret],s.[OlusturmaTarihi] FROM [PersonelTakipDB].dbo.[Devamsizlik] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[AbsenceRecord] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[AbsenceRecord])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[Devamsizlik]) THROW 51000,N''Migration count verification failed.'',1;
SET IDENTITY_INSERT dbo.[AuditLog] ON;
INSERT dbo.[AuditLog]([AuditId],[UserId],[Action],[TableName],[AffectedId],[Description],[LoggedAt])
SELECT s.[SistemLogId],s.[KullaniciId],s.[Islem],s.[TabloAdi],s.[EtkilenenId],s.[Aciklama],s.[LogTarihi] FROM [PersonelTakipDB].dbo.[SistemLog] s WITH (TABLOCKX,HOLDLOCK);
SET IDENTITY_INSERT dbo.[AuditLog] OFF;
IF (SELECT COUNT_BIG(*) FROM dbo.[AuditLog])<>(SELECT COUNT_BIG(*) FROM [PersonelTakipDB].dbo.[SistemLog]) THROW 51000,N''Migration count verification failed.'',1;

__OptionalHistory__
UPDATE g SET
    CheckInAt=COALESCE(CheckInAt,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),CheckInTime),CAST(AttendanceDate AS datetime))),
    CheckOutAt=COALESCE(CheckOutAt,CASE WHEN CheckOutTime IS NOT NULL THEN DATEADD(day,CASE WHEN CheckOutTime<CheckInTime THEN 1 ELSE 0 END,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),CheckOutTime),CAST(AttendanceDate AS datetime))) END),
    PlannedCheckInAt=COALESCE(g.PlannedCheckInAt,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.StartTime),CAST(g.AttendanceDate AS datetime))),
    PlannedCheckOutAt=COALESCE(g.PlannedCheckOutAt,DATEADD(day,CASE WHEN v.EndTime<=v.StartTime THEN 1 ELSE 0 END,DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.EndTime),CAST(g.AttendanceDate AS datetime)))),
    PlannedBreakMinutes=COALESCE(g.PlannedBreakMinutes,v.BreakMinutes),RecordedShiftName=COALESCE(g.RecordedShiftName,v.ShiftName)
FROM dbo.AttendanceRecord g JOIN dbo.Employee p ON p.EmployeeId=g.EmployeeId JOIN dbo.Shift v ON v.ShiftId=p.ShiftId;

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
