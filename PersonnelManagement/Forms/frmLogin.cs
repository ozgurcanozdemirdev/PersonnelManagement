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

    private readonly KullaniciDal _kullaniciDal = new KullaniciDal();

    public static Kullanici AktifKullanici { get; private set; }


    private void frmLogin_Load(object sender, EventArgs e)
    {
      txtSifre.PasswordChar = '*';
      txtSifre.MaxLength = 50;
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

      try
      {
        btnGiris.Enabled = false;
        lblHata.Visible = false;

        Kullanici kullanici = _kullaniciDal.GirisDogrula(
            txtKullaniciAdi.Text.Trim(),
            txtSifre.Text);

        if (kullanici == null)
        {
          HataGoster("Kullanıcı adı veya şifre hatalı!");
          txtSifre.Clear();
          txtSifre.Focus();
          return;
        }

        AktifKullanici = kullanici;

        frmAnaMenu anaMenu = new frmAnaMenu();
        anaMenu.Show();
        this.Hide();
      }
      catch (Exception ex)
      {
        HataGoster("Bağlantı hatası: " + ex.Message);
      }
      finally
      {
        btnGiris.Enabled = true;
      }
    }

    private void HataGoster(string mesaj)
    {
      lblHata.Text = mesaj;
      lblHata.Visible = true;
    }

    private void txtKullaniciAdi_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter) { txtSifre.Focus(); e.SuppressKeyPress = true; }
    }

    private void txtSifre_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter) { GirisYap(); e.SuppressKeyPress = true; }
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
