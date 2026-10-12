namespace PersonnelManagement.Forms
{
  partial class frmVardiyaYonetim
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
      this.btnVardiyaKaydet = new System.Windows.Forms.Button();
      this.btnVardiyaGuncelle = new System.Windows.Forms.Button();
      this.btnVardiyaSil = new System.Windows.Forms.Button();
      this.btnTemizle = new System.Windows.Forms.Button();
      this.btnTumHafta = new System.Windows.Forms.Button();
      this.btnHaftaIci = new System.Windows.Forms.Button();
      this.btnHaftaSonu = new System.Windows.Forms.Button();
      this.btnHicbiri = new System.Windows.Forms.Button();
      this.btnKapat = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.txtVardiyaAdi = new System.Windows.Forms.TextBox();
      this.txtMolaSuresi = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.dtpBaslangicSaati = new System.Windows.Forms.DateTimePicker();
      this.dtpBitisSaati = new System.Windows.Forms.DateTimePicker();
      this.chkPazar = new System.Windows.Forms.CheckBox();
      this.chkCumartesi = new System.Windows.Forms.CheckBox();
      this.chkCuma = new System.Windows.Forms.CheckBox();
      this.chkPersembe = new System.Windows.Forms.CheckBox();
      this.chkCarsamba = new System.Windows.Forms.CheckBox();
      this.chkSali = new System.Windows.Forms.CheckBox();
      this.chkPazartesi = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvVardiya = new System.Windows.Forms.DataGridView();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.lblToplamKayit = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvVardiya)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnKapat);
      this.panel1.Controls.Add(this.btnVardiyaKaydet);
      this.panel1.Controls.Add(this.btnHicbiri);
      this.panel1.Controls.Add(this.btnTemizle);
      this.panel1.Controls.Add(this.btnVardiyaSil);
      this.panel1.Controls.Add(this.btnHaftaSonu);
      this.panel1.Controls.Add(this.btnTumHafta);
      this.panel1.Controls.Add(this.btnVardiyaGuncelle);
      this.panel1.Controls.Add(this.btnHaftaIci);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 395);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(747, 57);
      this.panel1.TabIndex = 0;
      // 
      // btnVardiyaKaydet
      // 
      this.btnVardiyaKaydet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnVardiyaKaydet.Location = new System.Drawing.Point(3, 3);
      this.btnVardiyaKaydet.Name = "btnVardiyaKaydet";
      this.btnVardiyaKaydet.Size = new System.Drawing.Size(77, 51);
      this.btnVardiyaKaydet.TabIndex = 0;
      this.btnVardiyaKaydet.Text = "Kaydet";
      this.btnVardiyaKaydet.UseVisualStyleBackColor = true;
      this.btnVardiyaKaydet.Click += new System.EventHandler(this.btnVardiyaKaydet_Click);
      // 
      // btnVardiyaGuncelle
      // 
      this.btnVardiyaGuncelle.Location = new System.Drawing.Point(86, 3);
      this.btnVardiyaGuncelle.Name = "btnVardiyaGuncelle";
      this.btnVardiyaGuncelle.Size = new System.Drawing.Size(77, 51);
      this.btnVardiyaGuncelle.TabIndex = 1;
      this.btnVardiyaGuncelle.Text = "Güncelle";
      this.btnVardiyaGuncelle.UseVisualStyleBackColor = true;
      this.btnVardiyaGuncelle.Click += new System.EventHandler(this.btnVardiyaGuncelle_Click);
      // 
      // btnVardiyaSil
      // 
      this.btnVardiyaSil.Location = new System.Drawing.Point(169, 3);
      this.btnVardiyaSil.Name = "btnVardiyaSil";
      this.btnVardiyaSil.Size = new System.Drawing.Size(77, 51);
      this.btnVardiyaSil.TabIndex = 2;
      this.btnVardiyaSil.Text = "Sil";
      this.btnVardiyaSil.UseVisualStyleBackColor = true;
      this.btnVardiyaSil.Click += new System.EventHandler(this.btnVardiyaSil_Click);
      // 
      // btnTemizle
      // 
      this.btnTemizle.Location = new System.Drawing.Point(252, 3);
      this.btnTemizle.Name = "btnTemizle";
      this.btnTemizle.Size = new System.Drawing.Size(77, 51);
      this.btnTemizle.TabIndex = 3;
      this.btnTemizle.Text = "Formu Temizle";
      this.btnTemizle.UseVisualStyleBackColor = true;
      this.btnTemizle.Click += new System.EventHandler(this.btnTemizle_Click);
      // 
      // btnTumHafta
      // 
      this.btnTumHafta.Location = new System.Drawing.Point(335, 3);
      this.btnTumHafta.Name = "btnTumHafta";
      this.btnTumHafta.Size = new System.Drawing.Size(77, 51);
      this.btnTumHafta.TabIndex = 4;
      this.btnTumHafta.Text = "Tüm Günleri Seç";
      this.btnTumHafta.UseVisualStyleBackColor = true;
      this.btnTumHafta.Click += new System.EventHandler(this.btnTumHafta_Click);
      // 
      // btnHaftaIci
      // 
      this.btnHaftaIci.Location = new System.Drawing.Point(418, 3);
      this.btnHaftaIci.Name = "btnHaftaIci";
      this.btnHaftaIci.Size = new System.Drawing.Size(77, 51);
      this.btnHaftaIci.TabIndex = 5;
      this.btnHaftaIci.Text = "Pzt-Cum Seç";
      this.btnHaftaIci.UseVisualStyleBackColor = true;
      this.btnHaftaIci.Click += new System.EventHandler(this.btnHaftaIci_Click);
      // 
      // btnHaftaSonu
      // 
      this.btnHaftaSonu.Location = new System.Drawing.Point(501, 3);
      this.btnHaftaSonu.Name = "btnHaftaSonu";
      this.btnHaftaSonu.Size = new System.Drawing.Size(77, 51);
      this.btnHaftaSonu.TabIndex = 6;
      this.btnHaftaSonu.Text = "Cmt-Paz Seç";
      this.btnHaftaSonu.UseVisualStyleBackColor = true;
      this.btnHaftaSonu.Click += new System.EventHandler(this.btnHaftaSonu_Click);
      // 
      // btnHicbiri
      // 
      this.btnHicbiri.Location = new System.Drawing.Point(584, 3);
      this.btnHicbiri.Name = "btnHicbiri";
      this.btnHicbiri.Size = new System.Drawing.Size(77, 51);
      this.btnHicbiri.TabIndex = 7;
      this.btnHicbiri.Text = "Tüm Seçimleri Kaldır";
      this.btnHicbiri.UseVisualStyleBackColor = true;
      this.btnHicbiri.Click += new System.EventHandler(this.btnHicbiri_Click);
      // 
      // btnKapat
      // 
      this.btnKapat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnKapat.Location = new System.Drawing.Point(667, 3);
      this.btnKapat.Name = "btnKapat";
      this.btnKapat.Size = new System.Drawing.Size(77, 51);
      this.btnKapat.TabIndex = 8;
      this.btnKapat.Text = "Kapat";
      this.btnKapat.UseVisualStyleBackColor = true;
      this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 10);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(63, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Vardiya Adı ";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 44);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(65, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Mola Süresi ";
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
      this.txtVardiyaAdi.Location = new System.Drawing.Point(116, 12);
      this.txtVardiyaAdi.Name = "txtVardiyaAdi";
      this.txtVardiyaAdi.Size = new System.Drawing.Size(173, 20);
      this.txtVardiyaAdi.TabIndex = 6;
      // 
      // txtMolaSuresi
      // 
      this.txtMolaSuresi.Location = new System.Drawing.Point(116, 44);
      this.txtMolaSuresi.Name = "txtMolaSuresi";
      this.txtMolaSuresi.Size = new System.Drawing.Size(173, 20);
      this.txtMolaSuresi.TabIndex = 7;
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
      this.label7.Text = "Bitiş Saati";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(12, 74);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(80, 13);
      this.label8.TabIndex = 8;
      this.label8.Text = "Başlangıç Saati";
      // 
      // dtpBaslangicSaati
      // 
      this.dtpBaslangicSaati.Location = new System.Drawing.Point(116, 71);
      this.dtpBaslangicSaati.Name = "dtpBaslangicSaati";
      this.dtpBaslangicSaati.Size = new System.Drawing.Size(173, 20);
      this.dtpBaslangicSaati.TabIndex = 12;
      // 
      // dtpBitisSaati
      // 
      this.dtpBitisSaati.Location = new System.Drawing.Point(116, 105);
      this.dtpBitisSaati.Name = "dtpBitisSaati";
      this.dtpBitisSaati.Size = new System.Drawing.Size(173, 20);
      this.dtpBitisSaati.TabIndex = 13;
      // 
      // chkPazar
      // 
      this.chkPazar.AutoSize = true;
      this.chkPazar.Location = new System.Drawing.Point(6, 211);
      this.chkPazar.Name = "chkPazar";
      this.chkPazar.Size = new System.Drawing.Size(53, 17);
      this.chkPazar.TabIndex = 28;
      this.chkPazar.Text = "Pazar";
      this.chkPazar.UseVisualStyleBackColor = true;
      // 
      // chkCumartesi
      // 
      this.chkCumartesi.AutoSize = true;
      this.chkCumartesi.Location = new System.Drawing.Point(6, 179);
      this.chkCumartesi.Name = "chkCumartesi";
      this.chkCumartesi.Size = new System.Drawing.Size(72, 17);
      this.chkCumartesi.TabIndex = 29;
      this.chkCumartesi.Text = "Cumartesi";
      this.chkCumartesi.UseVisualStyleBackColor = true;
      // 
      // chkCuma
      // 
      this.chkCuma.AutoSize = true;
      this.chkCuma.Location = new System.Drawing.Point(6, 147);
      this.chkCuma.Name = "chkCuma";
      this.chkCuma.Size = new System.Drawing.Size(53, 17);
      this.chkCuma.TabIndex = 30;
      this.chkCuma.Text = "Cuma";
      this.chkCuma.UseVisualStyleBackColor = true;
      // 
      // chkPersembe
      // 
      this.chkPersembe.AutoSize = true;
      this.chkPersembe.Location = new System.Drawing.Point(6, 115);
      this.chkPersembe.Name = "chkPersembe";
      this.chkPersembe.Size = new System.Drawing.Size(73, 17);
      this.chkPersembe.TabIndex = 31;
      this.chkPersembe.Text = "Perşembe";
      this.chkPersembe.UseVisualStyleBackColor = true;
      // 
      // chkCarsamba
      // 
      this.chkCarsamba.AutoSize = true;
      this.chkCarsamba.Location = new System.Drawing.Point(6, 83);
      this.chkCarsamba.Name = "chkCarsamba";
      this.chkCarsamba.Size = new System.Drawing.Size(73, 17);
      this.chkCarsamba.TabIndex = 32;
      this.chkCarsamba.Text = "Çarşamba";
      this.chkCarsamba.UseVisualStyleBackColor = true;
      // 
      // chkSali
      // 
      this.chkSali.AutoSize = true;
      this.chkSali.Location = new System.Drawing.Point(6, 51);
      this.chkSali.Name = "chkSali";
      this.chkSali.Size = new System.Drawing.Size(43, 17);
      this.chkSali.TabIndex = 33;
      this.chkSali.Text = "Salı";
      this.chkSali.UseVisualStyleBackColor = true;
      // 
      // chkPazartesi
      // 
      this.chkPazartesi.AutoSize = true;
      this.chkPazartesi.Location = new System.Drawing.Point(6, 19);
      this.chkPazartesi.Name = "chkPazartesi";
      this.chkPazartesi.Size = new System.Drawing.Size(69, 17);
      this.chkPazartesi.TabIndex = 34;
      this.chkPazartesi.Text = "Pazartesi";
      this.chkPazartesi.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.chkPazartesi);
      this.groupBox1.Controls.Add(this.chkPazar);
      this.groupBox1.Controls.Add(this.chkSali);
      this.groupBox1.Controls.Add(this.chkCumartesi);
      this.groupBox1.Controls.Add(this.chkCarsamba);
      this.groupBox1.Controls.Add(this.chkCuma);
      this.groupBox1.Controls.Add(this.chkPersembe);
      this.groupBox1.Location = new System.Drawing.Point(15, 153);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(271, 236);
      this.groupBox1.TabIndex = 35;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Günler";
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvVardiya);
      this.panel2.Location = new System.Drawing.Point(295, 12);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(449, 380);
      this.panel2.TabIndex = 36;
      // 
      // dgvVardiya
      // 
      this.dgvVardiya.AllowUserToAddRows = false;
      this.dgvVardiya.AllowUserToDeleteRows = false;
      this.dgvVardiya.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvVardiya.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvVardiya.Location = new System.Drawing.Point(0, 0);
      this.dgvVardiya.Name = "dgvVardiya";
      this.dgvVardiya.ReadOnly = true;
      this.dgvVardiya.Size = new System.Drawing.Size(449, 380);
      this.dgvVardiya.TabIndex = 0;
      this.dgvVardiya.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVardiya_CellClick);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(12, 137);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(68, 13);
      this.label9.TabIndex = 37;
      this.label9.Text = "Toplam Kayıt";
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
      this.lblToplamKayit.AutoSize = true;
      this.lblToplamKayit.Location = new System.Drawing.Point(116, 137);
      this.lblToplamKayit.Name = "lblToplamKayit";
      this.lblToplamKayit.Size = new System.Drawing.Size(10, 13);
      this.lblToplamKayit.TabIndex = 39;
      this.lblToplamKayit.Text = ".";
      // 
      // frmVardiyaYonetim
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(747, 452);
      this.ControlBox = false;
      this.Controls.Add(this.lblToplamKayit);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.dtpBitisSaati);
      this.Controls.Add(this.dtpBaslangicSaati);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.txtMolaSuresi);
      this.Controls.Add(this.txtVardiyaAdi);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.panel1);
      this.Name = "frmVardiyaYonetim";
      this.Text = "Vardiya Yönetim Ekranı";
      this.Load += new System.EventHandler(this.frmVardiyaYonetim_Load);
      this.panel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvVardiya)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnKapat;
    private System.Windows.Forms.Button btnHicbiri;
    private System.Windows.Forms.Button btnHaftaSonu;
    private System.Windows.Forms.Button btnHaftaIci;
    private System.Windows.Forms.Button btnTumHafta;
    private System.Windows.Forms.Button btnTemizle;
    private System.Windows.Forms.Button btnVardiyaSil;
    private System.Windows.Forms.Button btnVardiyaGuncelle;
    private System.Windows.Forms.Button btnVardiyaKaydet;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtVardiyaAdi;
    private System.Windows.Forms.TextBox txtMolaSuresi;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.DateTimePicker dtpBaslangicSaati;
    private System.Windows.Forms.DateTimePicker dtpBitisSaati;
    private System.Windows.Forms.CheckBox chkPazar;
    private System.Windows.Forms.CheckBox chkCumartesi;
    private System.Windows.Forms.CheckBox chkCuma;
    private System.Windows.Forms.CheckBox chkPersembe;
    private System.Windows.Forms.CheckBox chkCarsamba;
    private System.Windows.Forms.CheckBox chkSali;
    private System.Windows.Forms.CheckBox chkPazartesi;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.DataGridView dgvVardiya;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label lblToplamKayit;
  }
}