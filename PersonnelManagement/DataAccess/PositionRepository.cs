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
  public class PositionRepository
  {
    public List<Position> GetAll(int? departmentId = null)
    {
      OperationAuthorization.Require(ApplicationPermission.ViewPersonnel);
      var list = new List<Position>();
      string sql = departmentId.HasValue ? "SELECT * FROM Position WHERE IsActive = 1 AND DepartmentId = @DepartmentId ORDER BY PositionName" : "SELECT * FROM Position WHERE IsActive = 1 ORDER BY PositionName";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      if (departmentId.HasValue)
        cmd.Parameters.AddWithValue("@DepartmentId", departmentId.Value);
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          list.Add(new Position { PositionId = Convert.ToInt32(r["PositionId"]), DepartmentId = Convert.ToInt32(r["DepartmentId"]), PositionName = r["PositionName"].ToString(), Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(), IsActive = Convert.ToBoolean(r["IsActive"]) });
      return list;
    }

    public bool Save(Position p)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = "INSERT INTO Position (DepartmentId, PositionName, Description) VALUES (@DepartmentId, @Name, @Description); SELECT CAST(SCOPE_IDENTITY() AS int);";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@DepartmentId", p.DepartmentId);
        cmd.Parameters.AddWithValue("@Name", p.PositionName);
        cmd.Parameters.AddWithValue("@Description", (object)p.Description ?? DBNull.Value);
        int id = Convert.ToInt32(cmd.ExecuteScalar());
        AuditLogger.Log("Insert", "Position", id, p.PositionName, transaction: transaction);
        return id > 0;
      });
    }

    public bool Update(Position p)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = "UPDATE Position SET PositionName = @Name, Description = @Description WHERE PositionId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@Name", p.PositionName);
        cmd.Parameters.AddWithValue("@Description", (object)p.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Id", p.PositionId);
        bool changed = cmd.ExecuteNonQuery() > 0;
        if (changed)
          AuditLogger.Log("Update", "Position", p.PositionId, p.PositionName, transaction: transaction);
        return changed;
      });
    }

    public bool Deactivate(int positionId)
    {
      return OperationAuthorization.Execute(ApplicationPermission.ManagePersonnel, transaction =>
      {
        string sql = "UPDATE Position SET IsActive = 0 WHERE PositionId = @Id";
        var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction);
        cmd.Parameters.AddWithValue("@Id", positionId);
        bool changed = cmd.ExecuteNonQuery() > 0;
        if (changed)
          AuditLogger.Log("Deactivate", "Position", positionId, transaction: transaction);
        return changed;
      });
    }
  }
}
