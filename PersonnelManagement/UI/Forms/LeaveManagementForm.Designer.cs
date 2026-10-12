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
  partial class LeaveManagementForm
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
      this.btnClear = new System.Windows.Forms.Button();
      this.btnAllPending = new System.Windows.Forms.Button();
      this.btnAllLeaves = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnCancelLeave = new System.Windows.Forms.Button();
      this.btnReject = new System.Windows.Forms.Button();
      this.btnApprove = new System.Windows.Forms.Button();
      this.btnSaveLeave = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvLeaves = new System.Windows.Forms.DataGridView();
      this.cmbLeaveEmployee = new System.Windows.Forms.ComboBox();
      this.cmbLeaveType = new System.Windows.Forms.ComboBox();
      this.cmbEmployeeFilter = new System.Windows.Forms.ComboBox();
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
      this.dtpLeaveStartDate = new System.Windows.Forms.DateTimePicker();
      this.dtpLeaveEndDate = new System.Windows.Forms.DateTimePicker();
      this.txtLeaveDescription = new System.Windows.Forms.TextBox();
      this.label13 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.label16 = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.lblTotalDays = new System.Windows.Forms.Label();
      this.lblRemainingLeave = new System.Windows.Forms.Label();
      this.lblTotalLeave = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvLeaves)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClear);
      this.panel1.Controls.Add(this.btnAllPending);
      this.panel1.Controls.Add(this.btnAllLeaves);
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnCancelLeave);
      this.panel1.Controls.Add(this.btnReject);
      this.panel1.Controls.Add(this.btnApprove);
      this.panel1.Controls.Add(this.btnSaveLeave);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 353);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(719, 56);
      this.panel1.TabIndex = 1;
      // 
      // btnTemizle
      // 
      this.btnClear.Location = new System.Drawing.Point(463, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(71, 50);
      this.btnClear.TabIndex = 7;
      UiText.Bind(this.btnClear, "Clear");
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.BtnClearClick);
      // 
      // btnTumBekleyenler
      // 
      this.btnAllPending.Location = new System.Drawing.Point(386, 3);
      this.btnAllPending.Name = "btnAllPending";
      this.btnAllPending.Size = new System.Drawing.Size(71, 50);
      this.btnAllPending.TabIndex = 6;
      UiText.Bind(this.btnAllPending, "AllPendingRequests");
      this.btnAllPending.UseVisualStyleBackColor = true;
      this.btnAllPending.Click += new System.EventHandler(this.BtnAllPendingClick);
      // 
      // btnTumIzinler
      // 
      this.btnAllLeaves.Location = new System.Drawing.Point(309, 3);
      this.btnAllLeaves.Name = "btnAllLeaves";
      this.btnAllLeaves.Size = new System.Drawing.Size(71, 50);
      this.btnAllLeaves.TabIndex = 5;
      UiText.Bind(this.btnAllLeaves, "AllLeaveRequests");
      this.btnAllLeaves.UseVisualStyleBackColor = true;
      this.btnAllLeaves.Click += new System.EventHandler(this.BtnAllLeavesClick);
      // 
      // btnKapat
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(645, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(71, 50);
      this.btnClose.TabIndex = 4;
      UiText.Bind(this.btnClose, "Close");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      // 
      // btnIzinIptal
      // 
      this.btnCancelLeave.Location = new System.Drawing.Point(232, 3);
      this.btnCancelLeave.Name = "btnCancelLeave";
      this.btnCancelLeave.Size = new System.Drawing.Size(71, 50);
      this.btnCancelLeave.TabIndex = 3;
      UiText.Bind(this.btnCancelLeave, "CancelLeave");
      this.btnCancelLeave.UseVisualStyleBackColor = true;
      this.btnCancelLeave.Click += new System.EventHandler(this.BtnCancelLeaveClick);
      // 
      // btnReddet
      // 
      this.btnReject.Location = new System.Drawing.Point(157, 3);
      this.btnReject.Name = "btnReject";
      this.btnReject.Size = new System.Drawing.Size(71, 50);
      this.btnReject.TabIndex = 2;
      UiText.Bind(this.btnReject, "Reject");
      this.btnReject.UseVisualStyleBackColor = true;
      this.btnReject.Click += new System.EventHandler(this.BtnRejectClick);
      // 
      // btnOnayla
      // 
      this.btnApprove.Location = new System.Drawing.Point(80, 3);
      this.btnApprove.Name = "btnApprove";
      this.btnApprove.Size = new System.Drawing.Size(71, 50);
      this.btnApprove.TabIndex = 1;
      UiText.Bind(this.btnApprove, "Approve2");
      this.btnApprove.UseVisualStyleBackColor = true;
      this.btnApprove.Click += new System.EventHandler(this.BtnApproveClick);
      // 
      // btnIzinKaydet
      // 
      this.btnSaveLeave.Location = new System.Drawing.Point(3, 3);
      this.btnSaveLeave.Name = "btnSaveLeave";
      this.btnSaveLeave.Size = new System.Drawing.Size(71, 50);
      this.btnSaveLeave.TabIndex = 0;
      UiText.Bind(this.btnSaveLeave, "SaveLeave");
      this.btnSaveLeave.UseVisualStyleBackColor = true;
      this.btnSaveLeave.Click += new System.EventHandler(this.BtnSaveLeaveClick);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvLeaves);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel2.Location = new System.Drawing.Point(0, 232);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(719, 121);
      this.panel2.TabIndex = 2;
      // 
      // dgvIzinler
      // 
      this.dgvLeaves.AllowUserToAddRows = false;
      this.dgvLeaves.AllowUserToDeleteRows = false;
      this.dgvLeaves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvLeaves.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvLeaves.Location = new System.Drawing.Point(0, 0);
      this.dgvLeaves.Name = "dgvLeaves";
      this.dgvLeaves.ReadOnly = true;
      this.dgvLeaves.Size = new System.Drawing.Size(719, 121);
      this.dgvLeaves.TabIndex = 0;
      this.dgvLeaves.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvLeavesCellClick);
      // 
      // cmbIzinPersonel
      // 
      this.cmbLeaveEmployee.FormattingEnabled = true;
      this.cmbLeaveEmployee.Location = new System.Drawing.Point(170, 22);
      this.cmbLeaveEmployee.Name = "cmbLeaveEmployee";
      this.cmbLeaveEmployee.Size = new System.Drawing.Size(171, 21);
      this.cmbLeaveEmployee.TabIndex = 3;
      this.cmbLeaveEmployee.SelectedIndexChanged += new System.EventHandler(this.CmbLeaveEmployeeSelectedIndexChanged);
      // 
      // cmbIzinTuru
      // 
      this.cmbLeaveType.FormattingEnabled = true;
      this.cmbLeaveType.Location = new System.Drawing.Point(170, 54);
      this.cmbLeaveType.Name = "cmbLeaveType";
      this.cmbLeaveType.Size = new System.Drawing.Size(171, 21);
      this.cmbLeaveType.TabIndex = 4;
      // 
      // cmbFiltrePersonel
      // 
      this.cmbEmployeeFilter.FormattingEnabled = true;
      this.cmbEmployeeFilter.Location = new System.Drawing.Point(170, 86);
      this.cmbEmployeeFilter.Name = "cmbEmployeeFilter";
      this.cmbEmployeeFilter.Size = new System.Drawing.Size(171, 21);
      this.cmbEmployeeFilter.TabIndex = 5;
      this.cmbEmployeeFilter.SelectedIndexChanged += new System.EventHandler(this.CmbEmployeeFilterSelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 6;
      UiText.Bind(this.label1, "SelectEmployee");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 57);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(70, 13);
      this.label2.TabIndex = 7;
      UiText.Bind(this.label2, "SelectLeaveType");
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 89);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(145, 13);
      this.label3.TabIndex = 8;
      UiText.Bind(this.label3, "FilterByEmployee");
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(372, 25);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(101, 13);
      this.label4.TabIndex = 9;
      UiText.Bind(this.label4, "LeaveStartDate");
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(372, 57);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(74, 13);
      this.label5.TabIndex = 10;
      UiText.Bind(this.label5, "LeaveEndDate");
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(372, 92);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(69, 13);
      this.label6.TabIndex = 11;
      UiText.Bind(this.label6, "LeaveDescription");
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(514, 94);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(10, 13);
      this.label7.TabIndex = 12;
      this.label7.Text = ":";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(514, 57);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(10, 13);
      this.label8.TabIndex = 13;
      this.label8.Text = ":";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(514, 25);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(10, 13);
      this.label9.TabIndex = 14;
      this.label9.Text = ":";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(154, 89);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(10, 13);
      this.label10.TabIndex = 15;
      this.label10.Text = ":";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(154, 57);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(10, 13);
      this.label11.TabIndex = 16;
      this.label11.Text = ":";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(154, 25);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(10, 13);
      this.label12.TabIndex = 17;
      this.label12.Text = ":";
      // 
      // dtpIzinBaslangic
      // 
      this.dtpLeaveStartDate.Location = new System.Drawing.Point(531, 25);
      this.dtpLeaveStartDate.Name = "dtpLeaveStartDate";
      this.dtpLeaveStartDate.Size = new System.Drawing.Size(170, 20);
      this.dtpLeaveStartDate.TabIndex = 18;
      // 
      // dtpIzinBitis
      // 
      this.dtpLeaveEndDate.Location = new System.Drawing.Point(531, 57);
      this.dtpLeaveEndDate.Name = "dtpLeaveEndDate";
      this.dtpLeaveEndDate.Size = new System.Drawing.Size(170, 20);
      this.dtpLeaveEndDate.TabIndex = 19;
      // 
      // txtIzinAciklama
      // 
      this.txtLeaveDescription.Location = new System.Drawing.Point(531, 89);
      this.txtLeaveDescription.Name = "txtLeaveDescription";
      this.txtLeaveDescription.Size = new System.Drawing.Size(170, 20);
      this.txtLeaveDescription.TabIndex = 20;
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(15, 120);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(162, 13);
      this.label13.TabIndex = 21;
      UiText.Bind(this.label13, "CalculatedDays");
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(192, 120);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(10, 13);
      this.label14.TabIndex = 24;
      this.label14.Text = ":";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(192, 152);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(10, 13);
      this.label15.TabIndex = 23;
      this.label15.Text = ":";
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(192, 184);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(10, 13);
      this.label16.TabIndex = 22;
      this.label16.Text = ":";
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(15, 152);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(129, 13);
      this.label17.TabIndex = 25;
      UiText.Bind(this.label17, "RemainingAnnualLeave2");
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(15, 184);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(108, 13);
      this.label18.TabIndex = 26;
      UiText.Bind(this.label18, "RecordCount");
      // 
      // lblToplamGun
      // 
      this.lblTotalDays.AutoSize = true;
      this.lblTotalDays.Location = new System.Drawing.Point(208, 120);
      this.lblTotalDays.Name = "lblTotalDays";
      this.lblTotalDays.Size = new System.Drawing.Size(10, 13);
      this.lblTotalDays.TabIndex = 29;
      this.lblTotalDays.Text = ".";
      // 
      // lblKalanIzin
      // 
      this.lblRemainingLeave.AutoSize = true;
      this.lblRemainingLeave.Location = new System.Drawing.Point(208, 152);
      this.lblRemainingLeave.Name = "lblRemainingLeave";
      this.lblRemainingLeave.Size = new System.Drawing.Size(10, 13);
      this.lblRemainingLeave.TabIndex = 28;
      this.lblRemainingLeave.Text = ".";
      // 
      // lblToplamIzin
      // 
      this.lblTotalLeave.AutoSize = true;
      this.lblTotalLeave.Location = new System.Drawing.Point(208, 184);
      this.lblTotalLeave.Name = "lblTotalLeave";
      this.lblTotalLeave.Size = new System.Drawing.Size(10, 13);
      this.lblTotalLeave.TabIndex = 27;
      this.lblTotalLeave.Text = ".";
      // 
      // frmIzinYonetim
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(719, 409);
      this.Controls.Add(this.lblTotalDays);
      this.Controls.Add(this.lblRemainingLeave);
      this.Controls.Add(this.lblTotalLeave);
      this.Controls.Add(this.label18);
      this.Controls.Add(this.label17);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.txtLeaveDescription);
      this.Controls.Add(this.dtpLeaveEndDate);
      this.Controls.Add(this.dtpLeaveStartDate);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmbEmployeeFilter);
      this.Controls.Add(this.cmbLeaveType);
      this.Controls.Add(this.cmbLeaveEmployee);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "LeaveManagementForm";
      UiText.Bind(this, "LeaveManagement");
      this.Load += new System.EventHandler(this.LeaveManagementFormLoad);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvLeaves)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

#endregion
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnCancelLeave;
    private System.Windows.Forms.Button btnReject;
    private System.Windows.Forms.Button btnApprove;
    private System.Windows.Forms.Button btnSaveLeave;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnAllPending;
    private System.Windows.Forms.Button btnAllLeaves;
    private System.Windows.Forms.DataGridView dgvLeaves;
    private System.Windows.Forms.ComboBox cmbLeaveEmployee;
    private System.Windows.Forms.ComboBox cmbLeaveType;
    private System.Windows.Forms.ComboBox cmbEmployeeFilter;
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
    private System.Windows.Forms.DateTimePicker dtpLeaveStartDate;
    private System.Windows.Forms.DateTimePicker dtpLeaveEndDate;
    private System.Windows.Forms.TextBox txtLeaveDescription;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.Label lblTotalDays;
    private System.Windows.Forms.Label lblRemainingLeave;
    private System.Windows.Forms.Label lblTotalLeave;
  }
}