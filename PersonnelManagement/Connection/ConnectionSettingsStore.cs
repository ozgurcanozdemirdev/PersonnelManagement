using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Connection
{
  public static class ConnectionSettingsStore
  {
    public const string EnvironmentVariable = "PERSONNELMANAGEMENT_V100_CONNECTION_STRING";
    public static string SettingsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PersonnelManagement", "V1.0.0", "English");
    public static string DefaultPath => Path.Combine(SettingsDirectory, "connection.dat");

    public static string Load()
    {
      string value = Environment.GetEnvironmentVariable(EnvironmentVariable);
      if (string.IsNullOrWhiteSpace(value))
        value = ReadSavedSettings(DefaultPath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connection.dat"));
      if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException(UiText.Get("TheDatabaseConnectionIsNotConfigured"));
      return Validate(value).ConnectionString;
    }

    public static SqlConnectionStringBuilder Validate(string connectionString)
    {
      var settings = new SqlConnectionStringBuilder(connectionString);
      if (settings.IntegratedSecurity || string.IsNullOrWhiteSpace(settings.DataSource) ||
          string.IsNullOrWhiteSpace(settings.InitialCatalog) || string.IsNullOrWhiteSpace(settings.UserID) ||
          string.IsNullOrEmpty(settings.Password))
        throw new ArgumentException(UiText.Get("SqlAuthenticationRequired"));
      if (string.Equals(settings.UserID, "sa", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(UiText.Get("AnSQLAdministratorAccountCannotBeUsedByTheApplicationConfigureAnAccountWithLimited"));
      settings.Encrypt = true;
      return settings;
    }

    private static string ReadSavedSettings(string profileFile, string applicationFile)
    {
      // V1.0'ın İngilizce şeması eski Türkçe bağlantı profilini kullanmaz.
      // Bozuk bir profil varsa başka veritabanına sessizce geçmek yerine hatayı bildiririz.
      if (File.Exists(profileFile)) return ReadProtectedFile(profileFile);
      if (File.Exists(applicationFile)) return ReadProtectedFile(applicationFile);
      return null;
    }

    private static string ReadProtectedFile(string file)
    {
      byte[] plain = ProtectedData.Unprotect(File.ReadAllBytes(file), null, DataProtectionScope.CurrentUser);
      try { return Encoding.UTF8.GetString(plain); }
      finally { Array.Clear(plain, 0, plain.Length); }
    }

    public static void Save(string connectionString)
    {
      WriteProtectedFile(Validate(connectionString).ConnectionString, DefaultPath);
    }

    private static void WriteProtectedFile(string connectionString, string destination)
    {
      byte[] plain = Encoding.UTF8.GetBytes(connectionString);
      byte[] encrypted;
      try { encrypted = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser); }
      finally { Array.Clear(plain, 0, plain.Length); }
      Directory.CreateDirectory(Path.GetDirectoryName(destination));
      string temporary = destination + "." + Guid.NewGuid().ToString("N") + ".tmp";
      try
      {
        File.WriteAllBytes(temporary, encrypted);
        if (File.Exists(destination)) File.Replace(temporary, destination, null);
        else File.Move(temporary, destination);
      }
      finally
      {
        if (File.Exists(temporary)) File.Delete(temporary);
        Array.Clear(encrypted, 0, encrypted.Length);
      }
    }
  }
}
