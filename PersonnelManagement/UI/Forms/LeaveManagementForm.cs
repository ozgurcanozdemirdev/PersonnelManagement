using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.UI.Forms
{
  public partial class LeaveManagementForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.ViewLeaves;

    public LeaveManagementForm()
    {
      InitializeComponent();
    }

    private readonly LeaveRepository _leaveRepository = new LeaveRepository();
    private readonly LeaveTypeRepository _leaveTypeRepository = new LeaveTypeRepository();
    private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();
    private int _selectedLeaveId = 0;
    private void LeaveManagementFormLoad(object sender, EventArgs e)
    {
      LoadComboBoxes();
      dtpLeaveStartDate.Value = DateTime.Today;
      dtpLeaveEndDate.Value = DateTime.Today;
      dtpLeaveStartDate.ValueChanged += (s, ev) => CalculateTotalDays();
      dtpLeaveEndDate.ValueChanged += (s, ev) => CalculateTotalDays();
      CalculateTotalDays();
      LoadLeaves();
      ClearForm();
    }

    private void LoadComboBoxes()
    {
      var employees = _employeeRepository.GetLookup();
      cmbLeaveEmployee.DataSource = employees;
      cmbLeaveEmployee.DisplayMember = "FullName";
      cmbLeaveEmployee.ValueMember = "EmployeeId";
      cmbLeaveEmployee.SelectedIndex = -1;
      var leaveTypes = _leaveTypeRepository.GetAll();
      cmbLeaveType.DataSource = leaveTypes;
      cmbLeaveType.DisplayMember = "TypeName";
      cmbLeaveType.ValueMember = "LeaveTypeId";
      cmbLeaveType.SelectedIndex = -1;
      var filterList = new List<Employee>();
      filterList.Add(new Employee { EmployeeId = 0, FirstName = UiText.Get("All"), LastName = "" });
      filterList.AddRange(employees);
      cmbEmployeeFilter.DataSource = filterList;
      cmbEmployeeFilter.DisplayMember = "FullName";
      cmbEmployeeFilter.ValueMember = "EmployeeId";
      cmbEmployeeFilter.SelectedIndex = 0;
    }

    private void CmbLeaveEmployeeSelectedIndexChanged(object sender, EventArgs e)
    {
      var selected = cmbLeaveEmployee.SelectedItem as Employee;
      if (selected == null || selected.EmployeeId == 0)
      {
        UiText.BindComputed(lblRemainingLeave, () => UiText.Get("RemainingAnnualLeave"));
        lblRemainingLeave.ForeColor = Color.Black;
        return;
      }

      try
      {
        int remaining = _leaveRepository.GetRemainingAnnualLeave(selected.EmployeeId);
        UiText.Bind(lblRemainingLeave, "RemainingAnnualLeaveDays", remaining);
        lblRemainingLeave.ForeColor = remaining < 3 ? Color.Red : Color.Green;
      }
      catch
      {
        UiText.BindComputed(lblRemainingLeave, () => UiText.Get("RemainingAnnualLeave"));
      }
    }

    private void CalculateTotalDays()
    {
      if (dtpLeaveEndDate.Value.Date < dtpLeaveStartDate.Value.Date)
      {
        UiText.BindComputed(lblTotalDays, () => UiText.Get("EndDateCannotBeEarlierThanStartDate2"));
        lblTotalDays.ForeColor = Color.Red;
        return;
      }

      TimeSpan difference = dtpLeaveEndDate.Value.Date - dtpLeaveStartDate.Value.Date;
      int day = difference.Days + 1;
      UiText.Bind(lblTotalDays, "TotalDays", day);
      lblTotalDays.ForeColor = Color.DarkBlue;
    }

    private void LoadLeaves()
    {
      try
      {
        int? employeeId = null;
        var selected = cmbEmployeeFilter.SelectedItem as Employee;
        if (selected != null && selected.EmployeeId != 0)
          employeeId = selected.EmployeeId;
        List<LeaveRequest> list = employeeId.HasValue ? _leaveRepository.GetEmployeeLeaves(employeeId.Value) : _leaveRepository.GetByDateRange(null, null);
        dgvLeaves.DataSource = null;
        dgvLeaves.DataSource = list;
        ConfigureGridColumns();
        UiText.Bind(lblTotalLeave, "TotalRecords3", list.Count);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("LeaveRequestsCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ConfigureGridColumns()
    {
      string[] hiddenColumns =
      {
        "LeaveId",
        "EmployeeId",
        "LeaveTypeId",
        "Description",
        "ReviewedByUserId",
        "ReviewedAt",
        "RejectionReason",
        "CreatedAt"
      };
      foreach (string s in hiddenColumns)
        if (dgvLeaves.Columns[s] != null)
          dgvLeaves.Columns[s].Visible = false;
      if (dgvLeaves.Columns["EmployeeFullName"] != null)
      {
        UiText.BindHeader(dgvLeaves.Columns["EmployeeFullName"], "FullName");
        dgvLeaves.Columns["EmployeeFullName"].Width = 160;
      }

      if (dgvLeaves.Columns["LeaveTypeName"] != null)
      {
        UiText.BindHeader(dgvLeaves.Columns["LeaveTypeName"], "LeaveType");
        dgvLeaves.Columns["LeaveTypeName"].Width = 110;
      }

      if (dgvLeaves.Columns["StartDateText"] != null)
      {
        UiText.BindHeader(dgvLeaves.Columns["StartDateText"], "Start");
        dgvLeaves.Columns["StartDateText"].Width = 85;
      }

      if (dgvLeaves.Columns["EndDateText"] != null)
      {
        UiText.BindHeader(dgvLeaves.Columns["EndDateText"], "End");
        dgvLeaves.Columns["EndDateText"].Width = 85;
      }

      if (dgvLeaves.Columns["TotalDays"] != null)
      {
        UiText.BindHeader(dgvLeaves.Columns["TotalDays"], "Days");
        dgvLeaves.Columns["TotalDays"].Width = 45;
      }

      if (dgvLeaves.Columns["Status"] != null)
      {
        UiText.BindHeader(dgvLeaves.Columns["Status"], "Status");
        dgvLeaves.Columns["Status"].Width = 90;
      }

      foreach (DataGridViewRow row in dgvLeaves.Rows)
      {
        if (row.DataBoundItem is LeaveRequest leave)
        {
          switch (leave.Status)
          {
            case "Approved":
              row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
              row.DefaultCellStyle.ForeColor = Color.DarkGreen;
              break;
            case "Cancelled":
            case "Rejected":
              row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
              row.DefaultCellStyle.ForeColor = Color.DarkRed;
              break;
            case "Pending":
              row.DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 210);
              row.DefaultCellStyle.ForeColor = Color.DarkOrange;
              break;
          }
        }
      }
    }

    private void DgvLeavesCellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      if (dgvLeaves.Rows[e.RowIndex].DataBoundItem is LeaveRequest leave)
      {
        _selectedLeaveId = leave.LeaveId;
        // Event'leri geçici kapat — SelectedItem değişince LoadLeaves tetiklenmesin
        cmbLeaveEmployee.SelectedIndexChanged -= CmbLeaveEmployeeSelectedIndexChanged;
        cmbEmployeeFilter.SelectedIndexChanged -= CmbEmployeeFilterSelectedIndexChanged;
        foreach (Employee p in cmbLeaveEmployee.Items)
          if (p.EmployeeId == leave.EmployeeId)
          {
            cmbLeaveEmployee.SelectedItem = p;
            break;
          }

        foreach (LeaveType it in cmbLeaveType.Items)
          if (it.LeaveTypeId == leave.LeaveTypeId)
          {
            cmbLeaveType.SelectedItem = it;
            break;
          }

        // Event'leri geri aç
        cmbLeaveEmployee.SelectedIndexChanged += CmbLeaveEmployeeSelectedIndexChanged;
        cmbEmployeeFilter.SelectedIndexChanged += CmbEmployeeFilterSelectedIndexChanged;
        dtpLeaveStartDate.Value = leave.StartDate;
        dtpLeaveEndDate.Value = leave.EndDate;
        txtLeaveDescription.Text = leave.Description ?? "";
        // Kalan izni manuel hesapla
        CmbLeaveEmployeeSelectedIndexChanged(null, null);
        bool isPending = leave.Status == "Pending";
        bool canReview = OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ReviewLeave);
        btnSaveLeave.Enabled = false;
        btnApprove.Enabled = isPending && canReview;
        btnReject.Enabled = isPending && canReview;
        btnCancelLeave.Enabled = canReview && (isPending || leave.Status == "Approved");
        CalculateTotalDays();
      }
    }

    private void BtnSaveLeaveClick(object sender, EventArgs e)
    {
      if (!ValidateForm())
        return;
      try
      {
        var selectedEmployee = (Employee)cmbLeaveEmployee.SelectedItem;
        var selectedType = (LeaveType)cmbLeaveType.SelectedItem;
        var leave = new LeaveRequest
        {
          EmployeeId = selectedEmployee.EmployeeId,
          LeaveTypeId = selectedType.LeaveTypeId,
          StartDate = dtpLeaveStartDate.Value.Date,
          EndDate = dtpLeaveEndDate.Value.Date,
          Description = txtLeaveDescription.Text.Trim()
        };
        int newId = _leaveRepository.Save(leave);
        MessageBox.Show(UiText.Format("LeaveRequestSavedID", newId), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadLeaves();
        ClearForm();
      }
      catch (InvalidOperationException iex)
      {
        MessageBox.Show(iex.Message, UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("LeaveCouldNotBeSaved") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnApproveClick(object sender, EventArgs e)
    {
      if (_selectedLeaveId == 0)
        return;
      var approved = MessageBox.Show(UiText.Get("ApproveTheSelectedLeaveRequest"), UiText.Get("Approve"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (approved != DialogResult.Yes)
        return;
      try
      {
        bool ok = _leaveRepository.Approve(_selectedLeaveId, LoginForm.CurrentUser.UserId);
        if (ok)
        {
          MessageBox.Show(UiText.Get("LeaveRequestApproved"), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadLeaves();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("ApprovalFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnRejectClick(object sender, EventArgs e)
    {
      if (_selectedLeaveId == 0)
        return;
      string rejectionReason = null;
      using (Form frm = new Form())
      {
        UiText.BindComputed(frm, () => UiText.Get("RejectLeave"));
        frm.Width = 380;
        frm.Height = 150;
        frm.StartPosition = FormStartPosition.CenterParent;
        frm.FormBorderStyle = FormBorderStyle.FixedDialog;
        frm.MaximizeBox = false;
        frm.MinimizeBox = false;
        var lbl = new Label
        {
          Text = UiText.Get("EnterTheRejectionReason"),
          Location = new System.Drawing.Point(15, 15),
          Width = 340,
          Height = 20
        };
        var txt = new TextBox
        {
          Location = new System.Drawing.Point(15, 40),
          Width = 340,
          Height = 25
        };
        var btnOk = new Button
        {
          Text = UiText.Get("Reject"),
          Location = new System.Drawing.Point(185, 80),
          Width = 85,
          Height = 30,
          DialogResult = DialogResult.OK
        };
        var btnCancel = new Button
        {
          Text = UiText.Get("Cancel"),
          Location = new System.Drawing.Point(280, 80),
          Width = 75,
          Height = 30,
          DialogResult = DialogResult.Cancel
        };
        frm.Controls.Add(lbl);
        frm.Controls.Add(txt);
        frm.Controls.Add(btnOk);
        frm.Controls.Add(btnCancel);
        frm.AcceptButton = btnOk;
        frm.CancelButton = btnCancel;
        if (frm.ShowDialog() == DialogResult.OK)
          rejectionReason = txt.Text.Trim();
      }

      if (string.IsNullOrWhiteSpace(rejectionReason))
      {
        MessageBox.Show(UiText.Get("RejectionReasonIsRequired"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      try
      {
        bool ok = _leaveRepository.Reject(_selectedLeaveId, LoginForm.CurrentUser.UserId, rejectionReason);
        if (ok)
        {
          MessageBox.Show(UiText.Get("LeaveRequestRejected"), UiText.Get("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadLeaves();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("RejectionFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnCancelLeaveClick(object sender, EventArgs e)
    {
      if (_selectedLeaveId == 0)
        return;
      var approved = MessageBox.Show(UiText.Get("CancelTheSelectedLeaveRequest"), UiText.Get("ConfirmCancellation"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (approved != DialogResult.Yes)
        return;
      try
      {
        bool ok = _leaveRepository.Cancel(_selectedLeaveId, LoginForm.CurrentUser.UserId);
        if (ok)
        {
          MessageBox.Show(UiText.Get("LeaveRequestCancelled"), UiText.Get("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadLeaves();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("CancellationFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void CmbEmployeeFilterSelectedIndexChanged(object sender, EventArgs e)
    {
      LoadLeaves();
    }

    private void BtnAllLeavesClick(object sender, EventArgs e)
    {
      try
      {
        var list = new List<LeaveRequest>();
        list = _leaveRepository.GetByDateRange(null, null);
        dgvLeaves.DataSource = null;
        dgvLeaves.DataSource = list;
        ConfigureGridColumns();
        UiText.Bind(lblTotalLeave, "TotalRecords3", list.Count);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("Error2") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnAllPendingClick(object sender, EventArgs e)
    {
      try
      {
        var list = _leaveRepository.GetPendingLeaves();
        dgvLeaves.DataSource = null;
        dgvLeaves.DataSource = list;
        ConfigureGridColumns();
        lblTotalLeave.Text = $"Bekleyen: {list.Count} talep";
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("Error2") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnClearClick(object sender, EventArgs e)
    {
      ClearForm();
    }

    private void ClearForm()
    {
      _selectedLeaveId = 0;
      // Event'leri geçici kapat
      cmbLeaveEmployee.SelectedIndexChanged -= CmbLeaveEmployeeSelectedIndexChanged;
      cmbEmployeeFilter.SelectedIndexChanged -= CmbEmployeeFilterSelectedIndexChanged;
      cmbLeaveEmployee.SelectedIndex = -1;
      cmbLeaveType.SelectedIndex = -1;
      // Event'leri geri aç
      cmbLeaveEmployee.SelectedIndexChanged += CmbLeaveEmployeeSelectedIndexChanged;
      cmbEmployeeFilter.SelectedIndexChanged += CmbEmployeeFilterSelectedIndexChanged;
      dtpLeaveStartDate.Value = DateTime.Today;
      dtpLeaveEndDate.Value = DateTime.Today;
      txtLeaveDescription.Clear();
      UiText.BindComputed(lblRemainingLeave, () => UiText.Get("RemainingAnnualLeave"));
      lblRemainingLeave.ForeColor = Color.Black;
      CalculateTotalDays();
      btnSaveLeave.Enabled = true;
      btnApprove.Enabled = false;
      btnReject.Enabled = false;
      btnCancelLeave.Enabled = false;
    }

    private bool ValidateForm()
    {
      var selectedEmployee = cmbLeaveEmployee.SelectedItem as Employee;
      if (selectedEmployee == null || selectedEmployee.EmployeeId == 0)
      {
        MessageBox.Show(UiText.Get("SelectAnEmployee"), UiText.Get("MissingInformation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        cmbLeaveEmployee.Focus();
        return false;
      }

      if (cmbLeaveType.SelectedItem == null)
      {
        MessageBox.Show(UiText.Get("SelectALeaveType"), UiText.Get("MissingInformation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        cmbLeaveType.Focus();
        return false;
      }

      if (dtpLeaveEndDate.Value.Date < dtpLeaveStartDate.Value.Date)
      {
        MessageBox.Show(UiText.Get("EndDateCannotBeEarlierThanStartDate"), UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        dtpLeaveEndDate.Focus();
        return false;
      }

      return true;
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}