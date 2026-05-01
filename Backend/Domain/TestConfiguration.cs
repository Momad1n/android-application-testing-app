using System.Text.Json.Serialization;

namespace Backend.Domain;

public class TestConfiguration
{
    public int Id { get; set; }

    public int TestScenarioId { get; set; }

    [JsonIgnore]
    public TestScenario TestScenario { get; set; }

    public string DeviceName { get; set; }

    public string PlatformVersion { get; set; }

    public string AdditionalCapabilities { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}