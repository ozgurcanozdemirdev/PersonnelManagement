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
  public class Employee
  {
    public int EmployeeId { get; set; }
    public string NationalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhotoPath { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int PositionId { get; set; }
    public string PositionName { get; set; }
    public int ShiftId { get; set; }
    public string ShiftName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int BreakMinutes { get; set; }
    public DateTime EmploymentStartDate { get; set; }
    public DateTime? EmploymentEndDate { get; set; }
    public int AnnualLeaveAllowance { get; set; } = 14;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public override string ToString() => FullName;
  }
}