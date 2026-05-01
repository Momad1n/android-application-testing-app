using System.Text.Json.Serialization;

namespace Backend.Domain;

public class TestConfiguration
{
    public int Id { get; set; }

    public int TestScenarioId { get; set; }

    [JsonIgnore]
    public TestScenario TestScenario { get; set; } = null!;
    public string DeviceName { get; set; } = string.Empty;
    public string PlatformVersion { get; set; } = string.Empty;
    public string AdditionalCapabilities { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}