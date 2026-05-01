using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.SystemTests
{
    [TestClass]
    public class FunctionalChecklistTests
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7293") // поменяй порт
        };


        private async Task<int> CreateScenario()
        {
            var scenario = new
            {
                name = "Checklist Scenario",
                robotFile = "DemoTest.robot",
                description = "Test",
                testApplicationId = 3
            };

            var response = await _client.PostAsync("/api/TestScenarios",
                new StringContent(JsonSerializer.Serialize(scenario), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("id").GetInt32();
        }

        private async Task<int> CreateConfig(int scenarioId)
        {
            var config = new
            {
                testScenarioId = scenarioId,
                deviceName = "emulator-5554",
                platformVersion = "13",
                additionalCapabilities = ""
            };

            var response = await _client.PostAsync("/api/TestConfigurations",
                new StringContent(JsonSerializer.Serialize(config), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("id").GetInt32();
        }

        [TestMethod]
        public async Task CL01_Api_Is_Available()
        {
            var response = await _client.GetAsync("/api/testruns");
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task CL02_CreateScenario_Succeeds()
        {
            var id = await CreateScenario();
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public async Task CL03_InvalidScenario_ReturnsBadRequest()
        {
            var config = new
            {
                testScenarioId = 9999,
                deviceName = "device",
                platformVersion = "13"
            };

            var response = await _client.PostAsync("/api/testconfigurations",
                new StringContent(JsonSerializer.Serialize(config), Encoding.UTF8, "application/json"));

            Assert.IsFalse(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task CL04_CreateConfiguration_Succeeds()
        {
            var scenarioId = await CreateScenario();
            var configId = await CreateConfig(scenarioId);

            Assert.IsTrue(configId > 0);
        }

        [TestMethod]
        public async Task CL05_RunTest_CreatesTestRun()
        {
            var scenarioId = await CreateScenario();
            await CreateConfig(scenarioId);

            var response = await _client.PostAsync($"/api/tests/run/{scenarioId}", null);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task CL06_TestRun_Status_Is_Pending()
        {
            var scenarioId = await CreateScenario();
            await CreateConfig(scenarioId);

            var response = await _client.PostAsync($"/api/tests/run/{scenarioId}", null);
            var json = await response.Content.ReadAsStringAsync();

            var status = JsonDocument.Parse(json).RootElement.GetProperty("status").GetInt32();
            Assert.AreEqual(0, status);
        }

        [TestMethod]
        public async Task CL07_Worker_Processes_TestRun()
        {
            var scenarioId = await CreateScenario();
            await CreateConfig(scenarioId);

            var runResponse = await _client.PostAsync($"/api/tests/run/{scenarioId}", null);
            var runJson = await runResponse.Content.ReadAsStringAsync();
            var runId = JsonDocument.Parse(runJson).RootElement.GetProperty("id").GetInt32();

            bool finished = false;

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(3000);

                var response = await _client.GetAsync("/api/testruns");
                var json = await response.Content.ReadAsStringAsync();

                foreach (var run in JsonDocument.Parse(json).RootElement.EnumerateArray())
                {
                    if (run.GetProperty("id").GetInt32() == runId)
                    {
                        var status = run.GetProperty("status").GetInt32();

                        if (status == 2 || status == 3)
                        {
                            finished = true;
                            break;
                        }
                    }
                }

                if (finished) break;
            }

            Assert.IsTrue(finished, "Worker не завершил тест");
        }

        [TestMethod]
        public async Task CL08_TestRun_Log_Not_Empty()
        {
            var response = await _client.GetAsync("/api/testruns");
            var json = await response.Content.ReadAsStringAsync();

            var runs = JsonDocument.Parse(json).RootElement;

            foreach (var run in runs.EnumerateArray())
            {
                if (run.TryGetProperty("executionLog", out var log))
                {
                    Assert.IsNotNull(log.ToString());
                    return;
                }
            }

            Assert.Fail("Лог не найден");
        }

        [TestMethod]
        public async Task CL09_GetRuns_ReturnsList()
        {
            var response = await _client.GetAsync("/api/testruns");
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task CL10_Run_InvalidScenario_ReturnsNotFound()
        {
            var response = await _client.PostAsync("/api/tests/run/9999", null);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}