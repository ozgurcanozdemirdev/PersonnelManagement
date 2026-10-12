using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace PersonnelManagement.DAL
{
  public sealed class ReportPrintDocument : PrintDocument
  {
    private readonly string _title;
    private readonly string[] _headers;
    private readonly float[] _weights;
    private readonly List<string[]> _rows;
    private int _nextRow;
    private int _pageNumber;
    public ReportPrintDocument(DataGridView grid, string title)
    {
      if (grid == null)
        throw new ArgumentNullException(nameof(grid));
      _title = title ?? "Rapor";
      var columns = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
      if (columns.Count == 0)
        throw new ArgumentException("Yazdırılacak görünür sütun bulunamadı.");
      _headers = columns.Select(c => c.HeaderText).ToArray();
      _weights = columns.Select(c => (float)Math.Max(40, c.Width)).ToArray();
      // Önizleme sırasında tablo değişse bile bütün sayfalar aynı veriyi kullanır.
      _rows = grid.Rows.Cast<DataGridViewRow>().Where(r => r.Visible && !r.IsNewRow).Select(r => columns.Select(c => Convert.ToString(r.Cells[c.Index].FormattedValue)).ToArray()).ToList();
      DefaultPageSettings.Landscape = true;
      DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
      DocumentName = _title;
    }

    protected override void OnBeginPrint(PrintEventArgs e)
    {
      _nextRow = 0;
      _pageNumber = 0;
      base.OnBeginPrint(e);
    }

    protected override void OnPrintPage(PrintPageEventArgs e)
    {
      base.OnPrintPage(e);
      if (e.MarginBounds.Width <= 0 || e.MarginBounds.Height < 100)
        throw new InvalidOperationException("Yazdırma alanı rapor için yeterli değil.");
      _pageNumber++;
      using (var font = new Font("Arial", 8))
      using (var heading = new Font("Arial", 12, FontStyle.Bold))
      using (var format = new StringFormat
      {
        Trimming = StringTrimming.EllipsisCharacter
      }

      )
      {
        float y = e.MarginBounds.Top;
        e.Graphics.DrawString(_title + " — " + DateTime.Today.ToString("dd.MM.yyyy"), heading, Brushes.Black, e.MarginBounds.Left, y);
        y += heading.GetHeight(e.Graphics) + 12;
        float total = _weights.Sum();
        float[] widths = _weights.Select(w => w / total * e.MarginBounds.Width).ToArray();
        float headerHeight = MeasureRow(e.Graphics, _headers, widths, font, format);
        DrawRow(e.Graphics, _headers, widths, font, format, e.MarginBounds.Left, y, headerHeight, true);
        y += headerHeight;
        float bottom = e.MarginBounds.Bottom - font.GetHeight(e.Graphics) - 12;
        float bodyTop = y;
        // Satır indeksi sayfalar arasında korunur; ilk 50 kayıttan sonrası kaybolmaz.
        while (_nextRow < _rows.Count)
        {
          float height = Math.Min(MeasureRow(e.Graphics, _rows[_nextRow], widths, font, format), bottom - bodyTop);
          if (y + height > bottom && y > bodyTop)
            break;
          DrawRow(e.Graphics, _rows[_nextRow], widths, font, format, e.MarginBounds.Left, y, height, false);
          y += height;
          _nextRow++;
        }

        e.Graphics.DrawString("Sayfa " + _pageNumber, font, Brushes.Black, e.MarginBounds.Left, bottom + 8);
        e.HasMorePages = _nextRow < _rows.Count;
      }
    }

    private static float MeasureRow(Graphics graphics, string[] values, float[] widths, Font font, StringFormat format)
    {
      float height = font.GetHeight(graphics) + 8;
      for (int i = 0; i < values.Length; i++)
        height = Math.Max(height, graphics.MeasureString(values[i] ?? "", font, new SizeF(Math.Max(1, widths[i] - 8), 100000), format).Height + 8);
      return height;
    }

    private static void DrawRow(Graphics graphics, string[] values, float[] widths, Font font, StringFormat format, float left, float top, float height, bool header)
    {
      for (int i = 0; i < values.Length; i++)
      {
        var cell = new RectangleF(left, top, widths[i], height);
        if (header)
          graphics.FillRectangle(Brushes.LightGray, cell);
        graphics.DrawRectangle(Pens.Gray, cell.X, cell.Y, cell.Width, cell.Height);
        graphics.DrawString(values[i] ?? "", font, Brushes.Black, new RectangleF(left + 4, top + 4, Math.Max(1, widths[i] - 8), Math.Max(1, height - 8)), format);
        left += widths[i];
      }
    }
  }
}
