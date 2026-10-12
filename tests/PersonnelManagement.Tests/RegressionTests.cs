using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PersonnelManagement.Connection;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Logging;
using PersonnelManagement.Models;
using PersonnelManagement.Security;
using PersonnelManagement.UI.Forms;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Tests
{
  internal static partial class RegressionTests
  {
    private static SqlConnectionStringBuilder _server;
    private static SqlConnection _database;
    private static SqlConnection _application;
    private static readonly List<string> _databases = new List<string>();
    private static string _login;
    private static string _stage;
    private static int _checks;
    private static int _sequence;
    private static int _department, _position, _shift, _admin, _manager, _staff, _leaveType;
    private static readonly EmployeeRepository Employees = new EmployeeRepository();
    private static readonly ShiftRepository Shifts = new ShiftRepository();
    private static readonly UserRepository Users = new UserRepository();

    private static void Check(string name, bool success)
    {
      _stage = name;
      Console.WriteLine((success ? "PASS" : "FAIL") + " | " + name);
      if (!success) throw new InvalidOperationException(name);
      _checks++;
    }

    private static SqlConnection Connect(string database)
    {
      var builder = new SqlConnectionStringBuilder(_server.ConnectionString) { InitialCatalog = database, ConnectTimeout = 10 };
      var connection = new SqlConnection(builder.ConnectionString);
      connection.Open();
      return connection;
    }

    private static void Execute(SqlConnection connection, string sql)
    {
      using (var command = new SqlCommand(sql, connection) { CommandTimeout = 60 }) command.ExecuteNonQuery();
    }

    private static int Count(string sql)
    {
      using (var command = new SqlCommand(sql, _database)) return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void Script(SqlConnection connection, string file, string original, string target)
    {
      if (!Regex.IsMatch(target, "^(PersonnelManagementTest|PersonelYonetimiTest)_[a-f0-9]{32}$"))
        throw new InvalidOperationException("Temporary database name required");
      string source = File.ReadAllText(file).Replace(original, target);
      foreach (string batch in Regex.Split(source, @"^[ \t]*GO[ \t]*(?:--[^\r\n]*)?\r?$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        if (!string.IsNullOrWhiteSpace(batch)) Execute(connection, batch);
    }

    private static void Session(int id, string role)
    {
      typeof(LoginForm).GetProperty("CurrentUser", BindingFlags.Public | BindingFlags.Static).SetValue(null,
        new UserAccount { UserId = id, Username = "synthetic_" + role, Role = role, IsActive = true });
      AuditLogger.SetCurrentUser(id);
    }

    private static Employee NewEmployee()
    {
      return new Employee { NationalId = (83000000000L + ++_sequence).ToString(), FirstName = "Synthetic", LastName = "Person" + _sequence,
        DepartmentId = _department, PositionId = _position, ShiftId = _shift, EmploymentStartDate = DateTime.Today.AddYears(-2), AnnualLeaveAllowance = 14 };
    }

    private static bool Rejected(Action operation)
    {
      try { operation(); return false; }
      catch (ArgumentException) { return true; }
      catch (InvalidOperationException) { return true; }
      catch (UnauthorizedAccessException) { return true; }
      catch (SqlException) { return true; }
    }

    private static void Fixtures()
    {
      Execute(_database, "INSERT dbo.UserAccount(Username,PasswordHash,FirstName,LastName,Role) VALUES(N'synthetic_Admin',N'Synthetic123!',N'Synthetic',N'Admin',N'Admin'),(N'synthetic_Manager',N'Synthetic123!',N'Synthetic',N'Manager',N'Manager'),(N'synthetic_Employee',N'Synthetic123!',N'Synthetic',N'Employee',N'Employee');");
      _admin = Count("SELECT UserId FROM dbo.UserAccount WHERE Role=N'Admin'");
      _manager = Count("SELECT UserId FROM dbo.UserAccount WHERE Role=N'Manager'");
      _staff = Count("SELECT UserId FROM dbo.UserAccount WHERE Role=N'Employee'");
      Session(_admin, "Admin");
      new DepartmentRepository().Save(new Department { DepartmentName = "Synthetic department" });
      _department = Count("SELECT MAX(DepartmentId) FROM dbo.Department");
      new PositionRepository().Save(new Position { DepartmentId = _department, PositionName = "Synthetic position" });
      _position = Count("SELECT MAX(PositionId) FROM dbo.Position");
      _shift = Shifts.Save(new Shift { ShiftName = "Synthetic shift", StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(17), BreakMinutes = 60,
        Monday = true, Tuesday = true, Wednesday = true, Thursday = true, Friday = true, Saturday = true, Sunday = true });
      Execute(_database, "INSERT dbo.LeaveType(TypeName,AffectsAnnualAllowance) VALUES(N'Synthetic annual',1);");
      _leaveType = Count("SELECT MAX(LeaveTypeId) FROM dbo.LeaveType");
    }

    private static void ValidationChecks()
    {
      foreach (string value in new[] { null, "", "8700000000", "870000000012", "-8700000000", "+8700000000", "8700000000 ", "１２３４５６７８９０１", "8700000000A" })
        Check("national_id_invalid_" + (++_sequence), !EmployeeValidation.IsNationalIdValid(value));
      Check("national_id_eleven_digits", EmployeeValidation.IsNationalIdValid("83000000001"));
      var invalid = NewEmployee(); invalid.NationalId = "-8700000000";
      int before = Count("SELECT COUNT(*) FROM dbo.Employee");
      Check("repository_rejects_signed_id", Rejected(() => Employees.Save(invalid)));
      invalid = NewEmployee(); invalid.BirthDate = DateTime.Today.AddYears(1);
      Check("repository_rejects_future_birth", Rejected(() => Employees.Save(invalid)));
      invalid = NewEmployee(); invalid.BirthDate = DateTime.Today.AddDays(-1);
      Check("repository_rejects_birth_after_employment", Rejected(() => Employees.Save(invalid)));
      invalid = NewEmployee(); invalid.FirstName = new string('A', 51);
      Check("repository_rejects_long_name", Rejected(() => Employees.Save(invalid)));
      invalid = NewEmployee(); invalid.DepartmentId = 0;
      Check("repository_rejects_missing_department", Rejected(() => Employees.Save(invalid)));
      Check("invalid_personnel_not_persisted", Count("SELECT COUNT(*) FROM dbo.Employee") == before);
      Check("database_rejects_signed_id", Rejected(() => Execute(_database,
        "INSERT dbo.Employee(NationalId,FirstName,LastName,DepartmentId,PositionId,ShiftId,EmploymentStartDate) SELECT '-8700000000',N'Synthetic',N'Invalid'," + _department + "," + _position + "," + _shift + ",CONVERT(date,GETDATE());")));
      Check("database_rejects_short_padded_id", Rejected(() => Execute(_database,
        "INSERT dbo.Employee(NationalId,FirstName,LastName,DepartmentId,PositionId,ShiftId,EmploymentStartDate) SELECT '83999',N'Synthetic',N'Invalid'," + _department + "," + _position + "," + _shift + ",CONVERT(date,GETDATE());")));
      Check("database_rejects_future_birth", Rejected(() => Execute(_database,
        "INSERT dbo.Employee(NationalId,FirstName,LastName,BirthDate,DepartmentId,PositionId,ShiftId,EmploymentStartDate) SELECT '83999999999',N'Synthetic',N'Invalid',DATEADD(year,1,GETDATE())," + _department + "," + _position + "," + _shift + ",DATEADD(year,2,GETDATE());")));
      int id = Employees.Save(NewEmployee());
      var employee = Employees.GetById(id); employee.NationalId = "-8700000000";
      Check("update_rejects_signed_id", Rejected(() => Employees.Update(employee)));
      Check("update_preserves_valid_id", EmployeeValidation.IsNationalIdValid(Employees.GetById(id).NationalId));
      Check("assigned_shift_deactivation_denied", Rejected(() => Shifts.Deactivate(_shift)));
      Check("assigned_shift_remains_active", Shifts.GetById(_shift).IsActive);
      Check("assigned_personnel_can_check_in", new AttendanceRepository().CheckIn(id, _admin) > 0);
      int free = Shifts.Save(new Shift { ShiftName = "Synthetic unused", StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(17), Monday = true });
      Check("unused_shift_can_deactivate", Shifts.Deactivate(free));
      Check("shift_deactivation_is_audited", Count("SELECT COUNT(*) FROM dbo.AuditLog WHERE TableName=N'Shift' AND Action=N'Deactivate' AND AffectedId=" + free) == 1);
      Check("repeated_deactivation_no_change", !Shifts.Deactivate(free));
      employee = NewEmployee(); employee.ShiftId = free;
      Check("inactive_shift_assignment_denied", Rejected(() => Employees.Save(employee)));
      var position = new Position { DepartmentId = _department, PositionName = "Synthetic audit position" };
      new PositionRepository().Save(position);
      position.PositionId = Count("SELECT MAX(PositionId) FROM dbo.Position");
      position.PositionName += " updated";
      new PositionRepository().Update(position);
      new PositionRepository().Deactivate(position.PositionId);
      Check("all_position_writes_audited", Count("SELECT COUNT(*) FROM dbo.AuditLog WHERE TableName=N'Position' AND AffectedId=" + position.PositionId) == 3);
      foreach (string language in new[] { "en", "tr" })
      {
        LanguageManager.SetLanguage(language, false);
        string message = "";
        try { Users.Save(new UserAccount { Username = "", FirstName = "Synthetic", LastName = "Test", Role = "Employee" }, "Synthetic123!", _admin); }
        catch (ArgumentException ex) { message = ex.Message; }
        Check("localized_username_validation_" + language, language == "en" ? message.Contains("required") && !message.Contains("zorunludur") : message.Contains("zorunludur"));
      }
    }

    private static void AuditFailureChecks()
    {
      Session(_admin, "Admin");
      int employeeId = Employees.Save(NewEmployee());
      int leave = new LeaveRepository().Save(new LeaveRequest { EmployeeId = employeeId, LeaveTypeId = _leaveType, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(10) });
      Execute(_database, "DENY INSERT ON dbo.AuditLog TO TestApplication;");
      try
      {
        int before = Count("SELECT COUNT(*) FROM dbo.Employee");
        Check("audit_failure_rejects_employee_save", Rejected(() => Employees.Save(NewEmployee())));
        Check("audit_failure_rolls_back_employee_save", Count("SELECT COUNT(*) FROM dbo.Employee") == before);
        var employee = Employees.GetById(employeeId); employee.FirstName = "Must roll back";
        Check("audit_failure_rejects_employee_update", Rejected(() => Employees.Update(employee)));
        Check("audit_failure_rolls_back_employee_update", Employees.GetById(employeeId).FirstName == "Synthetic");
        Check("audit_failure_rejects_attendance", Rejected(() => new AttendanceRepository().CheckIn(employeeId, _admin)));
        Check("audit_failure_rolls_back_attendance", Count("SELECT COUNT(*) FROM dbo.AttendanceRecord WHERE EmployeeId=" + employeeId) == 0);
        Check("audit_failure_rejects_leave_review", Rejected(() => new LeaveRepository().Approve(leave, _admin)));
        Check("audit_failure_rolls_back_leave_review", Count("SELECT COUNT(*) FROM dbo.LeaveRequest WHERE LeaveId=" + leave + " AND Status=N'Pending'") == 1);
        Check("audit_failure_rejects_role_update", Rejected(() => Users.UpdateRole(_staff, "Manager", _admin)));
        Check("audit_failure_rolls_back_role_update", Count("SELECT COUNT(*) FROM dbo.UserAccount WHERE UserId=" + _staff + " AND Role=N'Employee'") == 1);
        Check("audit_failure_rejects_sign_in", Rejected(() => Users.Authenticate("synthetic_Admin", "Synthetic123!")));
        Check("audit_failure_leaves_no_session", UserSession.CurrentUser == null);
        Check("audit_failure_rolls_back_login_time", Count("SELECT COUNT(*) FROM dbo.UserAccount WHERE UserId=" + _admin + " AND LastLoginAt IS NOT NULL") == 0);
      }
      finally { Execute(_database, "REVOKE INSERT ON dbo.AuditLog TO TestApplication;"); Session(_admin, "Admin"); }
      Check("sign_in_succeeds_after_audit_recovery", Users.Authenticate("synthetic_Admin", "Synthetic123!") != null);
      Check("legacy_password_hash_upgraded", Count("SELECT COUNT(*) FROM dbo.UserAccount WHERE UserId=" + _admin + " AND PasswordHash LIKE N'PBKDF2$%'") == 1);
    }

    private static void ConcurrentShiftChecks()
    {
      Session(_admin, "Admin");
      int shift = Shifts.Save(new Shift { ShiftName = "Synthetic concurrent", StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(17), Monday = true });
      using (var transaction = _database.BeginTransaction())
      {
        using (var command = new SqlCommand("SELECT IsActive FROM dbo.Shift WITH (HOLDLOCK) WHERE ShiftId=@Id", _database, transaction))
        {
          command.Parameters.AddWithValue("@Id", shift); command.ExecuteScalar();
        }
        var pending = Task.Run(() =>
        {
          try { return Rejected(() => Shifts.Deactivate(shift)); }
          finally { SqlDatabaseConnection.CloseConnection(); }
        });
        Check("shift_deactivation_waits_for_assignment", !pending.Wait(150));
        using (var command = new SqlCommand("INSERT dbo.Employee(NationalId,FirstName,LastName,DepartmentId,PositionId,ShiftId,EmploymentStartDate) VALUES('83888888888',N'Synthetic',N'Concurrent',@Department,@Position,@Shift,@Start)", _database, transaction))
        {
          command.Parameters.AddWithValue("@Department", _department); command.Parameters.AddWithValue("@Position", _position);
          command.Parameters.AddWithValue("@Shift", shift); command.Parameters.Add("@Start", SqlDbType.Date).Value = DateTime.Today;
          command.ExecuteNonQuery();
        }
        transaction.Commit();
        Check("concurrent_assignment_prevents_deactivation", pending.GetAwaiter().GetResult());
      }
      Check("concurrently_assigned_shift_stays_active", Shifts.GetById(shift).IsActive);
    }

    private static void RoleAndAsyncChecks()
    {
      foreach (var account in new[] { Tuple.Create(_admin, "Admin"), Tuple.Create(_manager, "Manager"), Tuple.Create(_staff, "Employee") })
      {
        Session(account.Item1, account.Item2);
        foreach (ApplicationPermission permission in Enum.GetValues(typeof(ApplicationPermission)))
        {
          bool expected = account.Item2 == "Admin" || (account.Item2 == "Manager" && permission != ApplicationPermission.ManageUsers) ||
            (account.Item2 == "Employee" && new[] { ApplicationPermission.ViewDashboard, ApplicationPermission.ViewPersonnel, ApplicationPermission.EditPersonnel,
              ApplicationPermission.ViewShifts, ApplicationPermission.RecordAttendance, ApplicationPermission.ViewLeaves, ApplicationPermission.CreateLeave }.Contains(permission));
          Check("role_" + account.Item2 + "_" + permission, OperationAuthorization.HasPermission(UserSession.CurrentUser, permission) == expected);
        }
        var all = Employees.GetAll();
        var lookup = Employees.GetLookupAsync().GetAwaiter().GetResult();
        Check("lookup_ids_and_names_match_" + account.Item2, all.Select(x => x.EmployeeId + ":" + x.FullName).SequenceEqual(lookup.Select(x => x.EmployeeId + ":" + x.FullName)));
        Check("async_personnel_matches_sync_" + account.Item2, all.Select(x => x.EmployeeId).SequenceEqual(Employees.GetAllAsync().GetAwaiter().GetResult().Select(x => x.EmployeeId)));
        Check("async_search_matches_sync_" + account.Item2, Employees.Search("Synthetic").Select(x => x.EmployeeId).SequenceEqual(Employees.SearchAsync("Synthetic").GetAwaiter().GetResult().Select(x => x.EmployeeId)));
        if (account.Item2 != "Admin") Check("direct_user_management_denied_" + account.Item2, Rejected(() => Users.GetAll()));
        if (account.Item2 == "Employee")
        {
          Check("async_report_denied_to_hr_staff", Rejected(() => new ReportRepository().GetEmployeeSummaryAsync(DateTime.Today, DateTime.Today).GetAwaiter().GetResult()));
          Check("direct_leave_review_denied_to_hr_staff", Rejected(() => new LeaveRepository().Approve(0, _staff)));
          Check("direct_shift_write_denied_to_hr_staff", Rejected(() => Shifts.Deactivate(_shift)));
        }
        else
        {
          var reports = new ReportRepository();
          var first = DateTime.Today.AddDays(-7); var last = DateTime.Today.AddDays(-1);
          Check("async_summary_matches_sync_" + account.Item2, reports.GetEmployeeSummary(first, last).Select(x => x.EmployeeId + ":" + x.TotalAbsences).SequenceEqual(reports.GetEmployeeSummaryAsync(first, last).GetAwaiter().GetResult().Select(x => x.EmployeeId + ":" + x.TotalAbsences)));
          Check("async_absence_matches_sync_" + account.Item2, reports.GetAbsenceReport(first, last).Rows.Count == reports.GetAbsenceReportAsync(first, last).GetAwaiter().GetResult().Rows.Count);
        }
      }
      Session(_admin, "Admin");
      Check("last_admin_cannot_be_demoted", Rejected(() => Users.UpdateRole(_admin, "Employee", _admin)));
      var attendance = new AttendanceRepository();
      Check("async_attendance_matches_sync", attendance.GetByDateRange(DateTime.Today, DateTime.Today).Count == attendance.GetByDateRangeAsync(DateTime.Today, DateTime.Today).GetAwaiter().GetResult().Count);
      var leaves = new LeaveRepository();
      Check("async_leave_matches_sync", leaves.GetByDateRange(null, null).Select(x => x.LeaveId).SequenceEqual(leaves.GetByDateRangeAsync(null, null).GetAwaiter().GetResult().Select(x => x.LeaveId)));
      using (var cancellation = new CancellationTokenSource())
      {
        cancellation.Cancel();
        bool cancelled = false;
        try { Employees.GetAllAsync(cancellationToken: cancellation.Token).GetAwaiter().GetResult(); }
        catch (OperationCanceledException) { cancelled = true; }
        Check("pre_cancelled_read_is_cancelled", cancelled);
      }
      using (var transaction = _database.BeginTransaction())
      using (var command = new SqlCommand("SELECT TOP(1) EmployeeId FROM dbo.Employee WITH (TABLOCKX,HOLDLOCK)", _database, transaction))
      using (var cancellation = new CancellationTokenSource())
      {
        command.ExecuteScalar();
        Task<List<Employee>> pending = Employees.GetAllAsync(cancellationToken: cancellation.Token);
        Check("locked_read_returns_pending_task", !pending.Wait(150));
        cancellation.Cancel();
        bool cancelled = false;
        try { pending.GetAwaiter().GetResult(); }
        catch (OperationCanceledException) { cancelled = true; }
        Check("inflight_read_can_be_cancelled", cancelled);
        transaction.Rollback();
      }
    }

    private static void PerformanceChecks()
    {
      Execute(_database, @"WITH Numbers AS (SELECT TOP(10000) ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS N FROM sys.all_objects a CROSS JOIN sys.all_objects b)
        INSERT dbo.Employee(NationalId,FirstName,LastName,Address,PhotoPath,DepartmentId,PositionId,ShiftId,EmploymentStartDate)
        SELECT CONVERT(char(11),84000000000+N),N'Synthetic',N'Bulk'+CONVERT(nvarchar(10),N),REPLICATE(N'A',450),REPLICATE(N'P',400)," + _department + "," + _position + "," + _shift + ",CONVERT(date,'20200101') FROM Numbers;");
      _application.StatisticsEnabled = true;
      Employees.GetAll(); Employees.GetLookup();
      var detailTimes = new List<double>(); var lookupTimes = new List<double>();
      long detailBytes = 0, lookupBytes = 0;
      for (int run = 0; run < 5; run++)
      {
        _application.ResetStatistics(); var watch = Stopwatch.StartNew(); var details = Employees.GetAll(); watch.Stop();
        detailTimes.Add(watch.Elapsed.TotalMilliseconds); detailBytes = Convert.ToInt64(_application.RetrieveStatistics()["BytesReceived"]);
        _application.ResetStatistics(); watch.Restart(); var lookup = Employees.GetLookup(); watch.Stop();
        lookupTimes.Add(watch.Elapsed.TotalMilliseconds); lookupBytes = Convert.ToInt64(_application.RetrieveStatistics()["BytesReceived"]);
        Check("bulk_lookup_retains_all_choices_" + run, details.Select(x => x.EmployeeId + ":" + x.FullName).SequenceEqual(lookup.Select(x => x.EmployeeId + ":" + x.FullName)));
      }
      Check("lookup_transfers_less_data", lookupBytes > 0 && lookupBytes < detailBytes / 2);
      Console.WriteLine("PERFORMANCE_ROWS=" + Count("SELECT COUNT(*) FROM dbo.Employee"));
      Console.WriteLine("DETAIL_MEDIAN_MS=" + detailTimes.OrderBy(x => x).ElementAt(2).ToString("F2", CultureInfo.InvariantCulture));
      Console.WriteLine("LOOKUP_MEDIAN_MS=" + lookupTimes.OrderBy(x => x).ElementAt(2).ToString("F2", CultureInfo.InvariantCulture));
      Console.WriteLine("DETAIL_BYTES=" + detailBytes);
      Console.WriteLine("LOOKUP_BYTES=" + lookupBytes);
      _application.StatisticsEnabled = false;
    }

    private static int RunBootstrap(string script, string database, bool reportFailure = false)
    {
      string assembly = typeof(UserRepository).Assembly.Location.Replace("'", "''");
      string command = "$ErrorActionPreference='Stop';$settings=[Data.SqlClient.SqlConnectionStringBuilder]::new($env:PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING);" +
        "$credential=[pscredential]::new($settings['User ID'],(ConvertTo-SecureString $settings['Password'] -AsPlainText -Force));" +
        "& '" + script.Replace("'", "''") + "' -Server $settings['Data Source'] -Database '" + database +
        "' -SqlCredential $credential -AdminPassword (ConvertTo-SecureString 'Synthetic123!' -AsPlainText -Force) -AssemblyPath '" + assembly +
        "' -TrustServerCertificate:([bool]$settings['TrustServerCertificate']);";
      var info = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "WindowsPowerShell", "v1.0", "powershell.exe"))
      {
        Arguments = "-NoProfile -NonInteractive -ExecutionPolicy RemoteSigned -EncodedCommand " + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(command)),
        UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true
      };
      info.EnvironmentVariables["PSModulePath"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "WindowsPowerShell", "v1.0", "Modules");
      using (var process = Process.Start(info))
      {
        var output = process.StandardOutput.ReadToEndAsync(); var errors = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(30000)) { process.Kill(); throw new InvalidOperationException("Bootstrap command timed out"); }
        Task.WaitAll(output, errors);
        if (process.ExitCode != 0 && reportFailure)
          Console.WriteLine("BOOTSTRAP_DIAGNOSTIC | " + errors.Result.Replace(_server.Password, "[redacted]"));
        return process.ExitCode;
      }
    }

    private static void BootstrapChecks(string source, string language, string database, string table, string hashColumn, string auditTable)
    {
      string script = Path.Combine(source, language, "InitializeAdmin.ps1");
      string initialScript = language == "English" ? Path.GetFullPath(Path.Combine(source, "..", "..", "tools", "CreateAdmin.ps1")) : script;
      Check("bootstrap_creates_first_admin_" + language, RunBootstrap(initialScript, database, true) == 0);
      using (var connection = Connect(database))
      {
        using (var command = new SqlCommand("SELECT " + hashColumn + " FROM dbo." + table, connection))
          Check("bootstrap_stores_valid_password_hash_" + language, PasswordHasher.Verify("Synthetic123!", Convert.ToString(command.ExecuteScalar())));
        using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo." + auditTable, connection))
          Check("bootstrap_is_audited_" + language, Convert.ToInt32(command.ExecuteScalar()) == 1);
        Check("bootstrap_refuses_populated_accounts_" + language, RunBootstrap(script, database) != 0);
        using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo." + table, connection))
          Check("bootstrap_preserves_existing_account_" + language, Convert.ToInt32(command.ExecuteScalar()) == 1);
      }
    }

    [STAThread]
    private static int Main(string[] args)
    {
      string initialLanguage = LanguageManager.Language;
      string previousOverride = Environment.GetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable);
      int exit = 0;
      try
      {
        _stage = "test configuration";
        string value = Environment.GetEnvironmentVariable("PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("Test admin connection is required");
        _server = new SqlConnectionStringBuilder(value);
        string host = _server.DataSource.Split('\\')[0].Split(',')[0];
        if (!new[] { Environment.MachineName, "localhost", "127.0.0.1", ".", "(local)" }.Contains(host, StringComparer.OrdinalIgnoreCase))
          throw new InvalidOperationException("Tests require a local SQL Server instance");
        if (_server.IntegratedSecurity) throw new InvalidOperationException("SQL Server Authentication is required");
        string source = Path.GetFullPath(args[0]);
        string english = "PersonnelManagementTest_" + Guid.NewGuid().ToString("N");
        _databases.Add(english);
        _stage = "English schema installation";
        using (var master = Connect("master")) Script(master, Path.Combine(source, "English", "DatabaseSetup.sql"), "PersonnelManagementDB", english);
        _database = Connect(english);
        Check("schema_only_is_empty", Count("SELECT COUNT(*) FROM dbo.UserAccount") == 0);
        Script(_database, Path.Combine(source, "English", "DatabaseUpdate.sql"), "PersonnelManagementDB", english);
        Script(_database, Path.Combine(source, "English", "DatabaseUpdate.sql"), "PersonnelManagementDB", english);
        Check("update_is_repeatable", Count("SELECT COUNT(*) FROM sys.indexes WHERE name IN (N'IX_Employee_NameLookup',N'IX_Employee_ActiveShift')") == 2);
        _login = "PersonnelManagementTest_" + Guid.NewGuid().ToString("N");
        string password = "Aa1!" + Guid.NewGuid().ToString("N");
        using (var master = Connect("master"))
        using (var command = new SqlCommand("DECLARE @Sql nvarchar(max)=N'CREATE LOGIN '+QUOTENAME(@Name)+N' WITH PASSWORD='+QUOTENAME(@Password,'''')+N',CHECK_POLICY=OFF'; EXEC sys.sp_executesql @Sql;", master))
        {
          command.Parameters.AddWithValue("@Name", _login); command.Parameters.AddWithValue("@Password", password); command.ExecuteNonQuery();
        }
        Execute(_database, "CREATE USER TestApplication FOR LOGIN [" + _login + "];GRANT SELECT,INSERT,UPDATE ON SCHEMA::dbo TO TestApplication;");
        var appSettings = new SqlConnectionStringBuilder(_server.ConnectionString) { InitialCatalog = english, UserID = _login, Password = password, IntegratedSecurity = false };
        Environment.SetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable, appSettings.ConnectionString);
        _application = new SqlConnection(appSettings.ConnectionString); _application.Open();
        typeof(SqlDatabaseConnection).GetField("_connection", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, _application);
        Fixtures(); ValidationChecks(); ConcurrentShiftChecks(); AuditFailureChecks(); RoleAndAsyncChecks();
        _checks += RoleChecks.Run(_database, _admin, _staff, _department, _position, _shift, _leaveType);
        Session(_admin, "Admin");
        ExtendedRegressionChecks(source);
        PerformanceChecks();
        _stage = "English bootstrap";
        string bootstrap = "PersonnelManagementTest_" + Guid.NewGuid().ToString("N"); _databases.Add(bootstrap);
        using (var master = Connect("master")) Script(master, Path.Combine(source, "English", "DatabaseSetup.sql"), "PersonnelManagementDB", bootstrap);
        BootstrapChecks(source, "English", bootstrap, "UserAccount", "PasswordHash", "AuditLog");
        _stage = "Turkish schema installation";
        string turkish = "PersonelYonetimiTest_" + Guid.NewGuid().ToString("N"); _databases.Add(turkish);
        using (var master = Connect("master")) Script(master, Path.Combine(source, "Turkish", "DatabaseSetup.sql"), "PersonelYonetimiDB", turkish);
        using (var database = Connect(turkish))
        {
          Script(database, Path.Combine(source, "Turkish", "DatabaseUpdate.sql"), "PersonelYonetimiDB", turkish);
          Script(database, Path.Combine(source, "Turkish", "DatabaseUpdate.sql"), "PersonelYonetimiDB", turkish);
          using (var command = new SqlCommand("SELECT COUNT(*) FROM sys.check_constraints WHERE name IN (N'CK_Personel_NationalIdDigits',N'CK_Personel_BirthDate')", database))
            Check("turkish_constraints_installed", Convert.ToInt32(command.ExecuteScalar()) == 2);
        }
        BootstrapChecks(source, "Turkish", turkish, "Kullanici", "Sifre", "SistemLog");
        Console.WriteLine("CHECKS_PASSED=" + _checks);
        Console.WriteLine("LIVE_DATABASE_WRITES=0");
      }
      catch (Exception ex)
      {
        Console.WriteLine("TEST_ERROR | stage=" + _stage + " | type=" + ex.GetType().Name + (ex is SqlException sql ? " | sql_number=" + sql.Number : ""));
        exit = 1;
      }
      finally
      {
        LanguageManager.SetLanguage(initialLanguage, false);
        Environment.SetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable, previousOverride);
        SqlDatabaseConnection.CloseConnection();
        _application?.Dispose(); _database?.Dispose();
        if (_databases.Count > 0) SqlConnection.ClearAllPools();
        if (_server != null)
        {
          bool cleanupSucceeded = true;
          foreach (string name in _databases)
          {
            try
            {
              if (!Regex.IsMatch(name, "^(PersonnelManagementTest|PersonelYonetimiTest)_[a-f0-9]{32}$")) throw new InvalidOperationException("Cleanup name guard");
              using (var master = Connect("master")) Execute(master, "IF DB_ID(N'" + name + "') IS NOT NULL BEGIN ALTER DATABASE [" + name + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + name + "]; END");
            }
            catch (Exception ex) { Console.WriteLine("CLEANUP_ERROR | " + ex.GetType().Name); cleanupSucceeded = false; exit = 1; }
          }
          if (_login != null)
          {
            try
            {
              if (!Regex.IsMatch(_login, "^PersonnelManagementTest_[a-f0-9]{32}$")) throw new InvalidOperationException("Login cleanup name guard");
              using (var master = Connect("master")) Execute(master, "IF SUSER_ID(N'" + _login + "') IS NOT NULL DROP LOGIN [" + _login + "];");
            }
            catch (Exception ex) { Console.WriteLine("LOGIN_CLEANUP_ERROR | " + ex.GetType().Name); cleanupSucceeded = false; exit = 1; }
          }
          foreach (string temporaryLogin in _additionalLogins)
          {
            try
            {
              if (!Regex.IsMatch(temporaryLogin, "^PersonnelManagementTest_[a-f0-9]{32}$")) throw new InvalidOperationException("Login cleanup name guard");
              using (var master = Connect("master")) Execute(master, "IF SUSER_ID(N'" + temporaryLogin + "') IS NOT NULL DROP LOGIN [" + temporaryLogin + "];");
            }
            catch (Exception ex) { Console.WriteLine("LOGIN_CLEANUP_ERROR | " + ex.GetType().Name); cleanupSucceeded = false; exit = 1; }
          }
          Console.WriteLine("SCRATCH_RESOURCES_REMOVED=" + (cleanupSucceeded ? "true" : "false"));
        }
      }
      return exit;
    }
  }
}
