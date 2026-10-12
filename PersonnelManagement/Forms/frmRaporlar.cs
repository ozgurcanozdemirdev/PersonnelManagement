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
      dtpRaporBaslangic.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      dtpRaporBitis.Value = DateTime.Today;

      var personeller = new List<Personel>();
      personeller.Add(new Personel { PersonelId = 0, Ad = "Tümü", Soyad = "" });
      personeller.AddRange(_personelDal.TumunuGetir());

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

      int toplamMesai = 0;
      int toplamDevamsizlik = 0;

      foreach (var r in liste)
      {
        toplamMesai += r.ToplamMesaiDakika;
        toplamDevamsizlik += r.ToplamDevamsizlik;
      }

      int saat = toplamMesai / 60;
      int dak = toplamMesai % 60;
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

        List<Izin> liste = personelId.HasValue
            ? izinDal.PersonelIzinleri(personelId.Value)
            : izinDal.BekleyenIzinler();

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
      string[] gizle = { "IzinId", "PersonelId", "IzinTuruId", "Aciklama", "OnayanKullaniciId", "OnayTarihi", "RedNedeni", "OlusturmaTarihi" };
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

        string sql = @"
                    SELECT
                        d.DevamsizlikId,
                        d.PersonelId,
                        p.Ad + ' ' + p.Soyad  AS PersonelAdSoyad,
                        dep.DepartmanAdi,
                        d.DevamsizlikTarihi,
                        d.Mazeret,
                        d.Aciklama
                    FROM       Devamsizlik d
                    INNER JOIN Personel    p   ON d.PersonelId  = p.PersonelId
                    INNER JOIN Departman   dep ON p.DepartmanId = dep.DepartmanId
                    WHERE d.DevamsizlikTarihi BETWEEN @Bas AND @Bit";

        if (personelId.HasValue) sql += " AND d.PersonelId = @Pid";
        sql += " ORDER BY d.DevamsizlikTarihi DESC";

        var cmd = new System.Data.SqlClient.SqlCommand(sql, SqlDBConnection.GetConnection());
        cmd.Parameters.AddWithValue("@Bas", dtpRaporBaslangic.Value.Date);
        cmd.Parameters.AddWithValue("@Bit", dtpRaporBitis.Value.Date);
        if (personelId.HasValue)
          cmd.Parameters.AddWithValue("@Pid", personelId.Value);

        var table = new System.Data.DataTable();
        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
          adapter.Fill(table);

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
        sfd.Filter = "Excel Dosyası (*.xlsx)|*.xlsx|CSV Dosyası (*.csv)|*.csv";
        sfd.FileName = $"Rapor_{DateTime.Now:yyyyMMdd_HHmmss}";

        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
          using (var writer = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
          {
            var basliklar = new List<string>();
            foreach (DataGridViewColumn col in dgvRapor.Columns)
              if (col.Visible)
                basliklar.Add($"\"{col.HeaderText}\"");
            writer.WriteLine(string.Join(";", basliklar));

            foreach (DataGridViewRow row in dgvRapor.Rows)
            {
              var satir = new List<string>();
              foreach (DataGridViewCell cell in row.Cells)
                if (dgvRapor.Columns[cell.ColumnIndex].Visible)
                  satir.Add($"\"{(cell.Value ?? "")}\"");
              writer.WriteLine(string.Join(";", satir));
            }
          }

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
    {/*
      if (dgvRapor.DataSource == null || dgvRapor.Rows.Count == 0)
      {
        MessageBox.Show("Yazdırılacak veri bulunamadı.",
            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      var pd = new System.Drawing.Printing.PrintDocument();
      pd.PrintPage += (s, ev) =>
      {
        var font = new System.Drawing.Font("Arial", 8);
        var boldFont = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
        int y = 50;
        int x = 30;
        int satirYuksekligi = 20;

        ev.Graphics.DrawString(
            $"Rapor: {cmbRaporTuru.SelectedItem}  —  {DateTime.Now:dd.MM.yyyy}",
            boldFont, System.Drawing.Brushes.Black, x, y);
        y += 30;

        foreach (DataGridViewColumn col in dgvRapor.Columns)
        {
          if (col.Visible)
            ev.Graphics.DrawString(col.HeaderText, font, System.Drawing.Brushes.Black, x, y);
          x += 80;
        }
        y += satirYuksekligi;
        x = 30;

        int satirSayisi = Math.Min(50, dgvRapor.Rows.Count);
        for (int i = 0; i < satirSayisi; i++)
        {
          foreach (DataGridViewCell cell in dgvRapor.Rows[i].Cells)
          {
            if (dgvRapor.Columns[cell.ColumnIndex].Visible)
              ev.Graphics.DrawString((cell.Value ?? "").ToString(), font,
                  System.Drawing.Brushes.Black, x, y);
            x += 80;
          }
          y += satirYuksekligi;
          x = 30;
        }

        ev.HasMorePages = false;
      };

      var pv = new PrintPreviewDialog { Document = pd };
      pv.ShowDialog();
      */
    }

    private void btnKapat_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}


