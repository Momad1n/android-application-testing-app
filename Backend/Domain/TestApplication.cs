using Backend.Domain;
using System.Text.Json.Serialization;

public class TestApplication
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string PackageName { get; set; } // путь к APK или идентификатор приложения

    public string Version { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public List<TestScenario> TestScenarios { get; set; } = new ();
}