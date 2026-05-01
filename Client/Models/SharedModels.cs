namespace Client.Models;

public enum TestRunStatus
{
    Pending = 0,
    Running = 1,
    Passed = 2,
    Failed = 3,
    Timeout = 4
}

public class TestRun
{
    public int Id { get; set; }
    public int TestScenarioId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TestRunStatus Status { get; set; }
    public string? ExecutionLog { get; set; }
}

public class EmulatorConfiguration
{
    public int Id { get; set; }
    public string AvdName { get; set; } = string.Empty;
    public string PlatformVersion { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}