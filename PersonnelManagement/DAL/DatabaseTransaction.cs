using System;
using System.Data.SqlClient;

namespace PersonnelManagement.DAL
{
  internal static class DatabaseTransaction
  {
    internal static T Execute<T>(Func<SqlTransaction, T> operation)
    {
      // Veri ve log aynı transaction'da yazılır; hata halinde ikisi de geri alınır.
      using (SqlTransaction transaction = SqlDBConnection.GetConnection().BeginTransaction())
      {
        T result = operation(transaction);
        transaction.Commit();
        return result;
      }
    }
  }
}
