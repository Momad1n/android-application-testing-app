using Microsoft.Playwright;

namespace Client.UITests.PageObjects;

// POM для страницы Test Runs
public class TestRunsPage
{
    private readonly IPage _page;

    private ILocator FirstViewLogButton => _page.Locator("button.btn-primary:has-text('View Log')").First;
    private ILocator LogModal => _page.Locator(".log-modal-content");
    private ILocator CloseModalButton => _page.Locator(".log-modal-content button:has-text('Close')");

    public TestRunsPage(IPage page)
    {
        _page = page;
    }

    public async Task OpenFirstLogAsync()
    {
        await FirstViewLogButton.ClickAsync();
    }

    public async Task<bool> IsModalVisibleAsync()
    {
        return await LogModal.IsVisibleAsync();
    }

    public async Task CloseModalAsync()
    {
        await CloseModalButton.ClickAsync();
    }
}