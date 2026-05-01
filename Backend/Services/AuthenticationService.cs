namespace Backend.Services;

public class AuthenticationService
{
    public static bool ValidateUserCheck(string username)
    {
        // Исправлено S2325: Метод помечен как static, так как не использует данные экземпляра
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        // Исправлено S1121: Используется сравнение (==) вместо присваивания (=)
        bool isAdmin = username.Equals("admin", StringComparison.OrdinalIgnoreCase);

        if (isAdmin)
        {
            Console.WriteLine($"User {username} has admin privileges.");
        }

        // Исправлено S2190: Рекурсивный вызов удален, предотвращено переполнение стека
        return true;
    }

    public bool IsPasswordStrong(string password)
    {
        return password.Length >= 8;
    }
}