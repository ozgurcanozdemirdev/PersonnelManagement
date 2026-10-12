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
  partial class LoginForm
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
      this.txtUsername = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.btnLogin = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.lblError = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // txtKullaniciAdi
      // 
      this.txtUsername.Location = new System.Drawing.Point(130, 16);
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.Size = new System.Drawing.Size(240, 20);
      this.txtUsername.TabIndex = 0;
      this.txtUsername.Text = "admin";
      this.txtUsername.TextChanged += new System.EventHandler(this.TxtUsernameTextChanged);
      this.txtUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtUsernameKeyDown);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 1;
      UiText.Bind(this.label1, "Username2");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 50);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(28, 13);
      this.label2.TabIndex = 2;
      UiText.BindComputed(this.label2, () => UiText.Get("Password") + " :");
      // 
      // txtSifre
      // 
      this.txtPassword.Location = new System.Drawing.Point(130, 49);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(240, 20);
      this.txtPassword.TabIndex = 3;
      this.txtPassword.TextChanged += new System.EventHandler(this.TxtPasswordTextChanged);
      this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtPasswordKeyDown);
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
      this.btnLogin.Location = new System.Drawing.Point(130, 85);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(240, 27);
      this.btnLogin.TabIndex = 6;
      UiText.Bind(this.btnLogin, "SignIn");
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.BtnLoginClick);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 110);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(63, 13);
      this.label5.TabIndex = 7;
      UiText.Bind(this.label5, "ErrorMessage");
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
      this.lblError.AutoSize = false;
      this.lblError.Location = new System.Drawing.Point(16, 161);
      this.lblError.Name = "lblError";
      this.lblError.Size = new System.Drawing.Size(354, 56);
      this.lblError.TabIndex = 9;
      this.lblError.Text = ".";
      // 
      // frmLogin
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(386, 230);
      this.Controls.Add(this.lblError);
      this.Controls.Add(this.btnLogin);
      this.Controls.Add(this.txtPassword);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtUsername);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Name = "LoginForm";
      UiText.Bind(this, "SignIn");
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoginFormFormClosed);
      this.Load += new System.EventHandler(this.LoginFormLoad);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

#endregion
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label lblError;
  }
}