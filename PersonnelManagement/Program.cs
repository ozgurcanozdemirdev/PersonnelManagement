using PersonnelManagement.UI.Localization;
using PersonnelManagement.UI.Forms;
using PersonnelManagement.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonnelManagement
{
  internal static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      LanguageManager.Initialize();
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.ApplicationExit += (s, e) => SqlDatabaseConnection.CloseConnection();
      Application.Run(new LoginForm());
    }
  }
}