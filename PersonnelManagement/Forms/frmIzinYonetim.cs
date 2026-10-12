using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static PersonnelManagement.DAL.IzinTuruDAL;

namespace PersonnelManagement.Forms
{
  public partial class frmIzinYonetim : Form
  {
    public frmIzinYonetim()
    {
      InitializeComponent();
    }

    private readonly IzinDal _izinDal = new IzinDal();
    private readonly IzinTuruDal _izinTuruDal = new IzinTuruDal();
    private readonly PersonelDal _personelDal = new PersonelDal();
    private int _seciliIzinId = 0;
    private bool _yukleniyor = true;
    private void frmIzinYonetim_Load(object sender, EventArgs e)
    {
      try
      {
        CombolariDoldur();
        dtpIzinBaslangic.Value = DateTime.Today;
        dtpIzinBitis.Value = DateTime.Today;
        dtpIzinBaslangic.ValueChanged += (s, ev) => ToplamGunHesapla();
        dtpIzinBitis.ValueChanged += (s, ev) => ToplamGunHesapla();
        ToplamGunHesapla();
        IzinleriYukle();
        FormTemizle();
        _yukleniyor = false;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Ekran yüklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Close();
      }
    }

    private void CombolariDoldur()
    {
      var personeller = _personelDal.TumunuGetir();
      cmbIzinPersonel.DataSource = personeller;
      cmbIzinPersonel.DisplayMember = "AdSoyad";
      cmbIzinPersonel.ValueMember = "PersonelId";
      cmbIzinPersonel.SelectedIndex = -1;
      var izinTurleri = _izinTuruDal.TumunuGetir();
      cmbIzinTuru.DataSource = izinTurleri;
      cmbIzinTuru.DisplayMember = "TuruAdi";
      cmbIzinTuru.ValueMember = "IzinTuruId";
      cmbIzinTuru.SelectedIndex = -1;
      var filtreListe = new List<Personel>();
      filtreListe.Add(new Personel { PersonelId = 0, Ad = "Tümü", Soyad = "" });
      filtreListe.AddRange(_personelDal.TumunuGetir(false));
      cmbFiltrePersonel.DataSource = filtreListe;
      cmbFiltrePersonel.DisplayMember = "AdSoyad";
      cmbFiltrePersonel.ValueMember = "PersonelId";
      cmbFiltrePersonel.SelectedIndex = 0;
    }

    private void cmbIzinPersonel_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_yukleniyor)
        return;
      var secilen = cmbIzinPersonel.SelectedItem as Personel;
      if (secilen == null || secilen.PersonelId == 0)
      {
        lblKalanIzin.Text = "Kalan yıllık izin: —";
        lblKalanIzin.ForeColor = Color.Black;
        return;
      }

