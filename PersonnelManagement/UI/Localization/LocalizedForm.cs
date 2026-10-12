using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PersonnelManagement.Models;
using PersonnelManagement.Security;

namespace PersonnelManagement.UI.Localization
{
  public class LocalizedForm : Form
  {
    private readonly HashSet<Control> _registeredControls = new HashSet<Control>();
    public LocalizedForm()
    {
      LanguageManager.LanguageChanged += HandleLanguageChanged;
    }

    protected virtual ApplicationPermission? RequiredPermission => null;

    private void ValidateAccess()
    {
      if (RequiredPermission.HasValue && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        OperationAuthorization.Require(RequiredPermission.Value);
    }

    protected override void SetVisibleCore(bool value)
    {
      if (value)
        ValidateAccess();
      base.SetVisibleCore(value);
    }

    protected override void OnLoad(EventArgs e)
    {
      ValidateAccess();
      ApplyLanguage();
      base.OnLoad(e);
      ApplyLanguage();
    }

    private void HandleLanguageChanged(object sender, EventArgs e)
    {
      if (IsDisposed)
        return;
      if (InvokeRequired)
      {
        BeginInvoke(new Action(ApplyLanguage));
        return;
      }

      ApplyLanguage();
    }

    public void ApplyLanguage()
    {
      SuspendLayout();
      TranslateControl(this);
      ResumeLayout(true);
    }

    private void TranslateControl(Control control)
    {
      if (!(control is TextBoxBase || control is ComboBox || control is DateTimePicker || control is NumericUpDown || control is DataGridView))
        if (!UiText.RefreshBinding(control))
        {
          // Aynı İngilizce metne çevrilen farklı Türkçe başlıklar kendi anahtarını korur.
          string key = UiText.FindKey(control.Text);
          if (key != null)
            UiText.Bind(control, key);
        }
      if (control is ComboBox combo)
      {
        if (_registeredControls.Add(control))
        {
          combo.FormattingEnabled = true;
          combo.Format += FormatItem;
        }

        combo.FormatInfo = LanguageManager.UiCulture;
      }

      if (control is DataGridView grid)
      {
        if (_registeredControls.Add(control))
        {
          grid.DataBindingComplete += (s, e) => TranslateHeaders(grid);
          grid.CellFormatting += FormatCell;
        }

        TranslateHeaders(grid);
        // Hesaplanan süre alanları da seçilen dilde yeniden okunur.
        grid.Invalidate();
      }

      foreach (Control child in control.Controls)
        TranslateControl(child);
    }

    private static void TranslateHeaders(DataGridView grid)
    {
      foreach (DataGridViewColumn column in grid.Columns)
        if (!UiText.RefreshHeaderBinding(column))
        {
          string key = UiText.FindKey(column.HeaderText);
          if (key != null)
            UiText.BindHeader(column, key);
        }
    }

    private static void FormatItem(object sender, ListControlConvertEventArgs e)
    {
      if (e.ListItem is Employee employee && employee.EmployeeId == 0)
        e.Value = UiText.Translate("Tümü");
      else if (e.ListItem is LeaveType type)
        e.Value = UiText.DisplayValue(type.TypeName);
      else if (e.ListItem is string)
        e.Value = UiText.DisplayValue(e.ListItem);
    }

    private static void FormatCell(object sender, DataGridViewCellFormattingEventArgs e)
    {
      var grid = (DataGridView)sender;
      string field = grid.Columns[e.ColumnIndex].DataPropertyName;
      string description = Convert.ToString(e.Value);
      if (field == "Description" &&
          (description == "Shift completed; no check-in or approved leave found." ||
           description == "Shift tamamlandı; giriş veya onaylı izin bulunamadı."))
      {
        // Önceki veritabanı fonksiyonu güncellenmemiş olsa da sistem açıklaması seçilen dilde gösterilir.
        e.Value = UiText.Get("ShiftCompletedNoCheckInOrApprovedLeaveFound");
        e.FormattingApplied = true;
      }
      if (field == "Role" || field == "Status" || field == "Gender" || field == "LeaveTypeName")
      {
        e.Value = UiText.DisplayValue(e.Value);
        e.FormattingApplied = true;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        LanguageManager.LanguageChanged -= HandleLanguageChanged;
        _registeredControls.Clear();
      }

      base.Dispose(disposing);
    }
  }
}
