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
  public partial class LoginForm : LocalizedForm
  {
    public LoginForm()
    {
      InitializeComponent();
      var languageButton = new Button
      {
        Name = "btnLanguage",
        Text = "TR / EN",
        Location = new Point(16, 85),
        Size = new Size(100, 27),
        TabIndex = 3
      };
      languageButton.Click += (sender, e) => LanguageManager.SetLanguage(LanguageManager.Language == "tr" ? "en" : "tr");
      Controls.Add(languageButton);
      var connectionButton = new Button
      {
        Name = "btnConnectionSettings",
        Text = UiText.Get("ConnectionSettings"),
        Location = new Point(16, 122),
        Size = new Size(354, 27),
        TabIndex = 4
      };
      UiText.Bind(connectionButton, "ConnectionSettings");
      connectionButton.Click += (sender, e) =>
      {
        using (var settings = new ConnectionSettingsForm())
          settings.ShowDialog(this);
      };
      Controls.Add(connectionButton);
    }

    private readonly UserRepository _userRepository = new UserRepository();
    public static UserAccount CurrentUser
    {
      get => UserSession.CurrentUser;
      private set => UserSession.CurrentUser = value;
    }

    private void LoginFormLoad(object sender, EventArgs e)
    {
      txtPassword.PasswordChar = '*';
      txtPassword.MaxLength = 50;
      txtUsername.MaxLength = 50;
      lblError.Visible = false;
      lblError.ForeColor = Color.Red;
      this.AcceptButton = btnLogin;
      txtUsername.Select();
    }

    private void BtnLoginClick(object sender, EventArgs e)
    {
      SignIn();
    }

    private void SignIn()
    {
      if (string.IsNullOrWhiteSpace(txtUsername.Text))
      {
        ShowError(UiText.Get("UsernameIsRequired"));
        txtUsername.Focus();
        return;
      }

      if (string.IsNullOrWhiteSpace(txtPassword.Text))
      {
        ShowError(UiText.Get("PasswordIsRequired"));
        txtPassword.Focus();
        return;
      }

      try
      {
        btnLogin.Enabled = false;
        lblError.Visible = false;
        UserAccount user = _userRepository.Authenticate(txtUsername.Text.Trim(), txtPassword.Text);
        if (user == null)
        {
          ShowError(UiText.Get("InvalidUsernameOrPassword"));
          txtPassword.Clear();
          txtPassword.Focus();
          return;
        }

        CurrentUser = user;
        MainForm mainForm = new MainForm();
        mainForm.Show();
        this.Hide();
      }
      catch (InvalidOperationException ex) when (ex.Message == UiText.Get("TheDatabaseConnectionIsNotConfigured"))
      {
        ShowError(ex.Message);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.TraceError(ex.ToString());
        ShowError(UiText.Get("ConnectionFailedCheckTheServerDatabaseAndAccountPermissions"));
      }
      finally
      {
        btnLogin.Enabled = true;
      }
    }

    private void ShowError(string message)
    {
      lblError.Text = message;
      lblError.Visible = true;
    }

    private void TxtUsernameKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        txtPassword.Focus();
        e.SuppressKeyPress = true;
      }
    }

    private void TxtPasswordKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        SignIn();
        e.SuppressKeyPress = true;
      }
    }

    private void TxtUsernameTextChanged(object sender, EventArgs e)
    {
      lblError.Visible = false;
    }

    private void TxtPasswordTextChanged(object sender, EventArgs e)
    {
      lblError.Visible = false;
    }

    private void LoginFormFormClosed(object sender, FormClosedEventArgs e)
    {
      if (CurrentUser == null)
        Application.Exit();
    }
  }
}