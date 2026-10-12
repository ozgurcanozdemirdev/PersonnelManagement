using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.UI.Forms
{
  partial class UserManagementForm
  {
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing)
    {
      if (disposing && components != null)
        components.Dispose();
      base.Dispose(disposing);
    }

#region Windows Form Designer generated code
    private void InitializeComponent()
    {
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.dgvUsers = new System.Windows.Forms.DataGridView();
      this.txtUsername = new System.Windows.Forms.TextBox();
      this.txtFirstName = new System.Windows.Forms.TextBox();
      this.txtLastName = new System.Windows.Forms.TextBox();
      this.txtEmail = new System.Windows.Forms.TextBox();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.cmbRole = new System.Windows.Forms.ComboBox();
      this.btnNew = new System.Windows.Forms.Button();
      this.btnAddUser = new System.Windows.Forms.Button();
      this.btnUpdateRole = new System.Windows.Forms.Button();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.lblTotalRecords = new System.Windows.Forms.Label();
      this.lblStatus = new System.Windows.Forms.Label();
      this.lblUsername = new System.Windows.Forms.Label();
      this.lblFirstName = new System.Windows.Forms.Label();
      this.lblLastName = new System.Windows.Forms.Label();
      this.lblEmail = new System.Windows.Forms.Label();
      this.lblPassword = new System.Windows.Forms.Label();
      this.lblRole = new System.Windows.Forms.Label();
      this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colFullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colRole = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colIsActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.groupBox1.SuspendLayout();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
      this.SuspendLayout();
      //
      // groupBox1
      //
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Size = new System.Drawing.Size(271, 340);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      UiText.Bind(this.groupBox1, "UserDetails");
      //
      // lblKullaniciAdi
      //
      this.lblUsername.AutoSize = true;
      this.lblUsername.Location = new System.Drawing.Point(10, 27);
      this.lblUsername.Name = "lblUsername";
      UiText.Bind(this.lblUsername, "Username");
      //
      // txtKullaniciAdi
      //
      this.txtUsername.Location = new System.Drawing.Point(98, 24);
      this.txtUsername.Size = new System.Drawing.Size(159, 20);
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.TabIndex = 0;
      this.txtUsername.MaxLength = 50;
      this.groupBox1.Controls.Add(this.lblUsername);
      this.groupBox1.Controls.Add(this.txtUsername);
      //
      // lblAd
      //
      this.lblFirstName.AutoSize = true;
      this.lblFirstName.Location = new System.Drawing.Point(10, 63);
      this.lblFirstName.Name = "lblFirstName";
      UiText.Bind(this.lblFirstName, "FirstName");
      //
      // txtAd
      //
      this.txtFirstName.Location = new System.Drawing.Point(98, 60);
      this.txtFirstName.Size = new System.Drawing.Size(159, 20);
      this.txtFirstName.Name = "txtFirstName";
      this.txtFirstName.TabIndex = 1;
      this.txtFirstName.MaxLength = 50;
      this.groupBox1.Controls.Add(this.lblFirstName);
      this.groupBox1.Controls.Add(this.txtFirstName);
      //
      // lblSoyad
      //
      this.lblLastName.AutoSize = true;
      this.lblLastName.Location = new System.Drawing.Point(10, 99);
      this.lblLastName.Name = "lblLastName";
      UiText.Bind(this.lblLastName, "LastName");
      //
      // txtSoyad
      //
      this.txtLastName.Location = new System.Drawing.Point(98, 96);
      this.txtLastName.Size = new System.Drawing.Size(159, 20);
      this.txtLastName.Name = "txtLastName";
      this.txtLastName.TabIndex = 2;
      this.txtLastName.MaxLength = 50;
      this.groupBox1.Controls.Add(this.lblLastName);
      this.groupBox1.Controls.Add(this.txtLastName);
      //
      // lblEmail
      //
      this.lblEmail.AutoSize = true;
      this.lblEmail.Location = new System.Drawing.Point(10, 135);
      this.lblEmail.Name = "lblEmail";
      UiText.Bind(this.lblEmail, "Email2");
      //
      // txtEmail
      //
      this.txtEmail.Location = new System.Drawing.Point(98, 132);
      this.txtEmail.Size = new System.Drawing.Size(159, 20);
      this.txtEmail.Name = "txtEmail";
      this.txtEmail.TabIndex = 3;
      this.txtEmail.MaxLength = 100;
      this.groupBox1.Controls.Add(this.lblEmail);
      this.groupBox1.Controls.Add(this.txtEmail);
      //
      // lblSifre
      //
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(10, 171);
      this.lblPassword.Name = "lblPassword";
      UiText.Bind(this.lblPassword, "Password");
      //
      // txtSifre
      //
      this.txtPassword.Location = new System.Drawing.Point(98, 168);
      this.txtPassword.Size = new System.Drawing.Size(159, 20);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.TabIndex = 4;
      this.txtPassword.MaxLength = 50;
      this.txtPassword.UseSystemPasswordChar = true;
      this.groupBox1.Controls.Add(this.lblPassword);
      this.groupBox1.Controls.Add(this.txtPassword);
      //
      // lblRol
      //
      this.lblRole.AutoSize = true;
      this.lblRole.Location = new System.Drawing.Point(10, 207);
      this.lblRole.Name = "lblRole";
      UiText.Bind(this.lblRole, "Role");
      //
      // cmbRol
      //
      this.cmbRole.Location = new System.Drawing.Point(98, 204);
      this.cmbRole.Size = new System.Drawing.Size(159, 20);
      this.cmbRole.Name = "cmbRole";
      this.cmbRole.TabIndex = 5;
      this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbRole.FormattingEnabled = true;
      this.groupBox1.Controls.Add(this.lblRole);
      this.groupBox1.Controls.Add(this.cmbRole);
      this.cmbRole.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.CmbRoleFormat);
      //
      // btnYeni
      //
      this.btnNew.Location = new System.Drawing.Point(10, 252);
      this.btnNew.Size = new System.Drawing.Size(77, 35);
      this.btnNew.Name = "btnNew";
      this.btnNew.TabIndex = 6;
      UiText.Bind(this.btnNew, "NewUser");
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.BtnNewClick);
      this.groupBox1.Controls.Add(this.btnNew);
      //
      // btnKullaniciEkle
      //
      this.btnAddUser.Location = new System.Drawing.Point(95, 252);
      this.btnAddUser.Size = new System.Drawing.Size(77, 35);
      this.btnAddUser.Name = "btnAddUser";
      this.btnAddUser.TabIndex = 7;
      UiText.Bind(this.btnAddUser, "AddUser");
      this.btnAddUser.UseVisualStyleBackColor = true;
      this.btnAddUser.Click += new System.EventHandler(this.BtnAddUserClick);
      this.groupBox1.Controls.Add(this.btnAddUser);
      //
      // btnRolGuncelle
      //
      this.btnUpdateRole.Location = new System.Drawing.Point(180, 252);
      this.btnUpdateRole.Size = new System.Drawing.Size(77, 35);
      this.btnUpdateRole.Name = "btnUpdateRole";
      this.btnUpdateRole.TabIndex = 8;
      UiText.Bind(this.btnUpdateRole, "UpdateRole");
      this.btnUpdateRole.UseVisualStyleBackColor = true;
      this.btnUpdateRole.Click += new System.EventHandler(this.BtnUpdateRoleClick);
      this.groupBox1.Controls.Add(this.btnUpdateRole);
      //
      // lblDurum
      //
      this.lblStatus.Location = new System.Drawing.Point(10, 296);
      this.lblStatus.Size = new System.Drawing.Size(247, 38);
      this.lblStatus.Name = "lblStatus";
      this.groupBox1.Controls.Add(this.lblStatus);
      //
      // dgvKullanicilar
      //
      this.dgvUsers.Location = new System.Drawing.Point(295, 12);
      this.dgvUsers.Size = new System.Drawing.Size(440, 380);
      this.dgvUsers.Name = "dgvUsers";
      this.dgvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.dgvUsers.AllowUserToAddRows = false;
      this.dgvUsers.AllowUserToDeleteRows = false;
      this.dgvUsers.ReadOnly = true;
      this.dgvUsers.MultiSelect = false;
      this.dgvUsers.RowHeadersVisible = false;
      this.dgvUsers.AutoGenerateColumns = false;
      this.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgvUsers.TabIndex = 1;
      this.dgvUsers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colId, this.colUsername, this.colFullName, this.colEmail, this.colRole, this.colIsActive });
      this.dgvUsers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvUsersCellClick);
      this.dgvUsers.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvUsersCellFormatting);
      //
      // colId
      //
      this.colId.DataPropertyName = "UserId";
      this.colId.HeaderText = "ID";
      this.colId.Name = "colId";
      this.colId.Width = 35;
      this.colId.ReadOnly = true;
      //
      // colKullaniciAdi
      //
      this.colUsername.DataPropertyName = "Username";
      UiText.BindHeader(this.colUsername, "Username");
      this.colUsername.Name = "colUsername";
      this.colUsername.Width = 95;
      this.colUsername.ReadOnly = true;
      //
      // colAdSoyad
      //
      this.colFullName.DataPropertyName = "FullName";
      UiText.BindHeader(this.colFullName, "FullName");
      this.colFullName.Name = "colFullName";
      this.colFullName.Width = 120;
      this.colFullName.ReadOnly = true;
      //
      // colEmail
      //
      this.colEmail.DataPropertyName = "Email";
      UiText.BindHeader(this.colEmail, "Email2");
      this.colEmail.Name = "colEmail";
      this.colEmail.Width = 120;
      this.colEmail.ReadOnly = true;
      //
      // colRol
      //
      this.colRole.DataPropertyName = "Role";
      UiText.BindHeader(this.colRole, "Role");
      this.colRole.Name = "colRole";
      this.colRole.Width = 75;
      this.colRole.ReadOnly = true;
      //
      // colAktifMi
      //
      this.colIsActive.DataPropertyName = "IsActive";
      UiText.BindHeader(this.colIsActive, "Active");
      this.colIsActive.Name = "colIsActive";
      this.colIsActive.Width = 45;
      this.colIsActive.ReadOnly = true;
      //
      // panel1
      //
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 406);
      this.panel1.Size = new System.Drawing.Size(747, 46);
      this.panel1.Name = "panel1";
      this.panel1.TabIndex = 2;
      //
      // lblToplamKayit
      //
      this.lblTotalRecords.AutoSize = true;
      this.lblTotalRecords.Location = new System.Drawing.Point(12, 16);
      this.lblTotalRecords.Name = "lblTotalRecords";
      UiText.Bind(this.lblTotalRecords, "Total0Users");
      this.panel1.Controls.Add(this.lblTotalRecords);
      //
      // btnYenile
      //
      this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRefresh.Location = new System.Drawing.Point(573, 7);
      this.btnRefresh.Size = new System.Drawing.Size(78, 32);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.TabIndex = 0;
      UiText.Bind(this.btnRefresh, "Refresh");
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.BtnRefreshClick);
      this.panel1.Controls.Add(this.btnRefresh);
      //
      // btnKapat
      //
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(657, 7);
      this.btnClose.Size = new System.Drawing.Size(78, 32);
      this.btnClose.Name = "btnClose";
      this.btnClose.TabIndex = 1;
      UiText.Bind(this.btnClose, "Close");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      this.panel1.Controls.Add(this.btnClose);
      //
      // frmKullaniciYonetim
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(747, 452);
      this.MinimumSize = new System.Drawing.Size(763, 491);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.dgvUsers);
      this.Controls.Add(this.panel1);
      this.Name = "UserManagementForm";
      UiText.Bind(this, "UserManagement");
      this.Load += new System.EventHandler(this.UserManagementFormLoad);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
      this.ResumeLayout(false);
    }

#endregion
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.DataGridView dgvUsers;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.TextBox txtFirstName;
    private System.Windows.Forms.TextBox txtLastName;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.ComboBox cmbRole;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnAddUser;
    private System.Windows.Forms.Button btnUpdateRole;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label lblTotalRecords;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.Label lblFirstName;
    private System.Windows.Forms.Label lblLastName;
    private System.Windows.Forms.Label lblEmail;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.Label lblRole;
    private System.Windows.Forms.DataGridViewTextBoxColumn colId;
    private System.Windows.Forms.DataGridViewTextBoxColumn colUsername;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFullName;
    private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
    private System.Windows.Forms.DataGridViewTextBoxColumn colRole;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colIsActive;
  }
}
