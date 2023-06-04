using Microsoft.Playwright;

namespace AIWebTester.Controllers;

public class SiteProber
{
    private IPage Page { get; init; }
    private IBrowser _browser;
    private IPlaywright _playwright;

    private SiteProber(IPage page, IBrowser browser, IPlaywright playwright)
    {
        Page = page;
        _browser = browser;
        _playwright = playwright;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">The URL where the prober will start.</param>
    /// <remarks>The prober will load the initial page and be ready to take actions
    /// starting from there.</remarks>
    public static async Task<SiteProber> CreateAsync(string url)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new ()
        {
            Args = new[] { "--allow-insecure-localhost"}
        });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);
        return new SiteProber(page, browser, playwright);
    }
    
    /// <summary>
    /// Chooses an action based on the current page and takes it.
    /// </summary>
    /// <returns></returns>
    public async Task<string> TakeActionAsync()
    {
        var html = await Page.ContentAsync();
        await Page.ScreenshotAsync(new()
        {
            Path = "screenshot-before.png"
        });
        Console.WriteLine(html.Length);
        html = HtmlCleaner.CleanHtml(html);
        Console.WriteLine(html.Length);
        var action = await ChatGptPrompter.PromptForActionAsync(html);
        await TakeActionOnPageAsync(action);
        await Page.ScreenshotAsync(new()
        {
            Path = "screenshot-after.png"
        });
        var htmlAfter = await Page.ContentAsync();
        htmlAfter = HtmlCleaner.CleanHtml(htmlAfter);
        var askResult = await ChatGptPrompter.PromptForIsExpectedAsync(
            html,
            action.Explain,
            htmlAfter);
        return askResult;
    }

    private async Task TakeActionOnPageAsync(Action action)
    {
        var element = await Page.QuerySelectorAsync($"#{action.ElementId}");

        if (element == null)
        {
            throw new ArgumentException($"Element {action.ElementId} doesn't exist on page.");
        }
        
        if (action.Kind == ActionKind.Click)
        {
            await element.ClickAsync();
        }
        else
        {
            await element.TypeAsync(action.TypeText);
        }
    }
}