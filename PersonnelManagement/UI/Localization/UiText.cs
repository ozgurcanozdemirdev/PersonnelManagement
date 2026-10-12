using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PersonnelManagement.UI.Localization
{
  public static class UiText
  {
    private sealed class TextBinding
    {
      public string Key;
      public object[] Arguments;
      public string LastText;
      public Func<string> Compute;
    }

    private static readonly ConditionalWeakTable<Control, TextBinding> _bindings = new ConditionalWeakTable<Control, TextBinding>();
    private static readonly ConditionalWeakTable<DataGridViewColumn, TextBinding> _headerBindings = new ConditionalWeakTable<DataGridViewColumn, TextBinding>();
    private static readonly Dictionary<string, string> _captionKeys = BuildCaptionKeys();
    public static string Get(string key) => Properties.Resources.ResourceManager.GetString(key, LanguageManager.UiCulture) ?? key;
    public static string Format(string key, params object[] arguments) => string.Format(LanguageManager.UiCulture, Get(key), arguments);
    public static void Bind(Control control, string key, params object[] arguments)
    {
      _bindings.Remove(control);
      var binding = new TextBinding
      {
        Key = key,
        Arguments = arguments,
        LastText = arguments.Length == 0 ? Get(key) : Format(key, arguments)
      };
      _bindings.Add(control, binding);
      control.Text = binding.LastText;
    }

    internal static bool RefreshBinding(Control control)
    {
      if (!_bindings.TryGetValue(control, out var binding))
        return false;
      // Bir olay metni sonradan temizlediyse eski durumu yeniden göstermeyin.
      if (control.Text != binding.LastText)
      {
        _bindings.Remove(control);
        return false;
      }

      control.Text = binding.LastText = binding.Compute != null ? binding.Compute() : binding.Arguments.Length == 0 ? Get(binding.Key) : Format(binding.Key, binding.Arguments);
      return true;
    }

    public static void BindHeader(DataGridViewColumn column, string key)
    {
      _headerBindings.Remove(column);
      var binding = new TextBinding { Key = key, LastText = Get(key) };
      _headerBindings.Add(column, binding);
      column.HeaderText = binding.LastText;
    }

    internal static bool RefreshHeaderBinding(DataGridViewColumn column)
    {
      if (!_headerBindings.TryGetValue(column, out var binding))
        return false;
      if (column.HeaderText != binding.LastText)
      {
        _headerBindings.Remove(column);
        return false;
      }

      column.HeaderText = binding.LastText = Get(binding.Key);
      return true;
    }
    public static void BindComputed(Control control, Func<string> compute)
    {
      _bindings.Remove(control);
      var binding = new TextBinding { Compute = compute, LastText = compute() };
      _bindings.Add(control, binding);
      control.Text = binding.LastText;
    }

    private static Dictionary<string, string> BuildCaptionKeys()
    {
      var result = new Dictionary<string, string>(StringComparer.Ordinal);
      foreach (string language in new[]
      {
        "tr-TR",
        "en-US"
      }

      )
      {
        ResourceSet set = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.GetCultureInfo(language), true, true);
        foreach (DictionaryEntry item in set)
          if (item.Value is string text && !text.Contains("{0"))
            result[text] = (string)item.Key;
      }

      return result;
    }

    public static string FindKey(string caption) => caption != null && _captionKeys.TryGetValue(caption, out string key) ? key : null;
    public static string Translate(string caption)
    {
      string key = FindKey(caption);
      return key == null ? caption : Get(key);
    }

    public static string DisplayValue(object value)
    {
      if (value == null || value == DBNull.Value)
        return "";
      if (value is bool flag)
        return Translate(flag ? "Evet" : "Hayır");
      string text = value.ToString();
      switch (text)
      {
        case "Admin":
          return Get("RoleAdmin");
        case "Manager":
          return Get("RoleManager");
        case "Employee":
        case "Calisan":
          return Get("RoleEmployee");
        case "User":
        case "UserAccount":
          return Translate("Kullanıcı");
        case "Pending":
          return Translate("Beklemede");
        case "Approved":
          return Translate("Onaylı");
        case "Rejected":
          return Translate("Reddedildi");
        case "Cancelled":
          return Translate("İptal Edildi");
        case "Annual Leave":
          return Translate("Yıllık İzin");
        case "Sick Leave":
          return Translate("Hastalık İzni");
        case "Personal Leave":
          return Translate("Mazeret İzni");
        case "Unpaid Leave":
          return Translate("Ücretsiz İzin");
        case "Maternity Leave":
          return Translate("Doğum İzni");
      }

      return Translate(text);
    }
  }
}
