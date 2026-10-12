using System.Threading;
using System.Threading.Tasks;
using PersonnelManagement.UI.Localization;
using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.UI.Forms
{
  public partial class ReportsForm : LocalizedForm
  {
    protected override ApplicationPermission? RequiredPermission => ApplicationPermission.ViewReports;

    public ReportsForm()
    {
      InitializeComponent();
    }



    private void ValidateReportAccess()
    {
      OperationAuthorization.Require(ApplicationPermission.ViewReports);
    }

    private CancellationTokenSource _reportCancellation;
    private CancellationTokenSource _lookupCancellation;
    private readonly ReportRepository _reportRepository = new ReportRepository();
    private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();
    private string _printTitle = UiText.Get("EmployeeReport");
    private string _printEmployeeName = "";
    private bool _printAllEmployees;
    private bool _printIncludesDateRange;
    private DateTime _printStartDate;
    private DateTime _printEndDate;
    private DateTime _printGeneratedAt;
    private async void ReportsFormLoad(object sender, EventArgs e)
    {
      dtpReportStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      dtpReportEndDate.Value = DateTime.Today;
      var employees = new List<Employee>();
      employees.Add(new Employee { EmployeeId = 0, FirstName = UiText.Get("All"), LastName = "" });

      cmbReportEmployee.DataSource = employees;
      cmbReportEmployee.DisplayMember = "FullName";
      cmbReportEmployee.ValueMember = "EmployeeId";
      cmbReportEmployee.SelectedIndex = 0;
      cmbReportType.Items.Clear();
      cmbReportType.Items.Add("Personel Özet Raporu");
      cmbReportType.Items.Add("Günlük Giriş-Çıkış");
      cmbReportType.Items.Add("İzin Raporu");
      cmbReportType.Items.Add("Devamsızlık Raporu");
      cmbReportType.SelectedIndex = 0;
      UiText.Bind(lblReportDate, "MonthlyReport", dtpReportStartDate.Value);
      var cancellation = new CancellationTokenSource();
      _lookupCancellation = cancellation;
      SetReportControlsEnabled(false);
      try
      {
        var lookup = await _employeeRepository.GetLookupAsync(activeOnly: false, cancellationToken: cancellation.Token);
        if (!IsDisposed)
        {
          employees.AddRange(lookup);
          cmbReportEmployee.DataSource = null;
          cmbReportEmployee.DataSource = employees;
          cmbReportEmployee.SelectedIndex = 0;
        }
      }
      catch (OperationCanceledException) { }
      catch (Exception ex)
      {
        if (!IsDisposed && !cancellation.IsCancellationRequested)
          MessageBox.Show(UiText.Get("EmployeesCouldNotBeLoaded") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        _lookupCancellation = null;
        cancellation.Dispose();
        if (!IsDisposed && _reportCancellation == null)
          SetReportControlsEnabled(true);
      }
    }

    private async void BtnGenerateReportClick(object sender, EventArgs e)
    {
      if (dtpReportEndDate.Value.Date < dtpReportStartDate.Value.Date)
      {
        MessageBox.Show(UiText.Get("EndDateCannotBeEarlierThanStartDate"), UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (_reportCancellation != null || _lookupCancellation != null)
        return;
      var cancellation = new CancellationTokenSource();
      _reportCancellation = cancellation;
      SetReportControlsEnabled(false);
      try
      {
        switch (cmbReportType.SelectedIndex)
        {
          case 0:
            await CreateEmployeeSummary(cancellation.Token);
            break;
          case 1:
            await CreateAttendanceReport(cancellation.Token);
            break;
          case 2:
            await CreateLeaveReport(cancellation.Token);
            break;
          case 3:
            await CreateAbsenceReport(cancellation.Token);
            break;
          default:
            MessageBox.Show(UiText.Get("SelectAValidReportType"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            break;
        }

        if (!IsDisposed)
          UiText.Bind(lblReportDate, "ReportDate", DateTime.Now);
      }
      catch (OperationCanceledException) { }
      catch (Exception ex)
      {
        if (IsDisposed || cancellation.IsCancellationRequested)
          return;
        MessageBox.Show(UiText.Get("ReportCouldNotBeGenerated") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        _reportCancellation = null;
        cancellation.Dispose();
        if (!IsDisposed)
          SetReportControlsEnabled(true);
      }
    }

    private async Task CreateEmployeeSummary(CancellationToken cancellationToken)
    {
      int? employeeId = GetSelectedEmployeeId();
      var list = await _reportRepository.GetEmployeeSummaryAsync(dtpReportStartDate.Value.Date, dtpReportEndDate.Value.Date, employeeId, cancellationToken: cancellationToken);
      if (IsDisposed || cancellationToken.IsCancellationRequested)
        return;
      dgvReport.DataSource = null;
      dgvReport.DataSource = list;
      ConfigureEmployeeSummaryGrid();
      CalculateSummaryStatistics(list);
      SavePrintDetails(UiText.Get("EmployeeSummary"));
    }

    private void ConfigureEmployeeSummaryGrid()
    {
      if (dgvReport.Columns["EmployeeId"] != null)
        dgvReport.Columns["EmployeeId"].Visible = false;
      if (dgvReport.Columns["FullName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["FullName"], "FullName");
        dgvReport.Columns["FullName"].Width = 150;
      }

      if (dgvReport.Columns["DepartmentName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["DepartmentName"], "Department");
        dgvReport.Columns["DepartmentName"].Width = 120;
      }

      if (dgvReport.Columns["TotalWorkDays"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["TotalWorkDays"], "Workdays");
        dgvReport.Columns["TotalWorkDays"].Width = 90;
      }

      if (dgvReport.Columns["TotalWorkMinutes"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["TotalWorkMinutes"], "WorkDuration");
        dgvReport.Columns["TotalWorkMinutes"].Width = 100;
      }

      if (dgvReport.Columns["TotalOvertimeMinutes"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["TotalOvertimeMinutes"], "Overtime");
        dgvReport.Columns["TotalOvertimeMinutes"].Width = 80;
      }

      if (dgvReport.Columns["LateArrivalCount"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["LateArrivalCount"], "LateArrivals");
        dgvReport.Columns["LateArrivalCount"].Width = 80;
      }

      if (dgvReport.Columns["EarlyDepartureCount"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["EarlyDepartureCount"], "EarlyDepartures");
        dgvReport.Columns["EarlyDepartureCount"].Width = 90;
      }

      if (dgvReport.Columns["TotalAbsences"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["TotalAbsences"], "Absence");
        dgvReport.Columns["TotalAbsences"].Width = 80;
      }

      if (dgvReport.Columns["TotalLeaveDays"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["TotalLeaveDays"], "LeaveDays");
        dgvReport.Columns["TotalLeaveDays"].Width = 70;
      }

      if (dgvReport.Columns["TotalWorkDurationText"] != null)
        dgvReport.Columns["TotalWorkDurationText"].Visible = false;
      if (dgvReport.Columns["TotalOvertimeDurationText"] != null)
        dgvReport.Columns["TotalOvertimeDurationText"].Visible = false;
    }

    private void CalculateSummaryStatistics(List<EmployeeSummaryReport> list)
    {
      if (list.Count == 0)
      {
        lblTotalOvertime.Text = "—";
        lblTotalAbsences.Text = "—";
        return;
      }

      int totalOvertime = 0;
      int totalAbsences = 0;
      foreach (var r in list)
      {
        totalOvertime += r.TotalOvertimeMinutes;
        totalAbsences += r.TotalAbsences;
      }

      int hour = totalOvertime / 60;
      int minute = totalOvertime % 60;
      UiText.Bind(lblTotalOvertime, "TotalOvertimeHMin", hour, minute);
      UiText.Bind(lblTotalAbsences, "TotalAbsenceDays2", totalAbsences);
    }

    private async Task CreateAttendanceReport(CancellationToken cancellationToken)
    {
      int? employeeId = GetSelectedEmployeeId();
      var attendanceRepository = new AttendanceRepository();
      var list = await attendanceRepository.GetByDateRangeAsync(dtpReportStartDate.Value.Date, dtpReportEndDate.Value.Date, employeeId, cancellationToken: cancellationToken);
      if (IsDisposed || cancellationToken.IsCancellationRequested)
        return;
      dgvReport.DataSource = null;
      dgvReport.DataSource = list;
      ConfigureAttendanceGrid();
      UiText.Bind(lblTotalOvertime, "TotalRecords2", list.Count);
      lblTotalAbsences.Text = "—";
      SavePrintDetails(UiText.Get("DailyAttendance"));
    }

    private void ConfigureAttendanceGrid()
    {
      string[] hiddenColumns =
      {
        "AttendanceId",
        "EmployeeId",
        "CheckInAt",
        "CheckOutAt",
        "OvertimeMinutes",
        "Description"
      };
      foreach (string s in hiddenColumns)
        if (dgvReport.Columns[s] != null)
          dgvReport.Columns[s].Visible = false;
      if (dgvReport.Columns["AttendanceDate"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["AttendanceDate"], "Date2");
        dgvReport.Columns["AttendanceDate"].Width = 90;
      }

      if (dgvReport.Columns["FullName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["FullName"], "FullName");
        dgvReport.Columns["FullName"].Width = 150;
      }

      if (dgvReport.Columns["DepartmentName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["DepartmentName"], "Department");
        dgvReport.Columns["DepartmentName"].Width = 110;
      }

      if (dgvReport.Columns["ShiftName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["ShiftName"], "Shift");
        dgvReport.Columns["ShiftName"].Width = 100;
      }

      if (dgvReport.Columns["CheckInTimeText"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["CheckInTimeText"], "CheckIn");
        dgvReport.Columns["CheckInTimeText"].Width = 70;
      }

      if (dgvReport.Columns["CheckOutTimeText"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["CheckOutTimeText"], "Exit");
        dgvReport.Columns["CheckOutTimeText"].Width = 70;
      }

      if (dgvReport.Columns["WorkDurationText"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["WorkDurationText"], "WorkDuration");
        dgvReport.Columns["WorkDurationText"].Width = 100;
      }

      if (dgvReport.Columns["IsLate"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["IsLate"], "Late");
        dgvReport.Columns["IsLate"].Width = 45;
      }

      if (dgvReport.Columns["LeftEarly"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["LeftEarly"], "LeftEarly");
        dgvReport.Columns["LeftEarly"].Width = 80;
      }

      if (dgvReport.Columns["AttendanceDateText"] != null)
        dgvReport.Columns["AttendanceDateText"].Visible = false;
      if (dgvReport.Columns["CheckInTime"] != null)
        dgvReport.Columns["CheckInTime"].Visible = false;
      if (dgvReport.Columns["CheckOutTime"] != null)
        dgvReport.Columns["CheckOutTime"].Visible = false;
      if (dgvReport.Columns["WorkMinutes"] != null)
        dgvReport.Columns["WorkMinutes"].Visible = false;
    }

    private async Task CreateLeaveReport(CancellationToken cancellationToken)
    {
      int? employeeId = GetSelectedEmployeeId();
      var leaveRepository = new LeaveRepository();
      var list = await leaveRepository.GetByDateRangeAsync(dtpReportStartDate.Value.Date, dtpReportEndDate.Value.Date, employeeId, cancellationToken: cancellationToken);
      if (IsDisposed || cancellationToken.IsCancellationRequested)
        return;
      dgvReport.DataSource = null;
      dgvReport.DataSource = list;
      ConfigureLeaveGrid();
      UiText.Bind(lblTotalOvertime, "TotalRequests", list.Count);
      lblTotalAbsences.Text = "—";
      SavePrintDetails(UiText.Get("LeaveReport"));
    }

    private void ConfigureLeaveGrid()
    {
      string[] hiddenColumns =
      {
        "LeaveId",
        "EmployeeId",
        "LeaveTypeId",
        "Description",
        "ReviewedByUserId",
        "ReviewedAt",
        "RejectionReason",
        "CreatedAt"
      };
      foreach (string s in hiddenColumns)
        if (dgvReport.Columns[s] != null)
          dgvReport.Columns[s].Visible = false;
      if (dgvReport.Columns["EmployeeFullName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["EmployeeFullName"], "FullName");
        dgvReport.Columns["EmployeeFullName"].Width = 150;
      }

      if (dgvReport.Columns["LeaveTypeName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["LeaveTypeName"], "LeaveType");
        dgvReport.Columns["LeaveTypeName"].Width = 110;
      }

      if (dgvReport.Columns["StartDateText"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["StartDateText"], "Start");
        dgvReport.Columns["StartDateText"].Width = 85;
      }

      if (dgvReport.Columns["EndDateText"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["EndDateText"], "End");
        dgvReport.Columns["EndDateText"].Width = 85;
      }

      if (dgvReport.Columns["TotalDays"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["TotalDays"], "Days");
        dgvReport.Columns["TotalDays"].Width = 45;
      }

      if (dgvReport.Columns["Status"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["Status"], "Status");
        dgvReport.Columns["Status"].Width = 90;
      }
    }

    private async Task CreateAbsenceReport(CancellationToken cancellationToken)
    {
      int? employeeId = GetSelectedEmployeeId();
      var table = await _reportRepository.GetAbsenceReportAsync(dtpReportStartDate.Value.Date, dtpReportEndDate.Value.Date, employeeId, cancellationToken: cancellationToken);
      if (IsDisposed || cancellationToken.IsCancellationRequested)
        return;
      dgvReport.DataSource = table;
      ConfigureAbsenceGrid();
      UiText.Bind(lblTotalOvertime, "TotalRecords2", table.Rows.Count);
      lblTotalAbsences.Text = "—";
      SavePrintDetails(UiText.Get("AbsenceReport"));
    }

    private void ConfigureAbsenceGrid()
    {
      string[] hiddenColumns =
      {
        "AbsenceId",
        "EmployeeId"
      };
      foreach (string s in hiddenColumns)
        if (dgvReport.Columns[s] != null)
          dgvReport.Columns[s].Visible = false;
      if (dgvReport.Columns["EmployeeFullName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["EmployeeFullName"], "FullName");
        dgvReport.Columns["EmployeeFullName"].Width = 150;
      }

      if (dgvReport.Columns["DepartmentName"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["DepartmentName"], "Department");
        dgvReport.Columns["DepartmentName"].Width = 120;
      }

      if (dgvReport.Columns["AbsenceDate"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["AbsenceDate"], "Date2");
        dgvReport.Columns["AbsenceDate"].Width = 90;
      }

      if (dgvReport.Columns["Excuse"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["Excuse"], "Excuse");
        dgvReport.Columns["Excuse"].Width = 150;
      }

      if (dgvReport.Columns["Description"] != null)
      {
        UiText.BindHeader(dgvReport.Columns["Description"], "Description");
        dgvReport.Columns["Description"].Width = 200;
      }
    }

    private int? GetSelectedEmployeeId()
    {
      if (cmbReportEmployee.SelectedValue == null)
        return null;
      int id = Convert.ToInt32(cmbReportEmployee.SelectedValue);
      return id == 0 ? (int? )null : id;
    }

    private void BtnExportExcelClick(object sender, EventArgs e)
    {
      if (dgvReport.DataSource == null || dgvReport.Rows.Count == 0)
      {
        MessageBox.Show(UiText.Get("ThereIsNoDataToExport"), UiText.Get("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      using (var sfd = new SaveFileDialog())
      {
        sfd.Filter = UiText.Get("ExcelFileXlsxXlsxCSVFileCsvCsv");
        sfd.FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
        if (sfd.ShowDialog() != DialogResult.OK)
          return;
        try
        {
          ValidateReportAccess();
          ReportExporter.Save(dgvReport, sfd.FileName);
          MessageBox.Show(UiText.Format("ReportSaved", sfd.FileName), UiText.Get("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show(UiText.Get("ExportFailed") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void SavePrintDetails(string title, bool dateRange = true)
    {
      // Filtreler sonradan değişse de çıktı, oluşturulan raporun bilgilerini taşısın.
      _printTitle = title;
      _printEmployeeName = cmbReportEmployee.Text;
      _printAllEmployees = GetSelectedEmployeeId() == null;
      _printIncludesDateRange = dateRange;
      _printStartDate = dtpReportStartDate.Value;
      _printEndDate = dtpReportEndDate.Value;
      _printGeneratedAt = DateTime.Now;
    }

    private string GetPrintDetails()
    {
      string details = UiText.Get("Employee3") + (_printAllEmployees ? UiText.Translate("Tümü") : _printEmployeeName);
      if (_printIncludesDateRange) details += UiText.Format("Date", _printStartDate, _printEndDate);
      return details + UiText.Format("Created", _printGeneratedAt);
    }

    private void BtnPrintClick(object sender, EventArgs e)
    {
      if (dgvReport.DataSource == null || dgvReport.Rows.Count == 0)
      {
        MessageBox.Show(UiText.Get("GenerateAReportContainingRecordsFirst"), UiText.Get("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      try
      {
        ValidateReportAccess();
        if (PrinterSettings.InstalledPrinters.Count == 0)
        {
          MessageBox.Show(UiText.Get("NoPrinterIsAvailableOnThisComputer"), UiText.Get("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
          return;
        }

        string summary = lblTotalOvertime.Text;
        if (lblTotalAbsences.Text != "—")
          summary += "   " + lblTotalAbsences.Text;
        using (var document = new ReportPrintDocument(dgvReport, UiText.Translate(_printTitle), GetPrintDetails(), summary))
        using (var printerSelection = new PrintDialog())
        {
          printerSelection.Document = document;
          printerSelection.UseEXDialog = true;
          printerSelection.AllowSomePages = false;
          printerSelection.AllowSelection = false;
          if (printerSelection.ShowDialog(this) != DialogResult.OK)
            return;
          if (!document.PrinterSettings.IsValid)
            throw new InvalidOperationException(UiText.Get("TheSelectedPrinterIsUnavailable"));
          // Yazıcı A4 destekliyorsa seç; diğer durumda yazıcının kendi kağıdını kullan.
          foreach (PaperSize kagit in document.PrinterSettings.PaperSizes)
            if (kagit.Kind == PaperKind.A4)
            {
              document.DefaultPageSettings.PaperSize = kagit;
              break;
            }

          document.DefaultPageSettings.Landscape = true;
          using (var preview = new PrintPreviewDialog())
          {
            preview.Document = document;
            UiText.BindComputed(preview, () => UiText.Get("PrintPreview"));
            preview.WindowState = FormWindowState.Maximized;
            preview.UseAntiAlias = true;
            preview.ShowDialog(this);
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(UiText.Get("ReportCouldNotBePrinted") + ex.Message, UiText.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private sealed class ReportPrintDocument : PrintDocument
    {
      private readonly List<string> _headers = new List<string>();
      private readonly List<int> _widths = new List<int>();
      private readonly List<string[]> _rows = new List<string[]>();
      private readonly string _title;
      private readonly string _details;
      private readonly string _summary;
      private int _rowIndex;
      private int _pageNumber;
      private int[] _cellPositions;
      public ReportPrintDocument(DataGridView table, string title, string details, string summary)
      {
        _title = title;
        _details = details;
        _summary = summary;
        DocumentName = "PersonnelManagement - " + title;
        OriginAtMargins = true;
        DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
        var columns = new List<DataGridViewColumn>();
        foreach (DataGridViewColumn sutun in table.Columns)
          if (sutun.Visible)
            columns.Add(sutun);
        columns.Sort((sol, sag) => sol.DisplayIndex.CompareTo(sag.DisplayIndex));
        foreach (var sutun in columns)
        {
          _headers.Add(sutun.HeaderText);
          _widths.Add(sutun.Width);
        }

        // Önizleme ve yazdırma aynı veriyi kullansın; gizli sütunları çıktıya alma.
        foreach (DataGridViewRow row in table.Rows)
        {
          if (row.IsNewRow || !row.Visible)
            continue;
          var values = new string[columns.Count];
          for (int i = 0; i < columns.Count; i++)
          {
            object value = row.Cells[columns[i].Index].FormattedValue;
            values[i] = value is bool ? ((bool)value ? UiText.Get("Yes") : UiText.Get("No")) : Convert.ToString(value);
          }

          _rows.Add(values);
        }

        if (_headers.Count == 0 || _rows.Count == 0)
          throw new InvalidOperationException(UiText.Get("ThereIsNoVisibleDataToPrint"));
      }

      protected override void OnBeginPrint(PrintEventArgs e)
      {
        // Önizleme de bir yazdırma işlemi başlatır. Gerçek çıktı için başa dön.
        _rowIndex = 0;
        _pageNumber = 0;
        _cellPositions = new int[_headers.Count];
        base.OnBeginPrint(e);
      }

      protected override void OnPrintPage(PrintPageEventArgs e)
      {
        using (var titleFont = new Font("Arial", 14, FontStyle.Bold))
        using (var columnFont = new Font("Arial", 9, FontStyle.Bold))
        using (var textFont = new Font("Arial", 9))
        using (var line = new Pen(Color.Gray, 0.5f))
        using (var format = new StringFormat
        {
          FormatFlags = StringFormatFlags.LineLimit
        }

        )
        {
          Graphics g = e.Graphics;
          float pageWidth = e.MarginBounds.Width;
          float lowerBound = e.MarginBounds.Height - 28;
          float totalWidth = 0;
          foreach (int genislik in _widths)
            totalWidth += genislik;
          var widths = new float[_widths.Count];
          for (int i = 0; i < widths.Length; i++)
          {
            widths[i] = pageWidth * _widths[i] / totalWidth;
            if (widths[i] <= 12)
              throw new InvalidOperationException(UiText.Get("ThePageIsTooNarrowForTheReportColumns"));
          }

          float y = 0;
          string[] aciklamalar =
          {
            _title,
            _details,
            _summary
          };
          for (int i = 0; i < aciklamalar.Length; i++)
          {
            if (string.IsNullOrWhiteSpace(aciklamalar[i]))
              continue;
            Font font = i == 0 ? titleFont : textFont;
            float height = (float)Math.Ceiling(g.MeasureString(aciklamalar[i], font, new SizeF(pageWidth, 100000), format).Height) + 4;
            g.DrawString(aciklamalar[i], font, Brushes.Black, new RectangleF(0, y, pageWidth, height), format);
            y += height + 4;
          }

          float titleHeight = GetRowHeight(g, _headers.ToArray(), new int[_headers.Count], columnFont, widths, format);
          float x = 0;
          for (int i = 0; i < _headers.Count; i++)
          {
            g.FillRectangle(Brushes.WhiteSmoke, x, y, widths[i], titleHeight);
            g.DrawRectangle(line, x, y, widths[i], titleHeight);
            g.DrawString(_headers[i], columnFont, Brushes.Black, new RectangleF(x + 4, y + 4, widths[i] - 8, titleHeight - 8), format);
            x += widths[i];
          }

          y += titleHeight;
          float tableStart = y;
          float minimumRows = textFont.GetHeight(g) + 8;
          if (lowerBound - y < minimumRows)
            throw new InvalidOperationException(UiText.Get("ThePageIsTooShortForTheReport"));
          while (_rowIndex < _rows.Count)
          {
            string[] row = _rows[_rowIndex];
            float height = GetRowHeight(g, row, _cellPositions, textFont, widths, format);
            float remainingSpace = lowerBound - y;
            if (remainingSpace < minimumRows)
              break;
            // Bir sayfaya sığabilen satırı bölmeden sonraki sayfaya taşı.
            if (height > remainingSpace && y > tableStart && height <= lowerBound - tableStart)
              break;
            height = Math.Min(height, remainingSpace);
            bool rowCompleted = true;
            x = 0;
            for (int i = 0; i < row.Length; i++)
            {
              g.DrawRectangle(line, x, y, widths[i], height);
              string remainingText = row[i].Substring(_cellPositions[i]);
              if (remainingText.Length > 0)
              {
                var field = new RectangleF(x + 4, y + 4, widths[i] - 8, height - 8);
                int characterCount, lineCount;
                g.MeasureString(remainingText, textFont, field.Size, format, out characterCount, out lineCount);
                if (characterCount == 0)
                  throw new InvalidOperationException(UiText.Get("CellTextCouldNotFitOnThePage"));
                g.DrawString(remainingText.Substring(0, characterCount), textFont, Brushes.Black, field, format);
                _cellPositions[i] += characterCount;
              }

              if (_cellPositions[i] < row[i].Length)
                rowCompleted = false;
              x += widths[i];
            }

            y += height;
            if (!rowCompleted)
              break; // Çok uzun açıklamanın kalanını sonraki sayfada yazdır.
            _rowIndex++;
            Array.Clear(_cellPositions, 0, _cellPositions.Length);
          }

          _pageNumber++;
          g.DrawString(UiText.Format("PageRecords", _pageNumber, _rows.Count), textFont, Brushes.Black, new RectangleF(0, lowerBound + 8, pageWidth, 20), format);
          e.HasMorePages = _rowIndex < _rows.Count;
        }

        base.OnPrintPage(e);
      }

      private static float GetRowHeight(Graphics g, string[] metinler, int[] konumlar, Font font, float[] widths, StringFormat format)
      {
        float height = font.GetHeight(g);
        for (int i = 0; i < metinler.Length; i++)
          height = Math.Max(height, g.MeasureString(metinler[i].Substring(konumlar[i]), font, new SizeF(widths[i] - 8, 100000), format).Height);
        return (float)Math.Ceiling(height) + 8;
      }
    }

    private void SetReportControlsEnabled(bool enabled)
    {
      btnGenerateReport.Enabled = enabled;
      btnExportExcel.Enabled = enabled;
      btnPrint.Enabled = enabled;
      cmbReportType.Enabled = enabled;
      cmbReportEmployee.Enabled = enabled;
      dtpReportStartDate.Enabled = enabled;
      dtpReportEndDate.Enabled = enabled;
      UseWaitCursor = !enabled;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      _reportCancellation?.Cancel();
      _lookupCancellation?.Cancel();
      base.OnFormClosed(e);
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
