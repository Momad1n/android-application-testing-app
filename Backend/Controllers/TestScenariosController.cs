using Backend.Data;
using Backend.Domain;
using Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestScenariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestScenariosController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/testscenarios
    [HttpPost]
    public async Task<ActionResult<TestScenario>> CreateScenario([FromBody] CreateTestScenarioRequest request)
    {
        // Проверяем, что TestApplication существует
        var appExists = await _context.TestApplications.AnyAsync(a => a.Id == request.TestApplicationId);
        if (!appExists)
        {
            return BadRequest($"TestApplication with Id {request.TestApplicationId} does not exist.");
        }

        var scenario = new TestScenario
        {
            Name = request.Name,
            RobotFile = request.RobotFile,
            Description = request.Description,
            TestApplicationId = request.TestApplicationId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TestScenarios.Add(scenario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetScenario), new { id = scenario.Id }, scenario);
    }

    // GET /api/testscenarios/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TestScenario>> GetScenario(int id)
    {
        var scenario = await _context.TestScenarios.FindAsync(id);
        if (scenario == null) return NotFound();
        return scenario;
    }
}