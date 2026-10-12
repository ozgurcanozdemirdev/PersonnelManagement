-- Refreshes report objects and covering indexes without deleting records.
-- Select the matching database in SSMS. Refreshes report objects without deleting records.
SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.Employee',N'U') IS NULL THROW 51000,N'Unexpected database schema.',1;
BEGIN TRY
BEGIN TRANSACTION;
-- Validate existing data before enabling personnel constraints; invalid rows are not rewritten.
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Employee') AND name=N'CK_Employee_NationalIdDigits')
    ALTER TABLE dbo.Employee WITH CHECK ADD CONSTRAINT CK_Employee_NationalIdDigits CHECK (LEN(NationalId)=11 AND NationalId COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Employee') AND name=N'CK_Employee_BirthDate')
    ALTER TABLE dbo.Employee WITH CHECK ADD CONSTRAINT CK_Employee_BirthDate CHECK (BirthDate IS NULL OR (BirthDate <= CONVERT(date,GETDATE()) AND BirthDate <= EmploymentStartDate));
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Employee') AND name=N'IX_Employee_NameLookup')
    CREATE INDEX IX_Employee_NameLookup ON dbo.Employee(FirstName,LastName,EmployeeId) INCLUDE (IsActive);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Employee') AND name=N'IX_Employee_ActiveShift')
    CREATE INDEX IX_Employee_ActiveShift ON dbo.Employee(ShiftId) INCLUDE (EmployeeId) WHERE IsActive=1;

-- Allow the HR manager role while preserving legacy account records.
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UserAccount') AND name=N'CK_UserAccount_Role' AND definition LIKE N'%''Manager''%')
BEGIN
    IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UserAccount') AND name=N'CK_UserAccount_Role')
        ALTER TABLE dbo.UserAccount DROP CONSTRAINT CK_UserAccount_Role;
    ALTER TABLE dbo.UserAccount WITH CHECK ADD CONSTRAINT CK_UserAccount_Role CHECK (Role IN (N'Admin',N'Manager',N'Employee',N'User'));
END;


IF NOT EXISTS(SELECT 1 FROM sys.indexes i WHERE i.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND i.name=N'UX_AttendanceRecord_EmployeeDate' AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'CheckInAt') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'WorkMinutes') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'OvertimeMinutes') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'IsLate') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'LeftEarly'))
BEGIN
    IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND name=N'UX_AttendanceRecord_EmployeeDate')
        CREATE UNIQUE INDEX UX_AttendanceRecord_EmployeeDate ON dbo.AttendanceRecord(EmployeeId,AttendanceDate) INCLUDE (CheckInAt,WorkMinutes,OvertimeMinutes,IsLate,LeftEarly) WITH (DROP_EXISTING=ON);
    ELSE
        CREATE UNIQUE INDEX UX_AttendanceRecord_EmployeeDate ON dbo.AttendanceRecord(EmployeeId,AttendanceDate) INCLUDE (CheckInAt,WorkMinutes,OvertimeMinutes,IsLate,LeftEarly);
END;

IF NOT EXISTS(SELECT 1 FROM sys.indexes i WHERE i.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND i.name=N'IX_AttendanceRecord_Date' AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'EmployeeId') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'WorkMinutes') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'OvertimeMinutes') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'IsLate') AND EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND ic.index_id=i.index_id AND ic.is_included_column=1 AND c.name=N'LeftEarly'))
BEGIN
    IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.AttendanceRecord') AND name=N'IX_AttendanceRecord_Date')
        CREATE INDEX IX_AttendanceRecord_Date ON dbo.AttendanceRecord(AttendanceDate) INCLUDE (EmployeeId,WorkMinutes,OvertimeMinutes,IsLate,LeftEarly) WITH (DROP_EXISTING=ON);
    ELSE
        CREATE INDEX IX_AttendanceRecord_Date ON dbo.AttendanceRecord(AttendanceDate) INCLUDE (EmployeeId,WorkMinutes,OvertimeMinutes,IsLate,LeftEarly);
END;

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
