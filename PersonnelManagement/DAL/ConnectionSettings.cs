using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PersonnelManagement.DAL
{
  public static class ConnectionSettings
  {
    public const string EnvironmentVariable = "PERSONNELMANAGEMENT_V080_CONNECTION_STRING";

    public static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PersonnelManagement", "V0.8.0", "connection.dat");

    public static string Read()
    {
      // Sürece özel ayar testleri ve farklı kurulumları ayırır. Kalıcı yerel ayar
      // Windows kullanıcısına bağlıdır; proje klasörüne bağlantı şifresi yazılmaz.
      string configured = Environment.GetEnvironmentVariable(EnvironmentVariable);
      if (string.IsNullOrWhiteSpace(configured) && File.Exists(SettingsPath))
        configured = ReadProtectedFile(SettingsPath);
      if (string.IsNullOrWhiteSpace(configured))
        configured = Properties.Settings.Default.DBConStr;
      if (string.IsNullOrWhiteSpace(configured))
        throw new InvalidOperationException("SQL bağlantısı ayarlanmamış. tools\\ConfigureConnection.ps1 dosyasıyla bağlantıyı tanımlayın.");

      SqlConnectionStringBuilder builder;
      try { builder = new SqlConnectionStringBuilder(configured); }
      catch (ArgumentException ex)
      {
        throw new InvalidOperationException("SQL bağlantı ayarı geçersiz. Bağlantıyı yeniden tanımlayın.", ex);
      }
      if (builder.IntegratedSecurity || string.IsNullOrWhiteSpace(builder.UserID)
          || string.IsNullOrEmpty(builder.Password))
        throw new InvalidOperationException("SQL Server Authentication için SQL kullanıcı adı ve şifresi gerekiyor.");
      if (string.IsNullOrWhiteSpace(builder.DataSource) || string.IsNullOrWhiteSpace(builder.InitialCatalog))
        throw new InvalidOperationException("SQL sunucusu ve veritabanı adı gerekiyor.");
      return builder.ConnectionString;
    }

    private static string ReadProtectedFile(string path)
    {
      byte[] plain = null;
      try
      {
        plain = ProtectedData.Unprotect(File.ReadAllBytes(path), null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(plain);
      }
      catch (Exception ex) when (ex is CryptographicException || ex is IOException || ex is UnauthorizedAccessException)
      {
        throw new InvalidOperationException("Kayıtlı SQL bağlantısı okunamadı. Bu Windows kullanıcısıyla bağlantıyı yeniden tanımlayın.", ex);
      }
      finally
      {
        if (plain != null) Array.Clear(plain, 0, plain.Length);
      }
    }
  }
}
