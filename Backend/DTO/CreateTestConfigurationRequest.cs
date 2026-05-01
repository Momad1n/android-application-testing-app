namespace Backend.DTO;

public class CreateTestConfigurationRequest
{
    public int TestScenarioId { get; set; }
    public string DeviceName { get; set; } = null!;
    public string PlatformVersion { get; set; } = null!;
    public string AdditionalCapabilities { get; set; } = string.Empty;
}