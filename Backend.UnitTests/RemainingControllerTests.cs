using Backend.Controllers;
using Backend.Data;
using Backend.Domain;
using Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.UnitTests;

[TestFixture]
public class RemainingControllersTests
{
    private AppDbContext _context = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task TestScenariosController_CreateAndGet_Works()
    {
        var controller = new TestScenariosController(_context);

        // Arrange
        var app = new TestApplication { Name = "App" };
        _context.TestApplications.Add(app);
        await _context.SaveChangesAsync();

        var request = new CreateTestScenarioRequest { Name = "Scen", RobotFile = "file", Description = "desc", TestApplicationId = app.Id };

        // Act - Create
        var createResult = await controller.CreateScenario(request);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as TestScenario;

        // Act - Get
        var getResult = await controller.GetScenario(created!.Id);

        // Assert
        Assert.That(created.Name, Is.EqualTo("Scen"));
        Assert.That(getResult.Value!.Id, Is.EqualTo(created.Id));
    }

    [Test]
    public async Task TestConfigurationsController_CreateAndGet_Works()
    {
        var controller = new TestConfigurationsController(_context);

        // Arrange
        var scenario = new TestScenario { Name = "Scen" };
        _context.TestScenarios.Add(scenario);
        await _context.SaveChangesAsync();

        var request = new CreateTestConfigurationRequest { TestScenarioId = scenario.Id, DeviceName = "Pixel", PlatformVersion = "11", AdditionalCapabilities = "" };

        // Act - Create
        var createResult = await controller.CreateConfiguration(request);
        var created = (createResult.Result as CreatedAtActionResult)?.Value as TestConfiguration;

        // Act - Get
        var getResult = await controller.GetConfiguration(created!.Id);
        var allResult = await controller.GetConfigurations();

        // Assert
        Assert.That(created.DeviceName, Is.EqualTo("Pixel"));
        Assert.That(getResult.Value!.Id, Is.EqualTo(created.Id));
        Assert.That(allResult.Value!.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task EmulatorConfigurationsController_FullCrud_Works()
    {
        var controller = new EmulatorConfigurationsController(_context);

        // Act - Create
        var emu = new EmulatorConfiguration { AvdName = "AVD", PlatformVersion = "11", IsActive = true };
        var createResult = await controller.Create(emu);
        var createdId = ((createResult.Result as CreatedAtActionResult)?.Value as EmulatorConfiguration)!.Id;

        // Act - Get
        var getResult = await controller.GetById(createdId!.Value);
        Assert.That(getResult.Value!.AvdName, Is.EqualTo("AVD"));

        // Act - Update
        emu.Id = createdId;
        emu.AvdName = "AVD_NEW";
        var updateResult = await controller.Update(createdId.Value, emu);
        Assert.That(updateResult, Is.InstanceOf<NoContentResult>());

        // Act - Delete
        var deleteResult = await controller.Delete(createdId.Value);
        Assert.That(deleteResult, Is.InstanceOf<NoContentResult>());

        var all = await controller.GetAll();
        Assert.That(all.Value!.Count(), Is.EqualTo(0));
    }
}