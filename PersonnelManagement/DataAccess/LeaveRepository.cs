using System.Threading;
using System.Threading.Tasks;
using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.DataAccess
{
  public class LeaveRepository
  {
    public List<LeaveRequest> GetEmployeeLeaves(int employeeId, int? year = null)
    {
      DateTime? start = year.HasValue ? (DateTime? )new DateTime(year.Value, 1, 1) : null;
      return GetByDateRange(start, start?.AddYears(1).AddDays(-1), employeeId);
    }

    public List<LeaveRequest> GetPendingLeaves() => GetByDateRange(null, null, null, "Pending");
    public List<LeaveRequest> GetByDateRange(DateTime? start, DateTime? end, int? employeeId = null, string status = null)
    {
      DateRangeValidation.Require(start, end);
      OperationAuthorization.Require(ApplicationPermission.ViewLeaves);
      var list = new List<LeaveRequest>();
      string sql = @"SELECT i.*,p.FirstName+' '+p.LastName AS EmployeeFullName,it.TypeName AS LeaveTypeName
          FROM dbo.LeaveRequest i JOIN dbo.Employee p ON p.EmployeeId=i.EmployeeId
          JOIN dbo.LeaveType it ON it.LeaveTypeId=i.LeaveTypeId WHERE 1=1" + (start.HasValue ? " AND i.EndDate\u003E=@Start" : "") + (end.HasValue ? " AND i.StartDate\u003C=@End" : "") + (employeeId.HasValue ? " AND i.EmployeeId=@EmployeeId" : "") + (status != null ? " AND i.Status=@Status" : "") + " ORDER BY i.StartDate DESC,i.LeaveId DESC";
      using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection()))
      {
        if (start.HasValue)
          cmd.Parameters.AddWithValue("@Start", start.Value.Date);
        if (end.HasValue)
          cmd.Parameters.AddWithValue("@End", end.Value.Date);
        if (employeeId.HasValue)
          cmd.Parameters.AddWithValue("@EmployeeId", employeeId.Value);
        if (status != null)
          cmd.Parameters.AddWithValue("@Status", status);
        using (var r = cmd.ExecuteReader())
          while (r.Read())
            list.Add(MapLeave(r));
      }

      return list;
    }

    public int Save(LeaveRequest leave)
    {
      if (leave == null || leave.EndDate.Date < leave.StartDate.Date)
        throw new ArgumentException(UiText.Get("InvalidLeaveDateRange"));
      if ((leave.Description ?? "").Length > 500)
        throw new ArgumentException(UiText.Get("DescriptionCannotExceed500Characters"));
      leave.StartDate = leave.StartDate.Date;
      leave.EndDate = leave.EndDate.Date;
      leave.TotalDays = (leave.EndDate - leave.StartDate).Days + 1;
      int id;
      using (var tx = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        OperationAuthorization.Require(ApplicationPermission.CreateLeave, tx);
        LockEmployee(leave.EmployeeId, tx, true);
        ValidateOverlap(leave, tx, 0, true);
        ValidateAllowance(leave, tx);
        using (var cmd = new SqlCommand(@"INSERT dbo.LeaveRequest
            (EmployeeId,LeaveTypeId,StartDate,EndDate,TotalDays,Description)
            VALUES (@EmployeeId,@LeaveTypeId,@Start,@End,@Days,@Description);SELECT CAST(SCOPE_IDENTITY() AS int);", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@EmployeeId", leave.EmployeeId);
          cmd.Parameters.AddWithValue("@LeaveTypeId", leave.LeaveTypeId);
          cmd.Parameters.AddWithValue("@Start", leave.StartDate);
          cmd.Parameters.AddWithValue("@End", leave.EndDate);
          cmd.Parameters.AddWithValue("@Days", leave.TotalDays);
          cmd.Parameters.AddWithValue("@Description", (object)leave.Description ?? DBNull.Value);
          id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        AuditLogger.Log("Insert", "LeaveRequest", id, $"EmployeeId:{leave.EmployeeId}", transaction: tx);
        tx.Commit();
      }
      return id;
    }

    public bool Approve(int leaveId, int reviewedByUserId) => ChangeStatus(leaveId, reviewedByUserId, "Approved", null);
    public bool Reject(int leaveId, int reviewedByUserId, string rejectionReason)
    {
      if (string.IsNullOrWhiteSpace(rejectionReason) || rejectionReason.Trim().Length > 500)
        throw new ArgumentException(UiText.Get("RejectionReasonIsRequiredAndCannotExceed500Characters"));
      return ChangeStatus(leaveId, reviewedByUserId, "Rejected", rejectionReason.Trim());
    }

    public bool Cancel(int leaveId, int actorUserId) => ChangeStatus(leaveId, actorUserId, "Cancelled", null);
    private bool ChangeStatus(int id, int userId, string target, string reason)
    {
      using (var tx = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        OperationAuthorization.Require(ApplicationPermission.ReviewLeave, tx, userId);
        int employeeId;
        using (var cmd = new SqlCommand("SELECT EmployeeId FROM dbo.LeaveRequest WHERE LeaveId=@Id", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", id);
          object value = cmd.ExecuteScalar();
          if (value == null)
            return false;
          employeeId = Convert.ToInt32(value);
        }

        // Personel kilidi, farklı taleplerin hak/çakışma kontrolünü de sıraya alır.
        LockEmployee(employeeId, tx, target == "Approved");
        LeaveRequest leave;
        string current;
        using (var cmd = new SqlCommand("SELECT * FROM dbo.LeaveRequest WITH (UPDLOCK,HOLDLOCK) WHERE LeaveId=@Id", SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Id", id);
          using (var r = cmd.ExecuteReader())
          {
            if (!r.Read())
              return false;
            if (Convert.ToInt32(r["EmployeeId"]) != employeeId)
              throw new InvalidOperationException(UiText.Get("TheLeaveRecordHasChangedRefreshTheList"));
            current = r["Status"].ToString();
            leave = new LeaveRequest
            {
              EmployeeId = employeeId,
              LeaveTypeId = Convert.ToInt32(r["LeaveTypeId"]),
              StartDate = Convert.ToDateTime(r["StartDate"]),
              EndDate = Convert.ToDateTime(r["EndDate"])
            };
          }
        }

        bool valid = target == "Cancelled" ? (current == "Pending" || current == "Approved") : current == "Pending";
        if (!valid)
          throw new InvalidOperationException(UiText.Get("ThisLeaveStatusDoesNotAllowTheOperationRefreshTheList"));
        if (target == "Approved")
        {
          ValidateOverlap(leave, tx, id, false);
          ValidateAllowance(leave, tx);
        }

        string sql = target == "Cancelled" ? "UPDATE dbo.LeaveRequest SET Status=@Status,CancelledByUserId=@UserId,CancelledAt=GETDATE() WHERE LeaveId=@Id AND Status=@Current" : "UPDATE dbo.LeaveRequest SET Status=@Status,ReviewedByUserId=@UserId,ReviewedAt=GETDATE(),RejectionReason=@Reason WHERE LeaveId=@Id AND Status=@Current";
        using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), tx))
        {
          cmd.Parameters.AddWithValue("@Status", target);
          cmd.Parameters.AddWithValue("@UserId", userId);
          cmd.Parameters.AddWithValue("@Id", id);
          cmd.Parameters.AddWithValue("@Current", current);
          if (target != "Cancelled")
            cmd.Parameters.AddWithValue("@Reason", (object)reason ?? DBNull.Value);
          if (cmd.ExecuteNonQuery() != 1)
            throw new InvalidOperationException(UiText.Get("LeaveStatusHasChangedRefreshTheList"));
        }

        AuditLogger.Log("Update", "LeaveRequest", id, target, transaction: tx);
        tx.Commit();
      }
      return true;
    }

    private void LockEmployee(int id, SqlTransaction tx, bool active)
    {
      using (var cmd = new SqlCommand("SELECT IsActive FROM dbo.Employee WITH (UPDLOCK,HOLDLOCK) WHERE EmployeeId=@Id", SqlDatabaseConnection.GetConnection(), tx))
      {
        cmd.Parameters.AddWithValue("@Id", id);
        object value = cmd.ExecuteScalar();
        if (value == null || (active && !Convert.ToBoolean(value)))
          throw new InvalidOperationException(UiText.Get("NoActiveEmployeeWasFound"));
      }
    }

    private void ValidateOverlap(LeaveRequest leave, SqlTransaction tx, int excludedId, bool includePending)
    {
      using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.LeaveRequest WITH (UPDLOCK,HOLDLOCK)
          WHERE EmployeeId=@EmployeeId AND LeaveId<>@Id AND (Status=N'Approved'" + (includePending ? " OR Status=N\u0027Pending\u0027" : "") + ") AND EndDate\u003E=@Start AND StartDate\u003C=@End", SqlDatabaseConnection.GetConnection(), tx))
      {
        cmd.Parameters.AddWithValue("@EmployeeId", leave.EmployeeId);
        cmd.Parameters.AddWithValue("@Id", excludedId);
        cmd.Parameters.AddWithValue("@Start", leave.StartDate);
        cmd.Parameters.AddWithValue("@End", leave.EndDate);
        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
          throw new InvalidOperationException(UiText.Get("AnOverlappingLeaveRequestExistsForTheseDates"));
      }
    }

    private void ValidateAllowance(LeaveRequest leave, SqlTransaction tx)
    {
      object affectsAllowance;
      using (var cmd = new SqlCommand("SELECT AffectsAnnualAllowance FROM dbo.LeaveType WHERE LeaveTypeId=@Id AND IsActive=1", SqlDatabaseConnection.GetConnection(), tx))
      {
        cmd.Parameters.AddWithValue("@Id", leave.LeaveTypeId);
        affectsAllowance = cmd.ExecuteScalar();
      }

      if (affectsAllowance == null || affectsAllowance == DBNull.Value)
        throw new InvalidOperationException(UiText.Get("NoActiveLeaveTypeWasFound"));
      if (!Convert.ToBoolean(affectsAllowance))
        return;
      // Yıl sınırını geçen talebin her yıl içindeki günleri ayrı kontrol edilir; devir eklenmez.
      for (int year = leave.StartDate.Year; year <= leave.EndDate.Year; year++)
      {
        DateTime start = new DateTime(year, 1, 1), end = new DateTime(year, 12, 31);
        DateTime first = leave.StartDate > start ? leave.StartDate : start, last = leave.EndDate < end ? leave.EndDate : end;
        int requested = (last - first).Days + 1;
        if (GetRemainingAnnualLeave(leave.EmployeeId, year, tx) < requested)
          throw new InvalidOperationException(UiText.Format("InsufficientRemainingAnnualLeaveFor", year));
      }
    }

    public int GetRemainingAnnualLeave(int employeeId) => GetRemainingAnnualLeave(employeeId, DateTime.Today.Year, null);
    public int GetRemainingAnnualLeave(int employeeId, int year) => GetRemainingAnnualLeave(employeeId, year, null);
    private int GetRemainingAnnualLeave(int employeeId, int year, SqlTransaction tx)
    {
      if (tx == null)
        OperationAuthorization.Require(ApplicationPermission.ViewLeaves);
      using (var cmd = new SqlCommand(@"SELECT p.AnnualLeaveAllowance-ISNULL((SELECT SUM(DATEDIFF(day,
            CASE WHEN i.StartDate<@Start THEN @Start ELSE i.StartDate END,
            CASE WHEN i.EndDate>@End THEN @End ELSE i.EndDate END)+1)
          FROM dbo.LeaveRequest i JOIN dbo.LeaveType it ON it.LeaveTypeId=i.LeaveTypeId
          WHERE i.EmployeeId=p.EmployeeId AND i.Status=N'Approved' AND it.AffectsAnnualAllowance=1
            AND i.EndDate>=@Start AND i.StartDate<=@End),0)
          FROM dbo.Employee p WHERE p.EmployeeId=@EmployeeId", SqlDatabaseConnection.GetConnection(), tx))
      {
        cmd.Parameters.AddWithValue("@Start", new DateTime(year, 1, 1));
        cmd.Parameters.AddWithValue("@End", new DateTime(year, 12, 31));
        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
        object value = cmd.ExecuteScalar();
        return value == null ? 0 : Convert.ToInt32(value);
      }
    }

    private LeaveRequest MapLeave(SqlDataReader r) => new LeaveRequest
    {
      LeaveId = Convert.ToInt32(r["LeaveId"]),
      EmployeeId = Convert.ToInt32(r["EmployeeId"]),
      EmployeeFullName = r["EmployeeFullName"].ToString(),
      LeaveTypeId = Convert.ToInt32(r["LeaveTypeId"]),
      LeaveTypeName = r["LeaveTypeName"].ToString(),
      StartDate = Convert.ToDateTime(r["StartDate"]),
      EndDate = Convert.ToDateTime(r["EndDate"]),
      TotalDays = Convert.ToInt32(r["TotalDays"]),
      Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
      Status = r["Status"].ToString(),
      RejectionReason = r["RejectionReason"] == DBNull.Value ? null : r["RejectionReason"].ToString(),
      ReviewedByUserId = r["ReviewedByUserId"] == DBNull.Value ? (int? )null : Convert.ToInt32(r["ReviewedByUserId"]),
      ReviewedAt = r["ReviewedAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["ReviewedAt"]),
      CreatedAt = Convert.ToDateTime(r["CreatedAt"])
    };
    public async Task<List<LeaveRequest>> GetByDateRangeAsync(DateTime? start, DateTime? end, int? employeeId = null, string status = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      DateRangeValidation.Require(start, end);
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.ViewLeaves, connection, cancellationToken).ConfigureAwait(false);
          var list = new List<LeaveRequest>();
          string sql = @"SELECT i.*,p.FirstName+' '+p.LastName AS EmployeeFullName,it.TypeName AS LeaveTypeName
              FROM dbo.LeaveRequest i JOIN dbo.Employee p ON p.EmployeeId=i.EmployeeId
              JOIN dbo.LeaveType it ON it.LeaveTypeId=i.LeaveTypeId WHERE 1=1" + (start.HasValue ? " AND i.EndDate\u003E=@Start" : "") + (end.HasValue ? " AND i.StartDate\u003C=@End" : "") + (employeeId.HasValue ? " AND i.EmployeeId=@EmployeeId" : "") + (status != null ? " AND i.Status=@Status" : "") + " ORDER BY i.StartDate DESC,i.LeaveId DESC";
          using (var cmd = new SqlCommand(sql, connection))
          {
            if (start.HasValue)
              cmd.Parameters.AddWithValue("@Start", start.Value.Date);
            if (end.HasValue)
              cmd.Parameters.AddWithValue("@End", end.Value.Date);
            if (employeeId.HasValue)
              cmd.Parameters.AddWithValue("@EmployeeId", employeeId.Value);
            if (status != null)
              cmd.Parameters.AddWithValue("@Status", status);
            using (var r = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
              while (await r.ReadAsync(cancellationToken).ConfigureAwait(false))
                list.Add(MapLeave(r));
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