using TechTalk.SpecFlow;
using FluentAssertions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Backend.BDDTests.StepDefinitions
{
    [Binding]
    public class TestManagementSteps
    {
        // Укажите URL вашего запущенного бэкенда
        private readonly HttpClient _client = new HttpClient { BaseAddress = new System.Uri("https://localhost:7293/") };
        private HttpResponseMessage _response = null!;
        private string _responseString = null!;

        // --- GIVEN ---

        [Given(@"система запущена и готова к работе")]
        public void GivenСистемаЗапущена()
        {
            _client.Should().NotBeNull();
        }

        [Given(@"в базе данных отсутствует тестовый сценарий с ID (.*)")]
        public void GivenНетСценарияСId(int id)
        {
            // Фиктивный шаг для контекста
        }

        // --- WHEN ---

        [When(@"администратор добавляет эмулятор с именем ""(.*)"" и версией платформы ""(.*)""")]
        public async Task WhenДобавляетЭмулятор(string avdName, string platformVersion)
        {
            var payload = new
            {
                avdName = avdName,
                platformVersion = platformVersion,
                isActive = true
            };

            _response = await _client.PostAsJsonAsync("api/EmulatorConfigurations", payload);
            _responseString = await _response.Content.ReadAsStringAsync();
        }

        [When(@"пользователь создает тестовое приложение ""(.*)"" с пакетом ""(.*)""")]
        public async Task WhenСоздаетТестовоеПриложение(string name, string packageName)
        {
            var payload = new
            {
                name = name,
                packageName = packageName,
                version = "1.0.0",
                description = "Тестовое описание"
            };

            _response = await _client.PostAsJsonAsync("api/TestApplications", payload);
            _responseString = await _response.Content.ReadAsStringAsync();
        }

        [When(@"пользователь запускает тест для сценария (.*)")]
        public async Task WhenЗапускаетТест(int scenarioId)
        {
            _response = await _client.PostAsync($"api/tests/run/{scenarioId}", null);
            _responseString = await _response.Content.ReadAsStringAsync();
        }


        // --- THEN ---

        [Then(@"система возвращает статус код (.*) (.*)")]
        public void ThenВозвращаетСтатусКод(int statusCode, string statusName)
        {
            int actualCode = (int)_response.StatusCode;
            actualCode.Should().Be(statusCode);
        }

        [Then(@"эмулятор ""(.*)"" должен отображаться в списке активных эмуляторов")]
        public async Task ThenЭмуляторОтображаетсяВСписке(string expectedName)
        {
            var getResponse = await _client.GetAsync("api/EmulatorConfigurations");
            var content = await getResponse.Content.ReadAsStringAsync();

            content.Should().Contain(expectedName);
        }

        [Then(@"тело ответа должно содержать имя ""(.*)""")]
        public void ThenТелоОтветаСодержитИмя(string expectedName)
        {
            _responseString.Should().Contain(expectedName);
        }

        [Then(@"система выдает сообщение об ошибке ""(.*)""")]
        public void ThenСистемаВыдаетОшибку(string expectedMessage)
        {
            _responseString.Should().Contain(expectedMessage);
        }
    }
}