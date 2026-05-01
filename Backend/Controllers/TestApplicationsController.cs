using Backend.Data;
using Backend.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestApplicationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/TestApplications
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestApplication>>> GetApplications()
    {
        return await _context.TestApplications.ToListAsync();
    }

    // GET /api/TestApplications/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TestApplication>> GetApplication(int id)
    {
        var app = await _context.TestApplications.FindAsync(id);
        if (app == null) return NotFound();
        return app;
    }

    // POST /api/TestApplications
    [HttpPost]
    public async Task<ActionResult<TestApplication>> CreateApplication(TestApplication app)
    {
        _context.TestApplications.Add(app);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = app.Id }, app);
    }

    // DELETE /api/TestApplications/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var app = await _context.TestApplications.FindAsync(id);
        if (app == null) return NotFound();

        _context.TestApplications.Remove(app);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}