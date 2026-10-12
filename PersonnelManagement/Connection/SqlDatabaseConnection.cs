using System.Threading;
using System.Threading.Tasks;
using PersonnelManagement.UI.Localization;
using System;
using System.Data;
using System.Data.SqlClient;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.Connection
{
  public static class SqlDatabaseConnection
  {
    [ThreadStatic]
    private static SqlConnection _connection = null;
    public static SqlConnection GetConnection()
    {
      if (_connection == null)
      {
        _connection = new SqlConnection(ConnectionSettingsStore.Load());
        try
        {
          _connection.Open();
          using (var cmd = new SqlCommand("SELECT CASE WHEN IS_SRVROLEMEMBER('sysadmin')=1 OR IS_MEMBER('db_owner')=1 THEN 1 ELSE 0 END", _connection))
            if (Convert.ToInt32(cmd.ExecuteScalar()) != 0)
              throw new InvalidOperationException(UiText.Get("TheApplicationAccountCannotBeSysadminOrDbOwnerConfigureAnAccountWithLimitedPermiss"));
        }
        catch
        {
          _connection.Dispose();
          _connection = null;
          throw;
        }
      }

      if (_connection.State == ConnectionState.Broken)
        _connection.Close();
      if (_connection.State == ConnectionState.Closed)
        _connection.Open();
      return _connection;
    }

    public static async Task<SqlConnection> OpenReadConnectionAsync(CancellationToken cancellationToken)
    {
      // await farklı iş parçacığında devam edebilir; okuma işlemi ThreadStatic bağlantıyı paylaşmaz.
      var connection = new SqlConnection(ConnectionSettingsStore.Load());
      try
      {
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        using (var command = new SqlCommand("SELECT CASE WHEN IS_SRVROLEMEMBER('sysadmin')=1 OR IS_MEMBER('db_owner')=1 THEN 1 ELSE 0 END", connection))
          if (Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false)) != 0)
            throw new InvalidOperationException(PersonnelManagement.UI.Localization.UiText.Get("TheApplicationAccountCannotBeSysadminOrDbOwnerConfigureAnAccountWithLimitedPermiss"));
        return connection;
      }
      catch
      {
        connection.Dispose();
        throw;
      }
    }

    public static void CloseConnection()
    {
      if (_connection != null)
      {
        _connection.Dispose();
        _connection = null;
      }
    }
  }
}