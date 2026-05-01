using System.Linq;

namespace Backend.Services;

public class AuthenticationService
{
    public bool IsPasswordStrong(string password)
    {
        // Если пароль пустой или null, он точно не надежный
        if (string.IsNullOrEmpty(password))
            return false;

        // Требование: длина >= 8 И содержит хотя бы одну цифру
        return password.Length >= 8 && password.Any(char.IsDigit);
    }
}