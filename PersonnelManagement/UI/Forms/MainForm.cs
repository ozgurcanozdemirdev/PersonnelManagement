using PersonnelManagement.UI.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.UI.Forms
{
  public partial class MainForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.ViewDashboard;

    public MainForm()
    {
      InitializeComponent();
      var languageButton = new Button
      {
        Name = "btnLanguage",
        Text = "TR / EN",
        Location = new Point(330, 153),
        Size = new Size(156, 28)
      };
      languageButton.Click += (sender, e) => LanguageManager.SetLanguage(LanguageManager.Language == "tr" ? "en" : "tr");
      Controls.Add(languageButton);
    }

    private readonly ReportRepository _reportRepository = new ReportRepository();
    private Timer _clockTimer;
    private void MainFormLoad(object sender, EventArgs e)
    {
      // Menüleri güncel İK rolünün yetkilerine göre aç.
      ApplyUserPermissions();
      UiText.Bind(lblWelcome, "Welcome", LoginForm.CurrentUser.FullName);
      if (components == null)
        components = new Container();
      _clockTimer?.Dispose();
      _clockTimer = new Timer(components);
      _clockTimer.Interval = 1000;
      _clockTimer.Tick += (s, ev) => UpdateClock();
      _clockTimer.Start();
      UpdateClock();
      LoadSummary();
    }

    private void ApplyUserPermissions()
    {
      var user = OperationAuthorization.RefreshCurrentUser();
      bool canManageUsers = OperationAuthorization.HasPermission(user, ApplicationPermission.ManageUsers);
      btnUsers.Visible = canManageUsers;
      btnUsers.Enabled = canManageUsers;
      btnReports.Enabled = OperationAuthorization.HasPermission(user, ApplicationPermission.ViewReports);
      btnEmployeeManagement.Enabled = OperationAuthorization.HasPermission(user, ApplicationPermission.ViewPersonnel);
      btnShiftManagement.Enabled = OperationAuthorization.HasPermission(user, ApplicationPermission.ViewShifts);
      btnAttendance.Enabled = OperationAuthorization.HasPermission(user, ApplicationPermission.RecordAttendance);
      btnLeaveManagement.Enabled = OperationAuthorization.HasPermission(user, ApplicationPermission.ViewLeaves);
    }

    private void ClockTimerTick(object sender, EventArgs e) => UpdateClock();
    private void UpdateClock()
    {
      lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss");
    }

    private void LoadSummary()
    {
      try
      {
        var summary = _reportRepository.GetTodaySummary();
        lblTotalEmployees.Text = summary.TotalEmployees.ToString();
        lblTodayCheckIns.Text = summary.TodayCheckIns.ToString();
        lblTodayLeaves.Text = summary.TodayLeaves.ToString();
        lblTodayAbsences.Text = summary.TodayAbsences.ToString();
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("StatisticsCouldNotBeLoaded") + ex.Message, UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void BtnExitClick(object sender, EventArgs e)
    {
      var result = MessageBox.Show(UiText.Get("ExitTheApplication"), UiText.Get("Exit"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (result == DialogResult.Yes)
      {
        try
        {
          AuditLogger.Log("Logout", null, null, UiText.Format("SignedOut", LoginForm.CurrentUser.Username));
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        _clockTimer?.Stop();
        Application.Exit();
      }
    }

    private void BtnEmployeeManagementClick(object sender, EventArgs e)
    {
      OpenForm(new EmployeeManagementForm());
    }

    private void BtnShiftManagementClick(object sender, EventArgs e)
    {
      OpenForm(new ShiftManagementForm());
    }

    private void BtnAttendanceClick(object sender, EventArgs e)
    {
      OpenForm(new AttendanceForm());
    }

    private void BtnReportsClick(object sender, EventArgs e)
    {
      OpenForm(new ReportsForm());
    }

    private void BtnLeaveManagementClick(object sender, EventArgs e)
    {
      OpenForm(new LeaveManagementForm());
    }

    private void BtnUsersClick(object sender, EventArgs e)
    {
      OpenForm(new UserManagementForm());
    }

    private void BtnRefreshClick(object sender, EventArgs e)
    {
      LoadSummary();
    }

    private void MainFormFormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        var result = MessageBox.Show(UiText.Get("ExitTheApplication"), UiText.Get("Exit"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.No)
        {
          e.Cancel = true;
          return;
        }

        _clockTimer?.Stop();
        Application.Exit();
      }
    }

    private void OpenForm(Form form)
    {
      form.FormClosed += (s, e) =>
      {
        ApplyUserPermissions();
        LoadSummary();
      };
      try
      {
        form.Show();
      }
      catch (UnauthorizedAccessException ex)
      {
        form.Dispose();
        ApplyUserPermissions();
        MessageBox.Show(ex.Message, UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }
  }
}
