using PersonnelManagement.Models;

namespace PersonnelManagement.Security
{
  public static class UserSession
  {
    private static UserAccount _currentUser;
    internal static int? UserId { get; private set; }
    public static UserAccount CurrentUser
    {
      get => _currentUser;
      internal set
      {
        _currentUser = value;
        UserId = value?.UserId;
      }
    }
  }
}
