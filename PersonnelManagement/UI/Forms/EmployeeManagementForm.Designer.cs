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
  partial class EmployeeManagementForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name = "disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }

      base.Dispose(disposing);
    }

#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.btnDeactivate = new System.Windows.Forms.Button();
      this.btnUpdate = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvEmployees = new System.Windows.Forms.DataGridView();
      this.panel3 = new System.Windows.Forms.Panel();
      this.btnSelectPhoto = new System.Windows.Forms.Button();
      this.picPhoto = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.txtFirstName = new System.Windows.Forms.TextBox();
      this.txtLastName = new System.Windows.Forms.TextBox();
      this.txtNationalId = new System.Windows.Forms.TextBox();
      this.txtPhone = new System.Windows.Forms.TextBox();
      this.txtEmail = new System.Windows.Forms.TextBox();
      this.txtAddress = new System.Windows.Forms.TextBox();
      this.label14 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.label16 = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.label20 = new System.Windows.Forms.Label();
      this.label21 = new System.Windows.Forms.Label();
      this.label22 = new System.Windows.Forms.Label();
      this.label23 = new System.Windows.Forms.Label();
      this.label24 = new System.Windows.Forms.Label();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.dtpEmploymentStartDate = new System.Windows.Forms.DateTimePicker();
      this.cmbDepartment = new System.Windows.Forms.ComboBox();
      this.cmbPosition = new System.Windows.Forms.ComboBox();
      this.cmbShift = new System.Windows.Forms.ComboBox();
      this.cmbGender = new System.Windows.Forms.ComboBox();
      this.txtSearch = new System.Windows.Forms.TextBox();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label13 = new System.Windows.Forms.Label();
      this.label19 = new System.Windows.Forms.Label();
      this.txtAnnualLeaveAllowance = new System.Windows.Forms.TextBox();
      this.label25 = new System.Windows.Forms.Label();
      this.label26 = new System.Windows.Forms.Label();
      this.dtpBirthDate = new System.Windows.Forms.DateTimePicker();
      this.label27 = new System.Windows.Forms.Label();
      this.label28 = new System.Windows.Forms.Label();
      this.lblTotalRecords = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
      this.panel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnClear);
      this.panel1.Controls.Add(this.btnDeactivate);
      this.panel1.Controls.Add(this.btnUpdate);
      this.panel1.Controls.Add(this.btnSave);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 409);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(793, 56);
      this.panel1.TabIndex = 0;
      // 
      // btnKapat
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(719, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(71, 50);
      this.btnClose.TabIndex = 4;
      UiText.Bind(this.btnClose, "Close");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      // 
      // btnTemizle
      // 
      this.btnClear.Location = new System.Drawing.Point(232, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(71, 50);
      this.btnClear.TabIndex = 3;
      UiText.Bind(this.btnClear, "Clear");
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.BtnClearClick);
      // 
      // btnSil
      // 
      this.btnDeactivate.Location = new System.Drawing.Point(157, 3);
      this.btnDeactivate.Name = "btnDeactivate";
      this.btnDeactivate.Size = new System.Drawing.Size(71, 50);
      this.btnDeactivate.TabIndex = 2;
      UiText.Bind(this.btnDeactivate, "Deactivate");
      this.btnDeactivate.UseVisualStyleBackColor = true;
      this.btnDeactivate.Click += new System.EventHandler(this.BtnDeactivateClick);
      // 
      // btnGuncelle
      // 
      this.btnUpdate.Location = new System.Drawing.Point(80, 3);
      this.btnUpdate.Name = "btnUpdate";
      this.btnUpdate.Size = new System.Drawing.Size(71, 50);
      this.btnUpdate.TabIndex = 1;
      UiText.Bind(this.btnUpdate, "Update");
      this.btnUpdate.UseVisualStyleBackColor = true;
      this.btnUpdate.Click += new System.EventHandler(this.BtnUpdateClick);
      // 
      // btnKaydet
      // 
      this.btnSave.Location = new System.Drawing.Point(3, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(71, 50);
      this.btnSave.TabIndex = 0;
      UiText.Bind(this.btnSave, "Save");
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.BtnSaveClick);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvEmployees);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel2.Location = new System.Drawing.Point(0, 216);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(793, 193);
      this.panel2.TabIndex = 1;
      // 
      // dgvPersonel
      // 
      this.dgvEmployees.AllowUserToAddRows = false;
      this.dgvEmployees.AllowUserToDeleteRows = false;
      this.dgvEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvEmployees.Location = new System.Drawing.Point(0, 0);
      this.dgvEmployees.Name = "dgvEmployees";
      this.dgvEmployees.ReadOnly = true;
      this.dgvEmployees.Size = new System.Drawing.Size(793, 193);
      this.dgvEmployees.TabIndex = 0;
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.btnSelectPhoto);
      this.panel3.Controls.Add(this.picPhoto);
      this.panel3.Location = new System.Drawing.Point(508, 12);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(145, 148);
      this.panel3.TabIndex = 2;
      // 
      // btnFotoSec
      // 
      this.btnSelectPhoto.Location = new System.Drawing.Point(3, 120);
      this.btnSelectPhoto.Name = "btnSelectPhoto";
      this.btnSelectPhoto.Size = new System.Drawing.Size(139, 23);
      this.btnSelectPhoto.TabIndex = 1;
      UiText.Bind(this.btnSelectPhoto, "SelectPhoto");
      this.btnSelectPhoto.UseVisualStyleBackColor = true;
      this.btnSelectPhoto.Click += new System.EventHandler(this.BtnSelectPhotoClick);
      // 
      // picFoto
      // 
      this.picPhoto.Location = new System.Drawing.Point(3, 4);
      this.picPhoto.Name = "picPhoto";
      this.picPhoto.Size = new System.Drawing.Size(139, 110);
      this.picPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picPhoto.TabIndex = 0;
      this.picPhoto.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(23, 13);
      this.label1.TabIndex = 3;
      UiText.Bind(this.label1, "FirstName2");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 41);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(37, 13);
      this.label2.TabIndex = 4;
      UiText.Bind(this.label2, "LastName");
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 70);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 13);
      this.label3.TabIndex = 5;
      UiText.Bind(this.label3, "NationalID2");
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 99);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(43, 13);
      this.label4.TabIndex = 6;
      UiText.Bind(this.label4, "Phone");
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 128);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(36, 13);
      this.label5.TabIndex = 7;
      UiText.Bind(this.label5, "Email4");
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(12, 157);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(34, 13);
      this.label6.TabIndex = 8;
      UiText.Bind(this.label6, "Address");
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(91, 158);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(10, 13);
      this.label7.TabIndex = 14;
      this.label7.Text = ":";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(91, 129);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(10, 13);
      this.label8.TabIndex = 13;
      this.label8.Text = ":";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(91, 100);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(10, 13);
      this.label9.TabIndex = 12;
      this.label9.Text = ":";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(91, 71);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(10, 13);
      this.label10.TabIndex = 11;
      this.label10.Text = ":";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(91, 42);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(10, 13);
      this.label11.TabIndex = 10;
      this.label11.Text = ":";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(91, 13);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(10, 13);
      this.label12.TabIndex = 9;
      this.label12.Text = ":";
      // 
      // txtAd
      // 
      this.txtFirstName.Location = new System.Drawing.Point(107, 13);
      this.txtFirstName.Name = "txtFirstName";
      this.txtFirstName.Size = new System.Drawing.Size(100, 20);
      this.txtFirstName.TabIndex = 15;
      // 
      // txtSoyad
      // 
      this.txtLastName.Location = new System.Drawing.Point(107, 41);
      this.txtLastName.Name = "txtLastName";
      this.txtLastName.Size = new System.Drawing.Size(100, 20);
      this.txtLastName.TabIndex = 16;
      // 
      // txtTCKimlik
      // 
      this.txtNationalId.Location = new System.Drawing.Point(107, 70);
      this.txtNationalId.Name = "txtNationalId";
      this.txtNationalId.Size = new System.Drawing.Size(100, 20);
      this.txtNationalId.TabIndex = 17;
      // 
      // txtTelefon
      // 
      this.txtPhone.Location = new System.Drawing.Point(107, 99);
      this.txtPhone.Name = "txtPhone";
      this.txtPhone.Size = new System.Drawing.Size(100, 20);
      this.txtPhone.TabIndex = 18;
      // 
      // txtEmail
      // 
      this.txtEmail.Location = new System.Drawing.Point(107, 128);
      this.txtEmail.Name = "txtEmail";
      this.txtEmail.Size = new System.Drawing.Size(100, 20);
      this.txtEmail.TabIndex = 19;
      // 
      // txtAdres
      // 
      this.txtAddress.Location = new System.Drawing.Point(107, 157);
      this.txtAddress.Name = "txtAddress";
      this.txtAddress.Size = new System.Drawing.Size(100, 20);
      this.txtAddress.TabIndex = 20;
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(222, 132);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(43, 13);
      this.label14.TabIndex = 25;
      UiText.Bind(this.label14, "Gender");
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(222, 103);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(64, 13);
      this.label15.TabIndex = 24;
      UiText.Bind(this.label15, "SelectShift");
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(222, 74);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(71, 13);
      this.label16.TabIndex = 23;
      UiText.Bind(this.label16, "SelectPosition");
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(222, 44);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(81, 13);
      this.label17.TabIndex = 22;
      UiText.Bind(this.label17, "SelectDepartment");
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(222, 16);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(73, 13);
      this.label18.TabIndex = 21;
      UiText.Bind(this.label18, "EmploymentStartDate");
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Location = new System.Drawing.Point(311, 132);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(10, 13);
      this.label20.TabIndex = 31;
      this.label20.Text = ":";
      // 
      // label21
      // 
      this.label21.AutoSize = true;
      this.label21.Location = new System.Drawing.Point(311, 103);
      this.label21.Name = "label21";
      this.label21.Size = new System.Drawing.Size(10, 13);
      this.label21.TabIndex = 30;
      this.label21.Text = ":";
      // 
      // label22
      // 
      this.label22.AutoSize = true;
      this.label22.Location = new System.Drawing.Point(311, 74);
      this.label22.Name = "label22";
      this.label22.Size = new System.Drawing.Size(10, 13);
      this.label22.TabIndex = 29;
      this.label22.Text = ":";
      // 
      // label23
      // 
      this.label23.AutoSize = true;
      this.label23.Location = new System.Drawing.Point(311, 45);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(10, 13);
      this.label23.TabIndex = 28;
      this.label23.Text = ":";
      // 
      // label24
      // 
      this.label24.AutoSize = true;
      this.label24.Location = new System.Drawing.Point(311, 16);
      this.label24.Name = "label24";
      this.label24.Size = new System.Drawing.Size(10, 13);
      this.label24.TabIndex = 27;
      this.label24.Text = ":";
      // 
      // dtpIseGirisTarihi
      // 
      this.dtpEmploymentStartDate.Location = new System.Drawing.Point(328, 12);
      this.dtpEmploymentStartDate.Name = "dtpEmploymentStartDate";
      this.dtpEmploymentStartDate.Size = new System.Drawing.Size(171, 20);
      this.dtpEmploymentStartDate.TabIndex = 32;
      // 
      // cmbDepartman
      // 
      this.cmbDepartment.FormattingEnabled = true;
      this.cmbDepartment.Location = new System.Drawing.Point(328, 39);
      this.cmbDepartment.Name = "cmbDepartment";
      this.cmbDepartment.Size = new System.Drawing.Size(171, 21);
      this.cmbDepartment.TabIndex = 33;
      this.cmbDepartment.SelectedIndexChanged += new System.EventHandler(this.CmbDepartmentSelectedIndexChanged);
      // 
      // cmbPozisyon
      // 
      this.cmbPosition.FormattingEnabled = true;
      this.cmbPosition.Location = new System.Drawing.Point(328, 69);
      this.cmbPosition.Name = "cmbPosition";
      this.cmbPosition.Size = new System.Drawing.Size(171, 21);
      this.cmbPosition.TabIndex = 34;
      // 
      // cmbVardiya
      // 
      this.cmbShift.FormattingEnabled = true;
      this.cmbShift.Location = new System.Drawing.Point(328, 96);
      this.cmbShift.Name = "cmbShift";
      this.cmbShift.Size = new System.Drawing.Size(171, 21);
      this.cmbShift.TabIndex = 35;
      // 
      // cmbCinsiyet
      // 
      this.cmbGender.FormattingEnabled = true;
      this.cmbGender.Location = new System.Drawing.Point(328, 125);
      this.cmbGender.Name = "cmbGender";
      this.cmbGender.Size = new System.Drawing.Size(171, 21);
      this.cmbGender.TabIndex = 36;
      // 
      // txtAra
      // 
      this.txtSearch.Location = new System.Drawing.Point(225, 184);
      this.txtSearch.Name = "txtSearch";
      this.txtSearch.Size = new System.Drawing.Size(171, 20);
      this.txtSearch.TabIndex = 37;
      this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearchTextChanged);
      this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSearchKeyDown);
      // 
      // btnAra
      // 
      this.btnSearch.Location = new System.Drawing.Point(402, 182);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(97, 23);
      this.btnSearch.TabIndex = 38;
      UiText.Bind(this.btnSearch, "Search");
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.BtnSearchClick);
      this.btnSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BtnSearchKeyDown);
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(12, 187);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(81, 13);
      this.label13.TabIndex = 39;
      UiText.Bind(this.label13, "AnnualLeaveAllowance");
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Location = new System.Drawing.Point(91, 187);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(10, 13);
      this.label19.TabIndex = 40;
      this.label19.Text = ":";
      // 
      // txtYillikIzinHakki
      // 
      this.txtAnnualLeaveAllowance.Location = new System.Drawing.Point(107, 183);
      this.txtAnnualLeaveAllowance.Name = "txtAnnualLeaveAllowance";
      this.txtAnnualLeaveAllowance.Size = new System.Drawing.Size(100, 20);
      this.txtAnnualLeaveAllowance.TabIndex = 41;
      // 
      // label25
      // 
      this.label25.AutoSize = true;
      this.label25.Location = new System.Drawing.Point(311, 160);
      this.label25.Name = "label25";
      this.label25.Size = new System.Drawing.Size(10, 13);
      this.label25.TabIndex = 43;
      this.label25.Text = ":";
      // 
      // label26
      // 
      this.label26.AutoSize = true;
      this.label26.Location = new System.Drawing.Point(222, 160);
      this.label26.Name = "label26";
      this.label26.Size = new System.Drawing.Size(70, 13);
      this.label26.TabIndex = 42;
      UiText.Bind(this.label26, "DateOfBirth");
      // 
      // dtpDogumTarihi
      // 
      this.dtpBirthDate.Location = new System.Drawing.Point(328, 156);
      this.dtpBirthDate.Name = "dtpBirthDate";
      this.dtpBirthDate.Size = new System.Drawing.Size(171, 20);
      this.dtpBirthDate.TabIndex = 44;
      // 
      // label27
      // 
      this.label27.AutoSize = true;
      this.label27.Location = new System.Drawing.Point(611, 183);
      this.label27.Name = "label27";
      this.label27.Size = new System.Drawing.Size(10, 13);
      this.label27.TabIndex = 46;
      this.label27.Text = ":";
      // 
      // label28
      // 
      this.label28.AutoSize = true;
      this.label28.Location = new System.Drawing.Point(505, 183);
      this.label28.Name = "label28";
      this.label28.Size = new System.Drawing.Size(116, 13);
      this.label28.TabIndex = 45;
      UiText.Bind(this.label28, "TotalEmployees");
      // 
      // lblToplamKayit
      // 
      this.lblTotalRecords.AutoSize = true;
      this.lblTotalRecords.Location = new System.Drawing.Point(627, 183);
      this.lblTotalRecords.Name = "lblTotalRecords";
      this.lblTotalRecords.Size = new System.Drawing.Size(10, 13);
      this.lblTotalRecords.TabIndex = 47;
      this.lblTotalRecords.Text = ".";
      // 
      // frmPersonnelManagement
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(793, 465);
      this.ControlBox = false;
      this.Controls.Add(this.lblTotalRecords);
      this.Controls.Add(this.label27);
      this.Controls.Add(this.label28);
      this.Controls.Add(this.dtpBirthDate);
      this.Controls.Add(this.label25);
      this.Controls.Add(this.label26);
      this.Controls.Add(this.txtAnnualLeaveAllowance);
      this.Controls.Add(this.label19);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.btnSearch);
      this.Controls.Add(this.txtSearch);
      this.Controls.Add(this.cmbGender);
      this.Controls.Add(this.cmbShift);
      this.Controls.Add(this.cmbPosition);
      this.Controls.Add(this.cmbDepartment);
      this.Controls.Add(this.dtpEmploymentStartDate);
      this.Controls.Add(this.label20);
      this.Controls.Add(this.label21);
      this.Controls.Add(this.label22);
      this.Controls.Add(this.label23);
      this.Controls.Add(this.label24);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.label17);
      this.Controls.Add(this.label18);
      this.Controls.Add(this.txtAddress);
      this.Controls.Add(this.txtEmail);
      this.Controls.Add(this.txtPhone);
      this.Controls.Add(this.txtNationalId);
      this.Controls.Add(this.txtLastName);
      this.Controls.Add(this.txtFirstName);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.panel3);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "EmployeeManagementForm";
      this.Text = "PersonnelManagement";
      this.Load += new System.EventHandler(this.EmployeeManagementFormLoad);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
      this.panel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

