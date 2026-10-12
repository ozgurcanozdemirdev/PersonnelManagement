using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PersonnelManagement.Forms
{
  public partial class frmPersonelYonetim : Form
  {
    public frmPersonelYonetim()
    {
      InitializeComponent();
      dtpDogumTarihi.ShowCheckBox = true;
      btnAra.Click -= btnAra_Click;
      btnAra.Click += btnAra_Click;
      txtAra.TextChanged -= txtAra_TextChanged;
      txtAra.TextChanged += txtAra_TextChanged;
      FormClosed += (s, e) =>
      {
        picFoto.Image?.Dispose();
      };
    }

    private readonly PersonelDal _personelDal = new PersonelDal();
    private readonly DepartmanDAL _departmanDal = new DepartmanDAL();
    private readonly PozisyonDal _pozisyonDal = new PozisyonDal();
    private readonly VardiyaDal _vardiyaDal = new VardiyaDal();
    private int _seciliPersonelId = 0;
    private string _seciliFotoPath = null;
    private void frmPersonelYonetim_Load(object sender, EventArgs e)
    {
      try
      {
        CombolariDoldur();
        PersonelleriYukle();
        FormTemizle();
        dgvPersonel.CellClick += dgvPersonel_CellClick;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Ekran yüklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Close();
      }
    }

    private void CombolariDoldur()
    {
      var departmanlar = _departmanDal.TumunuGetir();
      cmbDepartman.DataSource = departmanlar;
      cmbDepartman.DisplayMember = "DepartmanAdi";
      cmbDepartman.ValueMember = "DepartmanId";
      cmbDepartman.SelectedIndex = -1;
      var vardiyalar = _vardiyaDal.TumunuGetir();
      cmbVardiya.DataSource = vardiyalar;
      cmbVardiya.DisplayMember = "VardiyaAdi";
      cmbVardiya.ValueMember = "VardiyaId";
      cmbVardiya.SelectedIndex = -1;
      cmbCinsiyet.Items.Clear();
      cmbCinsiyet.Items.Add("Erkek");
      cmbCinsiyet.Items.Add("Kadın");
      cmbCinsiyet.SelectedIndex = -1;
    }

    private void cmbDepartman_SelectedIndexChanged(object sender, EventArgs e)
    {
      cmbPozisyon.DataSource = null;
      if (cmbDepartman.SelectedItem == null)
        return;
      // SelectedValue yerine SelectedItem üzerinden cast et
      int depId = ((Departman)cmbDepartman.SelectedItem).DepartmanId;
      var pozisyonlar = _pozisyonDal.TumunuGetir(depId);
      cmbPozisyon.DataSource = pozisyonlar;
      cmbPozisyon.DisplayMember = "PozisyonAdi";
      cmbPozisyon.ValueMember = "PozisyonId";
      cmbPozisyon.SelectedIndex = -1;
    }

    private void PersonelleriYukle(string aramaMetni = null)
    {
      try
      {
        List<Personel> liste = string.IsNullOrWhiteSpace(aramaMetni) ? _personelDal.TumunuGetir() : _personelDal.Ara(aramaMetni);
        dgvPersonel.DataSource = null;
        dgvPersonel.DataSource = liste;
        DgvSutunlariniAyarla();
        lblToplamKayit.Text = $"{liste.Count} Personel";
        FormTemizle(false);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Personel listesi yüklenirken hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvSutunlariniAyarla()
    {
      string[] gizle =
      {
        "TcKimlikNo",
        "Cinsiyet",
        "DogumTarihi",
        "Telefon",
        "Email",
        "Adres",
        "FotoPath",
        "DepartmanId",
        "PozisyonId",
        "VardiyaId",
        "BaslangicSaati",
        "BitisSaati",
        "MolaDakika",
        "IstenCikisTarihi",
        "OlusturmaTarihi",
        "GuncellemeTarihi",
        "AktifMi"
      };
      foreach (string s in gizle)
        if (dgvPersonel.Columns[s] != null)
          dgvPersonel.Columns[s].Visible = false;
      if (dgvPersonel.Columns["PersonelId"] != null)
      {
        dgvPersonel.Columns["PersonelId"].HeaderText = "ID";
        dgvPersonel.Columns["PersonelId"].Width = 40;
      }

      if (dgvPersonel.Columns["AdSoyad"] != null)
      {
        dgvPersonel.Columns["AdSoyad"].HeaderText = "Ad Soyad";
        dgvPersonel.Columns["AdSoyad"].Width = 160;
      }

      if (dgvPersonel.Columns["DepartmanAdi"] != null)
      {
        dgvPersonel.Columns["DepartmanAdi"].HeaderText = "Departman";
        dgvPersonel.Columns["DepartmanAdi"].Width = 130;
      }

      if (dgvPersonel.Columns["PozisyonAdi"] != null)
      {
        dgvPersonel.Columns["PozisyonAdi"].HeaderText = "Pozisyon";
        dgvPersonel.Columns["PozisyonAdi"].Width = 130;
      }

      if (dgvPersonel.Columns["VardiyaAdi"] != null)
      {
        dgvPersonel.Columns["VardiyaAdi"].HeaderText = "Vardiya";
        dgvPersonel.Columns["VardiyaAdi"].Width = 110;
      }

      if (dgvPersonel.Columns["IseGirisTarihi"] != null)
      {
        dgvPersonel.Columns["IseGirisTarihi"].HeaderText = "İşe Giriş";
        dgvPersonel.Columns["IseGirisTarihi"].Width = 90;
      }

      if (dgvPersonel.Columns["YillikIzinHakki"] != null)
      {
        dgvPersonel.Columns["YillikIzinHakki"].HeaderText = "İzin Hakkı";
        dgvPersonel.Columns["YillikIzinHakki"].Width = 70;
      }
    }

    private void dgvPersonel_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgvPersonel.Rows.Count)
        return;
      try
      {
        int personelId = Convert.ToInt32(dgvPersonel.Rows[e.RowIndex].Cells["PersonelId"].Value);
        Personel p = _personelDal.IdIleGetir(personelId);
        if (p == null)
        {
          FormTemizle(false);
          return;
        }

        _seciliPersonelId = p.PersonelId;
        _seciliFotoPath = p.FotoPath;
        txtAd.Text = p.Ad;
        txtSoyad.Text = p.Soyad;
        txtTCKimlik.Text = p.TcKimlikNo;
        txtTelefon.Text = p.Telefon;
        txtEmail.Text = p.Email;
        txtAdres.Text = p.Adres;
        txtYillikIzinHakki.Text = p.YillikIzinHakki.ToString();
        dtpIseGirisTarihi.Value = p.IseGirisTarihi;
        dtpDogumTarihi.Checked = false;
        if (p.DogumTarihi.HasValue)
        {
          dtpDogumTarihi.Value = p.DogumTarihi.Value;
          dtpDogumTarihi.Checked = true;
        }

        cmbDepartman.SelectedIndex = -1;
        cmbVardiya.SelectedIndex = -1;
        cmbCinsiyet.SelectedItem = p.Cinsiyet;
        foreach (Departman d in cmbDepartman.Items)
          if (d.DepartmanId == p.DepartmanId)
          {
            cmbDepartman.SelectedItem = d;
            break;
          }

        foreach (Pozisyon pz in cmbPozisyon.Items)
          if (pz.PozisyonId == p.PozisyonId)
          {
            cmbPozisyon.SelectedItem = pz;
            break;
          }

        foreach (Vardiya v in cmbVardiya.Items)
          if (v.VardiyaId == p.VardiyaId)
          {
            cmbVardiya.SelectedItem = v;
            break;
          }

        FotografYukle(p.FotoPath);
        btnKaydet.Enabled = false;
        btnGuncelle.Enabled = true;
        btnSil.Enabled = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Kayıt yüklenirken hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnKaydet_Click(object sender, EventArgs e)
    {
      if (!FormDogrula())
        return;
      try
      {
        Personel p = FormdenPersonelOlustur();
        if (_personelDal.TcKimlikMevcutMu(p.TcKimlikNo))
        {
          MessageBox.Show("Bu TC Kimlik No zaten kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          txtTCKimlik.Focus();
          return;
        }

        int yeniId = _personelDal.Kaydet(p);
        MessageBox.Show($"Personel başarıyla kaydedildi. (ID: {yeniId})", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        PersonelleriYukle();
        FormTemizle();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnGuncelle_Click(object sender, EventArgs e)
    {
      if (_seciliPersonelId == 0)
      {
        MessageBox.Show("Lütfen güncellenecek personeli listeden seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (!FormDogrula())
        return;
      try
      {
        Personel p = FormdenPersonelOlustur();
        p.PersonelId = _seciliPersonelId;
        if (_personelDal.TcKimlikMevcutMu(p.TcKimlikNo, _seciliPersonelId))
        {
          MessageBox.Show("Bu TC Kimlik No başka bir personele ait!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          txtTCKimlik.Focus();
          return;
        }

        bool ok = _personelDal.Guncelle(p);
        if (ok)
        {
          MessageBox.Show("Personel bilgileri güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
          PersonelleriYukle();
          FormTemizle();
        }
        else
        {
          MessageBox.Show("Personel kaydı artık aktif değil. Liste yenilenecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          PersonelleriYukle(txtAra.Text.Trim());
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Güncelleme sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnSil_Click(object sender, EventArgs e)
    {
      if (_seciliPersonelId == 0)
      {
        MessageBox.Show("Lütfen silinecek personeli listeden seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var onay = MessageBox.Show($"'{txtAd.Text} {txtSoyad.Text}' adlı personel pasife alınacak. Emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (onay != DialogResult.Yes)
        return;
      try
      {
        bool ok = _personelDal.Sil(_seciliPersonelId, DateTime.Today);
        if (ok)
        {
          MessageBox.Show("Personel pasife alındı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
          PersonelleriYukle();
          FormTemizle();
        }
        else
        {
          MessageBox.Show("Personel kaydı artık aktif değil. Liste yenilenecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          PersonelleriYukle(txtAra.Text.Trim());
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Silme sırasında hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnAra_Click(object sender, EventArgs e)
    {
      PersonelleriYukle(txtAra.Text.Trim());
    }

    private void btnAra_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        PersonelleriYukle(txtAra.Text.Trim());
        e.SuppressKeyPress = true;
      }
    }

    private void txtAra_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        PersonelleriYukle(txtAra.Text.Trim());
        e.SuppressKeyPress = true;
      }
    }

    private void txtAra_TextChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtAra.Text))
        PersonelleriYukle();
    }

    private void btnTemizle_Click(object sender, EventArgs e)
    {
      FormTemizle();
    }

    private void FormTemizle(bool aramayiTemizle = true)
    {
      _seciliPersonelId = 0;
      _seciliFotoPath = null;
      txtAd.Clear();
      txtSoyad.Clear();
      txtTCKimlik.Clear();
      txtTelefon.Clear();
      txtEmail.Clear();
      txtAdres.Clear();
      txtYillikIzinHakki.Text = "14";
      if (aramayiTemizle)
        txtAra.Clear();
      cmbDepartman.SelectedIndex = -1;
      cmbPozisyon.DataSource = null;
      cmbVardiya.SelectedIndex = -1;
      cmbCinsiyet.SelectedIndex = -1;
      dtpIseGirisTarihi.Value = DateTime.Today;
      dtpDogumTarihi.Value = DateTime.Today.AddYears(-25);
      dtpDogumTarihi.Checked = false;
      picFoto.Image?.Dispose();
      picFoto.Image = null;
      btnKaydet.Enabled = true;
      btnGuncelle.Enabled = false;
      btnSil.Enabled = false;
      txtAd.Focus();
    }

    private void btnFotoSec_Click(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog())
      {
        ofd.Title = "Fotoğraf Seç";
        ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
        if (ofd.ShowDialog() != DialogResult.OK)
          return;
        _seciliFotoPath = ofd.FileName;
        FotografYukle(_seciliFotoPath);
      }
    }

    private void FotografYukle(string path)
    {
      Image yeni = null;
      try
      {
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
          // Bitmap akıştan kopyalanır; dosya ve önceki resim açık tutulmaz.
          using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
          using (Image kaynak = Image.FromStream(stream))
            yeni = new Bitmap(kaynak);
        }
      }
      catch (ArgumentException)
      {
      }
      catch (IOException)
      {
      }
      catch (UnauthorizedAccessException)
      {
      }

      Image onceki = picFoto.Image;
      picFoto.Image = yeni;
      onceki?.Dispose();
    }

    private bool FormDogrula()
    {
      if (string.IsNullOrWhiteSpace(txtAd.Text))
      {
        Uyar("Ad alanı boş bırakılamaz.", txtAd);
        return false;
      }

      if (string.IsNullOrWhiteSpace(txtSoyad.Text))
      {
        Uyar("Soyad alanı boş bırakılamaz.", txtSoyad);
        return false;
      }

      if (string.IsNullOrWhiteSpace(txtTCKimlik.Text) || txtTCKimlik.Text.Length != 11)
      {
        Uyar("TC Kimlik No 11 haneli olmalıdır.", txtTCKimlik);
        return false;
      }

      if (txtTCKimlik.Text.Any(c => c < '0' || c > '9'))
      {
        Uyar("TC Kimlik No yalnızca rakamlardan oluşmalıdır.", txtTCKimlik);
        return false;
      }

      if (cmbDepartman.SelectedValue == null)
      {
        Uyar("Departman seçilmedi.", cmbDepartman);
        return false;
      }

      if (cmbPozisyon.SelectedValue == null)
      {
        Uyar("Pozisyon seçilmedi.", cmbPozisyon);
        return false;
      }

      if (cmbVardiya.SelectedValue == null)
      {
        Uyar("Vardiya seçilmedi.", cmbVardiya);
        return false;
      }

      if (!int.TryParse(txtYillikIzinHakki.Text, out int izin) || izin < 0)
      {
        Uyar("Geçerli bir yıllık izin hakkı girin.", txtYillikIzinHakki);
        return false;
      }

      try
      {
        EmployeeValidation.Validate(FormdenPersonelOlustur());
      }
      catch (ArgumentException ex)
      {
        Uyar(ex.Message, null);
        return false;
      }

      return true;
    }

    private void Uyar(string mesaj, Control odak)
    {
      MessageBox.Show(mesaj, "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      odak?.Focus();
    }

    private Personel FormdenPersonelOlustur() => new Personel
    {
      Ad = txtAd.Text.Trim(),
      Soyad = txtSoyad.Text.Trim(),
      TcKimlikNo = txtTCKimlik.Text.Trim(),
      Cinsiyet = cmbCinsiyet.SelectedItem?.ToString(),
      DogumTarihi = dtpDogumTarihi.Checked ? (DateTime? )dtpDogumTarihi.Value.Date : null,
      Telefon = txtTelefon.Text.Trim(),
      Email = txtEmail.Text.Trim(),
      Adres = txtAdres.Text.Trim(),
      FotoPath = _seciliFotoPath,
      DepartmanId = Convert.ToInt32(cmbDepartman.SelectedValue),
      PozisyonId = Convert.ToInt32(cmbPozisyon.SelectedValue),
      VardiyaId = Convert.ToInt32(cmbVardiya.SelectedValue),
      IseGirisTarihi = dtpIseGirisTarihi.Value.Date,
      YillikIzinHakki = Convert.ToInt32(txtYillikIzinHakki.Text)
    };
    private void btnKapat_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
