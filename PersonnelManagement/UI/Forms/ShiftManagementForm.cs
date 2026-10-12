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
  public partial class ShiftManagementForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.ViewShifts;

    public ShiftManagementForm()
    {
      InitializeComponent();
    }

    private readonly ShiftRepository _shiftRepository = new ShiftRepository();
    private int _selectedShiftId = 0;
    private void ShiftManagementFormLoad(object sender, EventArgs e)
    {
      dtpStartTime.Format = DateTimePickerFormat.Custom;
      dtpStartTime.CustomFormat = "HH:mm";
      dtpStartTime.ShowUpDown = true;
      dtpEndTime.Format = DateTimePickerFormat.Custom;
      dtpEndTime.CustomFormat = "HH:mm";
      dtpEndTime.ShowUpDown = true;
      dtpStartTime.Value = DateTime.Today.AddHours(8);
      dtpEndTime.Value = DateTime.Today.AddHours(17);
      LoadShifts();
      ClearForm();
    }

    private void LoadShifts()
    {
      try
      {
        var list = _shiftRepository.GetAll(activeOnly: false);
        dgvShifts.DataSource = null;
        dgvShifts.DataSource = list;
        ConfigureGridColumns();
        UiText.Bind(lblTotalRecords, "TotalShifts", list.Count);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("ShiftsCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ConfigureGridColumns()
    {
      if (dgvShifts.Columns["IsActive"] != null)
        dgvShifts.Columns["IsActive"].Visible = false;
      if (dgvShifts.Columns["ShiftId"] != null)
      {
        dgvShifts.Columns["ShiftId"].HeaderText = "ID";
        dgvShifts.Columns["ShiftId"].Width = 35;
      }

      if (dgvShifts.Columns["ShiftName"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["ShiftName"], "ShiftName");
        dgvShifts.Columns["ShiftName"].Width = 150;
      }

      if (dgvShifts.Columns["StartTime"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["StartTime"], "Start");
        dgvShifts.Columns["StartTime"].Width = 75;
      }

      if (dgvShifts.Columns["EndTime"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["EndTime"], "End");
        dgvShifts.Columns["EndTime"].Width = 75;
      }

      if (dgvShifts.Columns["BreakMinutes"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["BreakMinutes"], "BreakMin");
        dgvShifts.Columns["BreakMinutes"].Width = 70;
      }

      if (dgvShifts.Columns["Monday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Monday"], "Mon");
        dgvShifts.Columns["Monday"].Width = 38;
      }

      if (dgvShifts.Columns["Tuesday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Tuesday"], "Tue");
        dgvShifts.Columns["Tuesday"].Width = 38;
      }

      if (dgvShifts.Columns["Wednesday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Wednesday"], "Wed");
        dgvShifts.Columns["Wednesday"].Width = 38;
      }

      if (dgvShifts.Columns["Thursday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Thursday"], "Thu");
        dgvShifts.Columns["Thursday"].Width = 38;
      }

      if (dgvShifts.Columns["Friday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Friday"], "Fri");
        dgvShifts.Columns["Friday"].Width = 38;
      }

      if (dgvShifts.Columns["Saturday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Saturday"], "Sat");
        dgvShifts.Columns["Saturday"].Width = 38;
      }

      if (dgvShifts.Columns["Sunday"] != null)
      {
        UiText.BindHeader(dgvShifts.Columns["Sunday"], "Sun");
        dgvShifts.Columns["Sunday"].Width = 38;
      }
    }

    private void DgvShiftsCellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      try
      {
        int shiftId = Convert.ToInt32(dgvShifts.Rows[e.RowIndex].Cells["ShiftId"].Value);
        Shift v = _shiftRepository.GetById(shiftId);
        if (v == null)
          return;
        _selectedShiftId = v.ShiftId;
        txtShiftName.Text = v.ShiftName;
        txtBreakMinutes.Text = v.BreakMinutes.ToString();
        dtpStartTime.Value = DateTime.Today.Add(v.StartTime);
        dtpEndTime.Value = DateTime.Today.Add(v.EndTime);
        chkMonday.Checked = v.Monday;
        chkTuesday.Checked = v.Tuesday;
        chkWednesday.Checked = v.Wednesday;
        chkThursday.Checked = v.Thursday;
        chkFriday.Checked = v.Friday;
        chkSaturday.Checked = v.Saturday;
        chkSunday.Checked = v.Sunday;
        btnSaveShift.Enabled = false;
        btnUpdateShift.Enabled = OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ManageShifts);
        btnDeactivateShift.Enabled = OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ManageShifts);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("RecordCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnSaveShiftClick(object sender, EventArgs e)
    {
      if (!ValidateForm())
        return;
      try
      {
        int newId = _shiftRepository.Save(ReadShiftFromForm());
        MessageBox.Show(UiText.Format("ShiftSavedID", newId), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadShifts();
        ClearForm();
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("SaveFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnUpdateShiftClick(object sender, EventArgs e)
    {
      if (_selectedShiftId == 0)
      {
        MessageBox.Show(UiText.Get("SelectAShiftToUpdate"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (!ValidateForm())
        return;
      try
      {
        Shift v = ReadShiftFromForm();
        v.ShiftId = _selectedShiftId;
        bool ok = _shiftRepository.Update(v);
        if (ok)
        {
          MessageBox.Show(UiText.Get("ShiftUpdated"), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadShifts();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("UpdateFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnDeactivateShiftClick(object sender, EventArgs e)
    {
      if (_selectedShiftId == 0)
      {
        MessageBox.Show(UiText.Get("SelectAShiftToDeactivate"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var approved = MessageBox.Show(UiText.Format("DeactivateShift", txtShiftName.Text), UiText.Get("ConfirmDeactivation"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (approved != DialogResult.Yes)
        return;
      try
      {
        bool ok = _shiftRepository.Deactivate(_selectedShiftId);
        if (ok)
        {
          MessageBox.Show(UiText.Get("ShiftDeactivated"), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadShifts();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("DeactivationFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnAllWeekClick(object sender, EventArgs e)
    {
      chkMonday.Checked = chkTuesday.Checked = chkWednesday.Checked = chkThursday.Checked = chkFriday.Checked = chkSaturday.Checked = chkSunday.Checked = true;
    }

    private void BtnWeekdaysClick(object sender, EventArgs e)
    {
      chkMonday.Checked = chkTuesday.Checked = chkWednesday.Checked = chkThursday.Checked = chkFriday.Checked = true;
      chkSaturday.Checked = chkSunday.Checked = false;
    }

    private void BtnWeekendClick(object sender, EventArgs e)
    {
      chkMonday.Checked = chkTuesday.Checked = chkWednesday.Checked = chkThursday.Checked = chkFriday.Checked = false;
      chkSaturday.Checked = chkSunday.Checked = true;
    }

    private void BtnSelectNoneClick(object sender, EventArgs e)
    {
      chkMonday.Checked = chkTuesday.Checked = chkWednesday.Checked = chkThursday.Checked = chkFriday.Checked = chkSaturday.Checked = chkSunday.Checked = false;
    }

    private void BtnClearClick(object sender, EventArgs e)
    {
      ClearForm();
    }

    private void ClearForm()
    {
      _selectedShiftId = 0;
      txtShiftName.Clear();
      txtBreakMinutes.Text = "0";
      dtpStartTime.Value = DateTime.Today.AddHours(8);
      dtpEndTime.Value = DateTime.Today.AddHours(17);
      chkMonday.Checked = chkTuesday.Checked = chkWednesday.Checked = chkThursday.Checked = chkFriday.Checked = true;
      chkSaturday.Checked = chkSunday.Checked = false;
      bool canManage = OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ManageShifts);
      txtShiftName.ReadOnly = txtBreakMinutes.ReadOnly = !canManage;
      dtpStartTime.Enabled = dtpEndTime.Enabled = groupBox1.Enabled = canManage;
      btnAllWeek.Enabled = btnWeekdays.Enabled = btnWeekend.Enabled = btnSelectNone.Enabled = canManage;
      btnSaveShift.Enabled = canManage;
      btnUpdateShift.Enabled = false;
      btnDeactivateShift.Enabled = false;
      txtShiftName.Focus();
    }

    private bool ValidateForm()
    {
      if (string.IsNullOrWhiteSpace(txtShiftName.Text))
      {
        MessageBox.Show(UiText.Get("ShiftNameIsRequired"), UiText.Get("MissingInformation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtShiftName.Focus();
        return false;
      }

      if (!int.TryParse(txtBreakMinutes.Text, out int breakMinutes) || breakMinutes < 0)
      {
        MessageBox.Show(UiText.Get("EnterAValidBreakDurationInMinutes"), UiText.Get("MissingInformation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtBreakMinutes.Focus();
        return false;
      }

      var start = dtpStartTime.Value.TimeOfDay;
      var end = dtpEndTime.Value.TimeOfDay;
      double duration = (end - start).TotalMinutes;
      if (duration <= 0)
        duration += 24 * 60;
      if (breakMinutes >= duration)
      {
        MessageBox.Show(UiText.Get("BreakDurationMustBeShorterThanTheShift"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
      }

      if (!HasSelectedWorkday())
      {
        MessageBox.Show(UiText.Get("SelectAtLeastOneWorkingDay"), UiText.Get("MissingInformation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
      }

      return true;
    }

    private bool HasSelectedWorkday()
    {
      return chkMonday.Checked || chkTuesday.Checked || chkWednesday.Checked || chkThursday.Checked || chkFriday.Checked || chkSaturday.Checked || chkSunday.Checked;
    }

    private Shift ReadShiftFromForm()
    {
      return new Shift
      {
        ShiftName = txtShiftName.Text.Trim(),
        StartTime = dtpStartTime.Value.TimeOfDay,
        EndTime = dtpEndTime.Value.TimeOfDay,
        BreakMinutes = Convert.ToInt32(txtBreakMinutes.Text),
        Monday = chkMonday.Checked,
        Tuesday = chkTuesday.Checked,
        Wednesday = chkWednesday.Checked,
        Thursday = chkThursday.Checked,
        Friday = chkFriday.Checked,
        Saturday = chkSaturday.Checked,
        Sunday = chkSunday.Checked
      };
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}