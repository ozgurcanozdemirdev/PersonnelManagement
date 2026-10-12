using System.Threading;
using System.Threading.Tasks;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.DataAccess
{
  public class ReportRepository
  {
    public List<EmployeeSummaryReport> GetEmployeeSummary(DateTime startDate, DateTime endDate, int? employeeId = null)
    {
      DateRangeValidation.Require(startDate, endDate);
      OperationAuthorization.Require(ApplicationPermission.ViewReports);
      var list = new List<EmployeeSummaryReport>();
      // Dönem toplamlarını ayrı gruplamak, giriş ve izin satırlarının birbirini çoğaltmasını önler.
      string sql = @"WITH Employees AS
          (
            SELECT p.EmployeeId,p.FirstName,p.LastName,p.DepartmentId FROM dbo.Employee p
            WHERE p.EmploymentStartDate<=@End AND (p.EmploymentEndDate IS NULL OR p.EmploymentEndDate>=@Start)" +
          (employeeId.HasValue ? " AND p.EmployeeId=@EmployeeId" : "") + @"
          ), Attendance AS
          (
            SELECT g.EmployeeId,COUNT(*) AS TotalWorkDays,SUM(g.WorkMinutes) AS TotalWorkMinutes,
              SUM(g.OvertimeMinutes) AS TotalOvertimeMinutes,SUM(CAST(g.IsLate AS int)) AS LateArrivalCount,
              SUM(CAST(g.LeftEarly AS int)) AS EarlyDepartureCount
            FROM dbo.AttendanceRecord g JOIN Employees p ON p.EmployeeId=g.EmployeeId
            WHERE g.AttendanceDate BETWEEN @Start AND @End GROUP BY g.EmployeeId
          ), Leaves AS
          (
            SELECT i.EmployeeId,SUM(DATEDIFF(day,
              CASE WHEN i.StartDate<@Start THEN @Start ELSE i.StartDate END,
              CASE WHEN i.EndDate>@End THEN @End ELSE i.EndDate END)+1) AS TotalLeaveDays
            FROM dbo.LeaveRequest i JOIN Employees p ON p.EmployeeId=i.EmployeeId
            WHERE i.Status=N'Approved' AND i.EndDate>=@Start AND i.StartDate<=@End GROUP BY i.EmployeeId
          ), Absences AS
          (
            SELECT a.EmployeeId,COUNT(*) AS TotalAbsences
            FROM dbo.GetAbsences(@Start,@End,@Now) a JOIN Employees p ON p.EmployeeId=a.EmployeeId
            GROUP BY a.EmployeeId
          )
          SELECT p.EmployeeId,p.FirstName+' '+p.LastName AS FullName,d.DepartmentName,
            ISNULL(g.TotalWorkDays,0) AS TotalWorkDays,ISNULL(g.TotalWorkMinutes,0) AS TotalWorkMinutes,
            ISNULL(g.TotalOvertimeMinutes,0) AS TotalOvertimeMinutes,ISNULL(g.LateArrivalCount,0) AS LateArrivalCount,
            ISNULL(g.EarlyDepartureCount,0) AS EarlyDepartureCount,ISNULL(a.TotalAbsences,0) AS TotalAbsences,
            ISNULL(i.TotalLeaveDays,0) AS TotalLeaveDays
          FROM Employees p JOIN dbo.Department d ON d.DepartmentId=p.DepartmentId
          LEFT JOIN Attendance g ON g.EmployeeId=p.EmployeeId
          LEFT JOIN Leaves i ON i.EmployeeId=p.EmployeeId
          LEFT JOIN Absences a ON a.EmployeeId=p.EmployeeId
          ORDER BY p.FirstName,p.LastName OPTION (MAXRECURSION 0)";
      using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.Add("@Start", SqlDbType.Date).Value = startDate.Date;
        cmd.Parameters.Add("@End", SqlDbType.Date).Value = endDate.Date;
        cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.Now;
        if (employeeId.HasValue)
          cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId.Value;
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            list.Add(new EmployeeSummaryReport { EmployeeId = Convert.ToInt32(r["EmployeeId"]), FullName = r["FullName"].ToString(), DepartmentName = r["DepartmentName"].ToString(), TotalWorkDays = Convert.ToInt32(r["TotalWorkDays"]), TotalWorkMinutes = Convert.ToInt32(r["TotalWorkMinutes"]), TotalOvertimeMinutes = Convert.ToInt32(r["TotalOvertimeMinutes"]), TotalAbsences = Convert.ToInt32(r["TotalAbsences"]), TotalLeaveDays = Convert.ToInt32(r["TotalLeaveDays"]), LateArrivalCount = Convert.ToInt32(r["LateArrivalCount"]), EarlyDepartureCount = Convert.ToInt32(r["EarlyDepartureCount"]) });
      }

      return list;
    }

    public System.Data.DataTable GetAbsenceReport(DateTime startDate, DateTime endDate, int? employeeId = null)
    {
      DateRangeValidation.Require(startDate, endDate);
      OperationAuthorization.Require(ApplicationPermission.ViewReports);
      // Ana menü ve dönem raporu aynı gün/izin/vardiya bitişi kuralını kullanır.
      string sql = "SELECT * FROM dbo.GetAbsences(@Start,@End,@Now)" + (employeeId.HasValue ? " WHERE EmployeeId=@EmployeeId" : "") + " ORDER BY AbsenceDate DESC,EmployeeFullName OPTION (MAXRECURSION 0)";
      using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.Add("@Start", SqlDbType.Date).Value = startDate.Date;
        cmd.Parameters.Add("@End", SqlDbType.Date).Value = endDate.Date;
        cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.Now;
        if (employeeId.HasValue)
          cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId.Value;
        var table = new System.Data.DataTable();
        using (var adapter = new SqlDataAdapter(cmd))
          adapter.Fill(table);
        return table;
      }
    }

    public (int TotalEmployees, int TodayCheckIns, int TodayLeaves, int TodayAbsences) GetTodaySummary()
    {
      OperationAuthorization.Require(ApplicationPermission.ViewDashboard);
      using (var cmd = new SqlCommand(@"SELECT
          (SELECT COUNT(*) FROM dbo.Employee WHERE IsActive=1 AND EmploymentStartDate<=@Today),
          (SELECT COUNT(DISTINCT g.EmployeeId) FROM dbo.AttendanceRecord g JOIN dbo.Employee p ON p.EmployeeId=g.EmployeeId
            WHERE p.IsActive=1 AND g.AttendanceDate=@Today),
          (SELECT COUNT(DISTINCT i.EmployeeId) FROM dbo.LeaveRequest i JOIN dbo.Employee p ON p.EmployeeId=i.EmployeeId
            WHERE p.IsActive=1 AND p.EmploymentStartDate<=@Today AND i.Status=N'Approved' AND @Today BETWEEN i.StartDate AND i.EndDate),
          (SELECT COUNT(DISTINCT dv.EmployeeId) FROM dbo.GetAbsences(@Today,@Today,@Now) dv
            JOIN dbo.Employee p ON p.EmployeeId=dv.EmployeeId WHERE p.IsActive=1) OPTION (MAXRECURSION 0)", SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.Add("@Today", SqlDbType.Date).Value = DateTime.Today;
        cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.Now;
        using (var r = cmd.ExecuteReader())
          if (r.Read())
            return (Convert.ToInt32(r[0]), Convert.ToInt32(r[1]), Convert.ToInt32(r[2]), Convert.ToInt32(r[3]));
      }

      return (0, 0, 0, 0);
    }
    public async Task<List<EmployeeSummaryReport>> GetEmployeeSummaryAsync(DateTime startDate, DateTime endDate, int? employeeId = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      DateRangeValidation.Require(startDate, endDate);
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.ViewReports, connection, cancellationToken).ConfigureAwait(false);
          var list = new List<EmployeeSummaryReport>();
          // Dönem toplamlarını ayrı gruplamak, giriş ve izin satırlarının birbirini çoğaltmasını önler.
          string sql = @"WITH Employees AS
              (
                SELECT p.EmployeeId,p.FirstName,p.LastName,p.DepartmentId FROM dbo.Employee p
                WHERE p.EmploymentStartDate<=@End AND (p.EmploymentEndDate IS NULL OR p.EmploymentEndDate>=@Start)" +
              (employeeId.HasValue ? " AND p.EmployeeId=@EmployeeId" : "") + @"
              ), Attendance AS
              (
                SELECT g.EmployeeId,COUNT(*) AS TotalWorkDays,SUM(g.WorkMinutes) AS TotalWorkMinutes,
                  SUM(g.OvertimeMinutes) AS TotalOvertimeMinutes,SUM(CAST(g.IsLate AS int)) AS LateArrivalCount,
                  SUM(CAST(g.LeftEarly AS int)) AS EarlyDepartureCount
                FROM dbo.AttendanceRecord g JOIN Employees p ON p.EmployeeId=g.EmployeeId
                WHERE g.AttendanceDate BETWEEN @Start AND @End GROUP BY g.EmployeeId
              ), Leaves AS
              (
                SELECT i.EmployeeId,SUM(DATEDIFF(day,
                  CASE WHEN i.StartDate<@Start THEN @Start ELSE i.StartDate END,
                  CASE WHEN i.EndDate>@End THEN @End ELSE i.EndDate END)+1) AS TotalLeaveDays
                FROM dbo.LeaveRequest i JOIN Employees p ON p.EmployeeId=i.EmployeeId
                WHERE i.Status=N'Approved' AND i.EndDate>=@Start AND i.StartDate<=@End GROUP BY i.EmployeeId
              ), Absences AS
              (
                SELECT a.EmployeeId,COUNT(*) AS TotalAbsences
                FROM dbo.GetAbsences(@Start,@End,@Now) a JOIN Employees p ON p.EmployeeId=a.EmployeeId
                GROUP BY a.EmployeeId
              )
              SELECT p.EmployeeId,p.FirstName+' '+p.LastName AS FullName,d.DepartmentName,
                ISNULL(g.TotalWorkDays,0) AS TotalWorkDays,ISNULL(g.TotalWorkMinutes,0) AS TotalWorkMinutes,
                ISNULL(g.TotalOvertimeMinutes,0) AS TotalOvertimeMinutes,ISNULL(g.LateArrivalCount,0) AS LateArrivalCount,
                ISNULL(g.EarlyDepartureCount,0) AS EarlyDepartureCount,ISNULL(a.TotalAbsences,0) AS TotalAbsences,
                ISNULL(i.TotalLeaveDays,0) AS TotalLeaveDays
              FROM Employees p JOIN dbo.Department d ON d.DepartmentId=p.DepartmentId
              LEFT JOIN Attendance g ON g.EmployeeId=p.EmployeeId
              LEFT JOIN Leaves i ON i.EmployeeId=p.EmployeeId
              LEFT JOIN Absences a ON a.EmployeeId=p.EmployeeId
              ORDER BY p.FirstName,p.LastName OPTION (MAXRECURSION 0)";
          using (var cmd = new SqlCommand(sql, connection))
          {
            cmd.Parameters.Add("@Start", SqlDbType.Date).Value = startDate.Date;
            cmd.Parameters.Add("@End", SqlDbType.Date).Value = endDate.Date;
            cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.Now;
            if (employeeId.HasValue)
              cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId.Value;
            using (var r = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
              while (await r.ReadAsync(cancellationToken).ConfigureAwait(false))
                list.Add(new EmployeeSummaryReport { EmployeeId = Convert.ToInt32(r["EmployeeId"]), FullName = r["FullName"].ToString(), DepartmentName = r["DepartmentName"].ToString(), TotalWorkDays = Convert.ToInt32(r["TotalWorkDays"]), TotalWorkMinutes = Convert.ToInt32(r["TotalWorkMinutes"]), TotalOvertimeMinutes = Convert.ToInt32(r["TotalOvertimeMinutes"]), TotalAbsences = Convert.ToInt32(r["TotalAbsences"]), TotalLeaveDays = Convert.ToInt32(r["TotalLeaveDays"]), LateArrivalCount = Convert.ToInt32(r["LateArrivalCount"]), EarlyDepartureCount = Convert.ToInt32(r["EarlyDepartureCount"]) });
          }

          return list;
        }
      }
      catch (SqlException ex) when (cancellationToken.IsCancellationRequested)
      {
        // System.Data.SqlClient, devam eden SQL iptalini SqlException olarak da döndürebilir.
        throw new OperationCanceledException("The database read was cancelled.", ex, cancellationToken);
      }
    }

    public async Task<System.Data.DataTable> GetAbsenceReportAsync(DateTime startDate, DateTime endDate, int? employeeId = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      DateRangeValidation.Require(startDate, endDate);
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.ViewReports, connection, cancellationToken).ConfigureAwait(false);
          // Ana menü ve dönem raporu aynı gün/izin/vardiya bitişi kuralını kullanır.
          string sql = "SELECT * FROM dbo.GetAbsences(@Start,@End,@Now)" + (employeeId.HasValue ? " WHERE EmployeeId=@EmployeeId" : "") + " ORDER BY AbsenceDate DESC,EmployeeFullName OPTION (MAXRECURSION 0)";
          using (var cmd = new SqlCommand(sql, connection))
          {
            cmd.Parameters.Add("@Start", SqlDbType.Date).Value = startDate.Date;
            cmd.Parameters.Add("@End", SqlDbType.Date).Value = endDate.Date;
            cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.Now;
            if (employeeId.HasValue)
              cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId.Value;
            var table = new System.Data.DataTable();
            using (var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
              for (int index = 0; index < reader.FieldCount; index++)
                table.Columns.Add(reader.GetName(index), reader.GetFieldType(index));
              table.BeginLoadData();
              try
              {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                  var values = new object[reader.FieldCount];
                  reader.GetValues(values);
                  table.Rows.Add(values);
                }
              }
              finally { table.EndLoadData(); }
            }
            return table;
          }
        }
      }
      catch (SqlException ex) when (cancellationToken.IsCancellationRequested)
      {
        // System.Data.SqlClient, devam eden SQL iptalini SqlException olarak da döndürebilir.
        throw new OperationCanceledException("The database read was cancelled.", ex, cancellationToken);
      }
    }
  }
}
