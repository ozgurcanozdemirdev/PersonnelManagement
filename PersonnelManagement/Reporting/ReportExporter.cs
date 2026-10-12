using PersonnelManagement.UI.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.Reporting
{
  public static class ReportExporter
  {
    private const string SheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    public static void Save(DataGridView table, string file)
    {
      var columns = table.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
      var rows = new List<object[]>
      {
        columns.Select(c => (object)c.HeaderText).ToArray()
      };
      foreach (DataGridViewRow row in table.Rows)
      {
        if (row.IsNewRow || !row.Visible)
          continue;
        rows.Add(columns.Select(c =>
        {
          var cell = row.Cells[c.Index];
          object value = cell.Value;
          if (value is bool)
            return (object)((bool)value ? UiText.Get("Yes") : UiText.Get("No"));
          if (IsNumeric(value))
            return value;
          return (object)Convert.ToString(cell.FormattedValue, CultureInfo.CurrentCulture);
        }).ToArray());
      }

      Save(rows, file);
    }

    public static void Save(List<object[]> rows, string file)
    {
      string extension = Path.GetExtension(file).ToLowerInvariant();
      if (extension != ".xlsx" && extension != ".csv")
        throw new ArgumentException(UiText.Get("TheFileExtensionMustBeXlsxOrCsv"));
      if (rows.Count > 1048576 || rows.Any(r => r.Length > 16384))
        throw new ArgumentException(UiText.Get("DataExceedsExcelSRowOrColumnLimit"));
      // Yazma başarısız olursa mevcut hedef dosyayı koru; tamamlanan dosyayı tek adımda değiştir.
      string temporary = Path.GetFullPath(file) + "." + Guid.NewGuid().ToString("N") + ".tmp";
      try
      {
        if (extension == ".csv")
          WriteCsv(rows, temporary);
        else
          WriteExcel(rows, temporary);
        if (File.Exists(file))
          File.Replace(temporary, file, null);
        else
          File.Move(temporary, file);
      }
      finally
      {
        if (File.Exists(temporary))
          File.Delete(temporary);
      }
    }

    private static bool IsNumeric(object value) => value is byte || value is short || value is int || value is long || value is float || value is double || value is decimal;
    private static void WriteCsv(List<object[]> rows, string path)
    {
      using (var writer = new StreamWriter(path, false, new UTF8Encoding(true)))
        foreach (var row in rows)
          writer.WriteLine(string.Join(";", row.Select(value =>
          {
            string text = Convert.ToString(value, CultureInfo.CurrentCulture) ?? "";
            // Kullanıcı metnini Excel'in formül olarak çalıştırmasını önle; sayısal değerleri koru.
            string control = text.TrimStart();
            if (!IsNumeric(value) && control.Length > 0 && "=+-@".Contains(control[0]))
              text = "'" + text;
            return "\"" + text.Replace("\"", "\"\"") + "\"";
          })));
    }

    private static void WriteXml(ZipArchive zip, string name, Action<XmlWriter> write)
    {
      using (var stream = zip.CreateEntry(name).Open())
      using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false), CloseOutput = false }))
        write(writer);
    }

    private static void WriteExcel(List<object[]> rows, string path)
    {
      using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
      {
        // Workbook ve worksheet ilişkileri gerçek bir Open XML çalışma kitabı oluşturur.
        WriteXml(zip, "[Content_Types].xml", w =>
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
        WriteRelationship(zip, "_rels/.rels", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "xl/workbook.xml");
        WriteRelationship(zip, "xl/_rels/workbook.xml.rels", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "worksheets/sheet1.xml");
        WriteXml(zip, "xl/workbook.xml", w =>
        {
          w.WriteStartElement("workbook", SheetNs);
          w.WriteStartElement("sheets", SheetNs);
          w.WriteStartElement("sheet", SheetNs);
          w.WriteAttributeString("name", UiText.Get("Report"));
          w.WriteAttributeString("sheetId", "1");
          w.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId1");
          w.WriteEndElement();
          w.WriteEndElement();
          w.WriteEndElement();
        });
        WriteXml(zip, "xl/worksheets/sheet1.xml", w =>
        {
          w.WriteStartElement("worksheet", SheetNs);
          w.WriteStartElement("sheetData", SheetNs);
          for (int row = 0; row < rows.Count; row++)
          {
            w.WriteStartElement("row", SheetNs);
            w.WriteAttributeString("r", (row + 1).ToString(CultureInfo.InvariantCulture));
            for (int col = 0; col < rows[row].Length; col++)
            {
              object value = rows[row][col];
              w.WriteStartElement("c", SheetNs);
              w.WriteAttributeString("r", GetColumnName(col + 1) + (row + 1).ToString(CultureInfo.InvariantCulture));
              if (IsNumeric(value))
                w.WriteElementString("v", SheetNs, Convert.ToString(value, CultureInfo.InvariantCulture));
              else
              {
                string text = Convert.ToString(value, CultureInfo.CurrentCulture) ?? "";
                if (text.Length > 32767)
                  throw new ArgumentException(UiText.Get("CellTextExceedsExcelS32767CharacterLimit"));
                w.WriteAttributeString("t", "inlineStr");
                w.WriteStartElement("is", SheetNs);
                w.WriteStartElement("t", SheetNs);
                w.WriteAttributeString("xml", "space", "http://www.w3.org/XML/1998/namespace", "preserve");
                w.WriteString(text);
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

    private static void WriteRelationship(ZipArchive zip, string path, string type, string target)
    {
      WriteXml(zip, path, w =>
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

    private static string GetColumnName(int n)
    {
      string firstName = "";
      while (n > 0)
      {
        n--;
        firstName = (char)('A' + n % 26) + firstName;
        n /= 26;
      }

      return firstName;
    }
  }
}