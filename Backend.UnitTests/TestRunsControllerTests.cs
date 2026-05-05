using Backend.Controllers;
using Backend.Data;
using Backend.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.UnitTests;

[TestFixture]
public class TestRunsControllerTests
{
    private AppDbContext _context = null!;
    private TestRunsController _controller = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new TestRunsController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateTestRun_AddsRunToDatabase()
    {
        var result = await _controller.CreateTestRun(1, CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var run = okResult!.Value as TestRun;
        Assert.That(run, Is.Not.Null);
        Assert.That(run!.TestScenarioId, Is.EqualTo(1));
        Assert.That(run.Status, Is.EqualTo(TestRunStatus.Pending));

        Assert.That(_context.TestRuns.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetTestRuns_ReturnsList()
    {
        _context.TestRuns.Add(new TestRun { TestScenarioId = 1, Status = TestRunStatus.Pending });
        await _context.SaveChangesAsync();

        var result = await _controller.GetTestRuns(CancellationToken.None);
        var okResult = result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
    }
}