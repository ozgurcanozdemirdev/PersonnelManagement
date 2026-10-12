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
  public class Position
  {
    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public string PositionName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;

    public override string ToString() => PositionName;
  }
}