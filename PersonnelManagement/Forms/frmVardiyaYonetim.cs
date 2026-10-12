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
  public partial class frmVardiyaYonetim : Form
  {
    public frmVardiyaYonetim()
    {
      InitializeComponent();
    }
    private readonly VardiyaDal _vardiyaDal = new VardiyaDal();
    private int _seciliVardiyaId = 0;

    private void frmVardiyaYonetim_Load(object sender, EventArgs e)
    {
      dtpBaslangicSaati.Format = DateTimePickerFormat.Custom;
      dtpBaslangicSaati.CustomFormat = "HH:mm";
      dtpBaslangicSaati.ShowUpDown = true;

      dtpBitisSaati.Format = DateTimePickerFormat.Custom;
      dtpBitisSaati.CustomFormat = "HH:mm";
      dtpBitisSaati.ShowUpDown = true;

      dtpBaslangicSaati.Value = DateTime.Today.AddHours(8);
      dtpBitisSaati.Value = DateTime.Today.AddHours(17);

      VardiyalariYukle();
      FormTemizle();
    }

    private void VardiyalariYukle()
    {
      try
      {
        var liste = _vardiyaDal.TumunuGetir(sadeceAktif: false);
        dgvVardiya.DataSource = null;
        dgvVardiya.DataSource = liste;
        DgvSutunlariniAyarla();
        lblToplamKayit.Text = $"Toplam: {liste.Count} vardiya";
      }
      catch (Exception ex)
      {
        MessageBox.Show("Vardiya listesi yüklenirken hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvSutunlariniAyarla()
    {
      if (dgvVardiya.Columns["AktifMi"] != null)
        dgvVardiya.Columns["AktifMi"].Visible = false;
      if (dgvVardiya.Columns["VardiyaId"] != null) { dgvVardiya.Columns["VardiyaId"].HeaderText = "ID"; dgvVardiya.Columns["VardiyaId"].Width = 35; }
      if (dgvVardiya.Columns["VardiyaAdi"] != null) { dgvVardiya.Columns["VardiyaAdi"].HeaderText = "Vardiya Adı"; dgvVardiya.Columns["VardiyaAdi"].Width = 150; }
      if (dgvVardiya.Columns["BaslangicSaati"] != null) { dgvVardiya.Columns["BaslangicSaati"].HeaderText = "Başlangıç"; dgvVardiya.Columns["BaslangicSaati"].Width = 75; }
      if (dgvVardiya.Columns["BitisSaati"] != null) { dgvVardiya.Columns["BitisSaati"].HeaderText = "Bitiş"; dgvVardiya.Columns["BitisSaati"].Width = 75; }
      if (dgvVardiya.Columns["MolaDakika"] != null) { dgvVardiya.Columns["MolaDakika"].HeaderText = "Mola (dk)"; dgvVardiya.Columns["MolaDakika"].Width = 70; }
      if (dgvVardiya.Columns["Pazartesi"] != null) { dgvVardiya.Columns["Pazartesi"].HeaderText = "Pzt"; dgvVardiya.Columns["Pazartesi"].Width = 38; }
      if (dgvVardiya.Columns["Sali"] != null) { dgvVardiya.Columns["Sali"].HeaderText = "Sal"; dgvVardiya.Columns["Sali"].Width = 38; }
      if (dgvVardiya.Columns["Carsamba"] != null) { dgvVardiya.Columns["Carsamba"].HeaderText = "Çar"; dgvVardiya.Columns["Carsamba"].Width = 38; }
      if (dgvVardiya.Columns["Persembe"] != null) { dgvVardiya.Columns["Persembe"].HeaderText = "Per"; dgvVardiya.Columns["Persembe"].Width = 38; }
      if (dgvVardiya.Columns["Cuma"] != null) { dgvVardiya.Columns["Cuma"].HeaderText = "Cum"; dgvVardiya.Columns["Cuma"].Width = 38; }
      if (dgvVardiya.Columns["Cumartesi"] != null) { dgvVardiya.Columns["Cumartesi"].HeaderText = "Cmt"; dgvVardiya.Columns["Cumartesi"].Width = 38; }
      if (dgvVardiya.Columns["Pazar"] != null) { dgvVardiya.Columns["Pazar"].HeaderText = "Paz"; dgvVardiya.Columns["Pazar"].Width = 38; }

    }

    private void dgvVardiya_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0) return;

      try
      {
        int vardiyaId = Convert.ToInt32(dgvVardiya.Rows[e.RowIndex].Cells["VardiyaId"].Value);
        Vardiya v = _vardiyaDal.IdIleGetir(vardiyaId);
        if (v == null) return;

        _seciliVardiyaId = v.VardiyaId;

        txtVardiyaAdi.Text = v.VardiyaAdi;
        txtMolaSuresi.Text = v.MolaDakika.ToString();

        dtpBaslangicSaati.Value = DateTime.Today.Add(v.BaslangicSaati);
        dtpBitisSaati.Value = DateTime.Today.Add(v.BitisSaati);

        chkPazartesi.Checked = v.Pazartesi;
        chkSali.Checked = v.Sali;
        chkCarsamba.Checked = v.Carsamba;
        chkPersembe.Checked = v.Persembe;
        chkCuma.Checked = v.Cuma;
        chkCumartesi.Checked = v.Cumartesi;
        chkPazar.Checked = v.Pazar;

        btnVardiyaKaydet.Enabled = false;
        btnVardiyaGuncelle.Enabled = true;
        btnVardiyaSil.Enabled = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Kayıt yüklenirken hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnVardiyaKaydet_Click(object sender, EventArgs e)
    {
      if (!FormDogrula()) return;

      try
      {
        int yeniId = _vardiyaDal.Kaydet(FormdenVardiyaOlustur());
        MessageBox.Show($"Vardiya başarıyla kaydedildi. (ID: {yeniId})",
            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        VardiyalariYukle();
        FormTemizle();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnVardiyaGuncelle_Click(object sender, EventArgs e)
    {

      if (_seciliVardiyaId == 0)
      {
        MessageBox.Show("Lütfen güncellenecek vardiyayı seçin.",
            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (!FormDogrula()) return;

      try
      {
        Vardiya v = FormdenVardiyaOlustur();
        v.VardiyaId = _seciliVardiyaId;

        bool ok = _vardiyaDal.Guncelle(v);
        if (ok)
        {
          MessageBox.Show("Vardiya güncellendi.",
              "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
          VardiyalariYukle();
          FormTemizle();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Güncelleme sırasında hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnVardiyaSil_Click(object sender, EventArgs e)
    {
      if (_seciliVardiyaId == 0)
      {
        MessageBox.Show("Lütfen silinecek vardiyayı seçin.",
            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var onay = MessageBox.Show(
          $"'{txtVardiyaAdi.Text}' vardiyası pasife alınacak. Emin misiniz?",
          "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if (onay != DialogResult.Yes) return;

      try
      {
        bool ok = _vardiyaDal.Sil(_seciliVardiyaId);
        if (ok)
        {
          MessageBox.Show("Vardiya pasife alındı.",
              "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
          VardiyalariYukle();
          FormTemizle();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Silme sırasında hata:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnTumHafta_Click(object sender, EventArgs e)
    {
      chkPazartesi.Checked = chkSali.Checked = chkCarsamba.Checked =
            chkPersembe.Checked = chkCuma.Checked = chkCumartesi.Checked =
            chkPazar.Checked = true;
    }

    private void btnHaftaIci_Click(object sender, EventArgs e)
    {
      chkPazartesi.Checked = chkSali.Checked = chkCarsamba.Checked =
             chkPersembe.Checked = chkCuma.Checked = true;
      chkCumartesi.Checked = chkPazar.Checked = false;
    }

    private void btnHaftaSonu_Click(object sender, EventArgs e)
    {
      chkPazartesi.Checked = chkSali.Checked = chkCarsamba.Checked =
            chkPersembe.Checked = chkCuma.Checked = false;
      chkCumartesi.Checked = chkPazar.Checked = true;
    }

    private void btnHicbiri_Click(object sender, EventArgs e)
    {

      chkPazartesi.Checked = chkSali.Checked = chkCarsamba.Checked =
            chkPersembe.Checked = chkCuma.Checked = chkCumartesi.Checked =
            chkPazar.Checked = false;
    }

    private void btnTemizle_Click(object sender, EventArgs e)
    {
      FormTemizle();
    }

    private void FormTemizle()
    {
      _seciliVardiyaId = 0;

      txtVardiyaAdi.Clear();
      txtMolaSuresi.Text = "0";

      dtpBaslangicSaati.Value = DateTime.Today.AddHours(8);
      dtpBitisSaati.Value = DateTime.Today.AddHours(17);

      chkPazartesi.Checked = chkSali.Checked = chkCarsamba.Checked =
      chkPersembe.Checked = chkCuma.Checked = true;
      chkCumartesi.Checked = chkPazar.Checked = false;

      btnVardiyaKaydet.Enabled = true;
      btnVardiyaGuncelle.Enabled = false;
      btnVardiyaSil.Enabled = false;

      txtVardiyaAdi.Focus();
    }

    private bool FormDogrula()
    {

      if (string.IsNullOrWhiteSpace(txtVardiyaAdi.Text))
      {
        MessageBox.Show("Vardiya adı boş bırakılamaz.",
            "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtVardiyaAdi.Focus();
        return false;
      }

      if (!int.TryParse(txtMolaSuresi.Text, out int mola) || mola < 0)
      {
        MessageBox.Show("Geçerli bir mola süresi girin (dakika).",
            "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtMolaSuresi.Focus();
        return false;
      }

      if (!EnAzBirGunSecili())
      {
        MessageBox.Show("En az bir çalışma günü seçilmelidir.",
            "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
      }

      return true;
    }

    private bool EnAzBirGunSecili()
    {
      return chkPazartesi.Checked || chkSali.Checked ||
            chkCarsamba.Checked || chkPersembe.Checked ||
            chkCuma.Checked || chkCumartesi.Checked ||
            chkPazar.Checked;
    }

    private Vardiya FormdenVardiyaOlustur()
    {
      return new Vardiya
      {
        VardiyaAdi = txtVardiyaAdi.Text.Trim(),
        BaslangicSaati = dtpBaslangicSaati.Value.TimeOfDay,
        BitisSaati = dtpBitisSaati.Value.TimeOfDay,
        MolaDakika = Convert.ToInt32(txtMolaSuresi.Text),
        Pazartesi = chkPazartesi.Checked,
        Sali = chkSali.Checked,
        Carsamba = chkCarsamba.Checked,
        Persembe = chkPersembe.Checked,
        Cuma = chkCuma.Checked,
        Cumartesi = chkCumartesi.Checked,
        Pazar = chkPazar.Checked
      };
    }

    private void btnKapat_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
