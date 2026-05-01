using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.LoadTestConsole
{
    class Program
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7293") // поменяй порт
        };

        private const int UserCount = 1000; // количество параллельных "пользователей"

        static async Task Main(string[] args)
        {
            Console.WriteLine($"Запуск нагрузочного теста для {UserCount} пользователей...");

            var tasks = new List<Task>();

            for (int i = 0; i < UserCount; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        int scenarioId = await CreateScenarioAsync(idx);
                        int configId = await CreateConfigAsync(scenarioId, idx);
                        int runId = await RunTestAsync(scenarioId);

                        Console.WriteLine($"User {idx} -> Scenario {scenarioId}, Config {configId}, Run {runId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"User {idx} failed: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Нагрузочный тест завершен!");
        }

        private static async Task<int> CreateScenarioAsync(int idx)
        {
            var scenario = new
            {
                name = $"LoadTest_Scenario_{idx}",
                robotFile = "DemoTest.robot",
                description = "Load test scenario",
                testApplicationId = 3
            };

            var response = await _client.PostAsync("/api/TestScenarios",
                new StringContent(JsonSerializer.Serialize(scenario), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("id").GetInt32();
        }

        private static async Task<int> CreateConfigAsync(int scenarioId, int idx)
        {
            var config = new
            {
                testScenarioId = scenarioId,
                deviceName = $"emulator-{idx % 10}", // 10 эмуляторов для примера
                platformVersion = "16",
                additionalCapabilities = ""
            };

            var response = await _client.PostAsync("/api/TestConfigurations",
                new StringContent(JsonSerializer.Serialize(config), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("id").GetInt32();
        }

        private static async Task<int> RunTestAsync(int scenarioId)
        {
            var response = await _client.PostAsync($"/api/tests/run/{scenarioId}", null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("id").GetInt32();
        }
    }
}