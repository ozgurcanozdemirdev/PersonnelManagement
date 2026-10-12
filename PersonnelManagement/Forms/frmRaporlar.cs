using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static PersonnelManagement.DAL.IzinTuruDAL;

namespace PersonnelManagement.Forms
{
  public partial class frmRaporlar : Form
  {
    public frmRaporlar()
    {
      InitializeComponent();
    }
    private readonly RaporDal _raporDal = new RaporDal();
    private readonly PersonelDal _personelDal = new PersonelDal();

    private void frmRaporlar_Load(object sender, EventArgs e)
    {
      try
      {

      dtpRaporBaslangic.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      dtpRaporBitis.Value = DateTime.Today;

      var personeller = new List<Personel>();
      personeller.Add(new Personel { PersonelId = 0, Ad = "Tümü", Soyad = "" });
      personeller.AddRange(_personelDal.TumunuGetir(false));

      cmbRaporPersonel.DataSource = personeller;
      cmbRaporPersonel.DisplayMember = "AdSoyad";
      cmbRaporPersonel.ValueMember = "PersonelId";
      cmbRaporPersonel.SelectedIndex = 0;

      cmbRaporTuru.Items.Clear();
      cmbRaporTuru.Items.Add("Personel Özet Raporu");
      cmbRaporTuru.Items.Add("Günlük Giriş-Çıkış");
      cmbRaporTuru.Items.Add("İzin Raporu");
      cmbRaporTuru.Items.Add("Devamsızlık Raporu");
      cmbRaporTuru.SelectedIndex = 0;

      lblRaporTarih.Text = $"Aylık Rapor: {dtpRaporBaslangic.Value:MMMM yyyy}";
    
      }
      catch (Exception ex)
      {
        MessageBox.Show("Ekran yüklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Close();
      }
    }

    private void btnRaporOlustur_Click(object sender, EventArgs e)
    {
      if (dtpRaporBitis.Value.Date < dtpRaporBaslangic.Value.Date)
      {
        MessageBox.Show("Bitiş tarihi başlangıç tarihinden önce olamaz.",
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      try
      {
        switch (cmbRaporTuru.SelectedItem?.ToString())
        {
          case "Personel Özet Raporu": PersonelOzetRaporuOlustur(); break;
          case "Günlük Giriş-Çıkış": GunlukGirisRaporuOlustur(); break;
          case "İzin Raporu": IzinRaporuOlustur(); break;
          case "Devamsızlık Raporu": DevamsizlikRaporuOlustur(); break;
          default:
            MessageBox.Show("Geçerli bir rapor türü seçin.",
                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            break;
        }

        lblRaporTarih.Text = $"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}";
      }
      catch (Exception ex)
      {
        MessageBox.Show("Rapor oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void PersonelOzetRaporuOlustur()
    {
      try
      {
        int? personelId = SecilenPersonelId();
        var liste = _raporDal.PersonelOzetRaporu(
            dtpRaporBaslangic.Value.Date,
            dtpRaporBitis.Value.Date,
            personelId);

        dgvRapor.DataSource = null;
        dgvRapor.DataSource = liste;
        DgvPersonelOzetAyarla();
        OzetIstatistiklerHesapla(liste);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Özet raporu oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void DgvPersonelOzetAyarla()
    {
      if (dgvRapor.Columns["PersonelId"] != null)
        dgvRapor.Columns["PersonelId"].Visible = false;

      if (dgvRapor.Columns["AdSoyad"] != null) { dgvRapor.Columns["AdSoyad"].HeaderText = "Ad Soyad"; dgvRapor.Columns["AdSoyad"].Width = 150; }
      if (dgvRapor.Columns["DepartmanAdi"] != null) { dgvRapor.Columns["DepartmanAdi"].HeaderText = "Departman"; dgvRapor.Columns["DepartmanAdi"].Width = 120; }
      if (dgvRapor.Columns["ToplamCalismaGun"] != null) { dgvRapor.Columns["ToplamCalismaGun"].HeaderText = "Çalışma Günü"; dgvRapor.Columns["ToplamCalismaGun"].Width = 90; }
      if (dgvRapor.Columns["ToplamCalismaDakika"] != null) { dgvRapor.Columns["ToplamCalismaDakika"].HeaderText = "Çalışma Süresi"; dgvRapor.Columns["ToplamCalismaDakika"].Width = 100; }
      if (dgvRapor.Columns["ToplamMesaiDakika"] != null) { dgvRapor.Columns["ToplamMesaiDakika"].HeaderText = "Mesai"; dgvRapor.Columns["ToplamMesaiDakika"].Width = 80; }
      if (dgvRapor.Columns["GecGelmeSayisi"] != null) { dgvRapor.Columns["GecGelmeSayisi"].HeaderText = "Geç Gelmeler"; dgvRapor.Columns["GecGelmeSayisi"].Width = 80; }
      if (dgvRapor.Columns["ErkenCikisSayisi"] != null) { dgvRapor.Columns["ErkenCikisSayisi"].HeaderText = "Erken Çıkışlar"; dgvRapor.Columns["ErkenCikisSayisi"].Width = 90; }
      if (dgvRapor.Columns["ToplamDevamsizlik"] != null) { dgvRapor.Columns["ToplamDevamsizlik"].HeaderText = "Devamsızlık"; dgvRapor.Columns["ToplamDevamsizlik"].Width = 80; }
      if (dgvRapor.Columns["ToplamIzinGun"] != null) { dgvRapor.Columns["ToplamIzinGun"].HeaderText = "İzin Günü"; dgvRapor.Columns["ToplamIzinGun"].Width = 70; }
      if (dgvRapor.Columns["ToplamCalismaStr"] != null) dgvRapor.Columns["ToplamCalismaStr"].Visible = false;
      if (dgvRapor.Columns["ToplamMesaiStr"] != null) dgvRapor.Columns["ToplamMesaiStr"].Visible = false;
    }

    private void OzetIstatistiklerHesapla(List<PersonelRapor> liste)
    {
      if (liste.Count == 0) { lblToplamMesai.Text = "—"; lblToplamDevamsizlik.Text = "—"; return; }

      long toplamMesai = 0;
      long toplamDevamsizlik = 0;

      foreach (var r in liste)
      {
        toplamMesai += r.ToplamMesaiDakika;
        toplamDevamsizlik += r.ToplamDevamsizlik;
      }

      long saat = toplamMesai / 60;
      long dak = toplamMesai % 60;
      lblToplamMesai.Text = $"Toplam Mesai: {saat}s {dak}dk";
      lblToplamDevamsizlik.Text = $"Toplam Devamsızlık: {toplamDevamsizlik} gün";
    }

    private void GunlukGirisRaporuOlustur()
    {
      try
      {
        int? personelId = SecilenPersonelId();
        var girisKayitDal = new GirisKayitDAL();
        var liste = girisKayitDal.TarihAraligindaGetir(
            dtpRaporBaslangic.Value.Date,
            dtpRaporBitis.Value.Date,
            personelId);

        dgvRapor.DataSource = null;
        dgvRapor.DataSource = liste;
        DgvGunlukGirisAyarla();
        lblToplamMesai.Text = $"Toplam Kayıt: {liste.Count}";
        lblToplamDevamsizlik.Text = "—";
      }
      catch (Exception ex)
      {
        MessageBox.Show("Giriş raporu oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvGunlukGirisAyarla()
    {
      string[] gizle = { "GirisKayitId", "PersonelId", "GirisZamani", "CikisZamani", "MesaiDakika", "Aciklama" };
      foreach (string s in gizle)
        if (dgvRapor.Columns[s] != null)
          dgvRapor.Columns[s].Visible = false;

      if (dgvRapor.Columns["KayitTarihi"] != null) { dgvRapor.Columns["KayitTarihi"].HeaderText = "Tarih"; dgvRapor.Columns["KayitTarihi"].Width = 90; }
      if (dgvRapor.Columns["AdSoyad"] != null) { dgvRapor.Columns["AdSoyad"].HeaderText = "Ad Soyad"; dgvRapor.Columns["AdSoyad"].Width = 150; }
      if (dgvRapor.Columns["DepartmanAdi"] != null) { dgvRapor.Columns["DepartmanAdi"].HeaderText = "Departman"; dgvRapor.Columns["DepartmanAdi"].Width = 110; }
      if (dgvRapor.Columns["VardiyaAdi"] != null) { dgvRapor.Columns["VardiyaAdi"].HeaderText = "Vardiya"; dgvRapor.Columns["VardiyaAdi"].Width = 100; }
      if (dgvRapor.Columns["GirisSaatiStr"] != null) { dgvRapor.Columns["GirisSaatiStr"].HeaderText = "Giriş"; dgvRapor.Columns["GirisSaatiStr"].Width = 70; }
      if (dgvRapor.Columns["CikisSaatiStr"] != null) { dgvRapor.Columns["CikisSaatiStr"].HeaderText = "Çıkış"; dgvRapor.Columns["CikisSaatiStr"].Width = 70; }
      if (dgvRapor.Columns["CalismaSuresiStr"] != null) { dgvRapor.Columns["CalismaSuresiStr"].HeaderText = "Çalışma Süresi"; dgvRapor.Columns["CalismaSuresiStr"].Width = 100; }
      if (dgvRapor.Columns["GecGeldiMi"] != null) { dgvRapor.Columns["GecGeldiMi"].HeaderText = "Geç?"; dgvRapor.Columns["GecGeldiMi"].Width = 45; }
      if (dgvRapor.Columns["ErkenCiktiMi"] != null) { dgvRapor.Columns["ErkenCiktiMi"].HeaderText = "Erken Çıkış?"; dgvRapor.Columns["ErkenCiktiMi"].Width = 80; }
      if (dgvRapor.Columns["KayitTarihiStr"] != null) dgvRapor.Columns["KayitTarihiStr"].Visible = false;
      if (dgvRapor.Columns["GirisSaati"] != null) dgvRapor.Columns["GirisSaati"].Visible = false;
      if (dgvRapor.Columns["CikisSaati"] != null) dgvRapor.Columns["CikisSaati"].Visible = false;
      if (dgvRapor.Columns["CalismaDakika"] != null) dgvRapor.Columns["CalismaDakika"].Visible = false;
    }

    private void IzinRaporuOlustur()
    {
      try
      {
        int? personelId = SecilenPersonelId();
        var izinDal = new IzinDal();

        List<Izin> liste = izinDal.TarihAraligindaGetir(dtpRaporBaslangic.Value.Date, dtpRaporBitis.Value.Date, personelId);

        dgvRapor.DataSource = null;
        dgvRapor.DataSource = liste;
        DgvIzinAyarla();
        lblToplamMesai.Text = $"Toplam Talep: {liste.Count}";
        lblToplamDevamsizlik.Text = "—";
      }
      catch (Exception ex)
      {
        MessageBox.Show("İzin raporu oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvIzinAyarla()
    {
      string[] gizle = { "IzinId", "PersonelId", "IzinTuruId", "Aciklama", "OnayanKullaniciId", "OnayTarihi", "RedNedeni", "OlusturmaTarihi", "BaslangicTarihi", "BitisTarihi" };
      foreach (string s in gizle)
        if (dgvRapor.Columns[s] != null)
          dgvRapor.Columns[s].Visible = false;

      if (dgvRapor.Columns["PersonelAdSoyad"] != null) { dgvRapor.Columns["PersonelAdSoyad"].HeaderText = "Ad Soyad"; dgvRapor.Columns["PersonelAdSoyad"].Width = 150; }
      if (dgvRapor.Columns["IzinTuruAdi"] != null) { dgvRapor.Columns["IzinTuruAdi"].HeaderText = "İzin Türü"; dgvRapor.Columns["IzinTuruAdi"].Width = 110; }
      if (dgvRapor.Columns["BaslangicStr"] != null) { dgvRapor.Columns["BaslangicStr"].HeaderText = "Başlangıç"; dgvRapor.Columns["BaslangicStr"].Width = 85; }
      if (dgvRapor.Columns["BitisStr"] != null) { dgvRapor.Columns["BitisStr"].HeaderText = "Bitiş"; dgvRapor.Columns["BitisStr"].Width = 85; }
      if (dgvRapor.Columns["ToplamGun"] != null) { dgvRapor.Columns["ToplamGun"].HeaderText = "Gün"; dgvRapor.Columns["ToplamGun"].Width = 45; }
      if (dgvRapor.Columns["Durum"] != null) { dgvRapor.Columns["Durum"].HeaderText = "Durum"; dgvRapor.Columns["Durum"].Width = 90; }
    }

    private void DevamsizlikRaporuOlustur()
    {
      try
      {
        int? personelId = SecilenPersonelId();

        var table = _raporDal.DevamsizlikRaporu(dtpRaporBaslangic.Value.Date, dtpRaporBitis.Value.Date, personelId);

        dgvRapor.DataSource = table;
        DgvDevamsizlikAyarla();
        lblToplamMesai.Text = $"Toplam Kayıt: {table.Rows.Count}";
        lblToplamDevamsizlik.Text = "—";
      }
      catch (Exception ex)
      {
        MessageBox.Show("Devamsızlık raporu oluşturulamadı:\n" + ex.Message,
            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void DgvDevamsizlikAyarla()
    {
      string[] gizle = { "DevamsizlikId", "PersonelId" };
      foreach (string s in gizle)
        if (dgvRapor.Columns[s] != null)
          dgvRapor.Columns[s].Visible = false;

      if (dgvRapor.Columns["PersonelAdSoyad"] != null) { dgvRapor.Columns["PersonelAdSoyad"].HeaderText = "Ad Soyad"; dgvRapor.Columns["PersonelAdSoyad"].Width = 150; }
      if (dgvRapor.Columns["DepartmanAdi"] != null) { dgvRapor.Columns["DepartmanAdi"].HeaderText = "Departman"; dgvRapor.Columns["DepartmanAdi"].Width = 120; }
      if (dgvRapor.Columns["DevamsizlikTarihi"] != null) { dgvRapor.Columns["DevamsizlikTarihi"].HeaderText = "Tarih"; dgvRapor.Columns["DevamsizlikTarihi"].Width = 90; }
      if (dgvRapor.Columns["Mazeret"] != null) { dgvRapor.Columns["Mazeret"].HeaderText = "Mazeret"; dgvRapor.Columns["Mazeret"].Width = 150; }
      if (dgvRapor.Columns["Aciklama"] != null) { dgvRapor.Columns["Aciklama"].HeaderText = "Açıklama"; dgvRapor.Columns["Aciklama"].Width = 200; }
    }
    private int? SecilenPersonelId()
    {
      if (cmbRaporPersonel.SelectedValue == null) return null;
      int id = Convert.ToInt32(cmbRaporPersonel.SelectedValue);
      return id == 0 ? (int?)null : id;
    }

    private void btnExcelAktar_Click(object sender, EventArgs e)
    {
      if (dgvRapor.DataSource == null || dgvRapor.Rows.Count == 0)
      {
        MessageBox.Show("Aktarılacak veri bulunamadı.",
            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      using (var sfd = new SaveFileDialog())
      {
        sfd.DefaultExt = "xlsx";
        sfd.AddExtension = true;
        sfd.Filter = "Excel Dosyası (*.xlsx)|*.xlsx|CSV Dosyası (*.csv)|*.csv";
        sfd.FileName = $"Rapor_{DateTime.Now:yyyyMMdd_HHmmss}";

        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
          RaporDisariAktarma.Kaydet(dgvRapor, sfd.FileName);

          MessageBox.Show($"Rapor kaydedildi:\n{sfd.FileName}",
              "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show("Aktarım sırasında hata:\n" + ex.Message,
              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void btnYazdir_Click(object sender, EventArgs e)
    {
      if (dgvRapor.DataSource == null || dgvRapor.Rows.Count == 0)
      {
        MessageBox.Show("Yazdırılacak veri bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      try
      {
        using (var document = new ReportPrintDocument(dgvRapor, Convert.ToString(cmbRaporTuru.SelectedItem)))
        using (var preview = new PrintPreviewDialog { Document = document, Width = 1000, Height = 700 })
          preview.ShowDialog(this);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Yazdırma önizlemesi açılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnKapat_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}


