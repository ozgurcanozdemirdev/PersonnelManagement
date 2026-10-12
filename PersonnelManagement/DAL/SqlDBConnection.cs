using System;
using System.Data;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public static class SqlDBConnection
  {
    // Farklı iş parçacıkları aynı reader veya transaction'ı paylaşmaz.
    [ThreadStatic]
    private static SqlConnection _connection;
    public static SqlConnection GetConnection()
    {
      if (_connection != null && _connection.State == ConnectionState.Broken)
        CloseConnection();
      if (_connection == null)
      {
        _connection = new SqlConnection(ConnectionSettings.Read());
      }

      try
      {
        if (_connection.State == ConnectionState.Closed)
          _connection.Open();
        return _connection;
      }
      catch
      {
        CloseConnection();
        throw;
      }
    }

    public static void CloseConnection()
    {
      SqlConnection connection = _connection;
      _connection = null;
      connection?.Dispose();
    }
  }
}