      try
      {
        int kalan = _izinDal.KalanYillikIzin(secilen.PersonelId);
        lblKalanIzin.Text = $"Kalan yıllık izin: {kalan} gün";
        lblKalanIzin.ForeColor = kalan < 3 ? Color.Red : Color.Green;
      }
      catch
      {
        lblKalanIzin.Text = "Kalan yıllık izin: —";
      }
    }

    private void ToplamGunHesapla()
    {
      if (dtpIzinBitis.Value.Date < dtpIzinBaslangic.Value.Date)
      {
        lblToplamGun.Text = "⚠ Bitiş tarihi başlangıçtan önce olamaz!";
        lblToplamGun.ForeColor = Color.Red;
        return;
      }

      TimeSpan fark = dtpIzinBitis.Value.Date - dtpIzinBaslangic.Value.Date;
      int gun = fark.Days + 1;
      lblToplamGun.Text = $"Toplam: {gun} gün";
      lblToplamGun.ForeColor = Color.DarkBlue;
    }

    private void IzinleriYukle()
    {
      try
      {
        int? personelId = null;
        var secilen = cmbFiltrePersonel.SelectedItem as Personel;
        if (secilen != null && secilen.PersonelId != 0)
          personelId = secilen.PersonelId;
        List<Izin> liste = personelId.HasValue ? _izinDal.PersonelIzinleri(personelId.Value) : _izinDal.TarihAraligindaGetir(null, null);
        dgvIzinler.DataSource = null;
        dgvIzinler.DataSource = liste;
        DgvSutunlariniAyarla();
        lblToplamIzin.Text = $"Toplam: {liste.Count} kayıt";
        FormTemizle();
      }
      catch (Exception ex)
      {
        MessageBox.Show("İzin listesi yüklenirken hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvSutunlariniAyarla()
    {
      string[] gizle =
      {
        "IzinId",
        "PersonelId",
        "IzinTuruId",
        "Aciklama",
        "OnayanKullaniciId",
        "OnayTarihi",
        "RedNedeni",
        "OlusturmaTarihi",
        "BaslangicTarihi",
        "BitisTarihi"
      };
      foreach (string s in gizle)
        if (dgvIzinler.Columns[s] != null)
          dgvIzinler.Columns[s].Visible = false;
      if (dgvIzinler.Columns["PersonelAdSoyad"] != null)
      {
        dgvIzinler.Columns["PersonelAdSoyad"].HeaderText = "Ad Soyad";
        dgvIzinler.Columns["PersonelAdSoyad"].Width = 160;
      }

      if (dgvIzinler.Columns["IzinTuruAdi"] != null)
      {
        dgvIzinler.Columns["IzinTuruAdi"].HeaderText = "İzin Türü";
        dgvIzinler.Columns["IzinTuruAdi"].Width = 110;
      }

      if (dgvIzinler.Columns["BaslangicStr"] != null)
      {
        dgvIzinler.Columns["BaslangicStr"].HeaderText = "Başlangıç";
        dgvIzinler.Columns["BaslangicStr"].Width = 85;
      }

      if (dgvIzinler.Columns["BitisStr"] != null)
      {
        dgvIzinler.Columns["BitisStr"].HeaderText = "Bitiş";
        dgvIzinler.Columns["BitisStr"].Width = 85;
      }

      if (dgvIzinler.Columns["ToplamGun"] != null)
      {
        dgvIzinler.Columns["ToplamGun"].HeaderText = "Gün";
        dgvIzinler.Columns["ToplamGun"].Width = 45;
      }

      if (dgvIzinler.Columns["Durum"] != null)
      {
        dgvIzinler.Columns["Durum"].HeaderText = "Durum";
        dgvIzinler.Columns["Durum"].Width = 90;
      }

      foreach (DataGridViewRow row in dgvIzinler.Rows)
      {
        if (row.DataBoundItem is Izin izin)
        {
          switch (izin.Durum)
          {
            case "Onaylı":
              row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
              row.DefaultCellStyle.ForeColor = Color.DarkGreen;
              break;
            case "İptal Edildi":
            case "Reddedildi":
              row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
              row.DefaultCellStyle.ForeColor = Color.DarkRed;
              break;
            case "Beklemede":
              row.DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 210);
              row.DefaultCellStyle.ForeColor = Color.DarkOrange;
              break;
          }
        }
      }
    }

    private void dgvIzinler_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgvIzinler.Rows.Count)
        return;
      if (dgvIzinler.Rows[e.RowIndex].DataBoundItem is Izin izin)
      {
        _seciliIzinId = izin.IzinId;
        // Event'leri geçici kapat — SelectedItem değişince IzinleriYukle tetiklenmesin
        cmbIzinPersonel.SelectedIndexChanged -= cmbIzinPersonel_SelectedIndexChanged;
        cmbFiltrePersonel.SelectedIndexChanged -= cmbFiltrePersonel_SelectedIndexChanged;
        cmbIzinPersonel.SelectedIndex = -1;
        cmbIzinTuru.SelectedIndex = -1;
        foreach (Personel p in cmbIzinPersonel.Items)
          if (p.PersonelId == izin.PersonelId)
          {
            cmbIzinPersonel.SelectedItem = p;
            break;
          }

        foreach (IzinTuru it in cmbIzinTuru.Items)
          if (it.IzinTuruId == izin.IzinTuruId)
          {
            cmbIzinTuru.SelectedItem = it;
            break;
          }

        // Event'leri geri aç
        cmbIzinPersonel.SelectedIndexChanged += cmbIzinPersonel_SelectedIndexChanged;
        cmbFiltrePersonel.SelectedIndexChanged += cmbFiltrePersonel_SelectedIndexChanged;
        dtpIzinBaslangic.Value = izin.BaslangicTarihi;
        dtpIzinBitis.Value = izin.BitisTarihi;
        txtIzinAciklama.Text = izin.Aciklama ?? "";
        // Kalan izni manuel hesapla
        cmbIzinPersonel_SelectedIndexChanged(null, null);
        bool beklemede = izin.Durum == "Beklemede";
        bool adminMi = string.Equals(frmLogin.AktifKullanici?.Rol, "Admin", StringComparison.OrdinalIgnoreCase);
        btnIzinKaydet.Enabled = false;
        btnOnayla.Enabled = beklemede && adminMi;
        btnReddet.Enabled = beklemede && adminMi;
        btnIzinIptal.Enabled = adminMi && (izin.Durum == "Beklemede" || izin.Durum == "Onaylı");
        ToplamGunHesapla();
      }
    }

    private void btnIzinKaydet_Click(object sender, EventArgs e)
    {
      if (!FormDogrula())
        return;
      try
      {
        var seciliPersonel = (Personel)cmbIzinPersonel.SelectedItem;
        var seciliTur = (IzinTuru)cmbIzinTuru.SelectedItem;
        var izin = new Izin
        {
          PersonelId = seciliPersonel.PersonelId,
          IzinTuruId = seciliTur.IzinTuruId,
          BaslangicTarihi = dtpIzinBaslangic.Value.Date,
          BitisTarihi = dtpIzinBitis.Value.Date,
          Aciklama = txtIzinAciklama.Text.Trim()
        };
        int yeniId = _izinDal.Kaydet(izin);
        MessageBox.Show($"İzin talebi oluşturuldu. (ID: {yeniId})", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        IzinleriYukle();
        FormTemizle();
      }
      catch (InvalidOperationException iex)
      {
        MessageBox.Show(iex.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      catch (Exception ex)
      {
        MessageBox.Show("İzin kaydı sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnOnayla_Click(object sender, EventArgs e)
    {
      if (_seciliIzinId == 0)
        return;
      var onay = MessageBox.Show("Seçili izin talebi onaylanacak. Emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (onay != DialogResult.Yes)
        return;
      try
      {
        bool ok = _izinDal.Onayla(_seciliIzinId, frmLogin.AktifKullanici.KullaniciId);
        if (ok)
        {
          MessageBox.Show("İzin talebi onaylandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
          IzinleriYukle();
          FormTemizle();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Onay sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnReddet_Click(object sender, EventArgs e)
    {
      if (_seciliIzinId == 0)
        return;
      string redNedeni = null;
      using (Form frm = new Form())
      {
        frm.Text = "İzin Reddi";
        frm.Width = 380;
        frm.Height = 150;
        frm.StartPosition = FormStartPosition.CenterParent;
        frm.FormBorderStyle = FormBorderStyle.FixedDialog;
        frm.MaximizeBox = false;
        frm.MinimizeBox = false;
        var lbl = new Label
        {
          Text = "Red nedenini girin:",
          Location = new System.Drawing.Point(15, 15),
          Width = 340,
          Height = 20
        };
        var txt = new TextBox
        {
          Location = new System.Drawing.Point(15, 40),
          Width = 340,
          Height = 25
        };
        var btnOk = new Button
        {
          Text = "Reddet",
          Location = new System.Drawing.Point(185, 80),
          Width = 85,
          Height = 30,
          DialogResult = DialogResult.OK
        };
        var btnIpt = new Button
        {
          Text = "İptal",
          Location = new System.Drawing.Point(280, 80),
          Width = 75,
          Height = 30,
          DialogResult = DialogResult.Cancel
        };
        frm.Controls.Add(lbl);
        frm.Controls.Add(txt);
        frm.Controls.Add(btnOk);
        frm.Controls.Add(btnIpt);
        frm.AcceptButton = btnOk;
        frm.CancelButton = btnIpt;
        if (frm.ShowDialog(this) != DialogResult.OK)
          return;
        redNedeni = txt.Text.Trim();
      }

      if (string.IsNullOrWhiteSpace(redNedeni))
      {
        MessageBox.Show("Red nedeni boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      try
      {
        bool ok = _izinDal.Reddet(_seciliIzinId, frmLogin.AktifKullanici.KullaniciId, redNedeni);
        if (ok)
        {
          MessageBox.Show("İzin talebi reddedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
          IzinleriYukle();
          FormTemizle();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Red işlemi sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnIzinIptal_Click(object sender, EventArgs e)
    {
      if (_seciliIzinId == 0)
        return;
      var onay = MessageBox.Show("Seçili izin talebi iptal edilecek. Emin misiniz?", "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (onay != DialogResult.Yes)
        return;
      try
      {
        bool ok = _izinDal.Iptal(_seciliIzinId, frmLogin.AktifKullanici.KullaniciId);
        if (ok)
        {
          MessageBox.Show("İzin talebi iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
          IzinleriYukle();
          FormTemizle();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("İptal sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void cmbFiltrePersonel_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!_yukleniyor)
      {
        IzinleriYukle();
        FormTemizle();
      }
    }

    private void btnTumIzinler_Click(object sender, EventArgs e)
    {
      try
      {
        var liste = _izinDal.TarihAraligindaGetir(null, null);
        dgvIzinler.DataSource = null;
        dgvIzinler.DataSource = liste;
        DgvSutunlariniAyarla();
        lblToplamIzin.Text = $"Toplam: {liste.Count} kayıt";
        FormTemizle();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnTumBekleyenler_Click(object sender, EventArgs e)
    {
      try
      {
        var liste = _izinDal.BekleyenIzinler();
        dgvIzinler.DataSource = null;
        dgvIzinler.DataSource = liste;
        DgvSutunlariniAyarla();
        lblToplamIzin.Text = $"Bekleyen: {liste.Count} talep";
        FormTemizle();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnTemizle_Click(object sender, EventArgs e)
    {
      FormTemizle();
    }

    private void FormTemizle()
    {
      _seciliIzinId = 0;
      // Event'leri geçici kapat
      cmbIzinPersonel.SelectedIndexChanged -= cmbIzinPersonel_SelectedIndexChanged;
      cmbFiltrePersonel.SelectedIndexChanged -= cmbFiltrePersonel_SelectedIndexChanged;
      cmbIzinPersonel.SelectedIndex = -1;
      cmbIzinTuru.SelectedIndex = -1;
      // Event'leri geri aç
      cmbIzinPersonel.SelectedIndexChanged += cmbIzinPersonel_SelectedIndexChanged;
      cmbFiltrePersonel.SelectedIndexChanged += cmbFiltrePersonel_SelectedIndexChanged;
      dtpIzinBaslangic.Value = DateTime.Today;
      dtpIzinBitis.Value = DateTime.Today;
      txtIzinAciklama.Clear();
      lblKalanIzin.Text = "Kalan yıllık izin: —";
      lblKalanIzin.ForeColor = Color.Black;
      ToplamGunHesapla();
      btnIzinKaydet.Enabled = true;
      btnOnayla.Enabled = false;
      btnReddet.Enabled = false;
      btnIzinIptal.Enabled = false;
    }

    private bool FormDogrula()
    {
      var seciliPersonel = cmbIzinPersonel.SelectedItem as Personel;
      if (seciliPersonel == null || seciliPersonel.PersonelId == 0)
      {
        MessageBox.Show("Personel seçilmedi.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        cmbIzinPersonel.Focus();
        return false;
      }

      if (cmbIzinTuru.SelectedItem == null)
      {
        MessageBox.Show("İzin türü seçilmedi.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        cmbIzinTuru.Focus();
        return false;
      }

      if (dtpIzinBitis.Value.Date < dtpIzinBaslangic.Value.Date)
      {
        MessageBox.Show("Bitiş tarihi başlangıç tarihinden önce olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        dtpIzinBitis.Focus();
        return false;
      }

      return true;
    }

    private void btnKapat_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
