using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using PersonnelManagement.Connection;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Models;
using PersonnelManagement.Reporting;
using PersonnelManagement.Security;
using PersonnelManagement.UI.Forms;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Tests
{
  internal static partial class RegressionTests
  {
    private static void ExtendedRegressionChecks(string source)
    {
      Session(_admin, "Admin");
      ConnectionChecks(source);
      AttendanceAndLeaveChecks();
      ExportAndPrintChecks();
      DemoChecks(source);
      MigrationChecks(source);
    }

    private static void ConnectionChecks(string source)
    {
      string prior = Environment.GetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable);
      string folder = Path.Combine(Path.GetTempPath(), "PersonnelManagementTest_" + Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(folder);
      string file = Path.Combine(folder, "connection.dat");
      string alternate = Path.Combine(folder, "alternate.dat");
      var flags = BindingFlags.Static | BindingFlags.NonPublic;
      var write = typeof(ConnectionSettingsStore).GetMethod("WriteProtectedFile", flags);
      var read = typeof(ConnectionSettingsStore).GetMethod("ReadSavedSettings", flags);
      try
      {
        Check("v100_english_profile_is_separate", ConnectionSettingsStore.DefaultPath.EndsWith(Path.Combine("V1.0.0", "English", "connection.dat")));
        foreach (string invalid in new[] {
          "Data Source=localhost;Initial Catalog=test;Integrated Security=True",
          "Data Source=localhost;Initial Catalog=test;User ID=test",
          "Data Source=localhost;User ID=test;Password=synthetic",
          "Data Source=localhost;Initial Catalog=test;User ID=sa;Password=synthetic" })
        {
          Environment.SetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable, invalid);
          Check("invalid_sql_settings_" + (++_sequence), Rejected(() => ConnectionSettingsStore.Load()));
        }
        Environment.SetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable, prior);
        Check("process_override_selected", new SqlConnectionStringBuilder(ConnectionSettingsStore.Load()).InitialCatalog == _database.Database);
        Check("encryption_is_required", ConnectionSettingsStore.Validate(prior).Encrypt);
        write.Invoke(null, new object[] { prior, file });
        byte[] first = File.ReadAllBytes(file);
        Check("settings_are_encrypted", !Encoding.UTF8.GetString(first).Contains(new SqlConnectionStringBuilder(prior).Password));
        var other = new SqlConnectionStringBuilder(prior) { InitialCatalog = "SyntheticAlternate" };
        write.Invoke(null, new object[] { other.ConnectionString, alternate });
        string missing = Path.Combine(folder, "missing.dat");
        Check("application_file_fallback", (string)read.Invoke(null, new object[] { missing, file }) == prior);
        Check("profile_precedes_application_file", (string)read.Invoke(null, new object[] { file, alternate }) == prior);
        Check("missing_settings_return_null", read.Invoke(null, new object[] { missing, missing }) == null);
        write.Invoke(null, new object[] { other.ConnectionString, file });
        Check("encrypted_settings_can_be_replaced", (string)read.Invoke(null, new object[] { file, alternate }) == other.ConnectionString);
        File.WriteAllBytes(file, new byte[] { 1, 2, 3 });
        bool refused = false;
        try { read.Invoke(null, new object[] { file, alternate }); }
        catch (TargetInvocationException ex) { refused = ex.InnerException is System.Security.Cryptography.CryptographicException; }
        Check("corrupt_profile_does_not_silently_fallback", refused);

        string script = Path.GetFullPath(Path.Combine(source, "..", "..", "tools", "ConfigureConnection.ps1"));
        Check("configure_sql_auth_success", Configure(script, folder, false) == 0);
        Check("configure_saved_settings_are_readable", new SqlConnectionStringBuilder((string)read.Invoke(null, new object[] { file, alternate })).InitialCatalog == _database.Database);
        byte[] configured = File.ReadAllBytes(file);
        Check("configure_can_replace_existing_file", Configure(script, folder, false) == 0);
        Check("sources_configure_entry_point", Configure(Path.Combine(source, "ConfigureConnection.ps1"), folder, false) == 0);
        configured = File.ReadAllBytes(file);
        Check("configure_bad_database_refused", Configure(script, folder, true) != 0);
        Check("configure_failure_preserves_settings", configured.SequenceEqual(File.ReadAllBytes(file)));
        Check("configure_leaves_no_temp_files", Directory.GetFiles(folder, "*.tmp").Length == 0);
        foreach (string language in new[] { "tr", "en" })
        {
          LanguageManager.SetLanguage(language, false);
          using (var login = new LoginForm())
          using (var settings = new ConnectionSettingsForm())
          {
            Check("password_colon_" + language, login.Controls.Find("label2", true).Single().Text.EndsWith(":"));
            var certificate = (CheckBox)settings.Controls.Find("chkTrustServerCertificate", true).Single();
            Check("certificate_choice_matches_settings_" + language, certificate.Checked == new SqlConnectionStringBuilder(prior).TrustServerCertificate);
            Check("certificate_caption_translated_" + language, certificate.Text == UiText.Get("TrustServerCertificate"));
          }
        }
      }
      finally
      {
        Environment.SetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable, prior);
        // Bu GUID klasörü yalnız test tarafından oluşturulur; kullanıcı ayarına dokunulmaz.
        foreach (string testFile in Directory.GetFiles(folder)) File.Delete(testFile);
        Directory.Delete(folder);
      }
    }

    private static int Configure(string script, string directory, bool badDatabase)
    {
      string command = "$ErrorActionPreference='Stop';$s=[Data.SqlClient.SqlConnectionStringBuilder]::new($env:" + ConnectionSettingsStore.EnvironmentVariable + ");" +
        "$c=[pscredential]::new($s.UserID,(ConvertTo-SecureString $s.Password -AsPlainText -Force));" +
        "& '" + script.Replace("'", "''") + "' -Server $s.DataSource -Database " +
        (badDatabase ? "('Missing_'+[Guid]::NewGuid().ToString('N'))" : "$s.InitialCatalog") +
        " -Credential $c -SettingsDirectory '" + directory.Replace("'", "''") + "' -TrustServerCertificate:($s.TrustServerCertificate);";
      var info = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "WindowsPowerShell", "v1.0", "powershell.exe")) {
        Arguments = "-NoProfile -NonInteractive -ExecutionPolicy RemoteSigned -EncodedCommand " + Convert.ToBase64String(Encoding.Unicode.GetBytes(command)),
        UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
      info.EnvironmentVariables["PSModulePath"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "WindowsPowerShell", "v1.0", "Modules");
      using (var process = Process.Start(info))
      {
        var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(30000)) { process.Kill(); throw new InvalidOperationException("Configuration test timeout"); }
        System.Threading.Tasks.Task.WaitAll(output, error);
        if (process.ExitCode != 0 && !badDatabase)
        {
          string message = error.Result.Replace(_server.Password, "[redacted]");
          string applicationPassword = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable)).Password;
          if (!string.IsNullOrEmpty(applicationPassword)) message = message.Replace(applicationPassword, "[redacted]");
          Console.WriteLine("CONFIGURE_DIAGNOSTIC | " + message);
        }
        return process.ExitCode;
      }
    }

    private static void AttendanceAndLeaveChecks()
    {
      var attendance = new AttendanceRepository(); var leaves = new LeaveRepository(); var reports = new ReportRepository();
      var today = DateTime.Today;
      var plan = AttendanceRepository.GetShiftPlan(today.AddMinutes(30), TimeSpan.FromHours(22), TimeSpan.FromHours(6));
      Check("night_shift_day", plan.StartDate == today.AddDays(-1).AddHours(22) && plan.EndDate == today.AddHours(6));
      var duration = AttendanceRepository.CalculateWorkDuration(plan.StartDate, plan.EndDate, plan.StartDate, plan.EndDate, 30);
      Check("night_work_duration", duration.WorkMinutes == 450 && duration.OvertimeMinutes == 0);
      Check("backward_clock_rejected", Rejected(() => AttendanceRepository.CalculateWorkDuration(today.AddHours(8), today.AddHours(7), today.AddHours(8), today.AddHours(17), 60)));
      int employee = Employees.Save(NewEmployee());
      int entry = attendance.CheckIn(employee, _admin);
      Check("immediate_checkout", attendance.CheckOut(employee, _admin));
      Check("closed_entry_not_open", !attendance.HasOpenRecord(employee));
      Check("duplicate_shift_day_rejected", Rejected(() => attendance.CheckIn(employee, _admin)));
      Check("attendance_timestamp_precision", Count("SELECT COUNT(*) FROM dbo.AttendanceRecord WHERE AttendanceId=" + entry + " AND CheckOutAt>=CheckInAt") == 1);
      Execute(_database, "UPDATE dbo.Shift SET BreakMinutes=0,ShiftName=N'Changed synthetic shift' WHERE ShiftId=" + _shift);
      Check("saved_shift_snapshot_preserved", Count("SELECT COUNT(*) FROM dbo.AttendanceRecord WHERE AttendanceId=" + entry + " AND PlannedBreakMinutes=60 AND RecordedShiftName=N'Synthetic shift'") == 1);
      Execute(_database, "UPDATE dbo.Shift SET BreakMinutes=60,ShiftName=N'Synthetic shift' WHERE ShiftId=" + _shift);
      Check("reversed_attendance_range", Rejected(() => attendance.GetByDateRange(today, today.AddDays(-1))));
      Check("reversed_async_attendance_range", Rejected(() => attendance.GetByDateRangeAsync(today, today.AddDays(-1)).GetAwaiter().GetResult()));
      Check("reversed_leave_range", Rejected(() => leaves.GetByDateRange(today, today.AddDays(-1))));
      Check("reversed_async_leave_range", Rejected(() => leaves.GetByDateRangeAsync(today, today.AddDays(-1)).GetAwaiter().GetResult()));
      Check("reversed_report_range", Rejected(() => reports.GetEmployeeSummary(today, today.AddDays(-1))));
      Check("reversed_async_report_range", Rejected(() => reports.GetEmployeeSummaryAsync(today, today.AddDays(-1)).GetAwaiter().GetResult()));
      int leaveEmployee = Employees.Save(NewEmployee());
      Func<DateTime, DateTime, int> save = (start, end) => leaves.Save(new LeaveRequest { EmployeeId = leaveEmployee, LeaveTypeId = _leaveType, StartDate = start, EndDate = end });
      var year = new DateTime(today.Year, 1, 1);
      int leave = save(year.AddDays(-2), year.AddDays(2));
      Check("pending_leave_overlap_rejected", Rejected(() => save(year, year.AddDays(3))));
      Check("leave_approved", leaves.Approve(leave, _admin));
      Check("annual_balance_current_year", leaves.GetRemainingAnnualLeave(leaveEmployee, today.Year) == 11);
      Check("annual_balance_previous_year", leaves.GetRemainingAnnualLeave(leaveEmployee, today.Year - 1) == 12);
      Check("approved_leave_overlap_rejected", Rejected(() => save(year, year.AddDays(3))));
      Check("report_clips_leave_to_range", reports.GetEmployeeSummary(year, year.AddDays(2), leaveEmployee).Single().TotalLeaveDays == 3);
      Check("leave_filter_includes_overlap", leaves.GetByDateRange(year, year, leaveEmployee).Count == 1);
      Check("leave_filter_excludes_outside", leaves.GetByDateRange(year.AddDays(10), year.AddDays(11), leaveEmployee).Count == 0);
      Check("annual_allowance_cannot_be_exceeded", Rejected(() => save(year.AddDays(10), year.AddDays(21))));
      Check("approved_leave_cancelled", leaves.Cancel(leave, _admin));
      Check("cancel_restores_allowance", leaves.GetRemainingAnnualLeave(leaveEmployee, today.Year) == 14);
      Check("cancelled_leave_cannot_be_approved", Rejected(() => leaves.Approve(leave, _admin)));
      Check("empty_rejection_reason_rejected", Rejected(() => leaves.Reject(leave, _admin, "")));
    }

    private static void ExportAndPrintChecks()
    {
      string folder = Path.Combine(Path.GetTempPath(), "PersonnelManagementTest_" + Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(folder);
      try
      {
        string excel = Path.Combine(folder, "report.xlsx"), csv = Path.Combine(folder, "report.csv");
        var rows = new List<object[]> { new object[] { "Name", "Value" }, new object[] { "=1+1", 12.5m }, new object[] { "Türkçe\nText", -2 } };
        ReportExporter.Save(rows, excel); ReportExporter.Save(rows, csv);
        using (var zip = ZipFile.OpenRead(excel))
        {
          foreach (var part in zip.Entries.Where(x => x.FullName.EndsWith(".xml") || x.FullName.EndsWith(".rels")))
            using (var stream = part.Open()) XDocument.Load(stream);
          using (var stream = zip.GetEntry("xl/worksheets/sheet1.xml").Open())
          {
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            var xml = XDocument.Load(stream);
            Check("xlsx_numeric_cells", xml.Descendants(ns + "c").Any(c => c.Element(ns + "v")?.Value == "12.5"));
            Check("xlsx_user_text_not_formula", !xml.Descendants(ns + "f").Any());
          }
        }
        Check("csv_formula_escaped", File.ReadAllText(csv).Contains("\"'=1+1\""));
        byte[] before = File.ReadAllBytes(excel);
        Check("export_limits_rejected", Rejected(() => ReportExporter.Save(new List<object[]> { new object[16385] }, excel)));
        Check("failed_export_preserves_file", before.SequenceEqual(File.ReadAllBytes(excel)));
        Check("export_temp_files_removed", Directory.GetFiles(folder, "*.tmp").Length == 0);
        using (var grid = new DataGridView { AllowUserToAddRows = false })
        {
          grid.Columns.Add("First", "First"); grid.Columns.Add("Last", "Last"); grid.Columns.Add("Hidden", "Hidden");
          grid.Columns[2].Visible = false; grid.Columns[1].DisplayIndex = 0;
          for (int i = 0; i < 250; i++) grid.Rows.Add("Row " + i, i == 0 ? new string('A', 10000) : "Text", "Secret");
          ReportExporter.Save(grid, csv);
          Check("export_visible_column_order", File.ReadAllText(csv).StartsWith("\"Last\";\"First\""));
          var type = typeof(ReportsForm).GetNestedType("ReportPrintDocument", BindingFlags.NonPublic);
          using (var document = (PrintDocument)Activator.CreateInstance(type, new object[] { grid, "Synthetic report", "Synthetic date range", "" }))
          using (var bitmap = new Bitmap(1000, 700))
          using (var graphics = Graphics.FromImage(bitmap))
          {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var begin = type.GetMethod("OnBeginPrint", flags);
            begin.Invoke(document, new object[] { new PrintEventArgs() });
            int pages = 0; bool more;
            do {
              var page = new PrintPageEventArgs(graphics, new Rectangle(40, 40, 900, 600), new Rectangle(0, 0, 1000, 700), new PageSettings());
              type.GetMethod("OnPrintPage", flags).Invoke(document, new object[] { page });
              more = page.HasMorePages;
              if (++pages > 100) throw new InvalidOperationException("Printing did not finish");
            } while (more);
            Check("print_all_rows_and_long_text", pages > 1 && (int)type.GetField("_rowIndex", flags).GetValue(document) == 250);
            begin.Invoke(document, new object[] { new PrintEventArgs() });
            Check("print_preview_reset", (int)type.GetField("_rowIndex", flags).GetValue(document) == 0);
          }
        }
      }
      finally { foreach (string file in Directory.GetFiles(folder)) File.Delete(file); Directory.Delete(folder); }
    }

    private static void DemoChecks(string source)
    {
      foreach (string language in new[] { "English", "Turkish" })
      {
        string name = (language == "English" ? "PersonnelManagementTest_" : "PersonelYonetimiTest_") + Guid.NewGuid().ToString("N");
        _databases.Add(name);
        string original = language == "English" ? "PersonnelManagementDB" : "PersonelYonetimiDB";
        using (var master = Connect("master")) Script(master, Path.Combine(source, language, "DatabaseSetupWithDemo.sql"), original, name);
        using (var db = Connect(name))
        {
          string table = language == "English" ? "Employee" : "Personel";
          string account = language == "English" ? "UserAccount" : "Kullanici";
          using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo." + table, db)) Check("demo_personnel_" + language, Convert.ToInt32(command.ExecuteScalar()) > 0);
          using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo." + account, db)) Check("demo_three_roles_" + language, Convert.ToInt32(command.ExecuteScalar()) == 3);
          Check("demo_repeat_refused_" + language, Rejected(() => Script(db, Path.Combine(source, language, "DatabaseDemoData.sql"), original, name)));
        }
        ApplicationAccountChecks(source, language, name);
      }
    }
  }
}
