using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PersonnelManagement.Connection;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.UI.Forms
{
  public sealed class ConnectionSettingsForm : LocalizedForm
  {
    private readonly TextBox _server = new TextBox
    {
      Left = 145,
      Top = 16,
      Width = 270
    };
    private readonly TextBox _database = new TextBox
    {
      Left = 145,
      Top = 50,
      Width = 270
    };
    private readonly Label _authentication = new Label { Left = 16, Top = 86, Width = 399 };
    private readonly CheckBox _trustCertificate = new CheckBox { Name = "chkTrustServerCertificate", Left = 16, Top = 189, Width = 399 };
    private readonly TextBox _username = new TextBox
    {
      Left = 145,
      Top = 120,
      Width = 270
    };
    private readonly TextBox _password = new TextBox
    {
      Left = 145,
      Top = 154,
      Width = 270,
      UseSystemPasswordChar = true
    };
    private readonly Label _status = new Label
    {
      Left = 16,
      Top = 267,
      Width = 399,
      Height = 56,
      ForeColor = Color.Firebrick
    };
    public ConnectionSettingsForm()
    {
      Name = nameof(ConnectionSettingsForm);
      UiText.Bind(this, "ConnectionSettings");
      ClientSize = new Size(431, 331);
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      StartPosition = FormStartPosition.CenterParent;
      AddLabel("Server", 16);
      AddLabel("Database", 50);
      AddLabel("Username", 120);
      AddLabel("Password", 154);
      UiText.Bind(_authentication, "SqlServerAuthentication");
      UiText.Bind(_trustCertificate, "TrustServerCertificate");
      var save = new Button
      {
        Name = "btnSaveConnection",
        Text = UiText.Translate("Bağlantıyı Kaydet"),
        Left = 145,
        Top = 226,
        Width = 270,
        Height = 28
      };
      UiText.Bind(save, "SaveConnection");
      save.Click += SaveConnection;
      Controls.AddRange(new Control[] { _server, _database, _authentication, _username, _password, _trustCertificate, _status, save });
      AcceptButton = save;
      try
      {
        var settings = new SqlConnectionStringBuilder(ConnectionSettingsStore.Load());
        _server.Text = settings.DataSource;
        _database.Text = settings.InitialCatalog;
        _username.Text = settings.UserID;
        _trustCertificate.Checked = settings.TrustServerCertificate;
      }
      catch
      {
        _server.Text = @".\SQLEXPRESS";
        _database.Text = "PersonnelManagementDB";
        _trustCertificate.Checked = false;
      }
    }

    private void AddLabel(string key, int top)
    {
      var label = new Label { Left = 16, Top = top + 3, Width = 125, Height = 22 };
      UiText.Bind(label, key);
      Controls.Add(label);
    }
    private void SaveConnection(object sender, EventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionSettingsStore.EnvironmentVariable)))
      {
        _status.Text = UiText.Translate("Bağlantı, ortam değişkeni üzerinden ayarlanmış. Bu ayarı değiştirmek için ortam değişkenini güncelleyin.");
        return;
      }

      try
      {
        var settings = new SqlConnectionStringBuilder
        {
          DataSource = _server.Text.Trim(),
          InitialCatalog = _database.Text.Trim(),
          IntegratedSecurity = false,
          Encrypt = true,
          TrustServerCertificate = _trustCertificate.Checked,
          ConnectTimeout = 5
        };
        settings.UserID = _username.Text.Trim();
        settings.Password = _password.Text;
        settings = ConnectionSettingsStore.Validate(settings.ConnectionString);

        using (var connection = new SqlConnection(settings.ConnectionString))
        {
          connection.Open();
          using (var command = new SqlCommand("SELECT CASE WHEN IS_SRVROLEMEMBER('sysadmin')=1 OR IS_MEMBER('db_owner')=1 OR OBJECT_ID(N'dbo.Employee',N'U') IS NULL THEN 1 ELSE 0 END", connection))
            if (Convert.ToInt32(command.ExecuteScalar()) != 0)
              throw new InvalidOperationException();
          using (var command = new SqlCommand("SELECT TOP(0) EmployeeId FROM dbo.Employee; SELECT TOP(0) UserId FROM dbo.UserAccount", connection))
            command.ExecuteNonQuery();
        }

        ConnectionSettingsStore.Save(settings.ConnectionString);
        SqlDatabaseConnection.CloseConnection();
        _password.Clear();
        DialogResult = DialogResult.OK;
        Close();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.TraceError(ex.ToString());
        _status.Text = UiText.Translate("Bağlantı kurulamadı. Sunucu, veritabanı ve hesap yetkilerini kontrol edin.");
      }
    }
  }
}