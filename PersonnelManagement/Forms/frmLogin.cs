using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonnelManagement.Forms
{
  public partial class frmLogin : Form
  {
    public frmLogin()
    {
      InitializeComponent();
    }

    private bool _girisDevamEdiyor;
    private readonly KullaniciDal _kullaniciDal = new KullaniciDal();
    public static Kullanici AktifKullanici { get; private set; }

    private void frmLogin_Load(object sender, EventArgs e)
    {
      txtSifre.PasswordChar = '*';
      txtSifre.MaxLength = 100;
      txtKullaniciAdi.MaxLength = 50;
      lblHata.Visible = false;
      lblHata.ForeColor = Color.Red;
      this.AcceptButton = btnGiris;
      txtKullaniciAdi.Select();
    }

    private void btnGiris_Click(object sender, EventArgs e)
    {
      GirisYap();
    }

    private void GirisYap()
    {
      if (_girisDevamEdiyor)
        return;
      if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text))
      {
        HataGoster("Kullanıcı adı boş bırakılamaz.");
        txtKullaniciAdi.Focus();
        return;
      }

      if (string.IsNullOrWhiteSpace(txtSifre.Text))
      {
        HataGoster("Şifre boş bırakılamaz.");
        txtSifre.Focus();
        return;
      }

      frmAnaMenu anaMenu = null;
      try
      {
        _girisDevamEdiyor = true;
        AktifKullanici = null;
        btnGiris.Enabled = false;
        lblHata.Visible = false;
        Kullanici kullanici = _kullaniciDal.GirisDogrula(txtKullaniciAdi.Text.Trim(), txtSifre.Text);
        if (kullanici == null)
        {
          HataGoster("Kullanıcı adı veya şifre hatalı!");
          txtSifre.Clear();
          txtSifre.Focus();
          return;
        }

        AktifKullanici = kullanici;
        anaMenu = new frmAnaMenu();
        anaMenu.Show();
        if (anaMenu.IsDisposed)
          throw new InvalidOperationException("Ana menü açılamadı.");
        anaMenu.FormClosed += (s, ev) => Close();
        this.Hide();
      }
      catch (Exception ex)
      {
        anaMenu?.Dispose();
        AktifKullanici = null;
        SistemLogger.ClearKullanici();
        HataGoster("Giriş tamamlanamadı: " + ex.Message);
      }
      finally
      {
        _girisDevamEdiyor = false;
        if (!IsDisposed)
          btnGiris.Enabled = true;
      }
    }

    private void HataGoster(string mesaj)
    {
      lblHata.MaximumSize = new Size(Math.Max(80, ClientSize.Width - lblHata.Left - 8), 0);
      lblHata.Text = mesaj;
      ClientSize = new Size(ClientSize.Width, Math.Max(ClientSize.Height, lblHata.Bottom + 12));
      lblHata.Visible = true;
    }

    private void txtKullaniciAdi_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        txtSifre.Focus();
        e.SuppressKeyPress = true;
      }
    }

    private void txtSifre_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        GirisYap();
        e.SuppressKeyPress = true;
      }
    }

    private void txtKullaniciAdi_TextChanged(object sender, EventArgs e)
    {
      lblHata.Visible = false;
    }

    private void txtSifre_TextChanged(object sender, EventArgs e)
    {
      lblHata.Visible = false;
    }

    private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (AktifKullanici == null)
        Application.Exit();
    }
  }
}
