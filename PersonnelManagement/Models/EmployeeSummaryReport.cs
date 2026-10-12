using PersonnelManagement.UI.Localization;
using System;
using PersonnelManagement.Models;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.Models
{
  public class EmployeeSummaryReport
  {
    public int EmployeeId { get; set; }
    public string FullName { get; set; }
    public string DepartmentName { get; set; }
    public int TotalWorkDays { get; set; }
    public int TotalWorkMinutes { get; set; }
    public int TotalOvertimeMinutes { get; set; }
    public int TotalAbsences { get; set; }
    public int TotalLeaveDays { get; set; }
    public int LateArrivalCount { get; set; }
    public int EarlyDepartureCount { get; set; }

    public string TotalWorkDurationText
    {
      get
      {
        int hour = TotalWorkMinutes / 60;
        int minute = TotalWorkMinutes % 60;
        return UiText.Format("HMin", hour, minute);
      }
    }

    public string TotalOvertimeDurationText
    {
      get
      {
        int hour = TotalOvertimeMinutes / 60;
        int minute = TotalOvertimeMinutes % 60;
        return UiText.Format("HMin", hour, minute);
      }
    }
  }
}