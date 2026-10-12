using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace PersonnelManagement.DAL
{
  public static class RaporDisariAktarma
  {
    private const string SheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    public static void Kaydet(DataGridView tablo, string dosya)
    {
      var sutunlar = tablo.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
      var satirlar = new List<object[]>
      {
        sutunlar.Select(c => (object)c.HeaderText).ToArray()
      };
      foreach (DataGridViewRow satir in tablo.Rows)
      {
        if (satir.IsNewRow || !satir.Visible)
          continue;
        satirlar.Add(sutunlar.Select(c =>
        {
          var cell = satir.Cells[c.Index];
          object value = cell.Value;
          if (value is bool)
            return (object)((bool)value ? "Evet" : "Hayır");
          if (SayiMi(value))
            return value;
          return (object)Convert.ToString(cell.FormattedValue, CultureInfo.CurrentCulture);
        }).ToArray());
      }

      Kaydet(satirlar, dosya);
    }

    public static void Kaydet(List<object[]> satirlar, string dosya)
    {
      string uzanti = Path.GetExtension(dosya).ToLowerInvariant();
      if (uzanti != ".xlsx" && uzanti != ".csv")
        throw new ArgumentException("Dosya uzantısı .xlsx veya .csv olmalıdır.");
      if (satirlar.Count > 1048576 || satirlar.Any(r => r.Length > 16384))
        throw new ArgumentException("Veri Excel satır/sütun sınırını aşıyor.");
      // Yazma başarısız olursa mevcut hedef dosyayı koru; tamamlanan dosyayı tek adımda değiştir.
      string gecici = Path.GetFullPath(dosya) + "." + Guid.NewGuid().ToString("N") + ".tmp";
      try
      {
        if (uzanti == ".csv")
          CsvYaz(satirlar, gecici);
        else
          ExcelYaz(satirlar, gecici);
        if (File.Exists(dosya))
          File.Replace(gecici, dosya, null);
        else
          File.Move(gecici, dosya);
      }
      finally
      {
        if (File.Exists(gecici))
          File.Delete(gecici);
      }
    }

    private static bool SayiMi(object value) => value is byte || value is short || value is int || value is long || value is float || value is double || value is decimal;
    private static void CsvYaz(List<object[]> satirlar, string path)
    {
      using (var writer = new StreamWriter(path, false, new UTF8Encoding(true)))
        foreach (var satir in satirlar)
          writer.WriteLine(string.Join(";", satir.Select(value =>
          {
            string metin = Convert.ToString(value, CultureInfo.CurrentCulture) ?? "";
            // Kullanıcı metnini Excel'in formül olarak çalıştırmasını önle; sayısal değerleri koru.
            string kontrol = metin.TrimStart();
            if (!SayiMi(value) && kontrol.Length > 0 && "=+-@".Contains(kontrol[0]))
              metin = "'" + metin;
            return "\"" + metin.Replace("\"", "\"\"") + "\"";
          })));
    }

    private static void XmlYaz(ZipArchive zip, string name, Action<XmlWriter> yaz)
    {
      using (var stream = zip.CreateEntry(name).Open())
      using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false), CloseOutput = false }))
        yaz(writer);
    }

    private static void ExcelYaz(List<object[]> satirlar, string path)
    {
      using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
      {
        // Workbook ve worksheet ilişkileri gerçek bir Open XML çalışma kitabı oluşturur.
        XmlYaz(zip, "[Content_Types].xml", w =>
        {
          w.WriteStartElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");
          w.WriteStartElement("Default");
          w.WriteAttributeString("Extension", "rels");
          w.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.relationships+xml");
          w.WriteEndElement();
          w.WriteStartElement("Default");
          w.WriteAttributeString("Extension", "xml");
          w.WriteAttributeString("ContentType", "application/xml");
          w.WriteEndElement();
          foreach (var part in new[]
          {
            new[]
            {
              "/xl/workbook.xml",
              "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"
            },
            new[]
            {
              "/xl/worksheets/sheet1.xml",
              "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"
            }
          }

          )
          {
            w.WriteStartElement("Override");
            w.WriteAttributeString("PartName", part[0]);
            w.WriteAttributeString("ContentType", part[1]);
            w.WriteEndElement();
          }

          w.WriteEndElement();
        });
        IliskiYaz(zip, "_rels/.rels", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "xl/workbook.xml");
        IliskiYaz(zip, "xl/_rels/workbook.xml.rels", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "worksheets/sheet1.xml");
        XmlYaz(zip, "xl/workbook.xml", w =>
        {
          w.WriteStartElement("workbook", SheetNs);
          w.WriteStartElement("sheets", SheetNs);
          w.WriteStartElement("sheet", SheetNs);
          w.WriteAttributeString("name", "Rapor");
          w.WriteAttributeString("sheetId", "1");
          w.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId1");
          w.WriteEndElement();
          w.WriteEndElement();
          w.WriteEndElement();
        });
        XmlYaz(zip, "xl/worksheets/sheet1.xml", w =>
        {
          w.WriteStartElement("worksheet", SheetNs);
          w.WriteStartElement("sheetData", SheetNs);
          for (int row = 0; row < satirlar.Count; row++)
          {
            w.WriteStartElement("row", SheetNs);
            w.WriteAttributeString("r", (row + 1).ToString(CultureInfo.InvariantCulture));
            for (int col = 0; col < satirlar[row].Length; col++)
            {
              object value = satirlar[row][col];
              w.WriteStartElement("c", SheetNs);
              w.WriteAttributeString("r", SutunAdi(col + 1) + (row + 1).ToString(CultureInfo.InvariantCulture));
              if (SayiMi(value))
                w.WriteElementString("v", SheetNs, Convert.ToString(value, CultureInfo.InvariantCulture));
              else
              {
                string metin = Convert.ToString(value, CultureInfo.CurrentCulture) ?? "";
                if (metin.Length > 32767)
                  throw new ArgumentException("Hücre metni Excel'in 32767 karakter sınırını aşıyor.");
                w.WriteAttributeString("t", "inlineStr");
                w.WriteStartElement("is", SheetNs);
                w.WriteStartElement("t", SheetNs);
                w.WriteAttributeString("xml", "space", "http://www.w3.org/XML/1998/namespace", "preserve");
                w.WriteString(metin);
                w.WriteEndElement();
                w.WriteEndElement();
              }

              w.WriteEndElement();
            }

            w.WriteEndElement();
          }

          w.WriteEndElement();
          w.WriteEndElement();
        });
      }
    }

    private static void IliskiYaz(ZipArchive zip, string path, string type, string target)
    {
      XmlYaz(zip, path, w =>
      {
        const string ns = "http://schemas.openxmlformats.org/package/2006/relationships";
        w.WriteStartElement("Relationships", ns);
        w.WriteStartElement("Relationship", ns);
        w.WriteAttributeString("Id", "rId1");
        w.WriteAttributeString("Type", type);
        w.WriteAttributeString("Target", target);
        w.WriteEndElement();
        w.WriteEndElement();
      });
    }

    private static string SutunAdi(int n)
    {
      string ad = "";
      while (n > 0)
      {
        n--;
        ad = (char)('A' + n % 26) + ad;
        n /= 26;
      }

      return ad;
    }
  }
}
