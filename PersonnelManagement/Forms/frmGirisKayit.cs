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
  public partial class frmGirisKayit : Form
  {
    public frmGirisKayit()
    {
      InitializeComponent();
    }
    private readonly GirisKayitDAL _girisKayitDal = new GirisKayitDAL();
    private readonly PersonelDal _personelDal = new PersonelDal();

    private Personel _seciliPersonel = null;
    private Timer _saatTimer;

    private void frmGirisKayit_Load(object sender, EventArgs e)
    {
      _saatTimer?.Dispose();
      _saatTimer = new Timer(components ?? (components = new Container()));
      _saatTimer.Interval = 1000;
      _saatTimer.Tick += (s, ev) => lblAnlikSaat.Text = DateTime.Now.ToString("HH:mm:ss");
      _saatTimer.Start();
      lblAnlikSaat.Text = DateTime.Now.ToString("HH:mm:ss");

      btnGirisYap.Enabled = false;
      btnCikisYap.Enabled = false;

      PersonelBilgiTemizle();
      GunlukKayitlariYukle();
      txtPersonelAra.Focus();
    }

    private void btnPersonelBul_Click(object sender, EventArgs e)
    {
      PersonelAra();
    }

    private void txtPersonelAra_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        PersonelAra();
        e.SuppressKeyPress = true;
      }
    }

    private void PersonelAra()
    {
      string arananPersonel = txtPersonelAra.Text.Trim();

      if (string.IsNullOrWhiteSpace(arananPersonel))
      {
        MessageBox.Show("Lütfen personel Adı veya TC Kimlik Numarası Giriniz.",
            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtPersonelAra.Focus();
        return;
      }

      try
      {
        PersonelBilgiTemizle();
        var liste = _personelDal.Ara(arananPersonel);

        if (liste.Count == 0)
        {
          MessageBox.Show("Personel Bulunamadı.",
              "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
          PersonelBilgiTemizle();
          return;
        }

        if (liste.Count > 1)
        {
          PersonelSec(liste);
          return;
        }

        PersonelYukle(liste[0]);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Arama sırasında hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void PersonelSec(List<Personel> liste)
    {
      using (var frm = new Form())
      {
        frm.Text = "Personel Seçin";
        frm.Size = new Size(420, 340);
        frm.StartPosition = FormStartPosition.CenterParent;
        frm.FormBorderStyle = FormBorderStyle.FixedDialog;
        frm.MaximizeBox = false;
        frm.MinimizeBox = false;

        var dgv = new DataGridView
        {
          Dock = DockStyle.Fill,
          ReadOnly = true,
          SelectionMode = DataGridViewSelectionMode.FullRowSelect,
          MultiSelect = false,
          AllowUserToAddRows = false,
          DataSource = liste
        };

        dgv.DataBindingComplete += (s, e) =>
        {
          foreach (DataGridViewColumn col in dgv.Columns)
            col.Visible = false;

          if (dgv.Columns["PersonelId"] != null) { dgv.Columns["PersonelId"].Visible = true; dgv.Columns["PersonelId"].HeaderText = "Id"; dgv.Columns["PersonelId"].Width = 40; }
          if (dgv.Columns["AdSoyad"] != null) { dgv.Columns["AdSoyad"].Visible = true; dgv.Columns["AdSoyad"].HeaderText = "Ad Soyad"; dgv.Columns["AdSoyad"].Width = 160; }
          if (dgv.Columns["DepartmanAdi"] != null) { dgv.Columns["DepartmanAdi"].Visible = true; dgv.Columns["DepartmanAdi"].HeaderText = "Departman"; dgv.Columns["DepartmanAdi"].Width = 130; }
        };

        var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 45 };
        var btnSec = new Button
        {
          Text = "Seç",
          Size = new Size(100, 30),
          Location = new Point(150, 8),
          DialogResult = DialogResult.OK
        };

        pnlAlt.Controls.Add(btnSec);
        frm.Controls.Add(dgv);
        frm.Controls.Add(pnlAlt);
        frm.AcceptButton = btnSec;

        if (frm.ShowDialog() == DialogResult.OK && dgv.SelectedRows.Count > 0)
        {
          var secilen = dgv.SelectedRows[0].DataBoundItem as Personel;
          if (secilen != null)
            PersonelYukle(secilen);
        }
      }
    }

    private void PersonelYukle(Personel p)
    {
      _seciliPersonel = p;

      lblPersonelBilgi.Text = $"{p.AdSoyad}  (Id: {p.PersonelId})";
      lblVardiyaBilgi.Text = $"Vardiya: {p.VardiyaAdi}  |  " +
                               $"Giriş: {p.BaslangicSaati:hh\\:mm}  —  " +
                               $"Çıkış: {p.BitisSaati:hh\\:mm}";
      lblDepartmanBilgi.Text = $"Departman: {p.DepartmanAdi}  |  Pozisyon: {p.PozisyonAdi}";

      bool girisVar = _girisKayitDal.BugunkuGirisVarMi(p.PersonelId);
      bool acikKayit = _girisKayitDal.AcikKayitVarMi(p.PersonelId);

      btnGirisYap.Enabled = !girisVar && !acikKayit;
      btnCikisYap.Enabled = acikKayit;

      if (!girisVar && !acikKayit)
      {
        lblDurumBilgi.Text = "● Henüz giriş yapmadı";
        lblDurumBilgi.ForeColor = Color.Gray;
      }
      else if (acikKayit)
      {
        lblDurumBilgi.Text = "● İşyerinde";
        lblDurumBilgi.ForeColor = Color.Green;
      }
      else
      {
        lblDurumBilgi.Text = "● Çıkış yapıldı";
        lblDurumBilgi.ForeColor = Color.OrangeRed;
      }
    }

    private void PersonelBilgiTemizle()
    {
      _seciliPersonel = null;
      lblPersonelBilgi.Text = "—";
      lblVardiyaBilgi.Text = "—";
      lblDepartmanBilgi.Text = "—";
      lblDurumBilgi.Text = "—";
      lblDurumBilgi.ForeColor = Color.Gray;
      btnGirisYap.Enabled = false;
      btnCikisYap.Enabled = false;
    }

    private void btnGirisYap_Click(object sender, EventArgs e)
    {
      if (_seciliPersonel == null) return;

      var onay = MessageBox.Show(
          $"{_seciliPersonel.AdSoyad} için giriş kaydedilecek.\n" +
          $"Saat: {DateTime.Now:HH:mm}\n\nOnaylıyor musunuz?",
          "Giriş Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if (onay != DialogResult.Yes) return;

      try
      {
        _girisKayitDal.GirisYap(
            _seciliPersonel.PersonelId,
            frmLogin.AktifKullanici.KullaniciId);

        MessageBox.Show(
            $"{_seciliPersonel.AdSoyad} giriş kaydı oluşturuldu.\nSaat: {DateTime.Now:HH:mm}",
            "Giriş Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

        PersonelYukle(_seciliPersonel);
        GunlukKayitlariYukle();
        txtPersonelAra.Clear();
        txtPersonelAra.Focus();
      }
      catch (InvalidOperationException iex)
      {
        MessageBox.Show(iex.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Giriş kaydı oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnCikisYap_Click(object sender, EventArgs e)
    {
      if (_seciliPersonel == null) return;

      var onay = MessageBox.Show(
          $"{_seciliPersonel.AdSoyad} için çıkış kaydedilecek.\n" +
          $"Saat: {DateTime.Now:HH:mm}\n\nOnaylıyor musunuz?",
          "Çıkış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if (onay != DialogResult.Yes) return;

      try
      {
        bool ok = _girisKayitDal.CikisYap(
            _seciliPersonel.PersonelId,
            frmLogin.AktifKullanici.KullaniciId);

        if (ok)
        {
          MessageBox.Show(
              $"{_seciliPersonel.AdSoyad} çıkış kaydı oluşturuldu.\nSaat: {DateTime.Now:HH:mm}",
              "Çıkış Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

          PersonelYukle(_seciliPersonel);
          GunlukKayitlariYukle();
          txtPersonelAra.Clear();
          txtPersonelAra.Focus();
        }
      }
      catch (InvalidOperationException iex)
      {
        MessageBox.Show(iex.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Çıkış kaydı oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void GunlukKayitlariYukle()
    {
      try
      {
        var liste = _girisKayitDal.BugunkuKayitlar();

        dgvGunlukKayitlar.DataSource = null;
        dgvGunlukKayitlar.DataSource = liste;
        DgvSutunlariniAyarla();

        lblGunlukToplamGiris.Text =
            $"Bugün/açık kayıtlar: {liste.Count}  |  Tarih: {DateTime.Today:dd.MM.yyyy}";
      }
      catch (Exception ex)
      {
        MessageBox.Show("Günlük kayıtlar yüklenirken hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvSutunlariniAyarla()
    {
      string[] gizle = {
                "GirisKayitId","PersonelId","GirisZamani","CikisZamani",
                "MesaiDakika","Aciklama","KayitTarihi","DepartmanAdi"
            };
      foreach (string s in gizle)
        if (dgvGunlukKayitlar.Columns[s] != null)
          dgvGunlukKayitlar.Columns[s].Visible = false;

      if (dgvGunlukKayitlar.Columns["AdSoyad"] != null) { dgvGunlukKayitlar.Columns["AdSoyad"].HeaderText = "Ad Soyad"; dgvGunlukKayitlar.Columns["AdSoyad"].Width = 160; }
      if (dgvGunlukKayitlar.Columns["VardiyaAdi"] != null) { dgvGunlukKayitlar.Columns["VardiyaAdi"].HeaderText = "Vardiya"; dgvGunlukKayitlar.Columns["VardiyaAdi"].Width = 120; }
      if (dgvGunlukKayitlar.Columns["GirisSaatiStr"] != null) { dgvGunlukKayitlar.Columns["GirisSaatiStr"].HeaderText = "Giriş"; dgvGunlukKayitlar.Columns["GirisSaatiStr"].Width = 70; }
      if (dgvGunlukKayitlar.Columns["CikisSaatiStr"] != null) { dgvGunlukKayitlar.Columns["CikisSaatiStr"].HeaderText = "Çıkış"; dgvGunlukKayitlar.Columns["CikisSaatiStr"].Width = 70; }
      if (dgvGunlukKayitlar.Columns["CalismaSuresiStr"] != null) { dgvGunlukKayitlar.Columns["CalismaSuresiStr"].HeaderText = "Çalışma Süresi"; dgvGunlukKayitlar.Columns["CalismaSuresiStr"].Width = 100; }
      if (dgvGunlukKayitlar.Columns["GecGeldiMi"] != null) { dgvGunlukKayitlar.Columns["GecGeldiMi"].HeaderText = "Geç Geldi Mi?"; dgvGunlukKayitlar.Columns["GecGeldiMi"].Width = 45; }
      if (dgvGunlukKayitlar.Columns["ErkenCiktiMi"] != null) { dgvGunlukKayitlar.Columns["ErkenCiktiMi"].HeaderText = "Erken Çıkıştı Mı?"; dgvGunlukKayitlar.Columns["ErkenCiktiMi"].Width = 80; }

      foreach (DataGridViewRow row in dgvGunlukKayitlar.Rows)
      {
        if (row.DataBoundItem is GirisKayit kayit)
        {
          if (kayit.GecGeldiMi)
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
          else if (!kayit.CikisSaati.HasValue)
            row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
        }
      }
    }

    private void btnYenile_Click(object sender, EventArgs e)
    {
      GunlukKayitlariYukle();
      PersonelBilgiTemizle();
      txtPersonelAra.Clear();
      txtPersonelAra.Focus();
    }

    private void frmGirisKayit_FormClosing(object sender, FormClosingEventArgs e)
    {
      _saatTimer?.Stop();
      _saatTimer?.Dispose();
    }

    private void btnKapat_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
