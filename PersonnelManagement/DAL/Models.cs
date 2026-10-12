using System;
namespace PersonnelManagement.Models
{
  public class Departman
  {
    public int DepartmanId { get; set; }
    public string DepartmanAdi { get; set; }
    public string Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; }

    public override string ToString() => DepartmanAdi;
  }

  public class Pozisyon
  {
    public int PozisyonId { get; set; }
    public int DepartmanId { get; set; }
    public string PozisyonAdi { get; set; }
    public string Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;

    public override string ToString() => PozisyonAdi;
  }

  public class Vardiya
  {
    public int VardiyaId { get; set; }
    public string VardiyaAdi { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public int MolaDakika { get; set; }
    public bool Pazartesi { get; set; }
    public bool Sali { get; set; }
    public bool Carsamba { get; set; }
    public bool Persembe { get; set; }
    public bool Cuma { get; set; }
    public bool Cumartesi { get; set; }
    public bool Pazar { get; set; }
    public bool AktifMi { get; set; } = true;

    public override string ToString() => VardiyaAdi;
  }

  public class Personel
  {
    public int PersonelId { get; set; }
    public string TcKimlikNo { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string AdSoyad => $"{Ad} {Soyad}";
    public string Cinsiyet { get; set; }
    public DateTime? DogumTarihi { get; set; }
    public string Telefon { get; set; }
    public string Email { get; set; }
    public string Adres { get; set; }
    public string FotoPath { get; set; }
    public int DepartmanId { get; set; }
    public string DepartmanAdi { get; set; }
    public int PozisyonId { get; set; }
    public string PozisyonAdi { get; set; }
    public int VardiyaId { get; set; }
    public string VardiyaAdi { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public int MolaDakika { get; set; }
    public DateTime IseGirisTarihi { get; set; }
    public DateTime? IstenCikisTarihi { get; set; }
    public int YillikIzinHakki { get; set; } = 14;
    public bool AktifMi { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; }
    public DateTime? GuncellemeTarihi { get; set; }

    public override string ToString() => AdSoyad;
  }

  public class Kullanici
  {
    public int KullaniciId { get; set; }
    public string KullaniciAdi { get; set; }
    public string Sifre { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string AdSoyad => $"{Ad} {Soyad}";
    public string Email { get; set; }
    public string Rol { get; set; }
    public bool AktifMi { get; set; } = true;
    public DateTime? SonGirisTarihi { get; set; }
  }

  public class GirisKayit
  {
    public int GirisKayitId { get; set; }
    public int PersonelId { get; set; }
    public string AdSoyad { get; set; }
    public string DepartmanAdi { get; set; }
    public string VardiyaAdi { get; set; }
    public DateTime KayitTarihi { get; set; }
    public TimeSpan? GirisSaati { get; set; }
    public TimeSpan? CikisSaati { get; set; }
    public DateTime? GirisZamani { get; set; }
    public DateTime? CikisZamani { get; set; }
    public int? CalismaDakika { get; set; }
    public int? MesaiDakika { get; set; }
    public bool GecGeldiMi { get; set; }
    public bool ErkenCiktiMi { get; set; }
    public string Aciklama { get; set; }
    public int? KaydedenId { get; set; }
    public DateTime OlusturmaTarihi { get; set; }

    public string GirisSaatiStr => GirisSaati.HasValue
        ? DateTime.Today.Add(GirisSaati.Value).ToString("HH:mm") : "-";
    public string CikisSaatiStr => CikisSaati.HasValue
        ? DateTime.Today.Add(CikisSaati.Value).ToString("HH:mm") : "-";
    public string KayitTarihiStr => KayitTarihi.ToString("dd.MM.yyyy");
    public string CalismaSuresiStr
    {
      get
      {
        if (!CalismaDakika.HasValue) return "-";
        int saat = CalismaDakika.Value / 60;
        int dak = CalismaDakika.Value % 60;
        return $"{saat}s {dak}dk";
      }
    }
  }

  public class IzinTuru
  {
    public int IzinTuruId { get; set; }
    public string TuruAdi { get; set; }
    public bool Ucretli { get; set; }
    public bool AktifMi { get; set; } = true;

    public override string ToString() => TuruAdi;
  }

  public class Izin
  {
    public int IzinId { get; set; }
    public int PersonelId { get; set; }
    public string PersonelAdSoyad { get; set; }
    public int IzinTuruId { get; set; }
    public string IzinTuruAdi { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public int ToplamGun { get; set; }
    public string Aciklama { get; set; }
    public string Durum { get; set; } = "Beklemede";
    public int? OnayanKullaniciId { get; set; }
    public DateTime? OnayTarihi { get; set; }
    public string RedNedeni { get; set; }
    public DateTime OlusturmaTarihi { get; set; }

    public string BaslangicStr => BaslangicTarihi.ToString("dd.MM.yyyy");
    public string BitisStr => BitisTarihi.ToString("dd.MM.yyyy");
  }

  public class Devamsizlik
  {
    public int DevamsizlikId { get; set; }
    public int PersonelId { get; set; }
    public string PersonelAdSoyad { get; set; }
    public DateTime DevamsizlikTarihi { get; set; }
    public string Aciklama { get; set; }
    public string Mazeret { get; set; }
    public DateTime OlusturmaTarihi { get; set; }

    public string TarihStr => DevamsizlikTarihi.ToString("dd.MM.yyyy");
  }

  public class PersonelRapor
  {
    public int PersonelId { get; set; }
    public string AdSoyad { get; set; }
    public string DepartmanAdi { get; set; }
    public int ToplamCalismaGun { get; set; }
    public int ToplamCalismaDakika { get; set; }
    public int ToplamMesaiDakika { get; set; }
    public int ToplamDevamsizlik { get; set; }
    public int ToplamIzinGun { get; set; }
    public int GecGelmeSayisi { get; set; }
    public int ErkenCikisSayisi { get; set; }

    public string ToplamCalismaStr
    {
      get
      {
        int saat = ToplamCalismaDakika / 60;
        int dak = ToplamCalismaDakika % 60;
        return $"{saat}s {dak}dk";
      }
    }
    public string ToplamMesaiStr
    {
      get
      {
        int saat = ToplamMesaiDakika / 60;
        int dak = ToplamMesaiDakika % 60;
        return $"{saat}s {dak}dk";
      }
    }
  }

  public class ZRaporuDto
  {
    public string Donem { get; set; } // Örn: "20.08.2026", "08-2026" veya "2026"
    public int ToplamSatisMiktari { get; set; }
    public int ToplamIadeMiktari { get; set; }
    public decimal ToplamGelir { get; set; }
    public decimal ToplamGider { get; set; }
  }
}

