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
  public class ReportTotals
  {
    public string Period { get; set; } // Örn: "20.08.2026", "08-2026" veya "2026"
    public int TotalSalesQuantity { get; set; }
    public int TotalReturnQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
  }
}