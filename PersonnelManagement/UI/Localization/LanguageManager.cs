using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace PersonnelManagement.UI.Localization
{
  public static class LanguageManager
  {
    private static string _language = "tr";
    private static readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PersonnelManagement", "V1.0.0", "English", "language.txt");
    public static event EventHandler LanguageChanged;
    public static string Language => _language;
    public static CultureInfo UiCulture => CultureInfo.GetCultureInfo(_language == "en" ? "en-US" : "tr-TR");

    public static void Initialize()
    {
      try
      {
        if (File.Exists(_settingsPath))
          _language = File.ReadAllText(_settingsPath).Trim() == "en" ? "en" : "tr";
      }
      catch (IOException)
      {
      }
      catch (UnauthorizedAccessException)
      {
      }

      Thread.CurrentThread.CurrentUICulture = UiCulture;
    }

    public static void SetLanguage(string language, bool persist = true)
    {
      if (language != "tr" && language != "en")
        throw new ArgumentException("Unsupported language.", nameof(language));
      if (_language == language)
        return;
      _language = language;
      // Sayı ve tarih giriş biçimleri değiştirilmez; yalnızca arayüz kültürü güncellenir.
      Thread.CurrentThread.CurrentUICulture = UiCulture;
      if (persist)
      {
        try
        {
          Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));
          File.WriteAllText(_settingsPath, language);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
      }

      LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
  }
}