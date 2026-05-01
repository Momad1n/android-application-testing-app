using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.SystemTests
{
    [TestClass]
    public class TestExecutionFullFlowTests
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7293") // поправь порт
        };

        private int _scenarioId;

        [TestInitialize]
        public async Task Setup()
        {
            // 1. Создаём TestScenario
            var scenario = new
            {
                name = "Test Scenario",
                robotFile = "DemoTest.robot",
                description = "Test",
                testApplicationId = 3
            };

            var content = new StringContent(
                JsonSerializer.Serialize(scenario),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/testscenarios", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var created = JsonDocument.Parse(json);

            _scenarioId = created.RootElement.GetProperty("id").GetInt32();
        }

        [TestMethod]
        public async Task TC01_Full_Test_Run_Flow_Succeeds()
        {
            // 2. Создаём конфигурацию
            var config = new
            {
                testScenarioId = _scenarioId,
                deviceName = "emulator-5554",
                platformVersion = "16",
                additionalCapabilities = ""
            };

            var configResponse = await _client.PostAsync(
                "/api/testconfigurations",
                new StringContent(JsonSerializer.Serialize(config), Encoding.UTF8, "application/json"));

            Assert.IsTrue(configResponse.IsSuccessStatusCode, "Ошибка создания конфигурации");

            // 3. Запускаем тест
            var runResponse = await _client.PostAsync($"/api/tests/run/{_scenarioId}", null);

            Assert.IsTrue(runResponse.IsSuccessStatusCode, "Ошибка запуска теста");

            var runJson = await runResponse.Content.ReadAsStringAsync();
            var runDoc = JsonDocument.Parse(runJson);

            var runId = runDoc.RootElement.GetProperty("id").GetInt32();
            var status = runDoc.RootElement.GetProperty("status").GetInt32();

            Assert.AreEqual(0, status, "Статус должен быть Pending");

            // 4. Ждём выполнения Worker
            int attempts = 10;
            bool finished = false;

            while (attempts-- > 0)
            {
                await Task.Delay(3000);

                var getResponse = await _client.GetAsync("/api/testruns");
                var runsJson = await getResponse.Content.ReadAsStringAsync();
                var runs = JsonDocument.Parse(runsJson);

                foreach (var run in runs.RootElement.EnumerateArray())
                {
                    if (run.GetProperty("id").GetInt32() == runId)
                    {
                        var currentStatus = run.GetProperty("status").GetInt32();

                        if (currentStatus == 2 || currentStatus == 3) // Passed или Failed
                        {
                            finished = true;
                            break;
                        }
                    }
                }

                if (finished) break;
            }

            Assert.IsTrue(finished, "Тест не завершился за отведённое время");
        }
    }
}