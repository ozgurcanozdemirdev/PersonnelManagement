namespace PersonnelManagement.Forms
{
  partial class frmLogin
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
      this.txtKullaniciAdi = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtSifre = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.btnGiris = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.lblHata = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // txtKullaniciAdi
      // 
      this.txtKullaniciAdi.Location = new System.Drawing.Point(101, 15);
      this.txtKullaniciAdi.Name = "txtKullaniciAdi";
      this.txtKullaniciAdi.Size = new System.Drawing.Size(127, 20);
      this.txtKullaniciAdi.TabIndex = 0;

      this.txtKullaniciAdi.TextChanged += new System.EventHandler(this.txtKullaniciAdi_TextChanged);
      this.txtKullaniciAdi.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKullaniciAdi_KeyDown);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Kullanıcı Adı";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 50);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(28, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Şifre";
      // 
      // txtSifre
      // 
      this.txtSifre.Location = new System.Drawing.Point(101, 47);
      this.txtSifre.Name = "txtSifre";
      this.txtSifre.PasswordChar = '*';
      this.txtSifre.Size = new System.Drawing.Size(127, 20);
      this.txtSifre.TabIndex = 3;
      this.txtSifre.TextChanged += new System.EventHandler(this.txtSifre_TextChanged);
      this.txtSifre.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSifre_KeyDown);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(72, 18);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(10, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = ":";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(71, 50);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(10, 13);
      this.label4.TabIndex = 5;
      this.label4.Text = ":";
      // 
      // btnGiris
      // 
      this.btnGiris.Location = new System.Drawing.Point(128, 73);
      this.btnGiris.Name = "btnGiris";
      this.btnGiris.Size = new System.Drawing.Size(100, 23);
      this.btnGiris.TabIndex = 6;
      this.btnGiris.Text = "Giriş";
      this.btnGiris.UseVisualStyleBackColor = true;
      this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 110);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(63, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Hata Mesajı";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(72, 110);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(10, 13);
      this.label6.TabIndex = 8;
      this.label6.Text = ":";
      // 
      // lblHata
      // 
      this.lblHata.AutoSize = true;
      this.lblHata.Location = new System.Drawing.Point(88, 110);
      this.lblHata.Name = "lblHata";
      this.lblHata.Size = new System.Drawing.Size(10, 13);
      this.lblHata.TabIndex = 9;
      this.lblHata.Text = ".";
      // 
      // frmLogin
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(240, 142);
      this.Controls.Add(this.lblHata);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.btnGiris);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txtSifre);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtKullaniciAdi);
      this.Name = "frmLogin";
      this.Text = "Giriş ";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLogin_FormClosed);
      this.Load += new System.EventHandler(this.frmLogin_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtKullaniciAdi;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtSifre;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnGiris;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label lblHata;
  }
}