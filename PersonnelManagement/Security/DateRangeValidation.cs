using System;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Security
{
  internal static class DateRangeValidation
  {
    internal static void Require(DateTime? start, DateTime? end)
    {
      // Aynı aralık kuralı senkron ve asenkron okumalarda uygulanır.
      // SQL datetime sınırından küçük tarihler sorguya gönderilmez.
      if ((start.HasValue && start.Value.Date < new DateTime(1753, 1, 1)) ||
          (end.HasValue && end.Value.Date < new DateTime(1753, 1, 1)) ||
          (start.HasValue && end.HasValue && end.Value.Date < start.Value.Date))
        throw new ArgumentException(UiText.Get("InvalidLeaveDateRange"));
    }
  }
}
