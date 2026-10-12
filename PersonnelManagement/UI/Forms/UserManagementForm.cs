using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
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
  public partial class UserManagementForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.ManageUsers;

    private readonly UserRepository _userRepository = new UserRepository();
    private int _selectedUserId;
    public UserManagementForm()
    {
      InitializeComponent();
    }



    private bool HasAdminAccess()
    {
      return OperationAuthorization.HasPermission(OperationAuthorization.RefreshCurrentUser(), ApplicationPermission.ManageUsers);
    }

    private void UserManagementFormLoad(object sender, EventArgs e)
    {
      try
      {
        cmbRole.DataSource = _userRepository.GetRoles();
        LoadUsers();
        ClearForm();
      }
      catch (Exception)
      {
        ShowStatus(UiText.Get("UsersCouldNotBeLoaded"), true);
      }
    }

    private void LoadUsers()
    {
      var list = _userRepository.GetAll();
      dgvUsers.DataSource = list;
      dgvUsers.ClearSelection();
      UiText.BindComputed(lblTotalRecords, () => UiText.Get("Total") + list.Count + UiText.Get("Users"));
    }

    private void ClearForm()
    {
      _selectedUserId = 0;
      txtUsername.Clear();
      txtFirstName.Clear();
      txtLastName.Clear();
      txtEmail.Clear();
      txtPassword.Clear();
      txtUsername.ReadOnly = false;
      txtFirstName.ReadOnly = false;
      txtLastName.ReadOnly = false;
      txtEmail.ReadOnly = false;
      txtPassword.Enabled = true;
      cmbRole.SelectedItem = "Employee";
      btnAddUser.Enabled = true;
      btnUpdateRole.Enabled = false;
      dgvUsers.ClearSelection();
      lblStatus.Text = "";
      txtUsername.Focus();
    }

    private void DgvUsersCellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      UserAccount user = dgvUsers.Rows[e.RowIndex].DataBoundItem as UserAccount;
      if (user == null)
        return;
      _selectedUserId = user.UserId;
      txtUsername.Text = user.Username;
      txtFirstName.Text = user.FirstName;
      txtLastName.Text = user.LastName;
      txtEmail.Text = user.Email ?? "";
      txtPassword.Clear();
      txtUsername.ReadOnly = true;
      txtFirstName.ReadOnly = true;
      txtLastName.ReadOnly = true;
      txtEmail.ReadOnly = true;
      txtPassword.Enabled = false;
      cmbRole.SelectedItem = user.Role;
      btnAddUser.Enabled = false;
      btnUpdateRole.Enabled = true;
      ShowStatus(UiText.Get("YouCanChangeTheSelectedUserSRole"), false);
    }

    private void BtnAddUserClick(object sender, EventArgs e)
    {
      try
      {
        if (!HasAdminAccess())
          throw new UnauthorizedAccessException(UiText.Get("AdministratorAccessIsRequiredForThisOperation"));
        var user = new UserAccount
        {
          Username = txtUsername.Text,
          FirstName = txtFirstName.Text,
          LastName = txtLastName.Text,
          Email = txtEmail.Text,
          Role = cmbRole.SelectedItem as string
        };
        _userRepository.Save(user, txtPassword.Text, LoginForm.CurrentUser.UserId);
        LoadUsers();
        ClearForm();
        ShowStatus(UiText.Get("UserAdded"), false);
      }
      catch (SqlException ex)
      {
        ShowStatus(ex.Number == 2601 || ex.Number == 2627 ? UiText.Get("ThisUsernameIsAlreadyRegistered") : UiText.Get("UserCouldNotBeAdded"), true);
      }
      catch (ArgumentException ex)
      {
        ShowStatus(ex.Message, true);
      }
      catch (FormatException)
      {
        ShowStatus(UiText.Get("EnterAValidEmailAddress"), true);
      }
      catch (UnauthorizedAccessException ex)
      {
        ShowStatus(ex.Message, true);
      }
      catch (Exception)
      {
        ShowStatus(UiText.Get("UserCouldNotBeAdded"), true);
      }
    }

    private void BtnUpdateRoleClick(object sender, EventArgs e)
    {
      try
      {
        if (!HasAdminAccess())
          throw new UnauthorizedAccessException(UiText.Get("AdministratorAccessIsRequiredForThisOperation"));
        if (_selectedUserId == 0)
          throw new ArgumentException(UiText.Get("SelectAUserFromTheList"));
        string role = cmbRole.SelectedItem as string;
        if (!_userRepository.UpdateRole(_selectedUserId, role, LoginForm.CurrentUser.UserId))
          throw new ArgumentException(UiText.Get("TheSelectedUserWasNotFound"));
        if (_selectedUserId == LoginForm.CurrentUser.UserId)
        {
          HasAdminAccess();
          if (!LoginForm.CurrentUser.IsAdmin)
          {
            Close();
            return;
          }
        }

        LoadUsers();
        ClearForm();
        ShowStatus(UiText.Get("RoleUpdated"), false);
      }
      catch (ArgumentException ex)
      {
        ShowStatus(ex.Message, true);
      }
      catch (InvalidOperationException ex)
      {
        ShowStatus(ex.Message, true);
      }
      catch (UnauthorizedAccessException ex)
      {
        ShowStatus(ex.Message, true);
      }
      catch (Exception)
      {
        ShowStatus(UiText.Get("RoleCouldNotBeUpdated"), true);
      }
    }

    private void ShowStatus(string message, bool error)
    {
      lblStatus.ForeColor = error ? Color.Red : Color.DarkGreen;
      lblStatus.Text = message;
    }

    private void BtnNewClick(object sender, EventArgs e)
    {
      ClearForm();
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
      Close();
    }

    private void BtnRefreshClick(object sender, EventArgs e)
    {
      try
      {
        if (!HasAdminAccess())
        {
          Close();
          return;
        }

        LoadUsers();
        ClearForm();
      }
      catch (Exception)
      {
        ShowStatus(UiText.Get("UsersCouldNotBeLoaded"), true);
      }
    }

    private void CmbRoleFormat(object sender, ListControlConvertEventArgs e)
    {
      e.Value = UiText.DisplayValue(e.ListItem);
    }

    private void DgvUsersCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (dgvUsers.Columns[e.ColumnIndex].Name != "colRole")
        return;
      e.Value = UiText.DisplayValue(e.Value);
      e.FormattingApplied = true;
    }
  }
}