using System.Threading;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Data.SqlClient;
using PersonnelManagement.Connection;
using PersonnelManagement.Models;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Security
{
  public static class OperationAuthorization
  {
    public static bool HasPermission(UserAccount user, ApplicationPermission permission)
    {
      if (user == null || !user.IsActive || !Enum.IsDefined(typeof(ApplicationPermission), permission))
        return false;
      string role = user.Role?.Trim();
      if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        return true;
      if (string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase))
        return permission != ApplicationPermission.ManageUsers;
      if (!string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
        return false;
      switch (permission)
      {
        case ApplicationPermission.ViewDashboard:
        case ApplicationPermission.ViewPersonnel:
        case ApplicationPermission.EditPersonnel:
        case ApplicationPermission.ViewShifts:
        case ApplicationPermission.RecordAttendance:
        case ApplicationPermission.ViewLeaves:
        case ApplicationPermission.CreateLeave:
          return true;
        default:
          return false;
      }
    }

    public static UserAccount RefreshCurrentUser(SqlTransaction transaction = null)
    {
      var user = UserSession.CurrentUser;
      if (user == null || !UserSession.UserId.HasValue)
        throw new UnauthorizedAccessException(UiText.Get("SignInRequired"));
      // Açık ekrandaki eski rol yerine güncel hesap kaydı kullanılır.
      using (var cmd = new SqlCommand("SELECT Role,IsActive FROM dbo.UserAccount WITH (HOLDLOCK) WHERE UserId=@Id", SqlDatabaseConnection.GetConnection(), transaction))
      {
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = UserSession.UserId.Value;
        using (var reader = cmd.ExecuteReader())
        {
          if (!reader.Read())
          {
            user.IsActive = false;
            throw new UnauthorizedAccessException(UiText.Get("InsufficientPermissions"));
          }
          user.UserId = UserSession.UserId.Value;
          user.Role = reader["Role"].ToString().Trim();
          user.IsActive = Convert.ToBoolean(reader["IsActive"]);
        }
      }
      return user;
    }

    public static UserAccount Require(ApplicationPermission permission, SqlTransaction transaction = null, int? actorUserId = null)
    {
      var user = RefreshCurrentUser(transaction);
      // İşlemi yapan kimlik, çağıranın verdiği başka bir hesap kimliği olamaz.
      if ((actorUserId.HasValue && actorUserId.Value != user.UserId) || !HasPermission(user, permission))
        throw new UnauthorizedAccessException(UiText.Get("InsufficientPermissions"));
      return user;
    }

    public static async Task RequireAsync(ApplicationPermission permission, SqlConnection connection, CancellationToken cancellationToken)
    {
      var user = UserSession.CurrentUser;
      int? userId = UserSession.UserId;
      if (user == null || !userId.HasValue)
        throw new UnauthorizedAccessException(UiText.Get("SignInRequired"));
      string role;
      bool active;
      using (var command = new SqlCommand("SELECT Role,IsActive FROM dbo.UserAccount WHERE UserId=@Id", connection))
      {
        command.Parameters.Add("@Id", SqlDbType.Int).Value = userId.Value;
        using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
        {
          if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            throw new UnauthorizedAccessException(UiText.Get("InsufficientPermissions"));
          role = reader.GetString(0).Trim();
          active = reader.GetBoolean(1);
        }
      }
      // Oturum sorgu sırasında değişirse önceki kullanıcının sonucu yeni ekrana taşınmaz.
      if (!ReferenceEquals(user, UserSession.CurrentUser) || userId != UserSession.UserId ||
          !HasPermission(new UserAccount { Role = role, IsActive = active }, permission))
        throw new UnauthorizedAccessException(UiText.Get("InsufficientPermissions"));
    }

    public static void RequireAdmin(int userId, SqlTransaction transaction = null)
    {
      Require(ApplicationPermission.ManageUsers, transaction, userId);
    }

    internal static T Execute<T>(ApplicationPermission permission, Func<SqlTransaction, T> operation)
    {
      // Yetki kontrolü ve yazma aynı işlemde kalır; hesap değişikliği araya giremez.
      using (var transaction = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        Require(permission, transaction);
        T result = operation(transaction);
        transaction.Commit();
        return result;
      }
    }
  }
}
