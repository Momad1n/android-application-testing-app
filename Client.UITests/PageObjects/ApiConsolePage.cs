using Microsoft.Playwright;

namespace Client.UITests.PageObjects;

public class ApiConsolePage
{
    private readonly IPage _page;

    // Локаторы
    private ILocator PresetSelect => _page.Locator("select.border-primary");
    private ILocator HttpMethodSelect => _page.Locator("select").Nth(1); // Второй селект на странице
    private ILocator EndpointInput => _page.Locator("input[placeholder='api/TestRuns']");
    private ILocator RequestBodyTextarea => _page.Locator("textarea");
    private ILocator SendButton => _page.Locator("button.btn-success");

    public ApiConsolePage(IPage page)
    {
        _page = page;
    }

    public async Task SelectPresetAsync(string presetName)
    {
        await PresetSelect.SelectOptionAsync(new[] { new SelectOptionValue { Label = presetName } });
    }

    public async Task<string> GetEndpointValueAsync() => await EndpointInput.InputValueAsync();
    public async Task<string> GetMethodValueAsync() => await HttpMethodSelect.InputValueAsync();
}