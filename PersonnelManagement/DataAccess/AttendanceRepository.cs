using System.Threading;
using System.Threading.Tasks;
using PersonnelManagement.UI.Localization;
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
  public class AttendanceRepository
  {
    public int CheckIn(int employeeId, int? recordedByUserId = null)
    {
      DateTime now = DatabaseTimestamp(DateTime.Now);
      int newId;
      using (var tx = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        recordedByUserId = OperationAuthorization.Require(ApplicationPermission.RecordAttendance, tx, recordedByUserId).UserId;
        // Personel kilidi, farklı terminallerdeki giriş/çıkış işlemlerini sıraya alır.
        LockEmployee(employeeId, tx, true);
        TimeSpan start, end;
        int breakMinutes;
        string firstName;
        using (var cmd = new SqlCommand(@"SELECT v.StartTime,v.EndTime,v.BreakMinutes,v.ShiftName
          FROM dbo.Employee p JOIN dbo.Shift v ON v.ShiftId=p.ShiftId
          WHERE p.EmployeeId=@EmployeeId AND v.IsActive=1", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
          using (var r = cmd.ExecuteReader())
          {
            if (!r.Read())
              throw new InvalidOperationException(UiText.Get("NoActiveShiftWasFound"));
            start = (TimeSpan)r[0];
            end = (TimeSpan)r[1];
            breakMinutes = Convert.ToInt32(r[2]);
            firstName = r[3].ToString();
          }
        }

        var shiftPlan = GetShiftPlan(now, start, end);
        if (breakMinutes < 0 || breakMinutes >= (shiftPlan.EndDate - shiftPlan.StartDate).TotalMinutes)
          throw new InvalidOperationException(UiText.Get("BreakDurationMustBeShorterThanTheShift"));
        using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.AttendanceRecord WITH (UPDLOCK,HOLDLOCK)
          WHERE EmployeeId=@EmployeeId AND (AttendanceDate=@Date OR CheckOutTime IS NULL)", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
          cmd.Parameters.AddWithValue("@Date", shiftPlan.StartDate.Date);
          if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            throw new InvalidOperationException(UiText.Get("AnAttendanceRecordAlreadyExistsForThisShiftDayOrAnEarlierRecordIsStillOpen"));
        }

        // Vardiya planının kopyası tutulur; sonraki düzenlemeler bu kaydın hesabını değiştirmez.
        using (var cmd = new SqlCommand(@"INSERT dbo.AttendanceRecord
          (EmployeeId,AttendanceDate,CheckInTime,CheckInAt,IsLate,RecordedByUserId,
           PlannedCheckInAt,PlannedCheckOutAt,PlannedBreakMinutes,RecordedShiftName)
          VALUES (@EmployeeId,@Date,@Time,@Now,@Late,@UserId,@Start,@End,@Break,@FirstName);
          SELECT CAST(SCOPE_IDENTITY() AS int);", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
          cmd.Parameters.AddWithValue("@Date", shiftPlan.StartDate.Date);
          cmd.Parameters.Add("@Time", SqlDbType.Time).Value = now.TimeOfDay;
          cmd.Parameters.AddWithValue("@Now", now);
          cmd.Parameters.AddWithValue("@Late", now > shiftPlan.StartDate.AddMinutes(5));
          cmd.Parameters.AddWithValue("@UserId", (object)recordedByUserId ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Start", shiftPlan.StartDate);
          cmd.Parameters.AddWithValue("@End", shiftPlan.EndDate);
          cmd.Parameters.AddWithValue("@Break", breakMinutes);
          cmd.Parameters.AddWithValue("@FirstName", firstName);
          newId = Convert.ToInt32(cmd.ExecuteScalar());
        }

        AuditLogger.Log("CheckIn", "AttendanceRecord", newId, $"EmployeeId:{employeeId} - {now:dd.MM.yyyy HH:mm}", transaction: tx);
        tx.Commit();
      }
      return newId;
    }

    public bool CheckOut(int employeeId, int? recordedByUserId = null)
    {
      DateTime now = DatabaseTimestamp(DateTime.Now);
      int id;
      using (var tx = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        recordedByUserId = OperationAuthorization.Require(ApplicationPermission.RecordAttendance, tx, recordedByUserId).UserId;
        LockEmployee(employeeId, tx, false);
        DateTime checkIn, start, end;
        int breakMinutes;
        // Açık kayıt tarihle sınırlandırılmaz; gece yarısı önceki günün girişini kapatmayı engellemez.
        using (var cmd = new SqlCommand(@"SELECT TOP (1) AttendanceId,CheckInAt,PlannedCheckInAt,
          PlannedCheckOutAt,PlannedBreakMinutes FROM dbo.AttendanceRecord WITH (UPDLOCK,HOLDLOCK)
          WHERE EmployeeId=@EmployeeId AND CheckOutTime IS NULL ORDER BY CheckInAt DESC,AttendanceId DESC", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
          using (var r = cmd.ExecuteReader())
          {
            if (!r.Read())
              throw new InvalidOperationException(UiText.Get("ThisEmployeeHasNoOpenAttendanceRecord"));
            if (r.IsDBNull(1) || r.IsDBNull(2) || r.IsDBNull(3) || r.IsDBNull(4))
              throw new InvalidOperationException(UiText.Get("AttendanceTimeOrShiftSnapshotIsMissingCheckTheDatabaseUpdate"));
            id = r.GetInt32(0);
            checkIn = r.GetDateTime(1);
            start = r.GetDateTime(2);
            end = r.GetDateTime(3);
            breakMinutes = r.GetInt32(4);
          }
        }

        var duration = CalculateWorkDuration(checkIn, now, start, end, breakMinutes);
        using (var cmd = new SqlCommand(@"UPDATE dbo.AttendanceRecord SET CheckOutTime=@Time,CheckOutAt=@Now,
          WorkMinutes=@WorkMinutes,OvertimeMinutes=@Overtime,LeftEarly=@Early,RecordedByUserId=ISNULL(RecordedByUserId,@UserId)
          WHERE AttendanceId=@Id AND CheckOutTime IS NULL", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.Add("@Time", SqlDbType.Time).Value = now.TimeOfDay;
          cmd.Parameters.AddWithValue("@Now", now);
          cmd.Parameters.AddWithValue("@WorkMinutes", duration.WorkMinutes);
          cmd.Parameters.AddWithValue("@Overtime", duration.OvertimeMinutes);
          cmd.Parameters.AddWithValue("@Early", now < end.AddMinutes(-5));
          cmd.Parameters.AddWithValue("@Id", id);
          cmd.Parameters.AddWithValue("@UserId", (object)recordedByUserId ?? DBNull.Value);
          if (cmd.ExecuteNonQuery() != 1)
            throw new InvalidOperationException(UiText.Get("AnotherOperationHasAlreadyClosedThisAttendanceRecord"));
        }

        AuditLogger.Log("CheckOut", "AttendanceRecord", id, $"EmployeeId:{employeeId} - {now:dd.MM.yyyy HH:mm}", transaction: tx);
        tx.Commit();
      }
      return true;
    }

    private static DateTime DatabaseTimestamp(DateTime value)
    {
      // SQL datetime zamanı 1/300 saniyeye yuvarlar. Anında çıkışta bellekteki
      // zamanın, kaydedilmiş giriş zamanından geride kalmasını önleriz.
      return new System.Data.SqlTypes.SqlDateTime(value).Value;
    }

    public static (DateTime StartDate, DateTime EndDate) GetShiftPlan(DateTime now, TimeSpan start, TimeSpan end)
    {
      bool overnight = end <= start;
      DateTime date = now.Date;
      if (overnight && now.TimeOfDay < end)
        date = date.AddDays(-1);
      return (date.Add(start), date.Add(end).AddDays(overnight ? 1 : 0));
    }

    public static (int WorkMinutes, int OvertimeMinutes) CalculateWorkDuration(DateTime checkIn, DateTime checkOut, DateTime start, DateTime end, int breakMinutes)
    {
      if (checkOut < checkIn || end <= start || breakMinutes < 0 || breakMinutes >= (end - start).TotalMinutes)
        throw new InvalidOperationException(UiText.Get("WorkTimesOrShiftBreakDurationAreInvalid"));
      int work = Math.Max(0, checked((int)(checkOut - checkIn).TotalMinutes) - breakMinutes);
      int normal = checked((int)(end - start).TotalMinutes) - breakMinutes;
      return (work, Math.Max(0, work - normal));
    }

    private void LockEmployee(int id, SqlTransaction tx, bool active)
    {
      using (var cmd = new SqlCommand("SELECT IsActive,EmploymentStartDate FROM dbo.Employee WITH (UPDLOCK,HOLDLOCK) WHERE EmployeeId=@Id", SqlDatabaseConnection.GetConnection(), tx))
      {
        cmd.Parameters.AddWithValue("@Id", id);
        using (var r = cmd.ExecuteReader())
          if (!r.Read() || (active && (!r.GetBoolean(0) || r.GetDateTime(1).Date > DateTime.Today)))
            throw new InvalidOperationException(UiText.Get("NoActiveEmployeeWhoHasStartedEmploymentWasFound"));
      }
    }

    public bool HasOpenRecord(int id)
    {
      OperationAuthorization.Require(ApplicationPermission.RecordAttendance);
      using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.AttendanceRecord WHERE EmployeeId=@EmployeeId AND CheckOutTime IS NULL", SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@EmployeeId", id);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
      }
    }

    public bool HasTodayCheckIn(int id)
    {
      OperationAuthorization.Require(ApplicationPermission.RecordAttendance);
      using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.AttendanceRecord g JOIN dbo.Employee p ON p.EmployeeId=g.EmployeeId
        JOIN dbo.Shift v ON v.ShiftId=p.ShiftId WHERE g.EmployeeId=@EmployeeId AND g.AttendanceDate=
        CASE WHEN v.EndTime<=v.StartTime AND CAST(@Now AS time)<v.EndTime
        THEN DATEADD(day,-1,CAST(@Now AS date)) ELSE CAST(@Now AS date) END", SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@EmployeeId", id);
        cmd.Parameters.AddWithValue("@Now", DateTime.Now);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
      }
    }

    public bool HasTodayCheckOut(int id) => HasTodayCheckIn(id) && !HasOpenRecord(id);
    public List<AttendanceRecord> GetTodayRecords() => GetRecords(DateTime.Today, DateTime.Today, null, true);
    public List<AttendanceRecord> GetByDateRange(DateTime startDate, DateTime endDate, int? employeeId = null) => GetRecords(startDate.Date, endDate.Date, employeeId, false);
    private List<AttendanceRecord> GetRecords(DateTime start, DateTime end, int? id, bool includeOpen)
    {
      DateRangeValidation.Require(start, end);
      OperationAuthorization.Require(ApplicationPermission.RecordAttendance);
      var list = new List<AttendanceRecord>();
      string sql = "SELECT * FROM dbo.DailyAttendanceSummary WHERE (AttendanceDate BETWEEN @Start AND @End" + (includeOpen ? " OR CheckOutTime IS NULL" : "") + ")" + (id.HasValue ? " AND EmployeeId=@EmployeeId" : "") + " ORDER BY AttendanceDate DESC,FullName";
      using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.AddWithValue("@Start", start);
        cmd.Parameters.AddWithValue("@End", end);
        if (id.HasValue)
          cmd.Parameters.AddWithValue("@EmployeeId", id.Value);
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            list.Add(new AttendanceRecord { AttendanceId = Convert.ToInt32(r["AttendanceId"]), EmployeeId = Convert.ToInt32(r["EmployeeId"]), FullName = r["FullName"].ToString(), DepartmentName = r["DepartmentName"].ToString(), ShiftName = r["ShiftName"].ToString(), AttendanceDate = Convert.ToDateTime(r["AttendanceDate"]), CheckInTime = r["CheckInTime"] == DBNull.Value ? (TimeSpan? )null : (TimeSpan)r["CheckInTime"], CheckOutTime = r["CheckOutTime"] == DBNull.Value ? (TimeSpan? )null : (TimeSpan)r["CheckOutTime"], CheckInAt = r["CheckInAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["CheckInAt"]), CheckOutAt = r["CheckOutAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["CheckOutAt"]), WorkMinutes = r["WorkMinutes"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["WorkMinutes"]), OvertimeMinutes = r["OvertimeMinutes"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["OvertimeMinutes"]), IsLate = Convert.ToBoolean(r["IsLate"]), LeftEarly = Convert.ToBoolean(r["LeftEarly"]), Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString() });
      }

      return list;
    }
    public async Task<List<AttendanceRecord>> GetByDateRangeAsync(DateTime start, DateTime end, int? id = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      DateRangeValidation.Require(start, end);
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.RecordAttendance, connection, cancellationToken).ConfigureAwait(false);
          var list = new List<AttendanceRecord>();
          string sql = "SELECT * FROM dbo.DailyAttendanceSummary WHERE (AttendanceDate BETWEEN @Start AND @End" + "" + ")" + (id.HasValue ? " AND EmployeeId=@EmployeeId" : "") + " ORDER BY AttendanceDate DESC,FullName";
          using (var cmd = new SqlCommand(sql, connection))
          {
            cmd.Parameters.AddWithValue("@Start", start);
            cmd.Parameters.AddWithValue("@End", end);
            if (id.HasValue)
              cmd.Parameters.AddWithValue("@EmployeeId", id.Value);
            using (var r = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
              while (await r.ReadAsync(cancellationToken).ConfigureAwait(false))
                list.Add(new AttendanceRecord { AttendanceId = Convert.ToInt32(r["AttendanceId"]), EmployeeId = Convert.ToInt32(r["EmployeeId"]), FullName = r["FullName"].ToString(), DepartmentName = r["DepartmentName"].ToString(), ShiftName = r["ShiftName"].ToString(), AttendanceDate = Convert.ToDateTime(r["AttendanceDate"]), CheckInTime = r["CheckInTime"] == DBNull.Value ? (TimeSpan? )null : (TimeSpan)r["CheckInTime"], CheckOutTime = r["CheckOutTime"] == DBNull.Value ? (TimeSpan? )null : (TimeSpan)r["CheckOutTime"], CheckInAt = r["CheckInAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["CheckInAt"]), CheckOutAt = r["CheckOutAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["CheckOutAt"]), WorkMinutes = r["WorkMinutes"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["WorkMinutes"]), OvertimeMinutes = r["OvertimeMinutes"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["OvertimeMinutes"]), IsLate = Convert.ToBoolean(r["IsLate"]), LeftEarly = Convert.ToBoolean(r["LeftEarly"]), Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString() });
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
  }
}
