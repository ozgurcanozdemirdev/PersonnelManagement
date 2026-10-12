using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.UI.Forms
{
  public partial class AttendanceForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.RecordAttendance;

    public AttendanceForm()
    {
      InitializeComponent();
    }

    private readonly AttendanceRepository _attendanceRepository = new AttendanceRepository();
    private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();
    private Employee _selectedEmployee = null;
    private Timer _clockTimer;
    private void AttendanceFormLoad(object sender, EventArgs e)
    {
      if (components == null)
        components = new Container();
      _clockTimer?.Dispose();
      _clockTimer = new Timer(components);
      _clockTimer.Interval = 1000;
      _clockTimer.Tick += (s, ev) => lblCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
      _clockTimer.Start();
      lblCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
      btnCheckIn.Enabled = false;
      btnCheckOut.Enabled = false;
      ClearEmployeeDetails();
      LoadDailyRecords();
      txtEmployeeSearch.Focus();
    }

    private void BtnFindEmployeeClick(object sender, EventArgs e)
    {
      SearchEmployee();
    }

    private void TxtEmployeeSearchKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        SearchEmployee();
        e.SuppressKeyPress = true;
      }
    }

    private void SearchEmployee()
    {
      string searchedEmployee = txtEmployeeSearch.Text.Trim();
      if (string.IsNullOrWhiteSpace(searchedEmployee))
      {
        MessageBox.Show(UiText.Get("EnterAnEmployeeNameOrNationalID"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtEmployeeSearch.Focus();
        return;
      }

      try
      {
        var list = _employeeRepository.Search(searchedEmployee);
        if (list.Count == 0)
        {
          MessageBox.Show(UiText.Get("EmployeeNotFound"), UiText.Get("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          ClearEmployeeDetails();
          return;
        }

        if (list.Count > 1)
        {
          SelectEmployee(list);
          return;
        }

        LoadEmployee(list[0]);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("SearchFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void SelectEmployee(List<Employee> list)
    {
      using (var frm = new Form())
      {
        UiText.BindComputed(frm, () => UiText.Get("SelectEmployee2"));
        frm.Size = new Size(420, 340);
        frm.StartPosition = FormStartPosition.CenterParent;
        frm.FormBorderStyle = FormBorderStyle.FixedDialog;
        frm.MaximizeBox = false;
        frm.MinimizeBox = false;
        var dgv = new DataGridView
        {
          Dock = DockStyle.Fill,
          ReadOnly = true,
          SelectionMode = DataGridViewSelectionMode.FullRowSelect,
          MultiSelect = false,
          AllowUserToAddRows = false,
          DataSource = list
        };
        dgv.DataBindingComplete += (s, e) =>
        {
          foreach (DataGridViewColumn col in dgv.Columns)
            col.Visible = false;
          if (dgv.Columns["EmployeeId"] != null)
          {
            dgv.Columns["EmployeeId"].Visible = true;
            dgv.Columns["EmployeeId"].HeaderText = "Id";
            dgv.Columns["EmployeeId"].Width = 40;
          }

          if (dgv.Columns["FullName"] != null)
          {
            dgv.Columns["FullName"].Visible = true;
            UiText.BindHeader(dgv.Columns["FullName"], "FullName");
            dgv.Columns["FullName"].Width = 160;
          }

          if (dgv.Columns["DepartmentName"] != null)
          {
            dgv.Columns["DepartmentName"].Visible = true;
            UiText.BindHeader(dgv.Columns["DepartmentName"], "Department");
            dgv.Columns["DepartmentName"].Width = 130;
          }
        };
        var pnlFooter = new Panel
        {
          Dock = DockStyle.Bottom,
          Height = 45
        };
        var btnSelect = new Button
        {
          Text = UiText.Get("Select"),
          Size = new Size(100, 30),
          Location = new Point(150, 8),
          DialogResult = DialogResult.OK
        };
        pnlFooter.Controls.Add(btnSelect);
        frm.Controls.Add(dgv);
        frm.Controls.Add(pnlFooter);
        frm.AcceptButton = btnSelect;
        if (frm.ShowDialog() == DialogResult.OK && dgv.SelectedRows.Count > 0)
        {
          var selected = dgv.SelectedRows[0].DataBoundItem as Employee;
          if (selected != null)
            LoadEmployee(selected);
        }
      }
    }

    private void LoadEmployee(Employee p)
    {
      _selectedEmployee = p;
      lblEmployeeDetails.Text = $"{p.FullName}  (Id: {p.EmployeeId})";
      UiText.BindComputed(lblShiftDetails, () => UiText.Format("Shift2", p.ShiftName) + UiText.Format("CheckIn3", p.StartTime) + UiText.Format("CheckOut2", p.EndTime));
      lblDepartmentDetails.Text = $"Departman: {p.DepartmentName}  |  Pozisyon: {p.PositionName}";
      bool hasCheckIn = _attendanceRepository.HasTodayCheckIn(p.EmployeeId);
      bool openRecord = _attendanceRepository.HasOpenRecord(p.EmployeeId);
      bool hasCheckOut = hasCheckIn && !openRecord;
      btnCheckIn.Enabled = !hasCheckIn && !openRecord;
      btnCheckOut.Enabled = openRecord;
      if (!hasCheckIn && !openRecord)
      {
        UiText.BindComputed(lblStatusDetails, () => UiText.Get("NotCheckedIn"));
        lblStatusDetails.ForeColor = Color.Gray;
      }
      else if (openRecord)
      {
        UiText.BindComputed(lblStatusDetails, () => UiText.Get("AtWork"));
        lblStatusDetails.ForeColor = Color.Green;
      }
      else
      {
        UiText.BindComputed(lblStatusDetails, () => UiText.Get("CheckedOut"));
        lblStatusDetails.ForeColor = Color.OrangeRed;
      }
    }

    private void ClearEmployeeDetails()
    {
      _selectedEmployee = null;
      lblEmployeeDetails.Text = "—";
      lblShiftDetails.Text = "—";
      lblDepartmentDetails.Text = "—";
      lblStatusDetails.Text = "—";
      lblStatusDetails.ForeColor = Color.Gray;
      btnCheckIn.Enabled = false;
      btnCheckOut.Enabled = false;
    }

    private void BtnCheckInClick(object sender, EventArgs e)
    {
      if (_selectedEmployee == null)
        return;
      var approved = MessageBox.Show(UiText.Format("RecordCheckInFor", _selectedEmployee.FullName) + UiText.Format("TimeDoYouConfirm", DateTime.Now), UiText.Get("ConfirmCheckIn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (approved != DialogResult.Yes)
        return;
      try
      {
        _attendanceRepository.CheckIn(_selectedEmployee.EmployeeId, LoginForm.CurrentUser.UserId);
        MessageBox.Show(UiText.Format("CheckedInTime", _selectedEmployee.FullName, DateTime.Now), UiText.Get("CheckInSuccessful"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadEmployee(_selectedEmployee);
        LoadDailyRecords();
        txtEmployeeSearch.Clear();
        txtEmployeeSearch.Focus();
      }
      catch (InvalidOperationException iex)
      {
        MessageBox.Show(iex.Message, UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("CheckInFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnCheckOutClick(object sender, EventArgs e)
    {
      if (_selectedEmployee == null)
        return;
      var approved = MessageBox.Show(UiText.Format("RecordCheckOutFor", _selectedEmployee.FullName) + UiText.Format("TimeDoYouConfirm", DateTime.Now), UiText.Get("ConfirmCheckOut"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (approved != DialogResult.Yes)
        return;
      try
      {
        bool ok = _attendanceRepository.CheckOut(_selectedEmployee.EmployeeId, LoginForm.CurrentUser.UserId);
        if (ok)
        {
          MessageBox.Show(UiText.Format("CheckedOutTime", _selectedEmployee.FullName, DateTime.Now), UiText.Get("CheckOutSuccessful"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadEmployee(_selectedEmployee);
          LoadDailyRecords();
          txtEmployeeSearch.Clear();
          txtEmployeeSearch.Focus();
        }
      }
      catch (InvalidOperationException iex)
      {
        MessageBox.Show(iex.Message, UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("CheckOutFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void LoadDailyRecords()
    {
      try
      {
        var list = _attendanceRepository.GetTodayRecords();
        dgvDailyRecords.DataSource = null;
        dgvDailyRecords.DataSource = list;
        ConfigureGridColumns();
        UiText.Bind(lblTodayTotalCheckIns, "TodaySAndOpenRecordsDate", list.Count, DateTime.Today);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("DailyRecordsCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ConfigureGridColumns()
    {
      string[] hiddenColumns =
      {
        "AttendanceId",
        "EmployeeId",
        "CheckInAt",
        "CheckOutAt",
        "OvertimeMinutes",
        "Description",
        "AttendanceDate",
        "DepartmentName"
      };
      foreach (string s in hiddenColumns)
        if (dgvDailyRecords.Columns[s] != null)
          dgvDailyRecords.Columns[s].Visible = false;
      if (dgvDailyRecords.Columns["FullName"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["FullName"], "FullName");
        dgvDailyRecords.Columns["FullName"].Width = 160;
      }

      if (dgvDailyRecords.Columns["ShiftName"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["ShiftName"], "Shift");
        dgvDailyRecords.Columns["ShiftName"].Width = 120;
      }

      if (dgvDailyRecords.Columns["CheckInTimeText"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["CheckInTimeText"], "CheckIn");
        dgvDailyRecords.Columns["CheckInTimeText"].Width = 70;
      }

      if (dgvDailyRecords.Columns["CheckOutTimeText"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["CheckOutTimeText"], "Exit");
        dgvDailyRecords.Columns["CheckOutTimeText"].Width = 70;
      }

      if (dgvDailyRecords.Columns["WorkDurationText"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["WorkDurationText"], "WorkDuration");
        dgvDailyRecords.Columns["WorkDurationText"].Width = 100;
      }

      if (dgvDailyRecords.Columns["IsLate"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["IsLate"], "ArrivedLate");
        dgvDailyRecords.Columns["IsLate"].Width = 45;
      }

      if (dgvDailyRecords.Columns["LeftEarly"] != null)
      {
        UiText.BindHeader(dgvDailyRecords.Columns["LeftEarly"], "LeftEarly2");
        dgvDailyRecords.Columns["LeftEarly"].Width = 80;
      }

      foreach (DataGridViewRow row in dgvDailyRecords.Rows)
      {
        if (row.DataBoundItem is AttendanceRecord record)
        {
          if (record.IsLate)
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
          else if (!record.CheckOutTime.HasValue)
            row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
        }
      }
    }

    private void BtnRefreshClick(object sender, EventArgs e)
    {
      LoadDailyRecords();
      ClearEmployeeDetails();
      txtEmployeeSearch.Clear();
      txtEmployeeSearch.Focus();
    }

    private void AttendanceFormFormClosing(object sender, FormClosingEventArgs e)
    {
      _clockTimer?.Stop();
      _clockTimer?.Dispose();
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
