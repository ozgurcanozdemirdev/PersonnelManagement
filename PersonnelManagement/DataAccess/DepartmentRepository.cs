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
  public class DepartmentRepository
  {
    public List<Department> GetAll(bool activeOnly = true)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      var list = new List<Department>();
      string sql = activeOnly ? "SELECT * FROM Department WHERE IsActive = 1 ORDER BY DepartmentName" : "SELECT * FROM Department ORDER BY DepartmentName";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          list.Add(new Department { DepartmentId = Convert.ToInt32(r["DepartmentId"]), DepartmentName = r["DepartmentName"].ToString(), Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(), IsActive = Convert.ToBoolean(r["IsActive"]), CreatedAt = Convert.ToDateTime(r["CreatedAt"]) });
      return list;
    }

    public bool Save(Department d)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = "INSERT INTO Department (DepartmentName, Description) VALUES (@Name, @Description)";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@Name", d.DepartmentName);
        cmd.Parameters.AddWithValue("@Description", (object)d.Description ?? DBNull.Value);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok)
          AuditLogger.Log("Insert", "Department", null, d.DepartmentName, transaction: transaction);
        return ok;
      });
    }

    public bool Update(Department d)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = "UPDATE Department SET DepartmentName = @Name, Description = @Description WHERE DepartmentId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@Name", d.DepartmentName);
        cmd.Parameters.AddWithValue("@Description", (object)d.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Id", d.DepartmentId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok)
          AuditLogger.Log("Update", "Department", d.DepartmentId, d.DepartmentName, transaction: transaction);
        return ok;
      });
    }

    public bool Deactivate(int departmentId)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = "UPDATE Department SET IsActive = 0 WHERE DepartmentId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@Id", departmentId);
        bool ok = cmd.ExecuteNonQuery() > 0;
        if (ok)
          AuditLogger.Log("Delete", "Department", departmentId, transaction: transaction);
        return ok;
      });
    }
  }
}
