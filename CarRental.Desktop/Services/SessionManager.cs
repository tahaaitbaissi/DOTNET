namespace CarRental.Desktop.Services;

public  class SessionManager
{
    public static bool IsLoggedIn { get; private set; }
    public static string? CurrentUsername { get; private set; }
    public static string? CurrentUserRole { get; private set; }
    public static DateTime LoginTime { get; private set; }

    public static void Login(string username, string role)
    {
        CurrentUsername = username;
        CurrentUserRole = role;
        IsLoggedIn = true;
        LoginTime = DateTime.Now;
    }

    public static void Logout()
    {
        CurrentUsername = null;
        CurrentUserRole = null;
        IsLoggedIn = false;
    }

    public static bool HasPermission(string permission)
    {
        if (!IsLoggedIn) return false;
        if (CurrentUserRole == "Admin") return true;
        return permission != "Delete"; // Les agents ne peuvent pas supprimer
    }
}