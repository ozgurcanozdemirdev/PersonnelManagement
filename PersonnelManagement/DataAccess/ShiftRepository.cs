using System.Data;
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
  // Vardiya kayıtları çalışma günleriyle birlikte saklanır.
  public class ShiftRepository
  {
    public List<Shift> GetAll(bool activeOnly = true)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewShifts);
      var list = new List<Shift>();
      string sql = activeOnly ? "SELECT * FROM Shift WHERE IsActive = 1 ORDER BY ShiftName" : "SELECT * FROM Shift ORDER BY ShiftName";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          list.Add(MapShift(r));
      return list;
    }

    public Shift GetById(int shiftId)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewShifts);
      string sql = "SELECT * FROM Shift WHERE ShiftId = @Id";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", shiftId);
      using (var r = cmd.ExecuteReader())
        return r.Read() ? MapShift(r) : null;
    }

    public int Save(Shift v)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManageShifts, transaction =>
      {
        ValidateShift(v);
        string sql = @"
                  INSERT INTO Shift
                      (ShiftName, StartTime, EndTime, BreakMinutes,
                       Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)
                  VALUES
                      (@Name, @Start, @End, @Break,
                       @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, @Saturday, @Sunday);
                  SELECT SCOPE_IDENTITY();";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        SetShiftParameters(cmd, v);
        int newId = Convert.ToInt32(cmd.ExecuteScalar());
        AuditLogger.Log("Insert", "Shift", newId, v.ShiftName, transaction: transaction);
        return newId;
      });
    }

    public bool Update(Shift v)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManageShifts, transaction =>
      {
        ValidateShift(v);
        string sql = @"
                  UPDATE Shift SET
                      ShiftName     = @Name,
                      StartTime = @Start,
                      EndTime     = @End,
                      BreakMinutes     = @Break,
                      Monday      = @Monday,
                      Tuesday           = @Tuesday,
                      Wednesday       = @Wednesday,
                      Thursday       = @Thursday,
                      Friday           = @Friday,
                      Saturday      = @Saturday,
                      Sunday          = @Sunday
                  WHERE ShiftId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        SetShiftParameters(cmd, v);
        cmd.Parameters.AddWithValue("@Id", v.ShiftId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok)
          AuditLogger.Log("Update", "Shift", v.ShiftId, v.ShiftName, transaction: transaction);
        return ok;
      });
    }

    public bool Deactivate(int shiftId)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManageShifts, transaction =>
      {
        // Kontrolden sonra yeni personel atanmasını da engellemek için yazma kilidi önce alınır.
        using (var check = new SqlCommand("SELECT IsActive FROM dbo.Shift WITH (XLOCK,HOLDLOCK) WHERE ShiftId=@Id", SqlDatabaseConnection.GetConnection(), transaction))
        {
          check.Parameters.AddWithValue("@Id", shiftId);
          object active = check.ExecuteScalar();
          if (active == null || !Convert.ToBoolean(active))
            return false;
        }
        using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.Employee WHERE ShiftId=@Id AND IsActive=1", SqlDatabaseConnection.GetConnection(), transaction))
        {
          check.Parameters.AddWithValue("@Id", shiftId);
          if (Convert.ToInt32(check.ExecuteScalar()) > 0)
            throw new InvalidOperationException(UiText.Get("AssignedShiftCannotBeDeactivated"));
        }
        using (var cmd = new SqlCommand("UPDATE dbo.Shift SET IsActive=0 WHERE ShiftId=@Id AND IsActive=1", SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.AddWithValue("@Id", shiftId);
          bool changed = cmd.ExecuteNonQuery() == 1;
          if (changed)
            AuditLogger.Log("Deactivate", "Shift", shiftId, transaction: transaction);
          return changed;
        }
      });
    }

    private static void ValidateShift(Shift shift)
    {
      if (shift == null)
        throw new ArgumentNullException(nameof(shift));
      if (string.IsNullOrWhiteSpace(shift.ShiftName))
        throw new ArgumentException(UiText.Get("ShiftNameIsRequired"));
      if (shift.StartTime < TimeSpan.Zero || shift.StartTime >= TimeSpan.FromDays(1) || shift.EndTime < TimeSpan.Zero || shift.EndTime >= TimeSpan.FromDays(1))
        throw new ArgumentException(UiText.Get("WorkTimesOrShiftBreakDurationAreInvalid"));
      double duration = (shift.EndTime - shift.StartTime).TotalMinutes;
      if (duration <= 0)
        duration += 1440;
      if (shift.BreakMinutes < 0 || shift.BreakMinutes >= duration)
        throw new ArgumentException(UiText.Get("BreakDurationMustBeShorterThanTheShift"));
      if (!(shift.Monday || shift.Tuesday || shift.Wednesday || shift.Thursday || shift.Friday || shift.Saturday || shift.Sunday))
        throw new ArgumentException(UiText.Get("SelectAtLeastOneWorkingDay"));
    }

    private void SetShiftParameters(SqlCommand cmd, Shift v)
    {
      cmd.Parameters.AddWithValue("@Name", v.ShiftName);
      cmd.Parameters.Add("@Start", SqlDbType.Time).Value = v.StartTime;
      cmd.Parameters.Add("@End", SqlDbType.Time).Value = v.EndTime;
      cmd.Parameters.AddWithValue("@Break", v.BreakMinutes);
      cmd.Parameters.AddWithValue("@Monday", v.Monday);
      cmd.Parameters.AddWithValue("@Tuesday", v.Tuesday);
      cmd.Parameters.AddWithValue("@Wednesday", v.Wednesday);
      cmd.Parameters.AddWithValue("@Thursday", v.Thursday);
      cmd.Parameters.AddWithValue("@Friday", v.Friday);
      cmd.Parameters.AddWithValue("@Saturday", v.Saturday);
      cmd.Parameters.AddWithValue("@Sunday", v.Sunday);
    }

    private Shift MapShift(SqlDataReader r) => new Shift
    {
      ShiftId = Convert.ToInt32(r["ShiftId"]),
      ShiftName = r["ShiftName"].ToString(),
      StartTime = (TimeSpan)r["StartTime"],
      EndTime = (TimeSpan)r["EndTime"],
      BreakMinutes = Convert.ToInt32(r["BreakMinutes"]),
      Monday = Convert.ToBoolean(r["Monday"]),
      Tuesday = Convert.ToBoolean(r["Tuesday"]),
      Wednesday = Convert.ToBoolean(r["Wednesday"]),
      Thursday = Convert.ToBoolean(r["Thursday"]),
      Friday = Convert.ToBoolean(r["Friday"]),
      Saturday = Convert.ToBoolean(r["Saturday"]),
      Sunday = Convert.ToBoolean(r["Sunday"]),
      IsActive = Convert.ToBoolean(r["IsActive"])
    };
  }
}
