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
  partial class MainForm
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
      this.btnRefresh = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lblDateTime = new System.Windows.Forms.Label();
      this.lblWelcome = new System.Windows.Forms.Label();
      this.btnExit = new System.Windows.Forms.Button();
      this.btnEmployeeManagement = new System.Windows.Forms.Button();
      this.btnShiftManagement = new System.Windows.Forms.Button();
      this.btnAttendance = new System.Windows.Forms.Button();
      this.btnReports = new System.Windows.Forms.Button();
      this.btnLeaveManagement = new System.Windows.Forms.Button();
      this.btnUsers = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.lblTodayAbsences = new System.Windows.Forms.Label();
      this.lblTodayLeaves = new System.Windows.Forms.Label();
      this.lblTodayCheckIns = new System.Windows.Forms.Label();
      this.lblTotalEmployees = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnRefresh);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Controls.Add(this.label4);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.lblDateTime);
      this.panel1.Controls.Add(this.lblWelcome);
      this.panel1.Controls.Add(this.btnExit);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 195);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(506, 58);
      this.panel1.TabIndex = 0;
      // 
      // btnYenile
      // 
      this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRefresh.Location = new System.Drawing.Point(351, 4);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(73, 51);
      this.btnRefresh.TabIndex = 7;
      UiText.Bind(this.btnRefresh, "Refresh");
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.BtnRefreshClick);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(121, 35);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(10, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = ":";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(121, 13);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(10, 13);
      this.label4.TabIndex = 5;
      this.label4.Text = ":";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 35);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 13);
      this.label1.TabIndex = 4;
      UiText.Bind(this.label1, "DateAndTime");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 13);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(103, 13);
      this.label2.TabIndex = 3;
      UiText.Bind(this.label2, "RecordedBy");
      // 
      // lblTarihSaat
      // 
      this.lblDateTime.AutoSize = true;
      this.lblDateTime.Location = new System.Drawing.Point(130, 35);
      this.lblDateTime.Name = "lblDateTime";
      this.lblDateTime.Size = new System.Drawing.Size(10, 13);
      this.lblDateTime.TabIndex = 2;
      this.lblDateTime.Text = ".";
      // 
      // lblHosgeldin
      // 
      this.lblWelcome.AutoSize = true;
      this.lblWelcome.Location = new System.Drawing.Point(130, 13);
      this.lblWelcome.Name = "lblWelcome";
      this.lblWelcome.Size = new System.Drawing.Size(10, 13);
      this.lblWelcome.TabIndex = 1;
      this.lblWelcome.Text = ".";
      // 
      // btnCikis
      // 
      this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExit.Location = new System.Drawing.Point(430, 4);
      this.btnExit.Name = "btnExit";
      this.btnExit.Size = new System.Drawing.Size(73, 51);
      this.btnExit.TabIndex = 0;
      UiText.Bind(this.btnExit, "Exit");
      this.btnExit.UseVisualStyleBackColor = true;
      this.btnExit.Click += new System.EventHandler(this.BtnExitClick);
      // 
      // btnPersonnelManagement
      // 
      this.btnEmployeeManagement.Location = new System.Drawing.Point(8, 12);
      this.btnEmployeeManagement.Name = "btnEmployeeManagement";
      this.btnEmployeeManagement.Size = new System.Drawing.Size(132, 35);
      this.btnEmployeeManagement.TabIndex = 1;
      UiText.Bind(this.btnEmployeeManagement, "Employees2");
      this.btnEmployeeManagement.UseVisualStyleBackColor = true;
      this.btnEmployeeManagement.Click += new System.EventHandler(this.BtnEmployeeManagementClick);
      // 
      // btnVardiyaYonetim
      // 
      this.btnShiftManagement.Location = new System.Drawing.Point(152, 12);
      this.btnShiftManagement.Name = "btnShiftManagement";
      this.btnShiftManagement.Size = new System.Drawing.Size(75, 35);
      this.btnShiftManagement.TabIndex = 2;
      UiText.Bind(this.btnShiftManagement, "ShiftManagement2");
      this.btnShiftManagement.UseVisualStyleBackColor = true;
      this.btnShiftManagement.Click += new System.EventHandler(this.BtnShiftManagementClick);
      // 
      // btnGirisKayit
      // 
      this.btnAttendance.Location = new System.Drawing.Point(241, 12);
      this.btnAttendance.Name = "btnAttendance";
      this.btnAttendance.Size = new System.Drawing.Size(75, 35);
      this.btnAttendance.TabIndex = 3;
      UiText.Bind(this.btnAttendance, "Attendance2");
      this.btnAttendance.UseVisualStyleBackColor = true;
      this.btnAttendance.Click += new System.EventHandler(this.BtnAttendanceClick);
      // 
      // btnRaporlar
      // 
      this.btnReports.Location = new System.Drawing.Point(330, 12);
      this.btnReports.Name = "btnReports";
      this.btnReports.Size = new System.Drawing.Size(75, 35);
      this.btnReports.TabIndex = 4;
      UiText.Bind(this.btnReports, "Reports2");
      this.btnReports.UseVisualStyleBackColor = true;
      this.btnReports.Click += new System.EventHandler(this.BtnReportsClick);
      // 
      // btnIzinYonetimi
      // 
      this.btnLeaveManagement.Location = new System.Drawing.Point(419, 12);
      this.btnLeaveManagement.Name = "btnLeaveManagement";
      this.btnLeaveManagement.Size = new System.Drawing.Size(75, 35);
      this.btnLeaveManagement.TabIndex = 5;
      UiText.Bind(this.btnLeaveManagement, "LeaveManagement2");
      this.btnLeaveManagement.UseVisualStyleBackColor = true;
      this.btnLeaveManagement.Click += new System.EventHandler(this.BtnLeaveManagementClick);
      // 
      // btnKullanicilar
      // 
      this.btnUsers.Location = new System.Drawing.Point(330, 66);
      this.btnUsers.Name = "btnUsers";
      this.btnUsers.Size = new System.Drawing.Size(75, 35);
      this.btnUsers.TabIndex = 16;
      UiText.Bind(this.btnUsers, "Users2");
      this.btnUsers.UseVisualStyleBackColor = true;
      this.btnUsers.Click += new System.EventHandler(this.BtnUsersClick);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 71);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(116, 13);
      this.label5.TabIndex = 6;
      UiText.Bind(this.label5, "TotalEmployees");
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(12, 96);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(128, 13);
      this.label6.TabIndex = 7;
      UiText.Bind(this.label6, "CheckedInToday");
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(12, 121);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(94, 13);
      this.label7.TabIndex = 8;
      UiText.Bind(this.label7, "OnLeaveToday");
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(12, 146);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(117, 13);
      this.label8.TabIndex = 9;
      UiText.Bind(this.label8, "AbsentToday");
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(146, 96);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(10, 13);
      this.label9.TabIndex = 9;
      this.label9.Text = ":";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(146, 71);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(10, 13);
      this.label10.TabIndex = 8;
      this.label10.Text = ":";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(146, 146);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(10, 13);
      this.label11.TabIndex = 11;
      this.label11.Text = ":";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(146, 121);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(10, 13);
      this.label12.TabIndex = 10;
      this.label12.Text = ":";
      // 
      // lblBugunDevamsiz
      // 
      this.lblTodayAbsences.AutoSize = true;
      this.lblTodayAbsences.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
      this.lblTodayAbsences.Location = new System.Drawing.Point(162, 146);
      this.lblTodayAbsences.Name = "lblTodayAbsences";
      this.lblTodayAbsences.Size = new System.Drawing.Size(11, 13);
      this.lblTodayAbsences.TabIndex = 15;
      this.lblTodayAbsences.Text = ".";
      // 
      // lblBugunIzinli
      // 
      this.lblTodayLeaves.AutoSize = true;
      this.lblTodayLeaves.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
      this.lblTodayLeaves.Location = new System.Drawing.Point(162, 121);
      this.lblTodayLeaves.Name = "lblTodayLeaves";
      this.lblTodayLeaves.Size = new System.Drawing.Size(11, 13);
      this.lblTodayLeaves.TabIndex = 14;
      this.lblTodayLeaves.Text = ".";
      // 
      // lblBugunGiren
      // 
      this.lblTodayCheckIns.AutoSize = true;
      this.lblTodayCheckIns.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
      this.lblTodayCheckIns.Location = new System.Drawing.Point(162, 96);
      this.lblTodayCheckIns.Name = "lblTodayCheckIns";
      this.lblTodayCheckIns.Size = new System.Drawing.Size(11, 13);
      this.lblTodayCheckIns.TabIndex = 13;
      this.lblTodayCheckIns.Text = ".";
      // 
      // lblToplamPersonel
      // 
      this.lblTotalEmployees.AutoSize = true;
      this.lblTotalEmployees.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
      this.lblTotalEmployees.Location = new System.Drawing.Point(162, 71);
      this.lblTotalEmployees.Name = "lblTotalEmployees";
      this.lblTotalEmployees.Size = new System.Drawing.Size(11, 13);
      this.lblTotalEmployees.TabIndex = 12;
      this.lblTotalEmployees.Text = ".";
      // 
      // frmAnaMenu
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(506, 253);
      this.ControlBox = false;
      this.Controls.Add(this.lblTodayAbsences);
      this.Controls.Add(this.lblTodayLeaves);
      this.Controls.Add(this.lblTodayCheckIns);
      this.Controls.Add(this.lblTotalEmployees);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.btnUsers);
      this.Controls.Add(this.btnLeaveManagement);
      this.Controls.Add(this.btnReports);
      this.Controls.Add(this.btnAttendance);
      this.Controls.Add(this.btnShiftManagement);
      this.Controls.Add(this.btnEmployeeManagement);
      this.Controls.Add(this.panel1);
      this.Name = "MainForm";
      UiText.Bind(this, "MainMenu");
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
      this.Load += new System.EventHandler(this.MainFormLoad);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

#endregion
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label lblDateTime;
    private System.Windows.Forms.Label lblWelcome;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnEmployeeManagement;
    private System.Windows.Forms.Button btnShiftManagement;
    private System.Windows.Forms.Button btnAttendance;
    private System.Windows.Forms.Button btnReports;
    private System.Windows.Forms.Button btnLeaveManagement;
    private System.Windows.Forms.Button btnUsers;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label lblTodayAbsences;
    private System.Windows.Forms.Label lblTodayLeaves;
    private System.Windows.Forms.Label lblTodayCheckIns;
    private System.Windows.Forms.Label lblTotalEmployees;
  }
}