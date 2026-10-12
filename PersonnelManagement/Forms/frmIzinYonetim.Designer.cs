namespace PersonnelManagement.Forms
{
  partial class frmIzinYonetim
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
      this.btnTemizle = new System.Windows.Forms.Button();
      this.btnTumBekleyenler = new System.Windows.Forms.Button();
      this.btnTumIzinler = new System.Windows.Forms.Button();
      this.btnKapat = new System.Windows.Forms.Button();
      this.btnIzinIptal = new System.Windows.Forms.Button();
      this.btnReddet = new System.Windows.Forms.Button();
      this.btnOnayla = new System.Windows.Forms.Button();
      this.btnIzinKaydet = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvIzinler = new System.Windows.Forms.DataGridView();
      this.cmbIzinPersonel = new System.Windows.Forms.ComboBox();
      this.cmbIzinTuru = new System.Windows.Forms.ComboBox();
      this.cmbFiltrePersonel = new System.Windows.Forms.ComboBox();
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
      this.dtpIzinBaslangic = new System.Windows.Forms.DateTimePicker();
      this.dtpIzinBitis = new System.Windows.Forms.DateTimePicker();
      this.txtIzinAciklama = new System.Windows.Forms.TextBox();
      this.label13 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.label16 = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.lblToplamGun = new System.Windows.Forms.Label();
      this.lblKalanIzin = new System.Windows.Forms.Label();
      this.lblToplamIzin = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvIzinler)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnTemizle);
      this.panel1.Controls.Add(this.btnTumBekleyenler);
      this.panel1.Controls.Add(this.btnTumIzinler);
      this.panel1.Controls.Add(this.btnKapat);
      this.panel1.Controls.Add(this.btnIzinIptal);
      this.panel1.Controls.Add(this.btnReddet);
      this.panel1.Controls.Add(this.btnOnayla);
      this.panel1.Controls.Add(this.btnIzinKaydet);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 353);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(719, 56);
      this.panel1.TabIndex = 1;
      // 
      // btnTemizle
      // 
      this.btnTemizle.Location = new System.Drawing.Point(463, 3);
      this.btnTemizle.Name = "btnTemizle";
      this.btnTemizle.Size = new System.Drawing.Size(71, 50);
      this.btnTemizle.TabIndex = 7;
      this.btnTemizle.Text = "Temizle";
      this.btnTemizle.UseVisualStyleBackColor = true;
      this.btnTemizle.Click += new System.EventHandler(this.btnTemizle_Click);
      // 
      // btnTumBekleyenler
      // 
      this.btnTumBekleyenler.Location = new System.Drawing.Point(386, 3);
      this.btnTumBekleyenler.Name = "btnTumBekleyenler";
      this.btnTumBekleyenler.Size = new System.Drawing.Size(71, 50);
      this.btnTumBekleyenler.TabIndex = 6;
      this.btnTumBekleyenler.Text = "Tüm Bekleyenler";
      this.btnTumBekleyenler.UseVisualStyleBackColor = true;
      this.btnTumBekleyenler.Click += new System.EventHandler(this.btnTumBekleyenler_Click);
      // 
      // btnTumIzinler
      // 
      this.btnTumIzinler.Location = new System.Drawing.Point(309, 3);
      this.btnTumIzinler.Name = "btnTumIzinler";
      this.btnTumIzinler.Size = new System.Drawing.Size(71, 50);
      this.btnTumIzinler.TabIndex = 5;
      this.btnTumIzinler.Text = "Tüm İzinler";
      this.btnTumIzinler.UseVisualStyleBackColor = true;
      this.btnTumIzinler.Click += new System.EventHandler(this.btnTumIzinler_Click);
      // 
      // btnKapat
      // 
      this.btnKapat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnKapat.Location = new System.Drawing.Point(645, 3);
      this.btnKapat.Name = "btnKapat";
      this.btnKapat.Size = new System.Drawing.Size(71, 50);
      this.btnKapat.TabIndex = 4;
      this.btnKapat.Text = "Kapat";
      this.btnKapat.UseVisualStyleBackColor = true;
      this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);
      // 
      // btnIzinIptal
      // 
      this.btnIzinIptal.Location = new System.Drawing.Point(232, 3);
      this.btnIzinIptal.Name = "btnIzinIptal";
      this.btnIzinIptal.Size = new System.Drawing.Size(71, 50);
      this.btnIzinIptal.TabIndex = 3;
      this.btnIzinIptal.Text = "İzin İptal";
      this.btnIzinIptal.UseVisualStyleBackColor = true;
      this.btnIzinIptal.Click += new System.EventHandler(this.btnIzinIptal_Click);
      // 
      // btnReddet
      // 
      this.btnReddet.Location = new System.Drawing.Point(157, 3);
      this.btnReddet.Name = "btnReddet";
      this.btnReddet.Size = new System.Drawing.Size(71, 50);
      this.btnReddet.TabIndex = 2;
      this.btnReddet.Text = "Reddet";
      this.btnReddet.UseVisualStyleBackColor = true;
      this.btnReddet.Click += new System.EventHandler(this.btnReddet_Click);
      // 
      // btnOnayla
      // 
      this.btnOnayla.Location = new System.Drawing.Point(80, 3);
      this.btnOnayla.Name = "btnOnayla";
      this.btnOnayla.Size = new System.Drawing.Size(71, 50);
      this.btnOnayla.TabIndex = 1;
      this.btnOnayla.Text = "Onayla";
      this.btnOnayla.UseVisualStyleBackColor = true;
      this.btnOnayla.Click += new System.EventHandler(this.btnOnayla_Click);
      // 
      // btnIzinKaydet
      // 
      this.btnIzinKaydet.Location = new System.Drawing.Point(3, 3);
      this.btnIzinKaydet.Name = "btnIzinKaydet";
      this.btnIzinKaydet.Size = new System.Drawing.Size(71, 50);
      this.btnIzinKaydet.TabIndex = 0;
      this.btnIzinKaydet.Text = "İzin Kaydet";
      this.btnIzinKaydet.UseVisualStyleBackColor = true;
      this.btnIzinKaydet.Click += new System.EventHandler(this.btnIzinKaydet_Click);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvIzinler);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel2.Location = new System.Drawing.Point(0, 232);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(719, 121);
      this.panel2.TabIndex = 2;
      // 
      // dgvIzinler
      // 
      this.dgvIzinler.AllowUserToAddRows = false;
      this.dgvIzinler.AllowUserToDeleteRows = false;
      this.dgvIzinler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvIzinler.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvIzinler.Location = new System.Drawing.Point(0, 0);
      this.dgvIzinler.Name = "dgvIzinler";
      this.dgvIzinler.ReadOnly = true;
      this.dgvIzinler.Size = new System.Drawing.Size(719, 121);
      this.dgvIzinler.TabIndex = 0;
      this.dgvIzinler.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIzinler_CellClick);
      // 
      // cmbIzinPersonel
      // 
      this.cmbIzinPersonel.FormattingEnabled = true;
      this.cmbIzinPersonel.Location = new System.Drawing.Point(170, 22);
      this.cmbIzinPersonel.Name = "cmbIzinPersonel";
      this.cmbIzinPersonel.Size = new System.Drawing.Size(171, 21);
      this.cmbIzinPersonel.TabIndex = 3;
      this.cmbIzinPersonel.SelectedIndexChanged += new System.EventHandler(this.cmbIzinPersonel_SelectedIndexChanged);
      // 
      // cmbIzinTuru
      // 
      this.cmbIzinTuru.FormattingEnabled = true;
      this.cmbIzinTuru.Location = new System.Drawing.Point(170, 54);
      this.cmbIzinTuru.Name = "cmbIzinTuru";
      this.cmbIzinTuru.Size = new System.Drawing.Size(171, 21);
      this.cmbIzinTuru.TabIndex = 4;
      // 
      // cmbFiltrePersonel
      // 
      this.cmbFiltrePersonel.FormattingEnabled = true;
      this.cmbFiltrePersonel.Location = new System.Drawing.Point(170, 86);
      this.cmbFiltrePersonel.Name = "cmbFiltrePersonel";
      this.cmbFiltrePersonel.Size = new System.Drawing.Size(171, 21);
      this.cmbFiltrePersonel.TabIndex = 5;
      this.cmbFiltrePersonel.SelectedIndexChanged += new System.EventHandler(this.cmbFiltrePersonel_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Personel Seç";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 57);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(70, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "İzin Türü Seç";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 89);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(145, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Listeyi Personele Göre Filtrele";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(372, 25);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(101, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "İzin Başlangıç Tarihi\r\n";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(372, 57);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(74, 13);
      this.label5.TabIndex = 10;
      this.label5.Text = "İzin Bitiş Tarihi\r\n";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(372, 92);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(69, 13);
      this.label6.TabIndex = 11;
      this.label6.Text = "İzin Açıklama";
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
      this.dtpIzinBaslangic.Location = new System.Drawing.Point(531, 25);
      this.dtpIzinBaslangic.Name = "dtpIzinBaslangic";
      this.dtpIzinBaslangic.Size = new System.Drawing.Size(170, 20);
      this.dtpIzinBaslangic.TabIndex = 18;
      // 
      // dtpIzinBitis
      // 
      this.dtpIzinBitis.Location = new System.Drawing.Point(531, 57);
      this.dtpIzinBitis.Name = "dtpIzinBitis";
      this.dtpIzinBitis.Size = new System.Drawing.Size(170, 20);
      this.dtpIzinBitis.TabIndex = 19;
      // 
      // txtIzinAciklama
      // 
      this.txtIzinAciklama.Location = new System.Drawing.Point(531, 89);
      this.txtIzinAciklama.Name = "txtIzinAciklama";
      this.txtIzinAciklama.Size = new System.Drawing.Size(170, 20);
      this.txtIzinAciklama.TabIndex = 20;
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(15, 120);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(162, 13);
      this.label13.TabIndex = 21;
      this.label13.Text = "Otomatik Hesaplanan Gün Sayısı";
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
      this.label17.Text = "Personelin Kalan Yıllık İzni";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(15, 184);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(108, 13);
      this.label18.TabIndex = 26;
      this.label18.Text = "Listede Kaç Kayıt Var";
      // 
      // lblToplamGun
      // 
      this.lblToplamGun.AutoSize = true;
      this.lblToplamGun.Location = new System.Drawing.Point(208, 120);
      this.lblToplamGun.Name = "lblToplamGun";
      this.lblToplamGun.Size = new System.Drawing.Size(10, 13);
      this.lblToplamGun.TabIndex = 29;
      this.lblToplamGun.Text = ".";
      // 
      // lblKalanIzin
      // 
      this.lblKalanIzin.AutoSize = true;
      this.lblKalanIzin.Location = new System.Drawing.Point(208, 152);
      this.lblKalanIzin.Name = "lblKalanIzin";
      this.lblKalanIzin.Size = new System.Drawing.Size(10, 13);
      this.lblKalanIzin.TabIndex = 28;
      this.lblKalanIzin.Text = ".";
      // 
      // lblToplamIzin
      // 
      this.lblToplamIzin.AutoSize = true;
      this.lblToplamIzin.Location = new System.Drawing.Point(208, 184);
      this.lblToplamIzin.Name = "lblToplamIzin";
      this.lblToplamIzin.Size = new System.Drawing.Size(10, 13);
      this.lblToplamIzin.TabIndex = 27;
      this.lblToplamIzin.Text = ".";
      // 
      // frmIzinYonetim
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(719, 409);
      this.Controls.Add(this.lblToplamGun);
      this.Controls.Add(this.lblKalanIzin);
      this.Controls.Add(this.lblToplamIzin);
      this.Controls.Add(this.label18);
      this.Controls.Add(this.label17);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.txtIzinAciklama);
      this.Controls.Add(this.dtpIzinBitis);
      this.Controls.Add(this.dtpIzinBaslangic);
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
      this.Controls.Add(this.cmbFiltrePersonel);
      this.Controls.Add(this.cmbIzinTuru);
      this.Controls.Add(this.cmbIzinPersonel);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "frmIzinYonetim";
      this.Text = "İzin Yönetim Ekranı";
      this.Load += new System.EventHandler(this.frmIzinYonetim_Load);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvIzinler)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnKapat;
    private System.Windows.Forms.Button btnIzinIptal;
    private System.Windows.Forms.Button btnReddet;
    private System.Windows.Forms.Button btnOnayla;
    private System.Windows.Forms.Button btnIzinKaydet;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btnTemizle;
    private System.Windows.Forms.Button btnTumBekleyenler;
    private System.Windows.Forms.Button btnTumIzinler;
    private System.Windows.Forms.DataGridView dgvIzinler;
    private System.Windows.Forms.ComboBox cmbIzinPersonel;
    private System.Windows.Forms.ComboBox cmbIzinTuru;
    private System.Windows.Forms.ComboBox cmbFiltrePersonel;
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
    private System.Windows.Forms.DateTimePicker dtpIzinBaslangic;
    private System.Windows.Forms.DateTimePicker dtpIzinBitis;
    private System.Windows.Forms.TextBox txtIzinAciklama;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.Label lblToplamGun;
    private System.Windows.Forms.Label lblKalanIzin;
    private System.Windows.Forms.Label lblToplamIzin;
  }
}