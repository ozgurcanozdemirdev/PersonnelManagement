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
  public class AbsenceRecord
  {
    public int AbsenceId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeFullName { get; set; }
    public DateTime AbsenceDate { get; set; }
    public string Description { get; set; }
    public string Excuse { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DateText => AbsenceDate.ToString("dd.MM.yyyy");
  }
}