using PersonnelManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PersonnelManagement.DataAccess;
using PersonnelManagement.Connection;
using PersonnelManagement.Security;
using PersonnelManagement.Logging;
using PersonnelManagement.Reporting;
using PersonnelManagement.UI.Forms;

namespace PersonnelManagement.DataAccess
{
  public class LeaveTypeRepository
  {
    public List<LeaveType> GetAll()
    {
      OperationAuthorization.Require(ApplicationPermission.ViewLeaves);
      var list = new List<LeaveType>();
      string sql = "SELECT * FROM LeaveType WHERE IsActive = 1 ORDER BY TypeName";
      var cmd = new SqlCommand(sql, SqlDatabaseConnection.GetConnection());
      using (var r = cmd.ExecuteReader())
        while (r.Read())
          list.Add(new LeaveType { LeaveTypeId = Convert.ToInt32(r["LeaveTypeId"]), TypeName = r["TypeName"].ToString(), AffectsAnnualAllowance = Convert.ToBoolean(r["AffectsAnnualAllowance"]), IsPaid = Convert.ToBoolean(r["IsPaid"]), IsActive = Convert.ToBoolean(r["IsActive"]) });
      return list;
    }
  }
}