#endregion
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnDeactivate;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Button btnSelectPhoto;
    private System.Windows.Forms.PictureBox picPhoto;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.TextBox txtFirstName;
    private System.Windows.Forms.TextBox txtLastName;
    private System.Windows.Forms.TextBox txtNationalId;
    private System.Windows.Forms.TextBox txtPhone;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.TextBox txtAddress;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.Label label20;
    private System.Windows.Forms.Label label21;
    private System.Windows.Forms.Label label22;
    private System.Windows.Forms.Label label23;
    private System.Windows.Forms.Label label24;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.DateTimePicker dtpEmploymentStartDate;
    private System.Windows.Forms.ComboBox cmbDepartment;
    private System.Windows.Forms.ComboBox cmbPosition;
    private System.Windows.Forms.ComboBox cmbShift;
    private System.Windows.Forms.ComboBox cmbGender;
    private System.Windows.Forms.TextBox txtSearch;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.DataGridView dgvEmployees;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.TextBox txtAnnualLeaveAllowance;
    private System.Windows.Forms.Label label25;
    private System.Windows.Forms.Label label26;
    private System.Windows.Forms.DateTimePicker dtpBirthDate;
    private System.Windows.Forms.Label label27;
    private System.Windows.Forms.Label label28;
    private System.Windows.Forms.Label lblTotalRecords;
  }
}