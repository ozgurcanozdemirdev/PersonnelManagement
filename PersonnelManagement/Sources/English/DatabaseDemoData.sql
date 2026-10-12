-- Run against the matching empty database. Synthetic data only.
-- Demo sign-in: admin / Admin123!, manager / Manager123!, employee / Employee123!
USE [PersonnelManagementDB];
GO
SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.Employee) OR EXISTS(SELECT 1 FROM dbo.UserAccount) OR EXISTS(SELECT 1 FROM dbo.Department) OR EXISTS(SELECT 1 FROM dbo.Shift) OR EXISTS(SELECT 1 FROM dbo.LeaveType)
    THROW 51000,N'Demo data requires an empty database.',1;
INSERT dbo.Department(DepartmentName,Description) VALUES(N'Operations',N'Synthetic demonstration department');
DECLARE @DepartmentId int=SCOPE_IDENTITY();
INSERT dbo.Position(DepartmentId,PositionName) VALUES(@DepartmentId,N'Specialist');
DECLARE @PositionId int=SCOPE_IDENTITY();
INSERT dbo.Shift(ShiftName,StartTime,EndTime,BreakMinutes) VALUES(N'Day shift','08:00','17:00',60);
DECLARE @ShiftId int=SCOPE_IDENTITY();
INSERT dbo.UserAccount(Username,PasswordHash,FirstName,LastName,Role) VALUES
(N'admin',N'PBKDF2$600000$qb39d3GEjthcLBeRnUC16Q==$HJ0OEYpRNeruTwuVP+Aaqc6UBffA8bo+7hwnRm1JtT4=',N'Demo',N'Administrator',N'Admin'),
(N'manager',N'PBKDF2$600000$SisSaX1gy4tx4inAqPh7sg==$h91hWE53X3CTZMY3bmgEdAIaKHZ81C5TfDMCdfNusqA=',N'Demo',N'HR Manager',N'Manager'),
(N'employee',N'PBKDF2$600000$ITfUowhAdPQbkeFeN3yThA==$pETUXstbKYPm/z+cUr+IibUzQdmllVTxxHn2NLfLorI=',N'Demo',N'HR Employee',N'Employee');
INSERT dbo.LeaveType(TypeName,IsPaid,AffectsAnnualAllowance) VALUES
(N'Annual Leave',1,1),(N'Sick Leave',1,0),(N'Personal Leave',1,0),(N'Unpaid Leave',0,0);
INSERT dbo.Employee(NationalId,FirstName,LastName,DepartmentId,PositionId,ShiftId,EmploymentStartDate,AnnualLeaveAllowance)
VALUES('00000000001',N'Demo',N'Employee',@DepartmentId,@PositionId,@ShiftId,DATEADD(year,-1,CAST(GETDATE() AS date)),14);
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;
GO
