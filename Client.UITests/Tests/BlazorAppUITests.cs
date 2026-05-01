using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Client.UITests.PageObjects;

namespace Client.UITests.Tests;

[TestFixture]
public class BlazorAppUITests : PageTest
{
    private NavigationMenu _navMenu;
    private string _appUrl = "https://localhost:7258/"; 

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(_appUrl);
        _navMenu = new NavigationMenu(Page);
    }

    [Test]
    public async Task Dashboard_ShouldLoadStatistics()
    {
        await _navMenu.GoToDashboardAsync();

        var statCards = Page.Locator(".stat-card");
        await Expect(statCards.First).ToBeVisibleAsync();

        int count = await statCards.CountAsync();

        Assert.That(count, Is.GreaterThan(0), "Карточки статистики не загрузились");
    }

    [Test]
    public async Task ApiConsole_SelectPreset_ShouldAutoFillForm()
    {
        await _navMenu.GoToApiConsoleAsync();
        var apiPage = new ApiConsolePage(Page);

        await apiPage.SelectPresetAsync("4. Создать сценарий (TestScenario)");

        string endpoint = await apiPage.GetEndpointValueAsync();
        string method = await apiPage.GetMethodValueAsync();

        // НОВЫЙ СИНТАКСИС NUNIT 4
        Assert.That(endpoint, Is.EqualTo("api/TestScenarios"));
        Assert.That(method, Is.EqualTo("POST"));
    }

    [Test]
    public async Task TestRuns_ClickViewLog_ShouldOpenModalWindow()
    {
        await _navMenu.GoToTestRunsAsync();
        var testRunsPage = new TestRunsPage(Page);

        await Expect(Page.Locator("button.btn-primary:has-text('View Log')").First).ToBeVisibleAsync();

        await testRunsPage.OpenFirstLogAsync();

        bool isVisible = await testRunsPage.IsModalVisibleAsync();

        // НОВЫЙ СИНТАКСИС NUNIT 4
        Assert.That(isVisible, Is.True, "Модальное окно с логом не открылось");

        await testRunsPage.CloseModalAsync();
    }
}