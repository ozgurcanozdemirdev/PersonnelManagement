-- Creates the schema without deleting an existing database. Run only one setup alternative.
SET NOEXEC OFF;
USE [master];
GO
EXEC sys.sp_set_session_context @key=N'PersonnelManagementSetupCreated',@value=0;
IF DB_ID(N'PersonnelManagementDB') IS NOT NULL
BEGIN
    RAISERROR(N'Database already exists. Use a different empty database; this script does not overwrite existing records.',16,1);
    SET NOEXEC ON;
END
ELSE
BEGIN
    EXEC(N'CREATE DATABASE [PersonnelManagementDB] COLLATE Turkish_CI_AS;');
    EXEC sys.sp_set_session_context @key=N'PersonnelManagementSetupCreated',@value=1;
END;
GO
USE [PersonnelManagementDB];
GO
IF ISNULL(CONVERT(int,SESSION_CONTEXT(N'PersonnelManagementSetupCreated')),0)<>1 THROW 51000,N'Setup did not create this database.',1;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
CREATE TABLE dbo.Department (
    [DepartmentId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Department PRIMARY KEY,
    [DepartmentName] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [Description] [nvarchar](255) COLLATE Turkish_CI_AS NULL,
    [IsActive] [bit] NOT NULL CONSTRAINT DF_Department_IsActive DEFAULT ((1)),
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_Department_CreatedAt DEFAULT (getdate())
);

CREATE TABLE dbo.Position (
    [PositionId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Position PRIMARY KEY,
    [DepartmentId] [int] NOT NULL,
    [PositionName] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [Description] [nvarchar](255) COLLATE Turkish_CI_AS NULL,
    [IsActive] [bit] NOT NULL CONSTRAINT DF_Position_IsActive DEFAULT ((1))
);

CREATE TABLE dbo.Shift (
    [ShiftId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Shift PRIMARY KEY,
    [ShiftName] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [StartTime] [time](0) NOT NULL,
    [EndTime] [time](0) NOT NULL,
    [BreakMinutes] [int] NOT NULL CONSTRAINT DF_Shift_BreakMinutes DEFAULT ((0)),
    [Monday] [bit] NOT NULL CONSTRAINT DF_Shift_Monday DEFAULT ((1)),
    [Tuesday] [bit] NOT NULL CONSTRAINT DF_Shift_Tuesday DEFAULT ((1)),
    [Wednesday] [bit] NOT NULL CONSTRAINT DF_Shift_Wednesday DEFAULT ((1)),
    [Thursday] [bit] NOT NULL CONSTRAINT DF_Shift_Thursday DEFAULT ((1)),
    [Friday] [bit] NOT NULL CONSTRAINT DF_Shift_Friday DEFAULT ((1)),
    [Saturday] [bit] NOT NULL CONSTRAINT DF_Shift_Saturday DEFAULT ((0)),
    [Sunday] [bit] NOT NULL CONSTRAINT DF_Shift_Sunday DEFAULT ((0)),
    [IsActive] [bit] NOT NULL CONSTRAINT DF_Shift_IsActive DEFAULT ((1)),
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_Shift_CreatedAt DEFAULT (getdate())
);

CREATE TABLE dbo.Employee (
    [EmployeeId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Employee PRIMARY KEY,
    [NationalId] [char](11) COLLATE Turkish_CI_AS NOT NULL,
    [FirstName] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [LastName] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Gender] [nvarchar](10) COLLATE Turkish_CI_AS NULL,
    [BirthDate] [date] NULL,
    [Phone] [nvarchar](20) COLLATE Turkish_CI_AS NULL,
    [Email] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
    [Address] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [PhotoPath] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [DepartmentId] [int] NOT NULL,
    [PositionId] [int] NOT NULL,
    [ShiftId] [int] NOT NULL,
    [EmploymentStartDate] [date] NOT NULL,
    [EmploymentEndDate] [date] NULL,
    [AnnualLeaveAllowance] [int] NOT NULL CONSTRAINT DF_Employee_AnnualLeaveAllowance DEFAULT ((14)),
    [IsActive] [bit] NOT NULL CONSTRAINT DF_Employee_IsActive DEFAULT ((1)),
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_Employee_CreatedAt DEFAULT (getdate()),
    [UpdatedAt] [datetime] NULL
);

CREATE TABLE dbo.AttendanceRecord (
    [AttendanceId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_AttendanceRecord PRIMARY KEY,
    [EmployeeId] [int] NOT NULL,
    [AttendanceDate] [date] NOT NULL,
    [CheckInTime] [time](0) NULL,
    [CheckOutTime] [time](0) NULL,
    [CheckInAt] [datetime] NULL,
    [CheckOutAt] [datetime] NULL,
    [WorkMinutes] [int] NULL,
    [OvertimeMinutes] [int] NULL CONSTRAINT DF_AttendanceRecord_OvertimeMinutes DEFAULT ((0)),
    [IsLate] [bit] NOT NULL CONSTRAINT DF_AttendanceRecord_IsLate DEFAULT ((0)),
    [LeftEarly] [bit] NOT NULL CONSTRAINT DF_AttendanceRecord_LeftEarly DEFAULT ((0)),
    [Description] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [RecordedByUserId] [int] NULL,
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_AttendanceRecord_CreatedAt DEFAULT (getdate()),
    [PlannedCheckInAt] datetime NULL,
    [PlannedCheckOutAt] datetime NULL,
    [PlannedBreakMinutes] int NULL,
    [RecordedShiftName] nvarchar(100) NULL
);

CREATE TABLE dbo.LeaveType (
    [LeaveTypeId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_LeaveType PRIMARY KEY,
    [TypeName] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [IsPaid] [bit] NOT NULL CONSTRAINT DF_LeaveType_IsPaid DEFAULT ((1)),
    [IsActive] [bit] NOT NULL CONSTRAINT DF_LeaveType_IsActive DEFAULT ((1)),
    [AffectsAnnualAllowance] bit NOT NULL CONSTRAINT DF_LeaveType_AffectsAnnualAllowance DEFAULT (0)
);

CREATE TABLE dbo.LeaveRequest (
    [LeaveId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_LeaveRequest PRIMARY KEY,
    [EmployeeId] [int] NOT NULL,
    [LeaveTypeId] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [EndDate] [date] NOT NULL,
    [TotalDays] [int] NOT NULL,
    [Description] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [Status] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL CONSTRAINT DF_LeaveRequest_Status DEFAULT ('Pending'),
    [ReviewedByUserId] [int] NULL,
    [ReviewedAt] [datetime] NULL,
    [RejectionReason] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_LeaveRequest_CreatedAt DEFAULT (getdate()),
    [CancelledByUserId] int NULL,
    [CancelledAt] datetime NULL
);

CREATE TABLE dbo.UserAccount (
    [UserId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserAccount PRIMARY KEY,
    [Username] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [PasswordHash] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [FirstName] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [LastName] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
    [Email] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
    [Role] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL CONSTRAINT DF_UserAccount_Role DEFAULT ('Employee'),
    [IsActive] [bit] NOT NULL CONSTRAINT DF_UserAccount_IsActive DEFAULT ((1)),
    [LastLoginAt] [datetime] NULL,
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_UserAccount_CreatedAt DEFAULT (getdate())
);

CREATE TABLE dbo.AuditLog (
    [AuditId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLog PRIMARY KEY,
    [UserId] [int] NULL,
    [Action] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
    [TableName] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
    [AffectedId] [int] NULL,
    [Description] [nvarchar](1000) COLLATE Turkish_CI_AS NULL,
    [LoggedAt] [datetime] NOT NULL CONSTRAINT DF_AuditLog_LoggedAt DEFAULT (getdate())
);

CREATE TABLE dbo.AbsenceRecord (
    [AbsenceId] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_AbsenceRecord PRIMARY KEY,
    [EmployeeId] [int] NOT NULL,
    [AbsenceDate] [date] NOT NULL,
    [Description] [nvarchar](500) COLLATE Turkish_CI_AS NULL,
    [Excuse] [nvarchar](200) COLLATE Turkish_CI_AS NULL,
    [CreatedAt] [datetime] NOT NULL CONSTRAINT DF_AbsenceRecord_CreatedAt DEFAULT (getdate())
);

ALTER TABLE dbo.UserAccount ADD CONSTRAINT UQ_UserAccount_Username UNIQUE (Username);
ALTER TABLE dbo.UserAccount ADD CONSTRAINT CK_UserAccount_Role CHECK (Role IN (N'Admin',N'Manager',N'Employee',N'User'));
ALTER TABLE dbo.Employee ADD CONSTRAINT UQ_Employee_NationalId UNIQUE (NationalId);
ALTER TABLE dbo.Employee ADD CONSTRAINT CK_Employee_NationalIdDigits CHECK (LEN(NationalId)=11 AND NationalId COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%');
ALTER TABLE dbo.Employee ADD CONSTRAINT CK_Employee_BirthDate CHECK (BirthDate IS NULL OR (BirthDate <= CONVERT(date,GETDATE()) AND BirthDate <= EmploymentStartDate));
CREATE INDEX IX_Employee_NameLookup ON dbo.Employee(FirstName,LastName,EmployeeId) INCLUDE (IsActive);
CREATE INDEX IX_Employee_ActiveShift ON dbo.Employee(ShiftId) INCLUDE (EmployeeId) WHERE IsActive=1;

ALTER TABLE dbo.Employee ADD CONSTRAINT CK_Employee_AnnualLeaveAllowance CHECK (AnnualLeaveAllowance >= 0);
ALTER TABLE dbo.Employee ADD CONSTRAINT CK_Employee_EmploymentDates CHECK (EmploymentEndDate IS NULL OR EmploymentEndDate >= EmploymentStartDate);
ALTER TABLE dbo.Position ADD CONSTRAINT UQ_Position_Department UNIQUE (PositionId,DepartmentId);
ALTER TABLE dbo.Position ADD CONSTRAINT FK_Position_Department FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(DepartmentId);
ALTER TABLE dbo.Employee ADD CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(DepartmentId);
ALTER TABLE dbo.Employee ADD CONSTRAINT FK_Employee_PositionDepartment FOREIGN KEY (PositionId,DepartmentId) REFERENCES dbo.Position(PositionId,DepartmentId);
ALTER TABLE dbo.Employee ADD CONSTRAINT FK_Employee_Shift FOREIGN KEY (ShiftId) REFERENCES dbo.Shift(ShiftId);
ALTER TABLE dbo.AttendanceRecord ADD CONSTRAINT FK_AttendanceRecord_Employee FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(EmployeeId);
ALTER TABLE dbo.AttendanceRecord ADD CONSTRAINT FK_AttendanceRecord_RecordedBy FOREIGN KEY (RecordedByUserId) REFERENCES dbo.UserAccount(UserId);
ALTER TABLE dbo.LeaveRequest ADD CONSTRAINT FK_LeaveRequest_Employee FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(EmployeeId);
ALTER TABLE dbo.LeaveRequest ADD CONSTRAINT FK_LeaveRequest_Type FOREIGN KEY (LeaveTypeId) REFERENCES dbo.LeaveType(LeaveTypeId);
ALTER TABLE dbo.LeaveRequest ADD CONSTRAINT FK_LeaveRequest_ReviewedBy FOREIGN KEY (ReviewedByUserId) REFERENCES dbo.UserAccount(UserId);
ALTER TABLE dbo.LeaveRequest ADD CONSTRAINT FK_LeaveRequest_CancelledBy FOREIGN KEY (CancelledByUserId) REFERENCES dbo.UserAccount(UserId);
ALTER TABLE dbo.AbsenceRecord ADD CONSTRAINT FK_AbsenceRecord_Employee FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(EmployeeId);
ALTER TABLE dbo.AuditLog ADD CONSTRAINT FK_AuditLog_User FOREIGN KEY (UserId) REFERENCES dbo.UserAccount(UserId);
ALTER TABLE dbo.Shift ADD CONSTRAINT CK_Shift_BreakMinutes CHECK (BreakMinutes >= 0 AND BreakMinutes < CASE WHEN EndTime > StartTime THEN DATEDIFF(minute,StartTime,EndTime) ELSE DATEDIFF(minute,StartTime,EndTime)+1440 END);
ALTER TABLE dbo.LeaveRequest ADD CONSTRAINT CK_LeaveRequest_Dates CHECK (EndDate >= StartDate AND TotalDays = DATEDIFF(day,StartDate,EndDate)+1);
ALTER TABLE dbo.LeaveRequest ADD CONSTRAINT CK_LeaveRequest_Status CHECK (Status IN (N'Pending',N'Approved',N'Rejected',N'Cancelled'));
ALTER TABLE dbo.AttendanceRecord ADD CONSTRAINT CK_AttendanceRecord_Duration CHECK ((WorkMinutes IS NULL OR WorkMinutes >= 0) AND (OvertimeMinutes IS NULL OR OvertimeMinutes >= 0) AND (CheckOutAt IS NULL OR CheckInAt IS NULL OR CheckOutAt >= CheckInAt));
CREATE UNIQUE INDEX UX_AttendanceRecord_EmployeeDate ON dbo.AttendanceRecord(EmployeeId,AttendanceDate) INCLUDE (CheckInAt,WorkMinutes,OvertimeMinutes,IsLate,LeftEarly);
CREATE UNIQUE INDEX UX_AttendanceRecord_Open ON dbo.AttendanceRecord(EmployeeId) WHERE CheckOutTime IS NULL;
CREATE UNIQUE INDEX UX_AbsenceRecord_EmployeeDate ON dbo.AbsenceRecord(EmployeeId,AbsenceDate);
CREATE INDEX IX_LeaveRequest_EmployeeDates ON dbo.LeaveRequest(EmployeeId,StartDate,EndDate) INCLUDE (Status,LeaveTypeId);
CREATE INDEX IX_AttendanceRecord_Date ON dbo.AttendanceRecord(AttendanceDate) INCLUDE (EmployeeId,WorkMinutes,OvertimeMinutes,IsLate,LeftEarly);

EXEC sys.sp_executesql N'CREATE OR ALTER VIEW EmployeeDetails AS
    SELECT
        p.EmployeeId,
        p.NationalId,
        p.FirstName,
        p.LastName,
        p.FirstName + '' '' + p.LastName  AS FullName,
        p.Gender,
        p.BirthDate,
        p.Phone,
        p.Email,
        p.Address,
        p.PhotoPath,
        p.DepartmentId,
        d.DepartmentName,
        p.PositionId,
        pz.PositionName,
        p.ShiftId,
        v.ShiftName,
        v.StartTime,
        v.EndTime,
        v.BreakMinutes,
        p.EmploymentStartDate,
        p.EmploymentEndDate,
        p.AnnualLeaveAllowance,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM       Employee  p
    INNER JOIN Department d  ON p.DepartmentId = d.DepartmentId
    INNER JOIN Position  pz ON p.PositionId  = pz.PositionId
    INNER JOIN Shift   v  ON p.ShiftId   = v.ShiftId;
';
EXEC sys.sp_executesql N'CREATE OR ALTER VIEW dbo.DailyAttendanceSummary AS
SELECT g.AttendanceId,g.AttendanceDate,p.EmployeeId,p.FirstName+'' ''+p.LastName AS FullName,d.DepartmentName,
    COALESCE(g.RecordedShiftName,v.ShiftName) AS ShiftName,
    CAST(g.PlannedCheckInAt AS time) AS PlannedCheckInTime,CAST(g.PlannedCheckOutAt AS time) AS PlannedCheckOutTime,
    g.CheckInTime,g.CheckOutTime,g.CheckInAt,g.CheckOutAt,g.WorkMinutes,g.OvertimeMinutes,g.IsLate,g.LeftEarly,g.Description
FROM dbo.AttendanceRecord g JOIN dbo.Employee p ON p.EmployeeId=g.EmployeeId
JOIN dbo.Department d ON d.DepartmentId=p.DepartmentId JOIN dbo.Shift v ON v.ShiftId=p.ShiftId;';
EXEC sys.sp_executesql N'CREATE OR ALTER VIEW dbo.AnnualLeaveSummary AS
SELECT p.EmployeeId,p.FirstName+'' ''+p.LastName AS FullName,p.AnnualLeaveAllowance,
    ISNULL(x.UsedDays,0) AS UsedAnnualLeaveDays,p.AnnualLeaveAllowance-ISNULL(x.UsedDays,0) AS RemainingAnnualLeaveDays
FROM dbo.Employee p OUTER APPLY
(
    SELECT SUM(DATEDIFF(day,
        CASE WHEN i.StartDate<DATEFROMPARTS(YEAR(GETDATE()),1,1) THEN DATEFROMPARTS(YEAR(GETDATE()),1,1) ELSE i.StartDate END,
        CASE WHEN i.EndDate>DATEFROMPARTS(YEAR(GETDATE()),12,31) THEN DATEFROMPARTS(YEAR(GETDATE()),12,31) ELSE i.EndDate END)+1) AS UsedDays
    FROM dbo.LeaveRequest i JOIN dbo.LeaveType it ON it.LeaveTypeId=i.LeaveTypeId
    WHERE i.EmployeeId=p.EmployeeId AND i.Status=N''Approved'' AND it.AffectsAnnualAllowance=1
      AND i.EndDate>=DATEFROMPARTS(YEAR(GETDATE()),1,1) AND i.StartDate<=DATEFROMPARTS(YEAR(GETDATE()),12,31)
) x;';
EXEC sys.sp_executesql N'CREATE OR ALTER FUNCTION dbo.GetAbsences(@Start date,@End date,@Now datetime)
RETURNS TABLE AS RETURN
(
    WITH Dates AS
    (
        SELECT @Start AS Date WHERE @Start<=@End
        UNION ALL SELECT DATEADD(day,1,Date) FROM Dates WHERE Date<@End
    ), Expected AS
    (
        SELECT p.EmployeeId,p.FirstName+'' ''+p.LastName AS EmployeeFullName,d.DepartmentName,t.Date,
            DATEADD(day,CASE WHEN v.EndTime<=v.StartTime THEN 1 ELSE 0 END,
                DATEADD(second,DATEDIFF(second,CAST(''00:00'' AS time),v.EndTime),CAST(t.Date AS datetime))) AS PlannedEnd
        FROM Dates t JOIN dbo.Employee p ON p.EmploymentStartDate<=t.Date
            AND (p.EmploymentEndDate IS NULL OR p.EmploymentEndDate>=t.Date)
            AND (p.IsActive=1 OR p.EmploymentEndDate IS NOT NULL)
        JOIN dbo.Shift v ON v.ShiftId=p.ShiftId JOIN dbo.Department d ON d.DepartmentId=p.DepartmentId
        WHERE CASE ((DATEDIFF(day,CONVERT(date,''19000101''),t.Date)%7)+7)%7
            WHEN 0 THEN v.Monday WHEN 1 THEN v.Tuesday WHEN 2 THEN v.Wednesday WHEN 3 THEN v.Thursday
            WHEN 4 THEN v.Friday WHEN 5 THEN v.Saturday ELSE v.Sunday END=1
    ), Calculated AS
    (
        SELECT b.* FROM Expected b WHERE b.PlannedEnd<=@Now
        AND NOT EXISTS(SELECT 1 FROM dbo.AttendanceRecord g WHERE g.EmployeeId=b.EmployeeId AND g.AttendanceDate=b.Date AND g.CheckInAt IS NOT NULL)
        AND NOT EXISTS(SELECT 1 FROM dbo.LeaveRequest i WHERE i.EmployeeId=b.EmployeeId AND i.Status=N''Approved'' AND b.Date BETWEEN i.StartDate AND i.EndDate)
    )
    SELECT dv.AbsenceId,dv.EmployeeId,p.FirstName+'' ''+p.LastName AS EmployeeFullName,d.DepartmentName,
        dv.AbsenceDate,dv.Excuse,dv.Description
    FROM dbo.AbsenceRecord dv JOIN dbo.Employee p ON p.EmployeeId=dv.EmployeeId JOIN dbo.Department d ON d.DepartmentId=p.DepartmentId
    WHERE dv.AbsenceDate BETWEEN @Start AND @End
    UNION ALL
    SELECT CAST(NULL AS int),o.EmployeeId,o.EmployeeFullName,o.DepartmentName,o.Date,
        CAST(NULL AS nvarchar(500)),CAST(N''Shift completed; no check-in or approved leave found.'' AS nvarchar(500))
    FROM Calculated o WHERE NOT EXISTS(SELECT 1 FROM dbo.AbsenceRecord dv WHERE dv.EmployeeId=o.EmployeeId AND dv.AbsenceDate=o.Date)
);';
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
