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
  public class UserAccount
  {
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; }
    public string Role { get; set; }
    // Admin yetkisi yalnızca aktif hesaplarda geçerlidir.
    public bool IsAdmin => IsActive && string.Equals(Role?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase);
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
  }
}