namespace Backend.Domain;

using System.Text.Json.Serialization;

public class TestRun
{
    public int Id { get; set; }

    public int TestScenarioId { get; set; }

    [JsonIgnore]
    public TestScenario TestScenario { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public TestRunStatus Status { get; set; }

    public string? ExecutionLog { get; set; }
}
