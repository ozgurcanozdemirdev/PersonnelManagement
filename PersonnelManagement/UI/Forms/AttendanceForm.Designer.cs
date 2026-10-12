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
  partial class AttendanceForm
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lblCurrentDate = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.lblCurrentTime = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.lblEmployeeDetails = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lblShiftDetails = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lblDepartmentDetails = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lblStatusDetails = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.lblTodayTotalCheckIns = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.btnCheckOut = new System.Windows.Forms.Button();
      this.btnCheckIn = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label20 = new System.Windows.Forms.Label();
      this.label19 = new System.Windows.Forms.Label();
      this.txtEmployeeSearch = new System.Windows.Forms.TextBox();
      this.btnFindEmployee = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvDailyRecords = new System.Windows.Forms.DataGridView();
      this.groupBox1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvDailyRecords)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lblCurrentDate);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.lblCurrentTime);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.lblEmployeeDetails);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.lblShiftDetails);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.lblDepartmentDetails);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.lblStatusDetails);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.lblTodayTotalCheckIns);
      this.groupBox1.Controls.Add(this.label12);
      this.groupBox1.Controls.Add(this.label7);
      this.groupBox1.Controls.Add(this.label11);
      this.groupBox1.Controls.Add(this.label8);
      this.groupBox1.Controls.Add(this.label10);
      this.groupBox1.Controls.Add(this.label9);
      this.groupBox1.Location = new System.Drawing.Point(3, 118);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(564, 221);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      UiText.Bind(this.groupBox1, "Details");
      // 
      // lblAnlikTarih
      // 
      this.lblCurrentDate.AutoSize = true;
      this.lblCurrentDate.Location = new System.Drawing.Point(289, 28);
      this.lblCurrentDate.Name = "lblCurrentDate";
      this.lblCurrentDate.Size = new System.Drawing.Size(10, 13);
      this.lblCurrentDate.TabIndex = 21;
      this.lblCurrentDate.Text = ".";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(7, 28);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(55, 13);
      this.label6.TabIndex = 6;
      UiText.Bind(this.label6, "CurrentTime");
      // 
      // lblAnlikSaat
      // 
      this.lblCurrentTime.AutoSize = true;
      this.lblCurrentTime.Location = new System.Drawing.Point(224, 28);
      this.lblCurrentTime.Name = "lblCurrentTime";
      this.lblCurrentTime.Size = new System.Drawing.Size(10, 13);
      this.lblCurrentTime.TabIndex = 18;
      this.lblCurrentTime.Text = ".";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 188);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(129, 13);
      this.label1.TabIndex = 1;
      UiText.Bind(this.label1, "TodaySCheckIns");
      // 
      // lblPersonelBilgi
      // 
      this.lblEmployeeDetails.AutoSize = true;
      this.lblEmployeeDetails.Location = new System.Drawing.Point(224, 60);
      this.lblEmployeeDetails.Name = "lblEmployeeDetails";
      this.lblEmployeeDetails.Size = new System.Drawing.Size(10, 13);
      this.lblEmployeeDetails.TabIndex = 17;
      this.lblEmployeeDetails.Text = ".";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(7, 156);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(199, 13);
      this.label2.TabIndex = 2;
      UiText.Bind(this.label2, "AtWorkCheckedOutNotArrived");
      // 
      // lblVardiyaBilgi
      // 
      this.lblShiftDetails.AutoSize = true;
      this.lblShiftDetails.Location = new System.Drawing.Point(224, 92);
      this.lblShiftDetails.Name = "lblShiftDetails";
      this.lblShiftDetails.Size = new System.Drawing.Size(10, 13);
      this.lblShiftDetails.TabIndex = 16;
      this.lblShiftDetails.Text = ".";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(7, 124);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(119, 13);
      this.label3.TabIndex = 3;
      UiText.Bind(this.label3, "DepartmentAndPosition");
      // 
      // lblDepartmanBilgi
      // 
      this.lblDepartmentDetails.AutoSize = true;
      this.lblDepartmentDetails.Location = new System.Drawing.Point(224, 124);
      this.lblDepartmentDetails.Name = "lblDepartmentDetails";
      this.lblDepartmentDetails.Size = new System.Drawing.Size(10, 13);
      this.lblDepartmentDetails.TabIndex = 15;
      this.lblDepartmentDetails.Text = ".";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(7, 92);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(95, 13);
      this.label4.TabIndex = 4;
      UiText.Bind(this.label4, "ShiftAndHours");
      // 
      // lblDurumBilgi
      // 
      this.lblStatusDetails.AutoSize = true;
      this.lblStatusDetails.Location = new System.Drawing.Point(224, 156);
      this.lblStatusDetails.Name = "lblStatusDetails";
      this.lblStatusDetails.Size = new System.Drawing.Size(10, 13);
      this.lblStatusDetails.TabIndex = 14;
      this.lblStatusDetails.Text = ".";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(7, 60);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(116, 13);
      this.label5.TabIndex = 5;
      UiText.Bind(this.label5, "SelectedEmployee");
      // 
      // lblGunlukToplamGiris
      // 
      this.lblTodayTotalCheckIns.AutoSize = true;
      this.lblTodayTotalCheckIns.Location = new System.Drawing.Point(224, 188);
      this.lblTodayTotalCheckIns.Name = "lblTodayTotalCheckIns";
      this.lblTodayTotalCheckIns.Size = new System.Drawing.Size(10, 13);
      this.lblTodayTotalCheckIns.TabIndex = 13;
      this.lblTodayTotalCheckIns.Text = ".";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(208, 188);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(10, 13);
      this.label12.TabIndex = 7;
      this.label12.Text = ":";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(208, 28);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(10, 13);
      this.label7.TabIndex = 12;
      this.label7.Text = ":";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(208, 156);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(10, 13);
      this.label11.TabIndex = 8;
      this.label11.Text = ":";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(208, 60);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(10, 13);
      this.label8.TabIndex = 11;
      this.label8.Text = ":";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(208, 124);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(10, 13);
      this.label10.TabIndex = 9;
      this.label10.Text = ":";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(208, 92);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(10, 13);
      this.label9.TabIndex = 10;
      this.label9.Text = ":";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnRefresh);
      this.panel1.Controls.Add(this.btnCheckOut);
      this.panel1.Controls.Add(this.btnCheckIn);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 345);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(1079, 56);
      this.panel1.TabIndex = 19;
      // 
      // btnKapat
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(1005, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(71, 50);
      this.btnClose.TabIndex = 4;
      UiText.Bind(this.btnClose, "Close");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      // 
      // btnYenile
      // 
      this.btnRefresh.Location = new System.Drawing.Point(157, 3);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(71, 50);
      this.btnRefresh.TabIndex = 2;
      UiText.Bind(this.btnRefresh, "Refresh");
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.BtnRefreshClick);
      // 
      // btnCikisYap
      // 
      this.btnCheckOut.Location = new System.Drawing.Point(80, 3);
      this.btnCheckOut.Name = "btnCheckOut";
      this.btnCheckOut.Size = new System.Drawing.Size(71, 50);
      this.btnCheckOut.TabIndex = 1;
      UiText.Bind(this.btnCheckOut, "CheckOut");
      this.btnCheckOut.UseVisualStyleBackColor = true;
      this.btnCheckOut.Click += new System.EventHandler(this.BtnCheckOutClick);
      // 
      // btnGirisYap
      // 
      this.btnCheckIn.Location = new System.Drawing.Point(3, 3);
      this.btnCheckIn.Name = "btnCheckIn";
      this.btnCheckIn.Size = new System.Drawing.Size(71, 50);
      this.btnCheckIn.TabIndex = 0;
      UiText.Bind(this.btnCheckIn, "CheckIn2");
      this.btnCheckIn.UseVisualStyleBackColor = true;
      this.btnCheckIn.Click += new System.EventHandler(this.BtnCheckInClick);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label20);
      this.groupBox2.Controls.Add(this.label19);
      this.groupBox2.Controls.Add(this.txtEmployeeSearch);
      this.groupBox2.Controls.Add(this.btnFindEmployee);
      this.groupBox2.Location = new System.Drawing.Point(3, 12);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(564, 100);
      this.groupBox2.TabIndex = 20;
      this.groupBox2.TabStop = false;
      UiText.Bind(this.groupBox2, "Search2");
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Location = new System.Drawing.Point(9, 84);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(244, 13);
      this.label20.TabIndex = 8;
      UiText.Bind(this.label20, "SearchByFirstNameLastNameOrNationalID");
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Location = new System.Drawing.Point(6, 23);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(103, 13);
      this.label19.TabIndex = 7;
      UiText.Bind(this.label19, "FindEmployee");
      // 
      // txtPersonelAra
      // 
      this.txtEmployeeSearch.Location = new System.Drawing.Point(115, 19);
      this.txtEmployeeSearch.Name = "txtEmployeeSearch";
      this.txtEmployeeSearch.Size = new System.Drawing.Size(100, 20);
      this.txtEmployeeSearch.TabIndex = 6;
      this.txtEmployeeSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtEmployeeSearchKeyDown);
      // 
      // btnPersonelBul
      // 
      this.btnFindEmployee.Location = new System.Drawing.Point(112, 48);
      this.btnFindEmployee.Name = "btnFindEmployee";
      this.btnFindEmployee.Size = new System.Drawing.Size(106, 32);
      this.btnFindEmployee.TabIndex = 5;
      UiText.Bind(this.btnFindEmployee, "FindEmployee2");
      this.btnFindEmployee.UseVisualStyleBackColor = true;
      this.btnFindEmployee.Click += new System.EventHandler(this.BtnFindEmployeeClick);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvDailyRecords);
      this.panel2.Location = new System.Drawing.Point(573, 9);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(503, 330);
      this.panel2.TabIndex = 21;
      // 
      // dgvGunlukKayitlar
      // 
      this.dgvDailyRecords.AllowUserToAddRows = false;
      this.dgvDailyRecords.AllowUserToDeleteRows = false;
      this.dgvDailyRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvDailyRecords.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvDailyRecords.Location = new System.Drawing.Point(0, 0);
      this.dgvDailyRecords.Name = "dgvDailyRecords";
      this.dgvDailyRecords.ReadOnly = true;
      this.dgvDailyRecords.Size = new System.Drawing.Size(503, 330);
      this.dgvDailyRecords.TabIndex = 0;
      // 
      // frmGirisKayit
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1079, 401);
      this.ControlBox = false;
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.groupBox1);
      this.Name = "AttendanceForm";
      UiText.Bind(this, "Attendance");
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AttendanceFormFormClosing);
      this.Load += new System.EventHandler(this.AttendanceFormLoad);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvDailyRecords)).EndInit();
      this.ResumeLayout(false);
    }

#endregion
    private System.Windows.Forms.GroupBox groupBox1;
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
    private System.Windows.Forms.Label lblCurrentTime;
    private System.Windows.Forms.Label lblEmployeeDetails;
    private System.Windows.Forms.Label lblShiftDetails;
    private System.Windows.Forms.Label lblDepartmentDetails;
    private System.Windows.Forms.Label lblStatusDetails;
    private System.Windows.Forms.Label lblTodayTotalCheckIns;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.Button btnCheckOut;
    private System.Windows.Forms.Button btnCheckIn;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox txtEmployeeSearch;
    private System.Windows.Forms.Button btnFindEmployee;
    private System.Windows.Forms.Label label20;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label lblCurrentDate;
    private System.Windows.Forms.DataGridView dgvDailyRecords;
  }
}