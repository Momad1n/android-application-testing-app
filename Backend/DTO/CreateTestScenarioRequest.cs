namespace Backend.DTO;

public class CreateTestScenarioRequest
{
    public string Name { get; set; } = null!;
    public string RobotFile { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TestApplicationId { get; set; }
}