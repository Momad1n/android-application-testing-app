using Backend.Controllers;
using Backend.Data;
using Backend.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.UnitTests;

[TestFixture]
public class TestApplicationsControllerTests
{
    private AppDbContext _context = null!;
    private TestApplicationsController _controller = null!;

    [SetUp]
    public void Setup()
    {
        // Создаем новую чистую базу в памяти для каждого теста
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new TestApplicationsController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Test]
    public async Task GetApplications_ReturnsAllItems()
    {
        _context.TestApplications.Add(new TestApplication { Name = "App1" });
        _context.TestApplications.Add(new TestApplication { Name = "App2" });
        await _context.SaveChangesAsync();

        var result = await _controller.GetApplications();
        Assert.That(result.Value!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetApplication_ReturnsNotFound_WhenInvalidId()
    {
        var result = await _controller.GetApplication(999);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateApplication_AddsToDatabase()
    {
        var app = new TestApplication { Name = "New App" };
        var result = await _controller.CreateApplication(app);

        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);

        var dbApp = await _context.TestApplications.FirstOrDefaultAsync();
        Assert.That(dbApp!.Name, Is.EqualTo("New App"));
    }

    [Test]
    public async Task DeleteApplication_RemovesFromDatabase()
    {
        var app = new TestApplication { Name = "To Delete" };
        _context.TestApplications.Add(app);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteApplication(app.Id!.Value);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        Assert.That(_context.TestApplications.Count(), Is.EqualTo(0));
    }
}