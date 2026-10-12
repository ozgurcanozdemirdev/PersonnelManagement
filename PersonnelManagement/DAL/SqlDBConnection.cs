using System;
using System.Data;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  public static class SqlDBConnection
  {
    private static SqlConnection _connection = null;

    public static SqlConnection GetConnection()
    {
      if (_connection == null)
        _connection = new SqlConnection(ConnectionSettings.Read());

      if (_connection.State == ConnectionState.Closed)
        _connection.Open();

      return _connection;
    }

    public static void CloseConnection()
    {
      if (_connection != null && _connection.State == ConnectionState.Open)
        _connection.Close();
    }
  }
}

