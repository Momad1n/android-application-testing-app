using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;

namespace Client.UITests.PageObjects;

// POM для бокового меню навигации
public class NavigationMenu
{
    private readonly IPage _page;

    // Локаторы
    private ILocator DashboardLink => _page.Locator("a.nav-link:has-text('Dashboard')");
    private ILocator TestRunsLink => _page.Locator("a.nav-link:has-text('Test Runs')");
    private ILocator ApiConsoleLink => _page.Locator("a.nav-link:has-text('API Console')");

    public NavigationMenu(IPage page)
    {
        _page = page;
    }

    // Методы взаимодействия
    public async Task GoToDashboardAsync() => await DashboardLink.ClickAsync();
    public async Task GoToTestRunsAsync() => await TestRunsLink.ClickAsync();
    public async Task GoToApiConsoleAsync() => await ApiConsoleLink.ClickAsync();
}