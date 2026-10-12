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
  public class EmployeeRepository
  {
    public List<Employee> GetAll(bool activeOnly = true)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      var list = new List<Employee>();
      string sql = activeOnly ? "SELECT * FROM EmployeeDetails WHERE IsActive = 1 ORDER BY FirstName, LastName, EmployeeId" : "SELECT * FROM EmployeeDetails ORDER BY FirstName, LastName, EmployeeId";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          list.Add(MapEmployee(r));
      return list;
    }

    public List<Employee> GetLookup(bool activeOnly = true)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      var list = new List<Employee>();
      // Seçim kutusu için fotoğraf, adres ve vardiya detaylarını taşımaya gerek yoktur.
      string sql = "SELECT EmployeeId,FirstName,LastName FROM dbo.Employee" + (activeOnly ? " WHERE IsActive=1" : "") + " ORDER BY FirstName,LastName,EmployeeId";
      using (var command = new SqlCommand(sql, SqlDatabaseConnection.GetConnection()))
      using (var reader = command.ExecuteReader())
        while (reader.Read())
          list.Add(new Employee { EmployeeId = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2) });
      return list;
    }

    public Employee GetById(int employeeId)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      string sql = "SELECT * FROM EmployeeDetails WHERE EmployeeId = @Id";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      cmd.Parameters.AddWithValue("@Id", employeeId);
      using (var r = cmd.ExecuteReader())
        return r.Read() ? MapEmployee(r) : null;
    }

    public List<Employee> Search(string searchText)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      var list = new List<Employee>();
      string sql = @"
                SELECT * FROM EmployeeDetails
                WHERE  IsActive = 1
                  AND (FirstName                  LIKE @Arama
                    OR LastName               LIKE @Arama
                    OR NationalId          LIKE @Arama
                    OR (FirstName + ' ' + LastName)  LIKE @Arama)
                ORDER BY FirstName, LastName, EmployeeId";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      cmd.Parameters.Add("@Arama", System.Data.SqlDbType.NVarChar, 4000).Value = "%" + searchText + "%";
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          list.Add(MapEmployee(r));
      return list;
    }

    public int Save(Employee p)
    {
      return OperationAuthorization.Execute(ApplicationPermission.EditPersonnel, transaction =>
      {
        EmployeeValidation.Validate(p);
        ValidateAssignments(p, transaction);
        ValidateLeaveAllowance(p, transaction, true);
        string sql = @"
                  INSERT INTO Employee
                      (NationalId, FirstName, LastName, Gender, BirthDate,
                       Phone, Email, Address, PhotoPath,
                       DepartmentId, PositionId, ShiftId,
                       EmploymentStartDate, AnnualLeaveAllowance)
                  VALUES
                      (@NationalId, @FirstName, @LastName, @Gender, @BirthDate,
                       @Phone, @Email, @Address, @PhotoPath,
                       @DepartmentId, @PositionId, @ShiftId,
                       @EmploymentStartDate, @AnnualLeaveAllowance);
                  SELECT SCOPE_IDENTITY();";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        SetEmployeeParameters(cmd, p);
        int newId = Convert.ToInt32(cmd.ExecuteScalar());
        AuditLogger.Log("Insert", "Employee", newId, p.FullName, transaction: transaction);
        return newId;
      });
    }

    public bool Update(Employee p)
    {
      return OperationAuthorization.Execute(ApplicationPermission.EditPersonnel, transaction =>
      {
        EmployeeValidation.Validate(p);
        ValidateAssignments(p, transaction);
        ValidateLeaveAllowance(p, transaction, false);
        string sql = @"
                  UPDATE Employee SET
                      NationalId       = @NationalId,
                      FirstName               = @FirstName,
                      LastName            = @LastName,
                      Gender         = @Gender,
                      BirthDate      = @BirthDate,
                      Phone          = @Phone,
                      Email            = @Email,
                      Address            = @Address,
                      PhotoPath         = @PhotoPath,
                      DepartmentId      = @DepartmentId,
                      PositionId       = @PositionId,
                      ShiftId        = @ShiftId,
                      EmploymentStartDate   = @EmploymentStartDate,
                      AnnualLeaveAllowance  = @AnnualLeaveAllowance,
                      UpdatedAt = GETDATE()
                  WHERE EmployeeId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        SetEmployeeParameters(cmd, p);
        cmd.Parameters.AddWithValue("@Id", p.EmployeeId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok)
          AuditLogger.Log("Update", "Employee", p.EmployeeId, p.FullName, transaction: transaction);
        return ok;
      });
    }

    public bool Deactivate(int employeeId, DateTime employmentEndDate)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = @"
                  UPDATE Employee SET
                      IsActive          = 0,
                      EmploymentEndDate = @CheckOut,
                      UpdatedAt = GETDATE()
                  WHERE EmployeeId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@CheckOut", employmentEndDate);
        cmd.Parameters.AddWithValue("@Id", employeeId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok)
          AuditLogger.Log("Delete", "Employee", employeeId, UiText.Get("Deactivated"), transaction: transaction);
        return ok;
      });
    }

    public bool NationalIdExists(string nationalId, int excludedEmployeeId = 0)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      string sql = @"
                SELECT COUNT(1) FROM Employee
                WHERE NationalId = @NationalId AND EmployeeId <> @ExcludedId";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      cmd.Parameters.AddWithValue("@NationalId", nationalId);
      cmd.Parameters.AddWithValue("@ExcludedId", excludedEmployeeId);
      return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    private void ValidateAssignments(Employee employee, SqlTransaction transaction)
    {
      // Tanım kilitleri kayda kadar korunur; aynı anda vardiya pasife alınamaz.
      using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.Shift WITH (HOLDLOCK)
          WHERE ShiftId=@ShiftId AND IsActive=1", SqlDatabaseConnection.GetConnection(), transaction))
      {
        cmd.Parameters.AddWithValue("@ShiftId", employee.ShiftId);
        if (Convert.ToInt32(cmd.ExecuteScalar()) != 1)
          throw new InvalidOperationException(UiText.Get("NoActiveShiftWasFound"));
      }
      using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.Position p WITH (HOLDLOCK)
          JOIN dbo.Department d WITH (HOLDLOCK) ON d.DepartmentId=p.DepartmentId
          WHERE p.PositionId=@PositionId AND p.DepartmentId=@DepartmentId AND p.IsActive=1 AND d.IsActive=1", SqlDatabaseConnection.GetConnection(), transaction))
      {
        cmd.Parameters.AddWithValue("@PositionId", employee.PositionId);
        cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
        if (Convert.ToInt32(cmd.ExecuteScalar()) != 1)
          throw new InvalidOperationException(UiText.Get("SelectActiveDepartmentAndPosition"));
      }
    }

    private void ValidateLeaveAllowance(Employee employee, SqlTransaction transaction, bool isNew)
    {
      if (OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ManagePersonnel))
        return;
      int existingAllowance = 14;
      if (!isNew)
      {
        using (var cmd = new SqlCommand("SELECT AnnualLeaveAllowance FROM dbo.Employee WITH (UPDLOCK,HOLDLOCK) WHERE EmployeeId=@Id", SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.AddWithValue("@Id", employee.EmployeeId);
          object value = cmd.ExecuteScalar();
          if (value == null)
            throw new InvalidOperationException(UiText.Get("EmployeeNotFound"));
          existingAllowance = Convert.ToInt32(value);
        }
      }
      if (employee.AnnualLeaveAllowance != existingAllowance)
        throw new UnauthorizedAccessException(UiText.Get("InsufficientPermissions"));
    }

    private void SetEmployeeParameters(SqlCommand cmd, Employee p)
    {
      cmd.Parameters.AddWithValue("@NationalId", p.NationalId);
      cmd.Parameters.AddWithValue("@FirstName", p.FirstName);
      cmd.Parameters.AddWithValue("@LastName", p.LastName);
      cmd.Parameters.AddWithValue("@Gender", (object)p.Gender ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@BirthDate", (object)p.BirthDate ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Phone", (object)p.Phone ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Email", (object)p.Email ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Address", (object)p.Address ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@PhotoPath", (object)p.PhotoPath ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@DepartmentId", p.DepartmentId);
      cmd.Parameters.AddWithValue("@PositionId", p.PositionId);
      cmd.Parameters.AddWithValue("@ShiftId", p.ShiftId);
      cmd.Parameters.AddWithValue("@EmploymentStartDate", p.EmploymentStartDate);
      cmd.Parameters.AddWithValue("@AnnualLeaveAllowance", p.AnnualLeaveAllowance);
    }

    private Employee MapEmployee(SqlDataReader r) => new Employee
    {
      EmployeeId = Convert.ToInt32(r["EmployeeId"]),
      NationalId = r["NationalId"].ToString(),
      FirstName = r["FirstName"].ToString(),
      LastName = r["LastName"].ToString(),
      Gender = r["Gender"] == DBNull.Value ? null : r["Gender"].ToString(),
      BirthDate = r["BirthDate"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["BirthDate"]),
      Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
      Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
      Address = r["Address"] == DBNull.Value ? null : r["Address"].ToString(),
      PhotoPath = r["PhotoPath"] == DBNull.Value ? null : r["PhotoPath"].ToString(),
      DepartmentId = Convert.ToInt32(r["DepartmentId"]),
      DepartmentName = r["DepartmentName"].ToString(),
      PositionId = Convert.ToInt32(r["PositionId"]),
      PositionName = r["PositionName"].ToString(),
      ShiftId = Convert.ToInt32(r["ShiftId"]),
      ShiftName = r["ShiftName"].ToString(),
      StartTime = (TimeSpan)r["StartTime"],
      EndTime = (TimeSpan)r["EndTime"],
      BreakMinutes = Convert.ToInt32(r["BreakMinutes"]),
      EmploymentStartDate = Convert.ToDateTime(r["EmploymentStartDate"]),
      EmploymentEndDate = r["EmploymentEndDate"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["EmploymentEndDate"]),
      AnnualLeaveAllowance = Convert.ToInt32(r["AnnualLeaveAllowance"]),
      IsActive = Convert.ToBoolean(r["IsActive"]),
      CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
      UpdatedAt = r["UpdatedAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["UpdatedAt"])
    };
    public async Task<List<Employee>> GetAllAsync(bool activeOnly = true, CancellationToken cancellationToken = default(CancellationToken))
    {
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.ViewPersonnel, connection, cancellationToken).ConfigureAwait(false);
          var list = new List<Employee>();
          string sql = activeOnly ? "SELECT * FROM EmployeeDetails WHERE IsActive = 1 ORDER BY FirstName, LastName, EmployeeId" : "SELECT * FROM EmployeeDetails ORDER BY FirstName, LastName, EmployeeId";
          var cmd = new SqlCommand(sql, connection);
          using (var r = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            while (await r.ReadAsync(cancellationToken).ConfigureAwait(false))
              list.Add(MapEmployee(r));
          return list;
        }
      }
      catch (SqlException ex) when (cancellationToken.IsCancellationRequested)
      {
        // System.Data.SqlClient, devam eden SQL iptalini SqlException olarak da döndürebilir.
        throw new OperationCanceledException("The database read was cancelled.", ex, cancellationToken);
      }
    }

    public async Task<List<Employee>> GetLookupAsync(bool activeOnly = true, CancellationToken cancellationToken = default(CancellationToken))
    {
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.ViewPersonnel, connection, cancellationToken).ConfigureAwait(false);
          var list = new List<Employee>();
          // Seçim kutusu için fotoğraf, adres ve vardiya detaylarını taşımaya gerek yoktur.
          string sql = "SELECT EmployeeId,FirstName,LastName FROM dbo.Employee" + (activeOnly ? " WHERE IsActive=1" : "") + " ORDER BY FirstName,LastName,EmployeeId";
          using (var command = new SqlCommand(sql, connection))
          using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
              list.Add(new Employee { EmployeeId = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2) });
          return list;
        }
      }
      catch (SqlException ex) when (cancellationToken.IsCancellationRequested)
      {
        // System.Data.SqlClient, devam eden SQL iptalini SqlException olarak da döndürebilir.
        throw new OperationCanceledException("The database read was cancelled.", ex, cancellationToken);
      }
    }

    public async Task<List<Employee>> SearchAsync(string searchText, CancellationToken cancellationToken = default(CancellationToken))
    {
      try
      {
        using (var connection = await SqlDatabaseConnection.OpenReadConnectionAsync(cancellationToken).ConfigureAwait(false))
        {
          await OperationAuthorization.RequireAsync(ApplicationPermission.ViewPersonnel, connection, cancellationToken).ConfigureAwait(false);
          var list = new List<Employee>();
          string sql = @"
                    SELECT * FROM EmployeeDetails
                    WHERE  IsActive = 1
                      AND (FirstName                  LIKE @Arama
                        OR LastName               LIKE @Arama
                        OR NationalId          LIKE @Arama
                        OR (FirstName + ' ' + LastName)  LIKE @Arama)
                    ORDER BY FirstName, LastName, EmployeeId";
          var cmd = new SqlCommand(sql, connection);
          cmd.Parameters.Add("@Arama", System.Data.SqlDbType.NVarChar, 4000).Value = "%" + searchText + "%";
          using (var r = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            while (await r.ReadAsync(cancellationToken).ConfigureAwait(false))
              list.Add(MapEmployee(r));
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
