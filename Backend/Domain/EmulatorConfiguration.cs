namespace Backend.Domain;

public class EmulatorConfiguration
{
    public int? Id { get; set; }
    public string AvdName { get; set; } = null!;       // Имя AVD
    public string PlatformVersion { get; set; } = null!; // Android версия
    public bool IsActive { get; set; } = true;         // Активен ли эмулятор
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}