using System;
using PersonnelManagement.Models;
using PersonnelManagement.UI.Localization;

namespace PersonnelManagement.Security
{
  public static class EmployeeValidation
  {
    public static bool IsNationalIdValid(string value)
    {
      if (value == null || value.Length != 11)
        return false;
      // TryParse işaretli sayıları da kabul eder; kimlik alanında yalnız ASCII rakamları geçerlidir.
      foreach (char character in value)
        if (character < '0' || character > '9')
          return false;
      return true;
    }

    public static string GetError(Employee employee, out string field)
    {
      field = null;
      if (employee == null)
        return UiText.Get("EmployeeDetailsAreIncomplete");
      field = nameof(Employee.FirstName);
      if (string.IsNullOrWhiteSpace(employee.FirstName) || employee.FirstName.Trim().Length > 50)
        return UiText.Format("RequiredTextMaximum", UiText.Get("FirstName"), 50);
      field = nameof(Employee.LastName);
      if (string.IsNullOrWhiteSpace(employee.LastName) || employee.LastName.Trim().Length > 50)
        return UiText.Format("RequiredTextMaximum", UiText.Get("LastName"), 50);
      field = nameof(Employee.NationalId);
      if (!IsNationalIdValid(employee.NationalId))
        return UiText.Get("NationalIDMustContain11Digits");
      field = nameof(Employee.BirthDate);
      if (employee.BirthDate.HasValue && (employee.BirthDate.Value.Date > DateTime.Today || employee.BirthDate.Value.Date > employee.EmploymentStartDate.Date))
        return UiText.Get("BirthDateMustPrecedeEmployment");
      field = nameof(Employee.EmploymentStartDate);
      if (employee.EmploymentEndDate.HasValue && employee.EmploymentEndDate.Value.Date < employee.EmploymentStartDate.Date)
        return UiText.Get("EndDateCannotBeEarlierThanStartDate");
      field = nameof(Employee.DepartmentId);
      if (employee.DepartmentId <= 0)
        return UiText.Get("SelectADepartment");
      field = nameof(Employee.PositionId);
      if (employee.PositionId <= 0)
        return UiText.Get("SelectAPosition");
      field = nameof(Employee.ShiftId);
      if (employee.ShiftId <= 0)
        return UiText.Get("SelectAShift");
      field = nameof(Employee.AnnualLeaveAllowance);
      if (employee.AnnualLeaveAllowance < 0)
        return UiText.Get("EnterAValidAnnualLeaveAllowance");
      foreach (var item in new[] {
        Tuple.Create(nameof(Employee.Phone), employee.Phone, 20),
        Tuple.Create(nameof(Employee.Email), employee.Email, 100),
        Tuple.Create(nameof(Employee.Address), employee.Address, 500),
        Tuple.Create(nameof(Employee.PhotoPath), employee.PhotoPath, 500),
        Tuple.Create(nameof(Employee.Gender), employee.Gender, 10) })
      {
        field = item.Item1;
        if ((item.Item2 ?? "").Length > item.Item3)
          return UiText.Format("TextMaximum", UiText.Get(item.Item1), item.Item3);
      }
      field = null;
      return null;
    }

    public static void Validate(Employee employee)
    {
      string error = GetError(employee, out string field);
      if (error != null)
        throw new ArgumentException(error, field);
    }
  }
}
