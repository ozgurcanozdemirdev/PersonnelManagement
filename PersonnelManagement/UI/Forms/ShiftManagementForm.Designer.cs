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
  partial class ShiftManagementForm
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
      this.btnSaveShift = new System.Windows.Forms.Button();
      this.btnUpdateShift = new System.Windows.Forms.Button();
      this.btnDeactivateShift = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.btnAllWeek = new System.Windows.Forms.Button();
      this.btnWeekdays = new System.Windows.Forms.Button();
      this.btnWeekend = new System.Windows.Forms.Button();
      this.btnSelectNone = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.txtShiftName = new System.Windows.Forms.TextBox();
      this.txtBreakMinutes = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
      this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
      this.chkSunday = new System.Windows.Forms.CheckBox();
      this.chkSaturday = new System.Windows.Forms.CheckBox();
      this.chkFriday = new System.Windows.Forms.CheckBox();
      this.chkThursday = new System.Windows.Forms.CheckBox();
      this.chkWednesday = new System.Windows.Forms.CheckBox();
      this.chkTuesday = new System.Windows.Forms.CheckBox();
      this.chkMonday = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvShifts = new System.Windows.Forms.DataGridView();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.lblTotalRecords = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvShifts)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnSaveShift);
      this.panel1.Controls.Add(this.btnSelectNone);
      this.panel1.Controls.Add(this.btnClear);
      this.panel1.Controls.Add(this.btnDeactivateShift);
      this.panel1.Controls.Add(this.btnWeekend);
      this.panel1.Controls.Add(this.btnAllWeek);
      this.panel1.Controls.Add(this.btnUpdateShift);
      this.panel1.Controls.Add(this.btnWeekdays);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 395);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(747, 57);
      this.panel1.TabIndex = 0;
      // 
      // btnVardiyaKaydet
      // 
      this.btnSaveShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnSaveShift.Location = new System.Drawing.Point(3, 3);
      this.btnSaveShift.Name = "btnSaveShift";
      this.btnSaveShift.Size = new System.Drawing.Size(77, 51);
      this.btnSaveShift.TabIndex = 0;
      UiText.Bind(this.btnSaveShift, "Save");
      this.btnSaveShift.UseVisualStyleBackColor = true;
      this.btnSaveShift.Click += new System.EventHandler(this.BtnSaveShiftClick);
      // 
      // btnVardiyaGuncelle
      // 
      this.btnUpdateShift.Location = new System.Drawing.Point(86, 3);
      this.btnUpdateShift.Name = "btnUpdateShift";
      this.btnUpdateShift.Size = new System.Drawing.Size(77, 51);
      this.btnUpdateShift.TabIndex = 1;
      UiText.Bind(this.btnUpdateShift, "Update");
      this.btnUpdateShift.UseVisualStyleBackColor = true;
      this.btnUpdateShift.Click += new System.EventHandler(this.BtnUpdateShiftClick);
      // 
      // btnVardiyaSil
      // 
      this.btnDeactivateShift.Location = new System.Drawing.Point(169, 3);
      this.btnDeactivateShift.Name = "btnDeactivateShift";
      this.btnDeactivateShift.Size = new System.Drawing.Size(77, 51);
      this.btnDeactivateShift.TabIndex = 2;
      UiText.Bind(this.btnDeactivateShift, "Deactivate");
      this.btnDeactivateShift.UseVisualStyleBackColor = true;
      this.btnDeactivateShift.Click += new System.EventHandler(this.BtnDeactivateShiftClick);
      // 
      // btnTemizle
      // 
      this.btnClear.Location = new System.Drawing.Point(252, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(77, 51);
      this.btnClear.TabIndex = 3;
      UiText.Bind(this.btnClear, "ClearForm");
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.BtnClearClick);
      // 
      // btnTumHafta
      // 
      this.btnAllWeek.Location = new System.Drawing.Point(335, 3);
      this.btnAllWeek.Name = "btnAllWeek";
      this.btnAllWeek.Size = new System.Drawing.Size(77, 51);
      this.btnAllWeek.TabIndex = 4;
      UiText.Bind(this.btnAllWeek, "SelectAllDays");
      this.btnAllWeek.UseVisualStyleBackColor = true;
      this.btnAllWeek.Click += new System.EventHandler(this.BtnAllWeekClick);
      // 
      // btnHaftaIci
      // 
      this.btnWeekdays.Location = new System.Drawing.Point(418, 3);
      this.btnWeekdays.Name = "btnWeekdays";
      this.btnWeekdays.Size = new System.Drawing.Size(77, 51);
      this.btnWeekdays.TabIndex = 5;
      UiText.Bind(this.btnWeekdays, "SelectMonFri");
      this.btnWeekdays.UseVisualStyleBackColor = true;
      this.btnWeekdays.Click += new System.EventHandler(this.BtnWeekdaysClick);
      // 
      // btnHaftaSonu
      // 
      this.btnWeekend.Location = new System.Drawing.Point(501, 3);
      this.btnWeekend.Name = "btnWeekend";
      this.btnWeekend.Size = new System.Drawing.Size(77, 51);
      this.btnWeekend.TabIndex = 6;
      UiText.Bind(this.btnWeekend, "SelectSatSun");
      this.btnWeekend.UseVisualStyleBackColor = true;
      this.btnWeekend.Click += new System.EventHandler(this.BtnWeekendClick);
      // 
      // btnHicbiri
      // 
      this.btnSelectNone.Location = new System.Drawing.Point(584, 3);
      this.btnSelectNone.Name = "btnSelectNone";
      this.btnSelectNone.Size = new System.Drawing.Size(77, 51);
      this.btnSelectNone.TabIndex = 7;
      UiText.Bind(this.btnSelectNone, "ClearSelection");
      this.btnSelectNone.UseVisualStyleBackColor = true;
      this.btnSelectNone.Click += new System.EventHandler(this.BtnSelectNoneClick);
      // 
      // btnKapat
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(667, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(77, 51);
      this.btnClose.TabIndex = 8;
      UiText.Bind(this.btnClose, "Close");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 10);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(63, 13);
      this.label1.TabIndex = 2;
      UiText.Bind(this.label1, "ShiftName2");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 44);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(65, 13);
      this.label2.TabIndex = 3;
      UiText.Bind(this.label2, "BreakDuration");
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(100, 10);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(10, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = ":";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(100, 47);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(10, 13);
      this.label4.TabIndex = 5;
      this.label4.Text = ":";
      // 
      // txtVardiyaAdi
      // 
      this.txtShiftName.Location = new System.Drawing.Point(116, 12);
      this.txtShiftName.Name = "txtShiftName";
      this.txtShiftName.Size = new System.Drawing.Size(173, 20);
      this.txtShiftName.TabIndex = 6;
      // 
      // txtMolaSuresi
      // 
      this.txtBreakMinutes.Location = new System.Drawing.Point(116, 44);
      this.txtBreakMinutes.Name = "txtBreakMinutes";
      this.txtBreakMinutes.Size = new System.Drawing.Size(173, 20);
      this.txtBreakMinutes.TabIndex = 7;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(100, 108);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(10, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = ":";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(100, 78);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(10, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = ":";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(12, 108);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(53, 13);
      this.label7.TabIndex = 9;
      UiText.Bind(this.label7, "EndTime");
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(12, 74);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(80, 13);
      this.label8.TabIndex = 8;
      UiText.Bind(this.label8, "StartTime");
      // 
      // dtpBaslangicSaati
      // 
      this.dtpStartTime.Location = new System.Drawing.Point(116, 71);
      this.dtpStartTime.Name = "dtpStartTime";
      this.dtpStartTime.Size = new System.Drawing.Size(173, 20);
      this.dtpStartTime.TabIndex = 12;
      // 
      // dtpBitisSaati
      // 
      this.dtpEndTime.Location = new System.Drawing.Point(116, 105);
      this.dtpEndTime.Name = "dtpEndTime";
      this.dtpEndTime.Size = new System.Drawing.Size(173, 20);
      this.dtpEndTime.TabIndex = 13;
      // 
      // chkPazar
      // 
      this.chkSunday.AutoSize = true;
      this.chkSunday.Location = new System.Drawing.Point(6, 211);
      this.chkSunday.Name = "chkSunday";
      this.chkSunday.Size = new System.Drawing.Size(53, 17);
      this.chkSunday.TabIndex = 28;
      UiText.Bind(this.chkSunday, "Sunday");
      this.chkSunday.UseVisualStyleBackColor = true;
      // 
      // chkCumartesi
      // 
      this.chkSaturday.AutoSize = true;
      this.chkSaturday.Location = new System.Drawing.Point(6, 179);
      this.chkSaturday.Name = "chkSaturday";
      this.chkSaturday.Size = new System.Drawing.Size(72, 17);
      this.chkSaturday.TabIndex = 29;
      UiText.Bind(this.chkSaturday, "Saturday");
      this.chkSaturday.UseVisualStyleBackColor = true;
      // 
      // chkCuma
      // 
      this.chkFriday.AutoSize = true;
      this.chkFriday.Location = new System.Drawing.Point(6, 147);
      this.chkFriday.Name = "chkFriday";
      this.chkFriday.Size = new System.Drawing.Size(53, 17);
      this.chkFriday.TabIndex = 30;
      UiText.Bind(this.chkFriday, "Friday");
      this.chkFriday.UseVisualStyleBackColor = true;
      // 
      // chkPersembe
      // 
      this.chkThursday.AutoSize = true;
      this.chkThursday.Location = new System.Drawing.Point(6, 115);
      this.chkThursday.Name = "chkThursday";
      this.chkThursday.Size = new System.Drawing.Size(73, 17);
      this.chkThursday.TabIndex = 31;
      UiText.Bind(this.chkThursday, "Thursday");
      this.chkThursday.UseVisualStyleBackColor = true;
      // 
      // chkCarsamba
      // 
      this.chkWednesday.AutoSize = true;
      this.chkWednesday.Location = new System.Drawing.Point(6, 83);
      this.chkWednesday.Name = "chkWednesday";
      this.chkWednesday.Size = new System.Drawing.Size(73, 17);
      this.chkWednesday.TabIndex = 32;
      UiText.Bind(this.chkWednesday, "Wednesday");
      this.chkWednesday.UseVisualStyleBackColor = true;
      // 
      // chkSali
      // 
      this.chkTuesday.AutoSize = true;
      this.chkTuesday.Location = new System.Drawing.Point(6, 51);
      this.chkTuesday.Name = "chkTuesday";
      this.chkTuesday.Size = new System.Drawing.Size(43, 17);
      this.chkTuesday.TabIndex = 33;
      UiText.Bind(this.chkTuesday, "Tuesday");
      this.chkTuesday.UseVisualStyleBackColor = true;
      // 
      // chkPazartesi
      // 
      this.chkMonday.AutoSize = true;
      this.chkMonday.Location = new System.Drawing.Point(6, 19);
      this.chkMonday.Name = "chkMonday";
      this.chkMonday.Size = new System.Drawing.Size(69, 17);
      this.chkMonday.TabIndex = 34;
      UiText.Bind(this.chkMonday, "Monday");
      this.chkMonday.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.chkMonday);
      this.groupBox1.Controls.Add(this.chkSunday);
      this.groupBox1.Controls.Add(this.chkTuesday);
      this.groupBox1.Controls.Add(this.chkSaturday);
      this.groupBox1.Controls.Add(this.chkWednesday);
      this.groupBox1.Controls.Add(this.chkFriday);
      this.groupBox1.Controls.Add(this.chkThursday);
      this.groupBox1.Location = new System.Drawing.Point(15, 153);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(271, 236);
      this.groupBox1.TabIndex = 35;
      this.groupBox1.TabStop = false;
      UiText.Bind(this.groupBox1, "WorkingDays");
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvShifts);
      this.panel2.Location = new System.Drawing.Point(295, 12);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(449, 380);
      this.panel2.TabIndex = 36;
      // 
      // dgvVardiya
      // 
      this.dgvShifts.AllowUserToAddRows = false;
      this.dgvShifts.AllowUserToDeleteRows = false;
      this.dgvShifts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvShifts.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvShifts.Location = new System.Drawing.Point(0, 0);
      this.dgvShifts.Name = "dgvShifts";
      this.dgvShifts.ReadOnly = true;
      this.dgvShifts.Size = new System.Drawing.Size(449, 380);
      this.dgvShifts.TabIndex = 0;
      this.dgvShifts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvShiftsCellClick);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(12, 137);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(68, 13);
      this.label9.TabIndex = 37;
      UiText.Bind(this.label9, "TotalRecords");
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(100, 137);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(10, 13);
      this.label10.TabIndex = 38;
      this.label10.Text = ":";
      // 
      // lblToplamKayit
      // 
      this.lblTotalRecords.AutoSize = true;
      this.lblTotalRecords.Location = new System.Drawing.Point(116, 137);
      this.lblTotalRecords.Name = "lblTotalRecords";
      this.lblTotalRecords.Size = new System.Drawing.Size(10, 13);
      this.lblTotalRecords.TabIndex = 39;
      this.lblTotalRecords.Text = ".";
      // 
      // frmVardiyaYonetim
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(747, 452);
      this.ControlBox = false;
      this.Controls.Add(this.lblTotalRecords);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.dtpEndTime);
      this.Controls.Add(this.dtpStartTime);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.txtBreakMinutes);
      this.Controls.Add(this.txtShiftName);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.panel1);
      this.Name = "ShiftManagementForm";
      UiText.Bind(this, "ShiftManagement");
      this.Load += new System.EventHandler(this.ShiftManagementFormLoad);
      this.panel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvShifts)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

#endregion
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSelectNone;
    private System.Windows.Forms.Button btnWeekend;
    private System.Windows.Forms.Button btnWeekdays;
    private System.Windows.Forms.Button btnAllWeek;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnDeactivateShift;
    private System.Windows.Forms.Button btnUpdateShift;
    private System.Windows.Forms.Button btnSaveShift;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtShiftName;
    private System.Windows.Forms.TextBox txtBreakMinutes;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.DateTimePicker dtpStartTime;
    private System.Windows.Forms.DateTimePicker dtpEndTime;
    private System.Windows.Forms.CheckBox chkSunday;
    private System.Windows.Forms.CheckBox chkSaturday;
    private System.Windows.Forms.CheckBox chkFriday;
    private System.Windows.Forms.CheckBox chkThursday;
    private System.Windows.Forms.CheckBox chkWednesday;
    private System.Windows.Forms.CheckBox chkTuesday;
    private System.Windows.Forms.CheckBox chkMonday;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.DataGridView dgvShifts;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label lblTotalRecords;
  }
}