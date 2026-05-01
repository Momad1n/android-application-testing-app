using Backend.Data;
using Backend.Domain;
using Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestConfigurationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestConfigurationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/testconfigurations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestConfiguration>>> GetConfigurations()
    {
        return await _context.TestConfigurations.ToListAsync();
    }

    // GET /api/testconfigurations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TestConfiguration>> GetConfiguration(int id)
    {
        var config = await _context.TestConfigurations.FindAsync(id);
        if (config == null) return NotFound();
        return config;
    }

    // POST /api/testconfigurations
    [HttpPost]
    public async Task<ActionResult<TestConfiguration>> CreateConfiguration([FromBody] CreateTestConfigurationRequest request)
    {
        // Проверяем, что TestScenario существует
        var scenarioExists = await _context.TestScenarios.AnyAsync(ts => ts.Id == request.TestScenarioId);
        if (!scenarioExists)
        {
            return BadRequest($"TestScenario with Id {request.TestScenarioId} does not exist.");
        }

        if (request.TestScenarioId == null)
        {
            return this.BadRequest("TestScenarioId is required.");
        }

        var config = new TestConfiguration
        {
            TestScenarioId = request.TestScenarioId.Value,
            DeviceName = request.DeviceName,
            PlatformVersion = request.PlatformVersion,
            AdditionalCapabilities = request.AdditionalCapabilities,
            CreatedAt = DateTime.UtcNow
        };

        _context.TestConfigurations.Add(config);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetConfiguration), new { id = config.Id }, config);
    }

    // DELETE /api/testconfigurations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConfiguration(int id)
    {
        var config = await _context.TestConfigurations.FindAsync(id);
        if (config == null) return NotFound();

        _context.TestConfigurations.Remove(config);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}