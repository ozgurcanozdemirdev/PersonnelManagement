using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Logging;
using PersonnelManagement.Models;
using PersonnelManagement.Security;
using PersonnelManagement.UI.Forms;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Tests
{
internal static class RoleChecks
{
  private static int checks;
  internal static void SetSession(UserAccount user)
  {
    typeof(LoginForm).GetProperty("CurrentUser", BindingFlags.Public | BindingFlags.Static).SetValue(null, user);
    AuditLogger.SetCurrentUser(user?.UserId);
  }
  private static void Check(string name, bool value)
  {
    Console.WriteLine((value ? "PASS" : "FAIL") + " | role_" + name);
    if (!value) throw new InvalidOperationException("role_" + name);
    checks++;
  }
  private static bool Denied(Action operation)
  {
    try { operation(); return false; }
    catch (UnauthorizedAccessException) { return true; }
    catch (TargetInvocationException ex) when (ex.InnerException is UnauthorizedAccessException) { return true; }
  }
  private static int Scalar(SqlConnection db, string sql)
  { using (var cmd = new SqlCommand(sql, db)) return Convert.ToInt32(cmd.ExecuteScalar()); }
  private static void Exec(SqlConnection db, string sql)
  { using (var cmd = new SqlCommand(sql, db)) cmd.ExecuteNonQuery(); }
  private static UserAccount Account(int id, string role) => new UserAccount { UserId=id, Role=role, IsActive=true, Username="fixture_"+role, FirstName="Synthetic", LastName=role };
  private static Control Find(Control form, string name) => form.Controls.Find(name,true).Single();
  private static void Load(LocalizedForm form)
  {
    form.CreateControl();
    typeof(LocalizedForm).GetMethod("OnLoad",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(form,new object[]{EventArgs.Empty});
    var timeout=System.Diagnostics.Stopwatch.StartNew();
    while (timeout.ElapsedMilliseconds<10000)
    {
      bool loaded=true;
      if(form is EmployeeManagementForm)
        loaded=form.GetType().GetField("_employeeLoadCancellation",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form)==null;
      if(form is ReportsForm)
        loaded=((ComboBox)Find(form,"cmbReportEmployee")).Items.Count>1;
      if(loaded)return;
      Application.DoEvents();
      System.Threading.Thread.Sleep(10);
    }
    throw new InvalidOperationException("Form data did not finish loading");
  }
  private static void SelectRow(LocalizedForm form,string method,string grid)
  {
    var rows=(DataGridView)Find(form,grid);
    Check(method+"_fixture_row",rows.Rows.Count>0);
    form.GetType().GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(form,new object[]{rows,new DataGridViewCellEventArgs(0,0)});
  }
  private static Employee NewEmployee(int sequence,int department,int position,int shift,int allowance=14) => new Employee {
    NationalId=(81000000000L+sequence).ToString(), FirstName="HR",LastName="Fixture"+sequence,
    DepartmentId=department,PositionId=position,ShiftId=shift,EmploymentStartDate=DateTime.Today.AddYears(-1),AnnualLeaveAllowance=allowance };
  internal static int Run(SqlConnection db,int admin,int worker,int department,int position,int shift,int leaveType)
  {
    string initialLanguage=LanguageManager.Language;
    foreach(string role in new[]{"Admin","Manager","Employee"})
      foreach(ApplicationPermission permission in Enum.GetValues(typeof(ApplicationPermission)))
      {
        bool expected=role=="Admin" || (role=="Manager" && permission!=ApplicationPermission.ManageUsers) || (role=="Employee" && new[]{ApplicationPermission.ViewDashboard,ApplicationPermission.ViewPersonnel,ApplicationPermission.EditPersonnel,ApplicationPermission.ViewShifts,ApplicationPermission.RecordAttendance,ApplicationPermission.ViewLeaves,ApplicationPermission.CreateLeave}.Contains(permission));
        Check(role+"_"+permission,OperationAuthorization.HasPermission(Account(1,role),permission)==expected);
      }
    Check("unknown_permission",!OperationAuthorization.HasPermission(Account(1,"Admin"),(ApplicationPermission)999));
    Check("unknown_role",!OperationAuthorization.HasPermission(Account(1,"User"),ApplicationPermission.ViewPersonnel));
    var inactive=Account(1,"Admin");inactive.IsActive=false;
    Check("inactive_policy",!OperationAuthorization.HasPermission(inactive,ApplicationPermission.ViewPersonnel));
    Check("null_policy",!OperationAuthorization.HasPermission(null,ApplicationPermission.ViewPersonnel));

    SetSession(Account(admin,"Admin"));
    using(var main=new MainForm())
    {
      Load(main);
      Check("admin_all_menus_enabled",new[]{"btnEmployeeManagement","btnShiftManagement","btnAttendance","btnLeaveManagement","btnReports","btnUsers"}.All(n=>Find(main,n).Enabled));
    }
    var users=new UserRepository();
    Check("assignable_roles",users.GetRoles().SequenceEqual(new[]{"Admin","Manager","Employee"}));
    Check("admin_creates_manager",users.Save(new UserAccount{Username="role_manager",FirstName="HR",LastName="Manager",Role="Manager"},"ManagerFixture123!",admin));
    int manager=Scalar(db,"SELECT UserId FROM dbo.UserAccount WHERE Username=N'role_manager'");
    Check("manager_authenticate",users.Authenticate("role_manager","ManagerFixture123!")?.Role=="Manager");
    Check("authentication_sets_session",UserSession.CurrentUser?.UserId==manager);
    Check("invalid_authentication_clears_session",users.Authenticate("role_manager","incorrect")==null&&UserSession.CurrentUser==null);
    SetSession(Account(admin,"Admin"));
    bool protectedAdmin=false;
    try{users.UpdateRole(admin,"Manager",admin);}catch(InvalidOperationException){protectedAdmin=true;}
    Check("last_admin_kept",protectedAdmin && Scalar(db,"SELECT COUNT(*) FROM dbo.UserAccount WHERE Role=N'Admin' AND IsActive=1")==1);

    SetSession(Account(worker,"Employee"));
    var employees=new EmployeeRepository(); var leaves=new LeaveRepository();var shifts=new ShiftRepository();var attendance=new AttendanceRepository();var reports=new ReportRepository();
    int staffEmployee=employees.Save(NewEmployee(1,department,position,shift));
    Check("staff_create_employee",staffEmployee>0);
    var record=employees.GetById(staffEmployee);record.Phone="0000000000";
    Check("staff_update_employee",employees.Update(record)&&employees.GetById(staffEmployee).Phone=="0000000000");
    int personnelCount=Scalar(db,"SELECT COUNT(*) FROM dbo.Employee");
    Check("staff_custom_allowance_create_denied",Denied(()=>employees.Save(NewEmployee(2,department,position,shift,30)))&&personnelCount==Scalar(db,"SELECT COUNT(*) FROM dbo.Employee"));
    record.AnnualLeaveAllowance=30;
    Check("staff_allowance_update_denied",Denied(()=>employees.Update(record))&&employees.GetById(staffEmployee).AnnualLeaveAllowance==14);
    Check("staff_deactivate_denied",Denied(()=>employees.Deactivate(staffEmployee,DateTime.Today))&&employees.GetById(staffEmployee).IsActive);
    Check("staff_shift_view",shifts.GetAll().Count>0&&shifts.GetById(shift)!=null);
    var newShift=new Shift{ShiftName="Role fixture",StartTime=TimeSpan.FromHours(8),EndTime=TimeSpan.FromHours(17),BreakMinutes=60,Monday=true};
    Check("staff_shift_create_denied",Denied(()=>shifts.Save(newShift)));
    Check("staff_shift_update_denied",Denied(()=>shifts.Update(shifts.GetById(shift))));
    Check("staff_shift_deactivate_denied",Denied(()=>shifts.Deactivate(shift)));
    Check("staff_department_create_denied",Denied(()=>new DepartmentRepository().Save(new Department{DepartmentName="Denied"})));
    Check("staff_department_update_denied",Denied(()=>new DepartmentRepository().Update(new Department{DepartmentId=department,DepartmentName="Denied"})));
    Check("staff_department_deactivate_denied",Denied(()=>new DepartmentRepository().Deactivate(department)));
    Check("staff_position_create_denied",Denied(()=>new PositionRepository().Save(new Position{DepartmentId=department,PositionName="Denied"})));
    Check("staff_position_update_denied",Denied(()=>new PositionRepository().Update(new Position{PositionId=position,DepartmentId=department,PositionName="Denied"})));
    Check("staff_position_deactivate_denied",Denied(()=>new PositionRepository().Deactivate(position)));
    Check("staff_dashboard",reports.GetTodaySummary().TotalEmployees>0);
    Check("staff_reports_denied",Denied(()=>reports.GetEmployeeSummary(DateTime.Today,DateTime.Today)));
    Check("staff_absence_report_denied",Denied(()=>reports.GetAbsenceReport(DateTime.Today,DateTime.Today)));
    Check("staff_attendance_actor_spoof_denied",Denied(()=>attendance.CheckIn(staffEmployee,admin)));
    Check("staff_attendance_create",attendance.CheckIn(staffEmployee)>0);
    Check("staff_attendance_close",attendance.CheckOut(staffEmployee));
    Check("staff_attendance_attribution",Scalar(db,"SELECT RecordedByUserId FROM dbo.AttendanceRecord WHERE EmployeeId="+staffEmployee)==worker);
    DateTime leaveDate=DateTime.Today.AddYears(3);
    int leave=leaves.Save(new LeaveRequest{EmployeeId=staffEmployee,LeaveTypeId=leaveType,StartDate=leaveDate,EndDate=leaveDate});
    Check("staff_leave_create",leave>0);
    Check("staff_leave_view",leaves.GetEmployeeLeaves(staffEmployee).Any(x=>x.LeaveId==leave)&&leaves.GetRemainingAnnualLeave(staffEmployee)>0);
    Check("staff_leave_approve_denied",Denied(()=>leaves.Approve(leave,worker)));
    Check("staff_leave_approve_actor_spoof_denied",Denied(()=>leaves.Approve(leave,admin)));
    Check("staff_leave_reject_denied",Denied(()=>leaves.Reject(leave,worker,"Denied")));
    Check("staff_leave_cancel_denied",Denied(()=>leaves.Cancel(leave,worker)));
    Check("staff_user_list_denied",Denied(()=>users.GetAll()));
    Check("staff_other_user_details_denied",Denied(()=>users.GetById(admin)));
    Check("staff_own_profile",users.GetById(worker)?.UserId==worker);
    var forbiddenUser=new UserAccount{Username="role_denied",FirstName="HR",LastName="Denied",Role="Employee"};
    Check("staff_user_create_denied",Denied(()=>users.Save(forbiddenUser,"FixturePass123!",worker)));
    Check("staff_user_create_actor_spoof_denied",Denied(()=>users.Save(forbiddenUser,"FixturePass123!",admin)));
    Check("staff_role_change_denied",Denied(()=>users.UpdateRole(worker,"Admin",admin)));
    Check("staff_password_change_denied",Denied(()=>users.ChangePassword(admin,"FixturePass123!",admin)));
    UserSession.CurrentUser.Role="Admin";
    Check("cached_role_escalation_denied",Denied(()=>reports.GetEmployeeSummary(DateTime.Today,DateTime.Today))&&UserSession.CurrentUser.Role=="Employee");
    UserSession.CurrentUser.UserId=admin;
    Check("cached_identity_escalation_denied",Denied(()=>reports.GetEmployeeSummary(DateTime.Today,DateTime.Today))&&UserSession.CurrentUser.UserId==worker);

    using(var main=new MainForm())using(var staff=new EmployeeManagementForm())using(var shiftForm=new ShiftManagementForm())using(var reportForm=new ReportsForm())using(var userForm=new UserManagementForm())
    {
      Load(main);Load(staff);Load(shiftForm);
      Check("staff_report_menu_disabled",!Find(main,"btnReports").Enabled);
      Check("staff_user_menu_disabled",!Find(main,"btnUsers").Enabled);
      Check("staff_ordinary_menus",new[]{"btnEmployeeManagement","btnShiftManagement","btnAttendance","btnLeaveManagement"}.All(n=>Find(main,n).Enabled));
      Check("staff_allowance_read_only",((TextBox)Find(staff,"txtAnnualLeaveAllowance")).ReadOnly);
      SelectRow(staff,"DgvEmployeesCellClick","dgvEmployees");
      Check("staff_selected_personnel_permissions",Find(staff,"btnUpdate").Enabled&&!Find(staff,"btnDeactivate").Enabled);
      Check("staff_shift_create_ui_denied",!Find(shiftForm,"btnSaveShift").Enabled&&!Find(shiftForm,"dtpStartTime").Enabled);
      SelectRow(shiftForm,"DgvShiftsCellClick","dgvShifts");
      Check("staff_selected_shift_permissions",!Find(shiftForm,"btnUpdateShift").Enabled&&!Find(shiftForm,"btnDeactivateShift").Enabled);
      Check("staff_report_direct_load_denied",Denied(()=>Load(reportForm)));
      Check("staff_user_direct_load_denied",Denied(()=>Load(userForm)));
    }
    SetSession(Account(manager,"Manager"));
    Check("manager_personnel_custom_allowance",employees.Save(NewEmployee(3,department,position,shift,30))>0);
    record=employees.GetById(staffEmployee);record.AnnualLeaveAllowance=25;
    Check("manager_allowance_update",employees.Update(record)&&employees.GetById(staffEmployee).AnnualLeaveAllowance==25);
    int createdShift=shifts.Save(newShift);newShift.ShiftId=createdShift;newShift.ShiftName="Role updated";
    Check("manager_shift_create_update",createdShift>0&&shifts.Update(newShift));
    Check("manager_shift_deactivate",shifts.Deactivate(createdShift));
    Check("manager_leave_approve",leaves.Approve(leave,manager));
    Check("manager_leave_approve_attribution",Scalar(db,"SELECT ReviewedByUserId FROM dbo.LeaveRequest WHERE LeaveId="+leave)==manager);
    Check("manager_leave_cancel",leaves.Cancel(leave,manager));
    int rejected=leaves.Save(new LeaveRequest{EmployeeId=staffEmployee,LeaveTypeId=leaveType,StartDate=leaveDate.AddDays(2),EndDate=leaveDate.AddDays(2)});
    Check("manager_leave_reject",leaves.Reject(rejected,manager,"Synthetic reason"));
    Check("manager_report_read",reports.GetEmployeeSummary(DateTime.Today,DateTime.Today).Count>0&&reports.GetAbsenceReport(DateTime.Today,DateTime.Today)!=null);
    Check("manager_user_list_denied",Denied(()=>users.GetAll()));
    Check("manager_user_create_denied",Denied(()=>users.Save(forbiddenUser,"FixturePass123!",manager)));
    Check("manager_role_change_denied",Denied(()=>users.UpdateRole(worker,"Manager",manager)));
    Check("manager_password_change_denied",Denied(()=>users.ChangePassword(worker,"FixturePass123!",manager)));
    using(var main=new MainForm())using(var reportForm=new ReportsForm())using(var userForm=new UserManagementForm())using(var leaveForm=new LeaveManagementForm())
    {
      Load(main);Load(reportForm);Load(leaveForm);
      Check("manager_reports_menu_enabled",Find(main,"btnReports").Enabled&&!Find(main,"btnUsers").Enabled);
      Check("manager_report_load_allowed",true);
      Check("manager_user_direct_load_denied",Denied(()=>Load(userForm)));
    }
    Exec(db,"UPDATE dbo.UserAccount SET Role=N'Employee' WHERE UserId="+manager);
    Check("database_role_revocation_immediate",Denied(()=>reports.GetEmployeeSummary(DateTime.Today,DateTime.Today))&&UserSession.CurrentUser.Role=="Employee");
    Exec(db,"UPDATE dbo.UserAccount SET Role=N'Manager',IsActive=0 WHERE UserId="+manager);
    Check("database_account_deactivation_immediate",Denied(()=>employees.GetAll()));
    Exec(db,"UPDATE dbo.UserAccount SET IsActive=1 WHERE UserId="+manager);
    SetSession(Account(worker,"Employee"));record=employees.GetById(staffEmployee);record.Email="fixture@example.invalid";
    Check("staff_preserves_manager_allowance",employees.Update(record)&&employees.GetById(staffEmployee).AnnualLeaveAllowance==25);
    SetSession(Account(manager,"Manager"));
    Check("manager_personnel_deactivation",employees.Deactivate(staffEmployee,DateTime.Today)&&!employees.GetById(staffEmployee).IsActive);
    Exec(db,"UPDATE dbo.UserAccount SET Role=N'User' WHERE UserId="+worker);
    SetSession(Account(worker,"Employee"));
    Check("legacy_role_operations_denied",Denied(()=>employees.GetAll()));
    Exec(db,"UPDATE dbo.UserAccount SET Role=N'Employee' WHERE UserId="+worker);
    SetSession(Account(999999,"Admin"));Check("missing_account_denied",Denied(()=>employees.GetAll()));
    SetSession(null);
    Check("anonymous_employee_read_denied",Denied(()=>employees.GetAll()));
    Check("anonymous_employee_write_denied",Denied(()=>employees.Save(NewEmployee(4,department,position,shift))));
    Check("anonymous_shift_read_denied",Denied(()=>shifts.GetById(shift)));
    Check("anonymous_attendance_read_denied",Denied(()=>attendance.HasTodayCheckIn(staffEmployee)));
    Check("anonymous_leave_read_denied",Denied(()=>leaves.GetPendingLeaves()));
    Check("anonymous_leave_allowance_denied",Denied(()=>leaves.GetRemainingAnnualLeave(staffEmployee)));
    Check("anonymous_leave_types_denied",Denied(()=>new LeaveTypeRepository().GetAll()));
    Check("anonymous_departments_denied",Denied(()=>new DepartmentRepository().GetAll()));
    Check("anonymous_positions_denied",Denied(()=>new PositionRepository().GetAll()));
    Check("anonymous_dashboard_denied",Denied(()=>reports.GetTodaySummary()));
    Check("anonymous_user_read_denied",Denied(()=>users.GetById(admin)));
    LanguageManager.SetLanguage("tr",false);
    Check("turkish_role_labels",UiText.DisplayValue("Admin")=="Sistem Yöneticisi"&&UiText.DisplayValue("Manager")=="İK Yöneticisi"&&UiText.DisplayValue("Employee")=="İK Çalışanı");
    LanguageManager.SetLanguage("en",false);
    Check("english_role_labels",UiText.DisplayValue("Admin")=="Administrator"&&UiText.DisplayValue("Manager")=="HR Manager"&&UiText.DisplayValue("Employee")=="HR Employee");
    LanguageManager.SetLanguage(initialLanguage,false);SetSession(Account(admin,"Admin"));
    Console.WriteLine("ROLE_CHECKS="+checks);
    return checks;
  }
}
}
