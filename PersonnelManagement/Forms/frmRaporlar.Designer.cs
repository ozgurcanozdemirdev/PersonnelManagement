namespace PersonnelManagement.Forms
{
  partial class frmRaporlar
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
      this.btnKapat = new System.Windows.Forms.Button();
      this.btnYazdir = new System.Windows.Forms.Button();
      this.btnExcelAktar = new System.Windows.Forms.Button();
      this.btnRaporOlustur = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.cmbRaporPersonel = new System.Windows.Forms.ComboBox();
      this.cmbRaporTuru = new System.Windows.Forms.ComboBox();
      this.dtpRaporBaslangic = new System.Windows.Forms.DateTimePicker();
      this.dtpRaporBitis = new System.Windows.Forms.DateTimePicker();
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
      this.dgvRapor = new System.Windows.Forms.DataGridView();
      this.lblRaporTarih = new System.Windows.Forms.Label();
      this.lblToplamDevamsizlik = new System.Windows.Forms.Label();
      this.lblToplamMesai = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvRapor)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnKapat);
      this.panel1.Controls.Add(this.btnYazdir);
      this.panel1.Controls.Add(this.btnExcelAktar);
      this.panel1.Controls.Add(this.btnRaporOlustur);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 250);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(800, 56);
      this.panel1.TabIndex = 20;
      // 
      // btnKapat
      // 
      this.btnKapat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnKapat.Location = new System.Drawing.Point(726, 3);
      this.btnKapat.Name = "btnKapat";
      this.btnKapat.Size = new System.Drawing.Size(71, 50);
      this.btnKapat.TabIndex = 4;
      this.btnKapat.Text = "Kapat";
      this.btnKapat.UseVisualStyleBackColor = true;
      this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);
      // 
      // btnYazdir
      // 
      this.btnYazdir.Location = new System.Drawing.Point(185, 3);
      this.btnYazdir.Name = "btnYazdir";
      this.btnYazdir.Size = new System.Drawing.Size(71, 50);
      this.btnYazdir.TabIndex = 2;
      this.btnYazdir.Text = "Yazdır";
      this.btnYazdir.UseVisualStyleBackColor = true;
      this.btnYazdir.Click += new System.EventHandler(this.btnYazdir_Click);
      // 
      // btnExcelAktar
      // 
      this.btnExcelAktar.Location = new System.Drawing.Point(80, 3);
      this.btnExcelAktar.Name = "btnExcelAktar";
      this.btnExcelAktar.Size = new System.Drawing.Size(96, 50);
      this.btnExcelAktar.TabIndex = 1;
      this.btnExcelAktar.Text = "Excel\'e Aktar (CSV formatında)";
      this.btnExcelAktar.UseVisualStyleBackColor = true;
      this.btnExcelAktar.Click += new System.EventHandler(this.btnExcelAktar_Click);
      // 
      // btnRaporOlustur
      // 
      this.btnRaporOlustur.Location = new System.Drawing.Point(3, 3);
      this.btnRaporOlustur.Name = "btnRaporOlustur";
      this.btnRaporOlustur.Size = new System.Drawing.Size(71, 50);
      this.btnRaporOlustur.TabIndex = 0;
      this.btnRaporOlustur.Text = "Rapor Oluştur";
      this.btnRaporOlustur.UseVisualStyleBackColor = true;
      this.btnRaporOlustur.Click += new System.EventHandler(this.btnRaporOlustur_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(161, 13);
      this.label1.TabIndex = 21;
      this.label1.Text = "Personel seç (Tümü / Ad Soyad)";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 44);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(83, 13);
      this.label2.TabIndex = 22;
      this.label2.Text = "Rapor Türü Seç";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 79);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(82, 13);
      this.label3.TabIndex = 23;
      this.label3.Text = "Başlangıç Tarihi";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 111);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(55, 13);
      this.label4.TabIndex = 24;
      this.label4.Text = "Bitiş Tarihi";
      // 
      // cmbRaporPersonel
      // 
      this.cmbRaporPersonel.FormattingEnabled = true;
      this.cmbRaporPersonel.Location = new System.Drawing.Point(195, 9);
      this.cmbRaporPersonel.Name = "cmbRaporPersonel";
      this.cmbRaporPersonel.Size = new System.Drawing.Size(172, 21);
      this.cmbRaporPersonel.TabIndex = 25;
      // 
      // cmbRaporTuru
      // 
      this.cmbRaporTuru.FormattingEnabled = true;
      this.cmbRaporTuru.Location = new System.Drawing.Point(195, 41);
      this.cmbRaporTuru.Name = "cmbRaporTuru";
      this.cmbRaporTuru.Size = new System.Drawing.Size(172, 21);
      this.cmbRaporTuru.TabIndex = 26;
      // 
      // dtpRaporBaslangic
      // 
      this.dtpRaporBaslangic.Location = new System.Drawing.Point(195, 72);
      this.dtpRaporBaslangic.Name = "dtpRaporBaslangic";
      this.dtpRaporBaslangic.Size = new System.Drawing.Size(172, 20);
      this.dtpRaporBaslangic.TabIndex = 27;
      // 
      // dtpRaporBitis
      // 
      this.dtpRaporBitis.Location = new System.Drawing.Point(195, 104);
      this.dtpRaporBitis.Name = "dtpRaporBitis";
      this.dtpRaporBitis.Size = new System.Drawing.Size(172, 20);
      this.dtpRaporBitis.TabIndex = 28;
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
      this.panel2.Controls.Add(this.lblRaporTarih);
      this.panel2.Controls.Add(this.lblToplamDevamsizlik);
      this.panel2.Controls.Add(this.lblToplamMesai);
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
      this.panel2.Controls.Add(this.cmbRaporPersonel);
      this.panel2.Controls.Add(this.dtpRaporBitis);
      this.panel2.Controls.Add(this.cmbRaporTuru);
      this.panel2.Controls.Add(this.dtpRaporBaslangic);
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
      this.label11.Text = "Toplam Devamsızlık Günü";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(3, 173);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(185, 13);
      this.label6.TabIndex = 36;
      this.label6.Text = "Toplam Mesai veya Rapor Satır Sayısı";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 139);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(218, 13);
      this.label5.TabIndex = 35;
      this.label5.Text = "Seçili Tarih Aralığı / Rapor Oluşturma Zamanı";
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.dgvRapor);
      this.panel3.Location = new System.Drawing.Point(391, 12);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(406, 232);
      this.panel3.TabIndex = 36;
      // 
      // dgvRapor
      // 
      this.dgvRapor.AllowUserToAddRows = false;
      this.dgvRapor.AllowUserToDeleteRows = false;
      this.dgvRapor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvRapor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvRapor.Location = new System.Drawing.Point(0, 0);
      this.dgvRapor.Name = "dgvRapor";
      this.dgvRapor.ReadOnly = true;
      this.dgvRapor.Size = new System.Drawing.Size(406, 232);
      this.dgvRapor.TabIndex = 0;
      // 
      // lblRaporTarih
      // 
      this.lblRaporTarih.AutoSize = true;
      this.lblRaporTarih.Location = new System.Drawing.Point(243, 139);
      this.lblRaporTarih.Name = "lblRaporTarih";
      this.lblRaporTarih.Size = new System.Drawing.Size(10, 13);
      this.lblRaporTarih.TabIndex = 43;
      this.lblRaporTarih.Text = ".";
      // 
      // lblToplamDevamsizlik
      // 
      this.lblToplamDevamsizlik.AutoSize = true;
      this.lblToplamDevamsizlik.Location = new System.Drawing.Point(243, 206);
      this.lblToplamDevamsizlik.Name = "lblToplamDevamsizlik";
      this.lblToplamDevamsizlik.Size = new System.Drawing.Size(10, 13);
      this.lblToplamDevamsizlik.TabIndex = 41;
      this.lblToplamDevamsizlik.Text = ".";
      // 
      // lblToplamMesai
      // 
      this.lblToplamMesai.AutoSize = true;
      this.lblToplamMesai.Location = new System.Drawing.Point(243, 173);
      this.lblToplamMesai.Name = "lblToplamMesai";
      this.lblToplamMesai.Size = new System.Drawing.Size(10, 13);
      this.lblToplamMesai.TabIndex = 42;
      this.lblToplamMesai.Text = ".";
      // 
      // frmRaporlar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 306);
      this.Controls.Add(this.panel3);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "frmRaporlar";
      this.Text = "Rapor Ekranı";
      this.Load += new System.EventHandler(this.frmRaporlar_Load);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvRapor)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnKapat;
    private System.Windows.Forms.Button btnYazdir;
    private System.Windows.Forms.Button btnExcelAktar;
    private System.Windows.Forms.Button btnRaporOlustur;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cmbRaporPersonel;
    private System.Windows.Forms.ComboBox cmbRaporTuru;
    private System.Windows.Forms.DateTimePicker dtpRaporBaslangic;
    private System.Windows.Forms.DateTimePicker dtpRaporBitis;
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
    private System.Windows.Forms.DataGridView dgvRapor;
    private System.Windows.Forms.Label lblRaporTarih;
    private System.Windows.Forms.Label lblToplamDevamsizlik;
    private System.Windows.Forms.Label lblToplamMesai;
  }
}