using System;
using System.Globalization;
using System.Security.Cryptography;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.Security
{
  public static class PasswordHasher
  {
    private const int IterationCount = 600000;
    public static string Hash(string password)
    {
      byte[] salt = new byte[16];
      using (var random = RandomNumberGenerator.Create())
        random.GetBytes(salt);
      // Her hesap için farklı salt kullanılır; veritabanında açık şifre tutulmaz.
      using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256))
        return "PBKDF2$" + IterationCount + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    public static bool Verify(string password, string storedPassword)
    {
      if (password == null || storedPassword == null)
        return false;
      // Eski kayıtların mevcut şifreleriyle giriş yapabilmesini koru.
      if (!storedPassword.StartsWith("PBKDF2$", StringComparison.Ordinal))
        return string.Equals(password, storedPassword, StringComparison.Ordinal);
      string[] parts = storedPassword.Split('$');
      int iterations;
      if (parts.Length != 4 || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out iterations) || iterations < 100000 || iterations > 2000000)
        return false;
      try
      {
        byte[] salt = Convert.FromBase64String(parts[2]);
        byte[] expected = Convert.FromBase64String(parts[3]);
        if (salt.Length != 16 || expected.Length != 32)
          return false;
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
        {
          byte[] calculated = pbkdf2.GetBytes(32);
          int difference = 0;
          for (int i = 0; i < expected.Length; i++)
            difference |= expected[i] ^ calculated[i];
          return difference == 0;
        }
      }
      catch (FormatException)
      {
        return false;
      }
    }
  }
}