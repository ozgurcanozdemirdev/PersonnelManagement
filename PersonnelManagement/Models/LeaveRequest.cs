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
  public class LeaveRequest
  {
    public int LeaveId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeFullName { get; set; }
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Description { get; set; }
    public string Status { get; set; } = "Pending";
    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StartDateText => StartDate.ToString("dd.MM.yyyy");
    public string EndDateText => EndDate.ToString("dd.MM.yyyy");
  }
}