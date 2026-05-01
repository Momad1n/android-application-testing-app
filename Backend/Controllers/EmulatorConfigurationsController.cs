using Backend.Data;
using Backend.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmulatorConfigurationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmulatorConfigurationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/EmulatorConfigurations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmulatorConfiguration>>> GetAll()
    {
        return await _context.EmulatorConfigurations.ToListAsync();
    }

    // GET: /api/EmulatorConfigurations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<EmulatorConfiguration>> GetById(int id)
    {
        var emulator = await _context.EmulatorConfigurations.FindAsync(id);
        if (emulator == null) return NotFound();
        return emulator;
    }

    // POST: /api/EmulatorConfigurations
    [HttpPost]
    public async Task<ActionResult<EmulatorConfiguration>> Create(EmulatorConfiguration model)
    {
        model.CreatedAt = DateTime.UtcNow;
        _context.EmulatorConfigurations.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    // PUT: /api/EmulatorConfigurations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EmulatorConfiguration model)
    {
        if (id != model.Id) return BadRequest();

        var existing = await _context.EmulatorConfigurations.FindAsync(id);
        if (existing == null) return NotFound();

        existing.AvdName = model.AvdName;
        existing.PlatformVersion = model.PlatformVersion;
        existing.IsActive = model.IsActive;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/EmulatorConfigurations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var emulator = await _context.EmulatorConfigurations.FindAsync(id);
        if (emulator == null) return NotFound();

        _context.EmulatorConfigurations.Remove(emulator);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}