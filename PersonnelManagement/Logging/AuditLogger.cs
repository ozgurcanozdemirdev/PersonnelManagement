using System;
using System.Data.SqlClient;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.Logging
{
  public static class AuditLogger
  {
    private static int? _currentUserId = null;
    public static void SetCurrentUser(int? userId) => _currentUserId = userId;
    public static void Log(string action, string tableName = null, int? affectedId = null, string description = null, SqlTransaction transaction = null, int? actorUserId = null)
    {
      try
      {
        string sql = @" INSERT INTO AuditLog (UserId, Action, TableName, AffectedId, Description) VALUES (@UserId, @Action, @TableName, @AffectedId,@Description)";
        using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.AddWithValue("@UserId", (object)(actorUserId ?? _currentUserId) ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Action", action);
          cmd.Parameters.AddWithValue("@TableName", (object)tableName ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@AffectedId", (object)affectedId ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
          cmd.ExecuteNonQuery();
        }
      }
      catch (SqlException ex)
      {
        // Yazma işleminin transaction'ı bu hata ile geri alınır; eksik denetim kaydı başarı sayılmaz.
        throw new InvalidOperationException(PersonnelManagement.UI.Localization.UiText.Get("AuditWriteFailed"), ex);
      }
    }
  }
}