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
  partial class ReportsForm
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
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.btnGenerateReport = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.cmbReportEmployee = new System.Windows.Forms.ComboBox();
      this.cmbReportType = new System.Windows.Forms.ComboBox();
      this.dtpReportStartDate = new System.Windows.Forms.DateTimePicker();
      this.dtpReportEndDate = new System.Windows.Forms.DateTimePicker();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.panel2 = new System.Windows.Forms.Panel();
      this.label12 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.panel3 = new System.Windows.Forms.Panel();
      this.dgvReport = new System.Windows.Forms.DataGridView();
      this.lblReportDate = new System.Windows.Forms.Label();
      this.lblTotalAbsences = new System.Windows.Forms.Label();
      this.lblTotalOvertime = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnPrint);
      this.panel1.Controls.Add(this.btnExportExcel);
      this.panel1.Controls.Add(this.btnGenerateReport);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 250);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(800, 56);
      this.panel1.TabIndex = 20;
      // 
      // btnKapat
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(726, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(71, 50);
      this.btnClose.TabIndex = 4;
      UiText.Bind(this.btnClose, "Close");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      // 
      // btnYazdir
      // 
      this.btnPrint.Location = new System.Drawing.Point(185, 3);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(71, 50);
      this.btnPrint.TabIndex = 2;
      UiText.Bind(this.btnPrint, "Print");
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.BtnPrintClick);
      // 
      // btnExcelAktar
      // 
      this.btnExportExcel.Location = new System.Drawing.Point(80, 3);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(96, 50);
      this.btnExportExcel.TabIndex = 1;
      UiText.Bind(this.btnExportExcel, "ExportToExcelCSV");
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.BtnExportExcelClick);
      // 
      // btnRaporOlustur
      // 
      this.btnGenerateReport.Location = new System.Drawing.Point(3, 3);
      this.btnGenerateReport.Name = "btnGenerateReport";
      this.btnGenerateReport.Size = new System.Drawing.Size(71, 50);
      this.btnGenerateReport.TabIndex = 0;
      UiText.Bind(this.btnGenerateReport, "GenerateReport");
      this.btnGenerateReport.UseVisualStyleBackColor = true;
      this.btnGenerateReport.Click += new System.EventHandler(this.BtnGenerateReportClick);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(161, 13);
      this.label1.TabIndex = 21;
      UiText.Bind(this.label1, "SelectEmployeeAllFullName");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 44);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(83, 13);
      this.label2.TabIndex = 22;
      UiText.Bind(this.label2, "SelectReportType");
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 79);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(82, 13);
      this.label3.TabIndex = 23;
      UiText.Bind(this.label3, "StartDate");
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 111);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(55, 13);
      this.label4.TabIndex = 24;
      UiText.Bind(this.label4, "EndDate");
      // 
      // cmbRaporPersonel
      // 
      this.cmbReportEmployee.FormattingEnabled = true;
      this.cmbReportEmployee.Location = new System.Drawing.Point(195, 9);
      this.cmbReportEmployee.Name = "cmbReportEmployee";
      this.cmbReportEmployee.Size = new System.Drawing.Size(172, 21);
      this.cmbReportEmployee.TabIndex = 25;
      // 
      // cmbRaporTuru
      // 
      this.cmbReportType.FormattingEnabled = true;
      this.cmbReportType.Location = new System.Drawing.Point(195, 41);
      this.cmbReportType.Name = "cmbReportType";
      this.cmbReportType.Size = new System.Drawing.Size(172, 21);
      this.cmbReportType.TabIndex = 26;
      // 
      // dtpRaporBaslangic
      // 
      this.dtpReportStartDate.Location = new System.Drawing.Point(195, 72);
      this.dtpReportStartDate.Name = "dtpReportStartDate";
      this.dtpReportStartDate.Size = new System.Drawing.Size(172, 20);
      this.dtpReportStartDate.TabIndex = 27;
      // 
      // dtpRaporBitis
      // 
      this.dtpReportEndDate.Location = new System.Drawing.Point(195, 104);
      this.dtpReportEndDate.Name = "dtpReportEndDate";
      this.dtpReportEndDate.Size = new System.Drawing.Size(172, 20);
      this.dtpReportEndDate.TabIndex = 28;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(170, 12);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(10, 13);
      this.label7.TabIndex = 34;
      this.label7.Text = ":";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(170, 44);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(10, 13);
      this.label8.TabIndex = 33;
      this.label8.Text = ":";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(170, 111);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(10, 13);
      this.label10.TabIndex = 31;
      this.label10.Text = ":";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(170, 78);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(10, 13);
      this.label9.TabIndex = 32;
      this.label9.Text = ":";
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.lblReportDate);
      this.panel2.Controls.Add(this.lblTotalAbsences);
      this.panel2.Controls.Add(this.lblTotalOvertime);
      this.panel2.Controls.Add(this.label12);
      this.panel2.Controls.Add(this.label13);
      this.panel2.Controls.Add(this.label14);
      this.panel2.Controls.Add(this.label11);
      this.panel2.Controls.Add(this.label6);
      this.panel2.Controls.Add(this.label5);
      this.panel2.Controls.Add(this.label1);
      this.panel2.Controls.Add(this.label7);
      this.panel2.Controls.Add(this.label2);
      this.panel2.Controls.Add(this.label8);
      this.panel2.Controls.Add(this.label3);
      this.panel2.Controls.Add(this.label10);
      this.panel2.Controls.Add(this.label4);
      this.panel2.Controls.Add(this.label9);
      this.panel2.Controls.Add(this.cmbReportEmployee);
      this.panel2.Controls.Add(this.dtpReportEndDate);
      this.panel2.Controls.Add(this.cmbReportType);
      this.panel2.Controls.Add(this.dtpReportStartDate);
      this.panel2.Location = new System.Drawing.Point(3, 12);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(382, 232);
      this.panel2.TabIndex = 35;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(227, 139);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(10, 13);
      this.label12.TabIndex = 40;
      this.label12.Text = ":";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(227, 206);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(10, 13);
      this.label13.TabIndex = 38;
      this.label13.Text = ":";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(227, 173);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(10, 13);
      this.label14.TabIndex = 39;
      this.label14.Text = ":";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(3, 206);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(130, 13);
      this.label11.TabIndex = 37;
      UiText.Bind(this.label11, "TotalAbsenceDays");
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(3, 173);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(185, 13);
      this.label6.TabIndex = 36;
      UiText.Bind(this.label6, "OvertimeReportRows");
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 139);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(218, 13);
      this.label5.TabIndex = 35;
      UiText.Bind(this.label5, "DateRangeReportTime");
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.dgvReport);
      this.panel3.Location = new System.Drawing.Point(391, 12);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(406, 232);
      this.panel3.TabIndex = 36;
      // 
      // dgvRapor
      // 
      this.dgvReport.AllowUserToAddRows = false;
      this.dgvReport.AllowUserToDeleteRows = false;
      this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvReport.Location = new System.Drawing.Point(0, 0);
      this.dgvReport.Name = "dgvReport";
      this.dgvReport.ReadOnly = true;
      this.dgvReport.Size = new System.Drawing.Size(406, 232);
      this.dgvReport.TabIndex = 0;
      // 
      // lblRaporTarih
      // 
      this.lblReportDate.AutoSize = true;
      this.lblReportDate.Location = new System.Drawing.Point(243, 139);
      this.lblReportDate.Name = "lblReportDate";
      this.lblReportDate.Size = new System.Drawing.Size(10, 13);
      this.lblReportDate.TabIndex = 43;
      this.lblReportDate.Text = ".";
      // 
      // lblToplamDevamsizlik
      // 
      this.lblTotalAbsences.AutoSize = true;
      this.lblTotalAbsences.Location = new System.Drawing.Point(243, 206);
      this.lblTotalAbsences.Name = "lblTotalAbsences";
      this.lblTotalAbsences.Size = new System.Drawing.Size(10, 13);
      this.lblTotalAbsences.TabIndex = 41;
      this.lblTotalAbsences.Text = ".";
      // 
      // lblToplamMesai
      // 
      this.lblTotalOvertime.AutoSize = true;
      this.lblTotalOvertime.Location = new System.Drawing.Point(243, 173);
      this.lblTotalOvertime.Name = "lblTotalOvertime";
      this.lblTotalOvertime.Size = new System.Drawing.Size(10, 13);
      this.lblTotalOvertime.TabIndex = 42;
      this.lblTotalOvertime.Text = ".";
      // 
      // frmRaporlar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 306);
      this.Controls.Add(this.panel3);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "ReportsForm";
      UiText.Bind(this, "Reports");
      this.Load += new System.EventHandler(this.ReportsFormLoad);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
      this.ResumeLayout(false);
    }

#endregion
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Button btnExportExcel;
    private System.Windows.Forms.Button btnGenerateReport;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cmbReportEmployee;
    private System.Windows.Forms.ComboBox cmbReportType;
    private System.Windows.Forms.DateTimePicker dtpReportStartDate;
    private System.Windows.Forms.DateTimePicker dtpReportEndDate;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.DataGridView dgvReport;
    private System.Windows.Forms.Label lblReportDate;
    private System.Windows.Forms.Label lblTotalAbsences;
    private System.Windows.Forms.Label lblTotalOvertime;
  }
}