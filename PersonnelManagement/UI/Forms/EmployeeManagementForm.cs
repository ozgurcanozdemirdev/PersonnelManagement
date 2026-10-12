using System.Threading;
using System.Threading.Tasks;
using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.UI.Forms
{
  public partial class EmployeeManagementForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.ViewPersonnel;

    public EmployeeManagementForm()
    {
      InitializeComponent();
    }

    private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();
    private readonly DepartmentRepository _departmentRepository = new DepartmentRepository();
    private readonly PositionRepository _positionRepository = new PositionRepository();
    private readonly ShiftRepository _shiftRepository = new ShiftRepository();
    private CancellationTokenSource _employeeLoadCancellation;
    private int _employeeLoadVersion;
    private bool _clearingForm;
    private int _selectedEmployeeId = 0;
    private string _selectedPhotoPath = null;
    private async void EmployeeManagementFormLoad(object sender, EventArgs e)
    {
      // İşaret kaldırıldığında bilinmeyen doğum tarihi null olarak korunur.
      dtpBirthDate.ShowCheckBox = true;
      LoadComboBoxes();
      ClearForm();
      await LoadEmployeesAsync();
      dgvEmployees.CellClick += DgvEmployeesCellClick;
    }

    private void LoadComboBoxes()
    {
      var departments = _departmentRepository.GetAll();
      cmbDepartment.DataSource = departments;
      cmbDepartment.DisplayMember = "DepartmentName";
      cmbDepartment.ValueMember = "DepartmentId";
      cmbDepartment.SelectedIndex = -1;
      var shifts = _shiftRepository.GetAll();
      cmbShift.DataSource = shifts;
      cmbShift.DisplayMember = "ShiftName";
      cmbShift.ValueMember = "ShiftId";
      cmbShift.SelectedIndex = -1;
      cmbGender.Items.Clear();
      cmbGender.Items.Add("Male");
      cmbGender.Items.Add("Female");
      cmbGender.SelectedIndex = -1;
    }

    private void CmbDepartmentSelectedIndexChanged(object sender, EventArgs e)
    {
      cmbPosition.DataSource = null;
      if (cmbDepartment.SelectedItem == null)
        return;
      // SelectedValue yerine SelectedItem üzerinden cast et
      int departmentId = ((Department)cmbDepartment.SelectedItem).DepartmentId;
      var positions = _positionRepository.GetAll(departmentId);
      cmbPosition.DataSource = positions;
      cmbPosition.DisplayMember = "PositionName";
      cmbPosition.ValueMember = "PositionId";
      cmbPosition.SelectedIndex = -1;
    }

    private async Task LoadEmployeesAsync(string searchText = null)
    {
      _employeeLoadCancellation?.Cancel();
      var cancellation = new CancellationTokenSource();
      _employeeLoadCancellation = cancellation;
      int version = ++_employeeLoadVersion;
      try
      {
        List<Employee> list = string.IsNullOrWhiteSpace(searchText)
          ? await _employeeRepository.GetAllAsync(cancellationToken: cancellation.Token)
          : await _employeeRepository.SearchAsync(searchText, cancellation.Token);
        if (IsDisposed || cancellation.IsCancellationRequested || version != _employeeLoadVersion)
          return;
        dgvEmployees.DataSource = list;
        ConfigureGridColumns();
        UiText.Bind(lblTotalRecords, "Employees", list.Count);
      }
      catch (OperationCanceledException) { }
      catch (Exception ex)
      {
        if (!IsDisposed && !cancellation.IsCancellationRequested && version == _employeeLoadVersion)
          MessageBox.Show(UiText.Get("EmployeesCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        if (ReferenceEquals(_employeeLoadCancellation, cancellation))
          _employeeLoadCancellation = null;
        cancellation.Dispose();
      }
    }

    private void ConfigureGridColumns()
    {
      string[] hiddenColumns =
      {
        "NationalId",
        "Gender",
        "BirthDate",
        "Phone",
        "Email",
        "Address",
        "PhotoPath",
        "DepartmentId",
        "PositionId",
        "ShiftId",
        "StartTime",
        "EndTime",
        "BreakMinutes",
        "EmploymentEndDate",
        "CreatedAt",
        "UpdatedAt",
        "IsActive"
      };
      foreach (string s in hiddenColumns)
        if (dgvEmployees.Columns[s] != null)
          dgvEmployees.Columns[s].Visible = false;
      if (dgvEmployees.Columns["EmployeeId"] != null)
      {
        dgvEmployees.Columns["EmployeeId"].HeaderText = "ID";
        dgvEmployees.Columns["EmployeeId"].Width = 40;
      }

      if (dgvEmployees.Columns["FullName"] != null)
      {
        UiText.BindHeader(dgvEmployees.Columns["FullName"], "FullName");
        dgvEmployees.Columns["FullName"].Width = 160;
      }

      if (dgvEmployees.Columns["DepartmentName"] != null)
      {
        UiText.BindHeader(dgvEmployees.Columns["DepartmentName"], "Department");
        dgvEmployees.Columns["DepartmentName"].Width = 130;
      }

      if (dgvEmployees.Columns["PositionName"] != null)
      {
        UiText.BindHeader(dgvEmployees.Columns["PositionName"], "Position");
        dgvEmployees.Columns["PositionName"].Width = 130;
      }

      if (dgvEmployees.Columns["ShiftName"] != null)
      {
        UiText.BindHeader(dgvEmployees.Columns["ShiftName"], "Shift");
        dgvEmployees.Columns["ShiftName"].Width = 110;
      }

      if (dgvEmployees.Columns["EmploymentStartDate"] != null)
      {
        UiText.BindHeader(dgvEmployees.Columns["EmploymentStartDate"], "EmploymentStart");
        dgvEmployees.Columns["EmploymentStartDate"].Width = 90;
      }

      if (dgvEmployees.Columns["AnnualLeaveAllowance"] != null)
      {
        UiText.BindHeader(dgvEmployees.Columns["AnnualLeaveAllowance"], "LeaveAllowance");
        dgvEmployees.Columns["AnnualLeaveAllowance"].Width = 70;
      }
    }

    private void DgvEmployeesCellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      try
      {
        int employeeId = Convert.ToInt32(dgvEmployees.Rows[e.RowIndex].Cells["EmployeeId"].Value);
        Employee p = _employeeRepository.GetById(employeeId);
        if (p == null)
          return;
        _selectedEmployeeId = p.EmployeeId;
        _selectedPhotoPath = p.PhotoPath;
        txtFirstName.Text = p.FirstName;
        txtLastName.Text = p.LastName;
        txtNationalId.Text = p.NationalId;
        txtPhone.Text = p.Phone;
        txtEmail.Text = p.Email;
        txtAddress.Text = p.Address;
        txtAnnualLeaveAllowance.Text = p.AnnualLeaveAllowance.ToString();
        dtpEmploymentStartDate.Value = p.EmploymentStartDate;
        dtpBirthDate.Value = p.BirthDate ?? DateTime.Today.AddYears(-25);
        dtpBirthDate.Checked = p.BirthDate.HasValue;
        cmbGender.SelectedItem = p.Gender;
        foreach (Department d in cmbDepartment.Items)
          if (d.DepartmentId == p.DepartmentId)
          {
            cmbDepartment.SelectedItem = d;
            break;
          }

        foreach (Position pz in cmbPosition.Items)
          if (pz.PositionId == p.PositionId)
          {
            cmbPosition.SelectedItem = pz;
            break;
          }

        foreach (Shift v in cmbShift.Items)
          if (v.ShiftId == p.ShiftId)
          {
            cmbShift.SelectedItem = v;
            break;
          }

        LoadPhoto(p.PhotoPath);
        btnSave.Enabled = false;
        btnUpdate.Enabled = true;
        btnDeactivate.Enabled = OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ManagePersonnel);
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("RecordCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void BtnSaveClick(object sender, EventArgs e)
    {
      if (!ValidateForm())
        return;
      try
      {
        Employee p = ReadEmployeeFromForm();
        if (_employeeRepository.NationalIdExists(p.NationalId))
        {
          MessageBox.Show(UiText.Get("ThisNationalIDIsAlreadyRegistered"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
          txtNationalId.Focus();
          return;
        }

        int newId = _employeeRepository.Save(p);
        MessageBox.Show(UiText.Format("EmployeeSavedID", newId), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        await LoadEmployeesAsync();
        ClearForm();
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("SaveFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void BtnUpdateClick(object sender, EventArgs e)
    {
      if (_selectedEmployeeId == 0)
      {
        MessageBox.Show(UiText.Get("SelectAnEmployeeToUpdate"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (!ValidateForm())
        return;
      try
      {
        Employee p = ReadEmployeeFromForm();
        p.EmployeeId = _selectedEmployeeId;
        if (_employeeRepository.NationalIdExists(p.NationalId, _selectedEmployeeId))
        {
          MessageBox.Show(UiText.Get("ThisNationalIDBelongsToAnotherEmployee"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
          txtNationalId.Focus();
          return;
        }

        bool ok = _employeeRepository.Update(p);
        if (ok)
        {
          MessageBox.Show(UiText.Get("EmployeeDetailsUpdated"), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          await LoadEmployeesAsync();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("UpdateFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void BtnDeactivateClick(object sender, EventArgs e)
    {
      if (_selectedEmployeeId == 0)
      {
        MessageBox.Show(UiText.Get("SelectAnEmployeeToDeactivate"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var approved = MessageBox.Show(UiText.Format("DeactivateEmployee", txtFirstName.Text, txtLastName.Text), UiText.Get("ConfirmDeactivation"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (approved != DialogResult.Yes)
        return;
      try
      {
        bool ok = _employeeRepository.Deactivate(_selectedEmployeeId, DateTime.Today);
        if (ok)
        {
          MessageBox.Show(UiText.Get("EmployeeDeactivated"), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          await LoadEmployeesAsync();
          ClearForm();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("DeactivationFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void BtnSearchClick(object sender, EventArgs e)
    {
      await LoadEmployeesAsync(txtSearch.Text.Trim());
    }

    private async void BtnSearchKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        await LoadEmployeesAsync(txtSearch.Text.Trim());
        e.SuppressKeyPress = true;
      }
    }

    private async void TxtSearchKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        await LoadEmployeesAsync(txtSearch.Text.Trim());
        e.SuppressKeyPress = true;
      }
    }

    private async void TxtSearchTextChanged(object sender, EventArgs e)
    {
      if (!_clearingForm && string.IsNullOrWhiteSpace(txtSearch.Text))
        await LoadEmployeesAsync();
    }

    private void BtnClearClick(object sender, EventArgs e)
    {
      ClearForm();
    }

    private void ClearForm()
    {
      _selectedEmployeeId = 0;
      _selectedPhotoPath = null;
      txtFirstName.Clear();
      txtLastName.Clear();
      txtNationalId.Clear();
      txtPhone.Clear();
      txtEmail.Clear();
      txtAddress.Clear();
      txtAnnualLeaveAllowance.Text = "14";
      // Programatik temizleme ikinci bir liste sorgusu başlatmasın.
      _clearingForm = true;
      try { txtSearch.Clear(); }
      finally { _clearingForm = false; }
      cmbDepartment.SelectedIndex = -1;
      cmbPosition.DataSource = null;
      cmbShift.SelectedIndex = -1;
      cmbGender.SelectedIndex = -1;
      dtpEmploymentStartDate.Value = DateTime.Today;
      dtpBirthDate.Value = DateTime.Today.AddYears(-25);
      dtpBirthDate.Checked = false;
      var previousPhoto = picPhoto.Image;
      picPhoto.Image = null;
      previousPhoto?.Dispose();
      btnSave.Enabled = true;
      btnUpdate.Enabled = false;
      btnDeactivate.Enabled = false;
      txtAnnualLeaveAllowance.ReadOnly = !OperationAuthorization.HasPermission(UserSession.CurrentUser, ApplicationPermission.ManagePersonnel);
      txtFirstName.Focus();
    }

    private void BtnSelectPhotoClick(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog())
      {
        ofd.Title = UiText.Get("SelectPhoto");
        ofd.Filter = UiText.Get("ImageFilesJpgJpegPngBmp");
        if (ofd.ShowDialog() != DialogResult.OK)
          return;
        _selectedPhotoPath = ofd.FileName;
        LoadPhoto(_selectedPhotoPath);
      }
    }

    private void LoadPhoto(string path)
    {
      if (string.IsNullOrEmpty(path) || !File.Exists(path))
      {
        var previousPhoto = picPhoto.Image;
        picPhoto.Image = null;
        previousPhoto?.Dispose();
        return;
      }

      try
      {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
          using (var image = Image.FromStream(stream))
          {
            var previous = picPhoto.Image;
            picPhoto.Image = new Bitmap(image);
            previous?.Dispose();
          }
        }
      }
      catch
      {
        var previousPhoto = picPhoto.Image;
        picPhoto.Image = null;
        previousPhoto?.Dispose();
      }
    }

    private bool ValidateForm()
    {
      if (!int.TryParse(txtAnnualLeaveAllowance.Text, out int leave) || leave < 0)
      {
        Warn(UiText.Get("EnterAValidAnnualLeaveAllowance"), txtAnnualLeaveAllowance);
        return false;
      }
      string error = EmployeeValidation.GetError(ReadEmployeeFromForm(), out string field);
      if (error == null)
        return true;
      Control focus = txtFirstName;
      switch (field)
      {
        case nameof(Employee.LastName): focus = txtLastName; break;
        case nameof(Employee.NationalId): focus = txtNationalId; break;
        case nameof(Employee.BirthDate): focus = dtpBirthDate; break;
        case nameof(Employee.EmploymentStartDate): focus = dtpEmploymentStartDate; break;
        case nameof(Employee.DepartmentId): focus = cmbDepartment; break;
        case nameof(Employee.PositionId): focus = cmbPosition; break;
        case nameof(Employee.ShiftId): focus = cmbShift; break;
        case nameof(Employee.Phone): focus = txtPhone; break;
        case nameof(Employee.Email): focus = txtEmail; break;
        case nameof(Employee.Address): focus = txtAddress; break;
      }
      Warn(error, focus);
      return false;
    }

    private void Warn(string message, Control odak)
    {
      MessageBox.Show(message, UiText.Get("MissingInformation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      odak?.Focus();
    }

    private Employee ReadEmployeeFromForm() => new Employee
    {
      FirstName = txtFirstName.Text.Trim(),
      LastName = txtLastName.Text.Trim(),
      NationalId = txtNationalId.Text.Trim(),
      Gender = cmbGender.SelectedItem?.ToString(),
      BirthDate = dtpBirthDate.Checked ? (DateTime? )dtpBirthDate.Value.Date : null,
      Phone = txtPhone.Text.Trim(),
      Email = txtEmail.Text.Trim(),
      Address = txtAddress.Text.Trim(),
      PhotoPath = _selectedPhotoPath,
      DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue),
      PositionId = Convert.ToInt32(cmbPosition.SelectedValue),
      ShiftId = Convert.ToInt32(cmbShift.SelectedValue),
      EmploymentStartDate = dtpEmploymentStartDate.Value.Date,
      AnnualLeaveAllowance = Convert.ToInt32(txtAnnualLeaveAllowance.Text)
    };
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      _employeeLoadCancellation?.Cancel();
      var image = picPhoto.Image;
      picPhoto.Image = null;
      image?.Dispose();
      base.OnFormClosed(e);
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
