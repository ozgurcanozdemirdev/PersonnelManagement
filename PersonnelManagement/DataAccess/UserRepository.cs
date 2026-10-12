using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.DataAccess
{
  public class UserRepository
  {
    private const string Columns = "UserId, Username, FirstName, LastName, Email, Role, IsActive, LastLoginAt";
    public UserAccount Authenticate(string username, string password)
    {
      UserSession.CurrentUser = null;
      AuditLogger.SetCurrentUser(null);
      UserAccount user;
      string storedPassword;
      string sql = "SELECT " + Columns + ", PasswordHash FROM dbo.UserAccount WHERE Username = @Username AND IsActive = 1";
      using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
        using (var reader = cmd.ExecuteReader())
        {
          if (!reader.Read())
            return null;
          user = MapUser(reader);
          storedPassword = reader["PasswordHash"].ToString();
        }
      }

      if (!OperationAuthorization.HasPermission(user, ApplicationPermission.ViewDashboard) || !PasswordHasher.Verify(password, storedPassword))
        return null;
      string upgradedHash = storedPassword.StartsWith("PBKDF2$", StringComparison.Ordinal) ? storedPassword : PasswordHasher.Hash(password);
      using (var transaction = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        // Şifre doğrulanırken hesap pasife alınmış veya değiştirilmiş olabilir; kilitli kaydı yeniden kontrol et.
        using (var check = new SqlCommand("SELECT Role,IsActive FROM dbo.UserAccount WITH (UPDLOCK,HOLDLOCK) WHERE UserId=@Id AND PasswordHash COLLATE Latin1_General_100_BIN2=@Previous", SqlDatabaseConnection.GetConnection(), transaction))
        {
          check.Parameters.Add("@Id", SqlDbType.Int).Value = user.UserId;
          check.Parameters.Add("@Previous", SqlDbType.NVarChar, 100).Value = storedPassword;
          using (var reader = check.ExecuteReader())
          {
            if (!reader.Read())
              return null;
            user.Role = reader.GetString(0).Trim();
            user.IsActive = reader.GetBoolean(1);
          }
        }
        if (!OperationAuthorization.HasPermission(user, ApplicationPermission.ViewDashboard))
          return null;
        using (var command = new SqlCommand("UPDATE dbo.UserAccount SET PasswordHash=@Hash,LastLoginAt=GETDATE() WHERE UserId=@Id", SqlDatabaseConnection.GetConnection(), transaction))
        {
          command.Parameters.Add("@Hash", SqlDbType.NVarChar, 100).Value = upgradedHash;
          command.Parameters.Add("@Id", SqlDbType.Int).Value = user.UserId;
          command.ExecuteNonQuery();
        }
        AuditLogger.Log("Login", "UserAccount", user.UserId, UiText.Get("SuccessfulSignIn"), transaction, user.UserId);
        transaction.Commit();
      }
      // Oturum ancak hesap güncellemesi ve denetim kaydı birlikte tamamlandığında açılır.
      UserSession.CurrentUser = user;
      AuditLogger.SetCurrentUser(user.UserId);
      return user;
    }

    public List<UserAccount> GetAll()
    {
      OperationAuthorization.Require(ApplicationPermission.ManageUsers);
      var list = new List<UserAccount>();
      using (var cmd = new SqlCommand("SELECT " + Columns + " FROM dbo.UserAccount ORDER BY FirstName, LastName", SqlDatabaseConnection.GetConnection()))
      using (var reader = cmd.ExecuteReader())
        while (reader.Read())
          list.Add(MapUser(reader));
      return list;
    }

    public UserAccount GetById(int userId)
    {
      var actor = OperationAuthorization.Require(ApplicationPermission.ViewDashboard);
      if (actor.UserId != userId)
        OperationAuthorization.RequireAdmin(actor.UserId);
      using (var cmd = new SqlCommand("SELECT " + Columns + " FROM dbo.UserAccount WHERE UserId = @Id", SqlDatabaseConnection.GetConnection()))
      {
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        using (var reader = cmd.ExecuteReader())
          return reader.Read() ? MapUser(reader) : null;
      }
    }

    public List<string> GetRoles()
    {
      return new List<string> { "Admin", "Manager", "Employee" };
    }

    public bool Save(UserAccount user, string password, int actorUserId)
    {
      if (user == null)
        throw new ArgumentException(UiText.Get("UserDetailsAreIncomplete"));
      string username = ValidateText(user.Username, UiText.Get("Username3"), 50);
      string firstName = ValidateText(user.FirstName, UiText.Get("FirstName"), 50);
      string lastName = ValidateText(user.LastName, UiText.Get("LastName"), 50);
      string role = ValidateRole(user.Role);
      ValidatePassword(password);
      string email = string.IsNullOrWhiteSpace(user.Email) ? null : user.Email.Trim();
      if (email != null && (email.Length > 100 || new MailAddress(email).Address != email))
        throw new ArgumentException(UiText.Get("EnterAValidEmailAddress"));
      string passwordHash = PasswordHasher.Hash(password);
      using (var transaction = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        // Yetkiyi oturum önbelleğinden değil, güncel SQL kaydından doğrula.
        ValidateAdminAccess(actorUserId, transaction);
        string sql = @"INSERT INTO dbo.UserAccount (Username, PasswordHash, FirstName, LastName, Email, Role, IsActive)
                       VALUES (@Username, @PasswordHash, @FirstName, @LastName, @Email, @Role, 1)";
        using (var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
          cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100).Value = passwordHash;
          cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = firstName;
          cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = lastName;
          cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object)email ?? DBNull.Value;
          cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = role;
          cmd.ExecuteNonQuery();
        }

        AuditLogger.Log("Insert", "User", null, username, transaction: transaction);
        transaction.Commit();
      }
      return true;
    }

    public bool UpdateRole(int userId, string role, int actorUserId)
    {
      role = ValidateRole(role);
      using (var transaction = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        int adminCount = ValidateAdminAccess(actorUserId, transaction);
        bool wasLastAdmin;
        using (var cmd = new SqlCommand("SELECT Role, IsActive FROM dbo.UserAccount WITH (UPDLOCK, HOLDLOCK) WHERE UserId = @Id", SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
          using (var reader = cmd.ExecuteReader())
          {
            if (!reader.Read())
              return false;
            wasLastAdmin = Convert.ToBoolean(reader["IsActive"]) && string.Equals(reader["Role"].ToString().Trim(), "Admin", StringComparison.OrdinalIgnoreCase);
          }
        }

        // Kontrol ve güncelleme aynı işlemde yapılır; son aktif admin korunur.
        if (wasLastAdmin && adminCount == 1 && !string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException(UiText.Get("TheLastActiveAdministratorCannotBeDemotedAssignAnotherActiveAdministratorFirst"));
        using (var cmd = new SqlCommand("UPDATE dbo.UserAccount SET Role = @Role WHERE UserId = @Id", SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
          cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = role;
          cmd.ExecuteNonQuery();
        }

        AuditLogger.Log("Update", "User", userId, UiText.Get("Role2") + role, transaction: transaction);
        transaction.Commit();
      }
      return true;
    }

    private int ValidateAdminAccess(int userId, SqlTransaction transaction)
    {
      OperationAuthorization.RequireAdmin(userId, transaction);
      int adminCount;
      using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.UserAccount WITH (UPDLOCK, HOLDLOCK) WHERE IsActive = 1 AND LTRIM(RTRIM(Role)) COLLATE Latin1_General_100_CI_AS = N\u0027Admin\u0027", SqlDatabaseConnection.GetConnection(), transaction))
        adminCount = Convert.ToInt32(cmd.ExecuteScalar());
      using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.UserAccount WHERE UserId = @Id AND IsActive = 1 AND LTRIM(RTRIM(Role)) COLLATE Latin1_General_100_CI_AS = N\u0027Admin\u0027", SqlDatabaseConnection.GetConnection(), transaction))
      {
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        if (Convert.ToInt32(cmd.ExecuteScalar()) != 1)
          throw new UnauthorizedAccessException(UiText.Get("AdministratorAccessIsRequiredForUserManagement"));
      }

      return adminCount;
    }

    private string ValidateRole(string role)
    {
      if (string.IsNullOrWhiteSpace(role) || role.Trim().Length > 20)
        throw new ArgumentException(UiText.Get("SelectARole"));
      string selected = GetRoles().Find(r => string.Equals(r, role.Trim(), StringComparison.OrdinalIgnoreCase));
      if (selected == null)
        throw new ArgumentException(UiText.Get("SelectAValidRole"));
      return selected;
    }

    private string ValidateText(string text, string field, int length)
    {
      if (string.IsNullOrWhiteSpace(text) || text.Trim().Length > length)
        throw new ArgumentException(UiText.Format("RequiredTextMaximum", field, length));
      return text.Trim();
    }

    private void ValidatePassword(string password)
    {
      if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 50)
        throw new ArgumentException(UiText.Get("PasswordMustContain850Characters"));
    }

    public bool ChangePassword(int userId, string newPassword, int actorUserId)
    {
      ValidatePassword(newPassword);
      using (var transaction = SqlDatabaseConnection.GetConnection().BeginTransaction())
      {
        // Başka hesabın şifresini değiştirme işlemi de güncel admin yetkisi ister.
        OperationAuthorization.RequireAdmin(actorUserId, transaction);
        using (var cmd = new SqlCommand("UPDATE dbo.UserAccount SET PasswordHash = @PasswordHash WHERE UserId = @Id", SqlDatabaseConnection.GetConnection(), transaction))
        {
          cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100).Value = PasswordHasher.Hash(newPassword);
          cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
          if (cmd.ExecuteNonQuery() != 1)
            return false;
        }

        AuditLogger.Log("Update", "User", userId, UiText.Get("PasswordChanged"), transaction: transaction);
        transaction.Commit();
      }
      return true;
    }

    private UserAccount MapUser(SqlDataReader r) => new UserAccount
    {
      UserId = Convert.ToInt32(r["UserId"]),
      Username = r["Username"].ToString(),
      FirstName = r["FirstName"].ToString(),
      LastName = r["LastName"].ToString(),
      Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
      Role = r["Role"].ToString(),
      IsActive = Convert.ToBoolean(r["IsActive"]),
      LastLoginAt = r["LastLoginAt"] == DBNull.Value ? (DateTime? )null : Convert.ToDateTime(r["LastLoginAt"])
    };
  }
}