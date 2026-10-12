namespace PersonnelManagement.Forms
{
  partial class frmPersonelYonetim
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
      this.btnTemizle = new System.Windows.Forms.Button();
      this.btnSil = new System.Windows.Forms.Button();
      this.btnGuncelle = new System.Windows.Forms.Button();
      this.btnKaydet = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.dgvPersonel = new System.Windows.Forms.DataGridView();
      this.panel3 = new System.Windows.Forms.Panel();
      this.btnFotoSec = new System.Windows.Forms.Button();
      this.picFoto = new System.Windows.Forms.PictureBox();
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
      this.txtAd = new System.Windows.Forms.TextBox();
      this.txtSoyad = new System.Windows.Forms.TextBox();
      this.txtTCKimlik = new System.Windows.Forms.TextBox();
      this.txtTelefon = new System.Windows.Forms.TextBox();
      this.txtEmail = new System.Windows.Forms.TextBox();
      this.txtAdres = new System.Windows.Forms.TextBox();
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
      this.dtpIseGirisTarihi = new System.Windows.Forms.DateTimePicker();
      this.cmbDepartman = new System.Windows.Forms.ComboBox();
      this.cmbPozisyon = new System.Windows.Forms.ComboBox();
      this.cmbVardiya = new System.Windows.Forms.ComboBox();
      this.cmbCinsiyet = new System.Windows.Forms.ComboBox();
      this.txtAra = new System.Windows.Forms.TextBox();
      this.btnAra = new System.Windows.Forms.Button();
      this.label13 = new System.Windows.Forms.Label();
      this.label19 = new System.Windows.Forms.Label();
      this.txtYillikIzinHakki = new System.Windows.Forms.TextBox();
      this.label25 = new System.Windows.Forms.Label();
      this.label26 = new System.Windows.Forms.Label();
      this.dtpDogumTarihi = new System.Windows.Forms.DateTimePicker();
      this.label27 = new System.Windows.Forms.Label();
      this.label28 = new System.Windows.Forms.Label();
      this.lblToplamKayit = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvPersonel)).BeginInit();
      this.panel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picFoto)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnKapat);
      this.panel1.Controls.Add(this.btnTemizle);
      this.panel1.Controls.Add(this.btnSil);
      this.panel1.Controls.Add(this.btnGuncelle);
      this.panel1.Controls.Add(this.btnKaydet);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 409);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(793, 56);
      this.panel1.TabIndex = 0;
      // 
      // btnKapat
      // 
      this.btnKapat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnKapat.Location = new System.Drawing.Point(719, 3);
      this.btnKapat.Name = "btnKapat";
      this.btnKapat.Size = new System.Drawing.Size(71, 50);
      this.btnKapat.TabIndex = 4;
      this.btnKapat.Text = "Kapat";
      this.btnKapat.UseVisualStyleBackColor = true;
      this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);
      // 
      // btnTemizle
      // 
      this.btnTemizle.Location = new System.Drawing.Point(232, 3);
      this.btnTemizle.Name = "btnTemizle";
      this.btnTemizle.Size = new System.Drawing.Size(71, 50);
      this.btnTemizle.TabIndex = 3;
      this.btnTemizle.Text = "Temizle";
      this.btnTemizle.UseVisualStyleBackColor = true;
      this.btnTemizle.Click += new System.EventHandler(this.btnTemizle_Click);
      // 
      // btnSil
      // 
      this.btnSil.Location = new System.Drawing.Point(157, 3);
      this.btnSil.Name = "btnSil";
      this.btnSil.Size = new System.Drawing.Size(71, 50);
      this.btnSil.TabIndex = 2;
      this.btnSil.Text = "Sil";
      this.btnSil.UseVisualStyleBackColor = true;
      this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
      // 
      // btnGuncelle
      // 
      this.btnGuncelle.Location = new System.Drawing.Point(80, 3);
      this.btnGuncelle.Name = "btnGuncelle";
      this.btnGuncelle.Size = new System.Drawing.Size(71, 50);
      this.btnGuncelle.TabIndex = 1;
      this.btnGuncelle.Text = "Güncelle";
      this.btnGuncelle.UseVisualStyleBackColor = true;
      this.btnGuncelle.Click += new System.EventHandler(this.btnGuncelle_Click);
      // 
      // btnKaydet
      // 
      this.btnKaydet.Location = new System.Drawing.Point(3, 3);
      this.btnKaydet.Name = "btnKaydet";
      this.btnKaydet.Size = new System.Drawing.Size(71, 50);
      this.btnKaydet.TabIndex = 0;
      this.btnKaydet.Text = "Kaydet";
      this.btnKaydet.UseVisualStyleBackColor = true;
      this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.dgvPersonel);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel2.Location = new System.Drawing.Point(0, 216);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(793, 193);
      this.panel2.TabIndex = 1;
      // 
      // dgvPersonel
      // 
      this.dgvPersonel.AllowUserToAddRows = false;
      this.dgvPersonel.AllowUserToDeleteRows = false;
      this.dgvPersonel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvPersonel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvPersonel.Location = new System.Drawing.Point(0, 0);
      this.dgvPersonel.Name = "dgvPersonel";
      this.dgvPersonel.ReadOnly = true;
      this.dgvPersonel.Size = new System.Drawing.Size(793, 193);
      this.dgvPersonel.TabIndex = 0;
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.btnFotoSec);
      this.panel3.Controls.Add(this.picFoto);
      this.panel3.Location = new System.Drawing.Point(508, 12);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(145, 148);
      this.panel3.TabIndex = 2;
      // 
      // btnFotoSec
      // 
      this.btnFotoSec.Location = new System.Drawing.Point(3, 120);
      this.btnFotoSec.Name = "btnFotoSec";
      this.btnFotoSec.Size = new System.Drawing.Size(139, 23);
      this.btnFotoSec.TabIndex = 1;
      this.btnFotoSec.Text = "Fotoğraf Seç";
      this.btnFotoSec.UseVisualStyleBackColor = true;
      this.btnFotoSec.Click += new System.EventHandler(this.btnFotoSec_Click);
      // 
      // picFoto
      // 
      this.picFoto.Location = new System.Drawing.Point(3, 4);
      this.picFoto.Name = "picFoto";
      this.picFoto.Size = new System.Drawing.Size(139, 110);
      this.picFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picFoto.TabIndex = 0;
      this.picFoto.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(23, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Ad ";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 41);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(37, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Soyad";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 70);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "T.C. Kimlik";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 99);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(43, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Telefon";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 128);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(36, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "E-Mail";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(12, 157);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(34, 13);
      this.label6.TabIndex = 8;
      this.label6.Text = "Adres";
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
      this.txtAd.Location = new System.Drawing.Point(107, 13);
      this.txtAd.Name = "txtAd";
      this.txtAd.Size = new System.Drawing.Size(100, 20);
      this.txtAd.TabIndex = 15;
      // 
      // txtSoyad
      // 
      this.txtSoyad.Location = new System.Drawing.Point(107, 41);
      this.txtSoyad.Name = "txtSoyad";
      this.txtSoyad.Size = new System.Drawing.Size(100, 20);
      this.txtSoyad.TabIndex = 16;
      // 
      // txtTCKimlik
      // 
      this.txtTCKimlik.Location = new System.Drawing.Point(107, 70);
      this.txtTCKimlik.Name = "txtTCKimlik";
      this.txtTCKimlik.Size = new System.Drawing.Size(100, 20);
      this.txtTCKimlik.TabIndex = 17;
      // 
      // txtTelefon
      // 
      this.txtTelefon.Location = new System.Drawing.Point(107, 99);
      this.txtTelefon.Name = "txtTelefon";
      this.txtTelefon.Size = new System.Drawing.Size(100, 20);
      this.txtTelefon.TabIndex = 18;
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
      this.txtAdres.Location = new System.Drawing.Point(107, 157);
      this.txtAdres.Name = "txtAdres";
      this.txtAdres.Size = new System.Drawing.Size(100, 20);
      this.txtAdres.TabIndex = 20;
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(222, 132);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(43, 13);
      this.label14.TabIndex = 25;
      this.label14.Text = "Cinsiyet";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(222, 103);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(64, 13);
      this.label15.TabIndex = 24;
      this.label15.Text = "Vardiya Seç";
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(222, 74);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(71, 13);
      this.label16.TabIndex = 23;
      this.label16.Text = "Pozisyon Seç";
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(222, 44);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(81, 13);
      this.label17.TabIndex = 22;
      this.label17.Text = "Departman Seç";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(222, 16);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(73, 13);
      this.label18.TabIndex = 21;
      this.label18.Text = "İşe Giriş Tarihi";
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
      this.dtpIseGirisTarihi.Location = new System.Drawing.Point(328, 12);
      this.dtpIseGirisTarihi.Name = "dtpIseGirisTarihi";
      this.dtpIseGirisTarihi.Size = new System.Drawing.Size(171, 20);
      this.dtpIseGirisTarihi.TabIndex = 32;
      // 
      // cmbDepartman
      // 
      this.cmbDepartman.FormattingEnabled = true;
      this.cmbDepartman.Location = new System.Drawing.Point(328, 39);
      this.cmbDepartman.Name = "cmbDepartman";
      this.cmbDepartman.Size = new System.Drawing.Size(171, 21);
      this.cmbDepartman.TabIndex = 33;
      this.cmbDepartman.SelectedIndexChanged += new System.EventHandler(this.cmbDepartman_SelectedIndexChanged);
      // 
      // cmbPozisyon
      // 
      this.cmbPozisyon.FormattingEnabled = true;
      this.cmbPozisyon.Location = new System.Drawing.Point(328, 69);
      this.cmbPozisyon.Name = "cmbPozisyon";
      this.cmbPozisyon.Size = new System.Drawing.Size(171, 21);
      this.cmbPozisyon.TabIndex = 34;
      // 
      // cmbVardiya
      // 
      this.cmbVardiya.FormattingEnabled = true;
      this.cmbVardiya.Location = new System.Drawing.Point(328, 96);
      this.cmbVardiya.Name = "cmbVardiya";
      this.cmbVardiya.Size = new System.Drawing.Size(171, 21);
      this.cmbVardiya.TabIndex = 35;
      // 
      // cmbCinsiyet
      // 
      this.cmbCinsiyet.FormattingEnabled = true;
      this.cmbCinsiyet.Location = new System.Drawing.Point(328, 125);
      this.cmbCinsiyet.Name = "cmbCinsiyet";
      this.cmbCinsiyet.Size = new System.Drawing.Size(171, 21);
      this.cmbCinsiyet.TabIndex = 36;
      // 
      // txtAra
      // 
      this.txtAra.Location = new System.Drawing.Point(225, 184);
      this.txtAra.Name = "txtAra";
      this.txtAra.Size = new System.Drawing.Size(171, 20);
      this.txtAra.TabIndex = 37;
      this.txtAra.TextChanged += new System.EventHandler(this.txtAra_TextChanged);
      this.txtAra.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAra_KeyDown);
      // 
      // btnAra
      // 
      this.btnAra.Location = new System.Drawing.Point(402, 182);
      this.btnAra.Name = "btnAra";
      this.btnAra.Size = new System.Drawing.Size(97, 23);
      this.btnAra.TabIndex = 38;
      this.btnAra.Text = "Ara";
      this.btnAra.UseVisualStyleBackColor = true;
      this.btnAra.Click += new System.EventHandler(this.btnAra_Click);
      this.btnAra.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnAra_KeyDown);
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(12, 187);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(81, 13);
      this.label13.TabIndex = 39;
      this.label13.Text = "Yıllık İzin Hakkı ";
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
      this.txtYillikIzinHakki.Location = new System.Drawing.Point(107, 183);
      this.txtYillikIzinHakki.Name = "txtYillikIzinHakki";
      this.txtYillikIzinHakki.Size = new System.Drawing.Size(100, 20);
      this.txtYillikIzinHakki.TabIndex = 41;
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
      this.label26.Text = "Doğum Tarihi";
      // 
      // dtpDogumTarihi
      // 
      this.dtpDogumTarihi.Location = new System.Drawing.Point(328, 156);
      this.dtpDogumTarihi.Name = "dtpDogumTarihi";
      this.dtpDogumTarihi.Size = new System.Drawing.Size(171, 20);
      this.dtpDogumTarihi.TabIndex = 44;
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
      this.label28.Text = "Toplam Personel Sayısı";
      // 
      // lblToplamKayit
      // 
      this.lblToplamKayit.AutoSize = true;
      this.lblToplamKayit.Location = new System.Drawing.Point(627, 183);
      this.lblToplamKayit.Name = "lblToplamKayit";
      this.lblToplamKayit.Size = new System.Drawing.Size(10, 13);
      this.lblToplamKayit.TabIndex = 47;
      this.lblToplamKayit.Text = ".";
      // 
      // frmPersonelYonetim
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(793, 465);
      this.ControlBox = false;
      this.Controls.Add(this.lblToplamKayit);
      this.Controls.Add(this.label27);
      this.Controls.Add(this.label28);
      this.Controls.Add(this.dtpDogumTarihi);
      this.Controls.Add(this.label25);
      this.Controls.Add(this.label26);
      this.Controls.Add(this.txtYillikIzinHakki);
      this.Controls.Add(this.label19);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.btnAra);
      this.Controls.Add(this.txtAra);
      this.Controls.Add(this.cmbCinsiyet);
      this.Controls.Add(this.cmbVardiya);
      this.Controls.Add(this.cmbPozisyon);
      this.Controls.Add(this.cmbDepartman);
      this.Controls.Add(this.dtpIseGirisTarihi);
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
      this.Controls.Add(this.txtAdres);
      this.Controls.Add(this.txtEmail);
      this.Controls.Add(this.txtTelefon);
      this.Controls.Add(this.txtTCKimlik);
      this.Controls.Add(this.txtSoyad);
      this.Controls.Add(this.txtAd);
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
      this.Name = "frmPersonelYonetim";
      this.Text = "Personel Yönetim Ekranı";
      this.Load += new System.EventHandler(this.frmPersonelYonetim_Load);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvPersonel)).EndInit();
      this.panel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.picFoto)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnGuncelle;
    private System.Windows.Forms.Button btnKaydet;
    private System.Windows.Forms.Button btnSil;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Button btnFotoSec;
    private System.Windows.Forms.PictureBox picFoto;
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
    private System.Windows.Forms.TextBox txtAd;
    private System.Windows.Forms.TextBox txtSoyad;
    private System.Windows.Forms.TextBox txtTCKimlik;
    private System.Windows.Forms.TextBox txtTelefon;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.TextBox txtAdres;
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
    private System.Windows.Forms.DateTimePicker dtpIseGirisTarihi;
    private System.Windows.Forms.ComboBox cmbDepartman;
    private System.Windows.Forms.ComboBox cmbPozisyon;
    private System.Windows.Forms.ComboBox cmbVardiya;
    private System.Windows.Forms.ComboBox cmbCinsiyet;
    private System.Windows.Forms.TextBox txtAra;
    private System.Windows.Forms.Button btnAra;
    private System.Windows.Forms.DataGridView dgvPersonel;
    private System.Windows.Forms.Button btnTemizle;
    private System.Windows.Forms.Button btnKapat;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.TextBox txtYillikIzinHakki;
    private System.Windows.Forms.Label label25;
    private System.Windows.Forms.Label label26;
    private System.Windows.Forms.DateTimePicker dtpDogumTarihi;
    private System.Windows.Forms.Label label27;
    private System.Windows.Forms.Label label28;
    private System.Windows.Forms.Label lblToplamKayit;
  }
}