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
    /// Like: mo-id="30" and mo-isvisible="true"
    /// </summary>
    public async Task SetAttributesOnVisibleElementsAsync()
    {
        var visibleElements = await Page.QuerySelectorAllAsync(":visible");
        var i = 0;
        
        foreach (var element in visibleElements)
        {
            i++;
            
            try
            {
                var html = await element.InnerHTMLAsync();
                await Page.EvaluateAsync<string>(
                    @"([element, id]) => {
                var existingId = element.getAttribute('id');
                if (existingId === null || existingId === '') {
                    // The element doesn't have an ID attribute, so set it
                    element.setAttribute('id', id);
                }
}",
                    new object[] { element, i });
                await Page.EvaluateAsync<string>(
                    "([element, isVisible]) => element.setAttribute('mo-isVisible', isVisible)",
                    new object[] { element, "true" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    
    /// <summary>
    /// Chooses an action based on the current page and takes it.
    /// </summary>
    /// <returns></returns>
    public async Task<ActionResult> TakeActionAsync()
    {
        var beforeScreenshotPath = "ClientApp/public/screenshot-before.png";
        var afterScreenshotPath = "ClientApp/public/screenshot-after.png";
        
        //await Page.ScreenshotAsync(new() { Path = beforeScreenshotPath });
        var beforeScreenshotBytes = await Page.ScreenshotAsync();
        await SetAttributesOnVisibleElementsAsync();
        var htmlBefore = await Page.ContentAsync();
        var htmlBeforeClean = HtmlCleaner.CleanHtml2(htmlBefore);
        
        var prompter = new GptPrompter();
        var action = await prompter.PromptForActionAsync(htmlBeforeClean);
        
        //var action = await ChatGptPrompter.PromptForActionAsync(htmlBeforeClean);
        await TakeActionOnPageAsync(action);
        //await Page.ScreenshotAsync(new() { Path = afterScreenshotPath });
        var afterScreenshotBytes = await Page.ScreenshotAsync();
        await SetAttributesOnVisibleElementsAsync();
        var htmlAfter = await Page.ContentAsync();
        var htmlAfterClean = HtmlCleaner.CleanHtml2(htmlAfter);
        var askResult = await ChatGptPrompter.PromptForIsExpectedAsync(
            htmlBeforeClean,
            action.Explain,
            htmlAfterClean);
        return new ActionResult()
        {
            Expected = askResult,
            BeforeScreenshotPath = beforeScreenshotPath,
            BeforeScreenshotBytes = Convert.ToBase64String(beforeScreenshotBytes),
            AfterScreenshotPath = afterScreenshotPath,
            AfterScreenshotBytes = Convert.ToBase64String(afterScreenshotBytes),
            Action = action
        };
    }

    private async Task TakeActionOnPageAsync(Action action)
    {
        var element = await Page.QuerySelectorAsync($"[id='{action.ElementId}']");

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