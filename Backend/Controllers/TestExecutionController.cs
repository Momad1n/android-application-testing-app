using Backend.Data;
using Backend.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tests")]
public class TestExecutionController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestExecutionController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/tests/run/{scenarioId}
    [HttpPost("run/{scenarioId}")]
    public async Task<ActionResult<TestRun>> RunTest(int scenarioId)
    {
        var scenario = await _context.TestScenarios.FindAsync(scenarioId);
        if (scenario == null) return NotFound("Scenario not found");

        var run = new TestRun
        {
            TestScenarioId = scenarioId,
            CreatedDate = DateTime.UtcNow,
            Status = TestRunStatus.Pending
        };

        _context.TestRuns.Add(run);
        await _context.SaveChangesAsync();

        return Ok(run);
    }
}