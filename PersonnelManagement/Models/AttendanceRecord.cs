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
  public class AttendanceRecord
  {
    public int AttendanceId { get; set; }
    public int EmployeeId { get; set; }
    public string FullName { get; set; }
    public string DepartmentName { get; set; }
    public string ShiftName { get; set; }
    public DateTime AttendanceDate { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    public int? WorkMinutes { get; set; }
    public int? OvertimeMinutes { get; set; }
    public bool IsLate { get; set; }
    public bool LeftEarly { get; set; }
    public string Description { get; set; }
    public int? RecordedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CheckInTimeText => CheckInTime.HasValue ? DateTime.Today.Add(CheckInTime.Value).ToString("HH:mm") : "-";
    public string CheckOutTimeText => CheckOutTime.HasValue ? DateTime.Today.Add(CheckOutTime.Value).ToString("HH:mm") : "-";
    public string AttendanceDateText => AttendanceDate.ToString("dd.MM.yyyy");

    public string WorkDurationText
    {
      get
      {
        if (!WorkMinutes.HasValue)
          return "-";
        int hour = WorkMinutes.Value / 60;
        int minute = WorkMinutes.Value % 60;
        return UiText.Format("HMin", hour, minute);
      }
    }
  }
}