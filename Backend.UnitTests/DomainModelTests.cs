using Backend.Domain;
using Backend.DTO;
using NUnit.Framework;
using System;

namespace Backend.UnitTests;

[TestFixture]
public class DomainModelTests
{
    [Test]
    public void DomainModels_SettersAndGetters_WorkCorrectly()
    {
        // Проверяем TestApplication
        var app = new TestApplication { Id = 1, Name = "App", PackageName = "com.app", Version = "1.0", Description = "Desc", CreatedAt = DateTime.UtcNow };
        Assert.That(app.Name, Is.EqualTo("App"));

        // Проверяем TestScenario
        var scenario = new TestScenario { Id = 1, Name = "Scen", RobotFile = "file", Description = "desc", TestApplicationId = 1, TestApplication = app };
        Assert.That(scenario.RobotFile, Is.EqualTo("file"));

        // Проверяем TestConfiguration
        var config = new TestConfiguration { Id = 1, TestScenarioId = 1, DeviceName = "dev", PlatformVersion = "11", AdditionalCapabilities = "cap", TestScenario = scenario };
        Assert.That(config.DeviceName, Is.EqualTo("dev"));

        // Проверяем TestRun
        var run = new TestRun { Id = 1, TestScenarioId = 1, Status = TestRunStatus.Passed, ExecutionLog = "log", TestScenario = scenario };
        Assert.That(run.Status, Is.EqualTo(TestRunStatus.Passed));

        // Проверяем EmulatorConfiguration
        var emu = new EmulatorConfiguration { Id = 1, AvdName = "avd", PlatformVersion = "11", IsActive = true };
        Assert.That(emu.IsActive, Is.True);
    }

    [Test]
    public void DTOs_SettersAndGetters_WorkCorrectly()
    {
        var req1 = new CreateTestScenarioRequest { Name = "A", RobotFile = "B", Description = "C", TestApplicationId = 1 };
        var req2 = new CreateTestConfigurationRequest { TestScenarioId = 1, DeviceName = "D", PlatformVersion = "E", AdditionalCapabilities = "F" };

        Assert.That(req1.Name, Is.EqualTo("A"));
        Assert.That(req2.DeviceName, Is.EqualTo("D"));
    }
}