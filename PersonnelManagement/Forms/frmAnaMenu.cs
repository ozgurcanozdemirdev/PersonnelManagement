using PersonnelManagement.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PersonnelManagement.DAL.IzinTuruDAL;

namespace PersonnelManagement.Forms
{
  public partial class frmAnaMenu : Form
  {
    public frmAnaMenu()
    {
      InitializeComponent();
    }
    private readonly RaporDal _raporDal = new RaporDal();
    private Timer _saatTimer;
    private void frmAnaMenu_Load(object sender, EventArgs e)
    {

      lblHosgeldin.Text = $"Hoş geldiniz,  {frmLogin.AktifKullanici.AdSoyad}";

      _saatTimer = new Timer();
      _saatTimer.Interval = 1000;
      _saatTimer.Tick += (s, ev) => SaatGuncelle();
      _saatTimer.Start();
      SaatGuncelle();

      OzetYukle();

      if (frmLogin.AktifKullanici.Rol != "Admin")
        btnRaporlar.Enabled = false;
    }
    private void SaatTimer_Tick(object sender, EventArgs e) => SaatGuncelle();

    private void SaatGuncelle()
    {
      lblTarihSaat.Text = DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss");
    }
    private void OzetYukle()
    {
      try
      {
        var ozet = _raporDal.BugunOzet();
        lblToplamPersonel.Text = ozet.toplamPersonel.ToString();
        lblBugunGiren.Text = ozet.bugunGiren.ToString();
        lblBugunIzinli.Text = ozet.bugunIzinli.ToString();
        lblBugunDevamsiz.Text = ozet.bugunDevamsiz.ToString();
      }
      catch (Exception ex)
      {
        MessageBox.Show("İstatistik yüklenirken hata: " + ex.Message,
            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void btnCikis_Click(object sender, EventArgs e)
    {
      var sonuc = MessageBox.Show(
                "Sistemden çıkmak istediğinize emin misiniz?",
                "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if (sonuc == DialogResult.Yes)
      {
        SistemLogger.Log("Logout", null, null, $"{frmLogin.AktifKullanici.KullaniciAdi} çıkış yaptı.");
        _saatTimer?.Stop();
        Application.Exit();
      }
    }

    private void btnPersonelYonetim_Click(object sender, EventArgs e)
    {
      AcForm(new frmPersonelYonetim());
    }

    private void btnVardiyaYonetim_Click(object sender, EventArgs e)
    {
      AcForm(new frmVardiyaYonetim());
    }

    private void btnGirisKayit_Click(object sender, EventArgs e)
    {
      AcForm(new frmGirisKayit());
    }

    private void btnRaporlar_Click(object sender, EventArgs e)
    {
      AcForm(new frmRaporlar());
    }

    private void btnIzinYonetimi_Click(object sender, EventArgs e)
    {
      AcForm(new frmIzinYonetim());
    }

    private void btnYenile_Click(object sender, EventArgs e)
    {
      OzetYukle();
    }

    private void frmAnaMenu_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        var sonuc = MessageBox.Show("Sistemden çıkmak istediğinize emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (sonuc == DialogResult.No) { e.Cancel = true; return; }

        _saatTimer?.Stop();
        Application.Exit();
      }
    }
    private void AcForm(Form form)
    {
      form.FormClosed += (s, e) => OzetYukle();
      form.Show();
    }
  }
}
