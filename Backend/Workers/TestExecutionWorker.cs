using Backend.Data;
using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace Backend.Workers;

public class TestExecutionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TestExecutionWorker> _logger;

    private readonly string _androidSdkEmulatorPath = @"C:\Users\Momadin\AppData\Local\Android\Sdk\emulator\emulator.exe";
    private readonly string _pythonPath = @"C:\Users\Momadin\AppData\Local\Programs\Python\Python314\python.exe";
    private readonly string _baseTestDir = @"C:\Users\Momadin\source\repos\AndroidApplicationTestApp\Backend\RobotTests";
    private readonly TimeSpan _robotTimeout = TimeSpan.FromMinutes(5);

    public TestExecutionWorker(IServiceScopeFactory scopeFactory, ILogger<TestExecutionWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TestExecutionWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var testRun = await db.TestRuns
                    .Where(tr => tr.Status == TestRunStatus.Pending)
                    .OrderBy(tr => tr.CreatedDate)
                    .FirstOrDefaultAsync(stoppingToken);

                if (testRun == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Found TestRun {Id}. Setting to Running.", testRun.Id);

                testRun.Status = TestRunStatus.Running;
                testRun.StartTime = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);

                var config = await db.TestConfigurations
                    .Where(c => c.TestScenarioId == testRun.TestScenarioId)
                    .OrderBy(c => c.CreatedAt)
                    .FirstOrDefaultAsync(stoppingToken);

                if (config == null)
                {
                    await MarkRunFailed(db, testRun, $"No TestConfiguration for scenario {testRun.TestScenarioId}");
                    continue;
                }

                var emulator = await db.EmulatorConfigurations
                    .Where(e => e.IsActive && e.PlatformVersion == config.PlatformVersion)
                    .FirstOrDefaultAsync(stoppingToken);

                if (emulator == null)
                {
                    await MarkRunFailed(db, testRun, $"No active emulator found for platform version {config.PlatformVersion}");
                    continue;
                }

                var avds = GetAvailableAvds();
                if (!avds.Contains(emulator.AvdName))
                {
                    await MarkRunFailed(db, testRun, $"AVD '{emulator.AvdName}' not found in system emulator list");
                    continue;
                }

                _logger.LogInformation("Starting emulator {AvdName}", emulator.AvdName);
                var emulatorProcess = StartEmulator(emulator.AvdName);

                // Ждём прогрева эмулятора
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

                string robotTestFile = Path.Combine(_baseTestDir, "DemoTest.robot");
                (var finalStatus, var log) = await RunRobotAsync(robotTestFile, testRun.Id.ToString(), config, stoppingToken);

                testRun.Status = finalStatus;
                testRun.ExecutionLog = log;
                testRun.EndTime = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);

                if (!emulatorProcess.HasExited)
                {
                    emulatorProcess.Kill(true);
                    _logger.LogInformation("Emulator {AvdName} killed.", emulator.AvdName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestExecutionWorker loop");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task MarkRunFailed(AppDbContext db, TestRun run, string message)
    {
        run.Status = TestRunStatus.Failed;
        run.ExecutionLog = message;
        run.EndTime = DateTime.UtcNow;
        await db.SaveChangesAsync();
        _logger.LogWarning(message);
    }

    private string[] GetAvailableAvds()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _androidSdkEmulatorPath,
                Arguments = "-list-avds",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private Process StartEmulator(string avdName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _androidSdkEmulatorPath,
                Arguments = $"-avd {avdName} -no-snapshot-load -no-window",
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        return process;
    }

    private async Task<(TestRunStatus Status, string Log)> RunRobotAsync(string testPath, string outputDir, TestConfiguration config, CancellationToken ct)
    {
        var outputBuilder = new StringBuilder();
        var tcs = new TaskCompletionSource<int>();

        var args = new List<string>
        {
            "-m", "robot",
            "-d", outputDir,
            "--loglevel", "INFO"
        };

        if (config != null)
        {
            args.Add("--variable");
            args.Add($"DEVICE_NAME:{config.DeviceName}");
            args.Add("--variable");
            args.Add($"PLATFORM_VERSION:{config.PlatformVersion}");
            if (!string.IsNullOrEmpty(config.AdditionalCapabilities))
            {
                args.Add("--variable");
                args.Add($"CAPABILITIES:{config.AdditionalCapabilities}");
            }
        }

        args.Add(testPath);

        var startInfo = new ProcessStartInfo
        {
            FileName = _pythonPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args)
            startInfo.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

        process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine("ERROR: " + e.Data); };
        process.Exited += (s, e) => { tcs.TrySetResult(process.ExitCode); };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var timeoutTask = Task.Delay(_robotTimeout, cts.Token);

            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                process.Kill(true);
                return (TestRunStatus.Timeout, "Timeout reached");
            }

            cts.Cancel();

            var log = outputBuilder.ToString();
            var status = log.Contains("| PASS |") ? TestRunStatus.Passed : TestRunStatus.Failed;
            return (status, log);
        }
        catch (Exception ex)
        {
            return (TestRunStatus.Failed, ex.Message);
        }
    }
}