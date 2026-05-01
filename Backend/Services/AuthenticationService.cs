namespace Backend.Services;

public class AuthenticationService
{
    // Метод с косяками для демонстрации работы анализатора
    public void ValidateUserCheck(string username)
    {
        // 1. Косяк S1121: Присваивание в условии (вместо сравнения ==)
        bool isAdmin = false;
        if (isAdmin = true)
        {
            // 2. Косяк S1135: TODO (сделаем его ошибкой)
            // TODO: Реализовать логику проверки админа
        }

        // 3. Косяк S2190: Рекурсия без выхода (бесконечный цикл, вешающий сервак)
        ValidateUserCheck(username); 
    }

    public bool IsPasswordStrong(string password)
    {
        return password.Length >= 8;
    }
}