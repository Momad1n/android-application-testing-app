using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend.Domain;

public class TestScenario
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string RobotFile { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TestApplicationId { get; set; }
    [JsonIgnore]
    public TestApplication TestApplication { get; set; } = null!;

    [JsonIgnore]
    public List<TestConfiguration> TestConfigurations { get; set; } = new();
    [JsonIgnore]
    public List<TestRun> TestRuns { get; set; } = new();
}