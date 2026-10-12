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
  public class LeaveType
  {
    public int LeaveTypeId { get; set; }
    public string TypeName { get; set; }
    public bool AffectsAnnualAllowance { get; set; }
    public bool IsPaid { get; set; }
    public bool IsActive { get; set; } = true;

    public override string ToString() => TypeName;
  }
}