using Microsoft.Playwright;

namespace AIWebTester.Controllers;

public class SiteProber
{
    private IPage Page { get; init; }
    private IBrowser _browser;
    private IPlaywright _playwright;
    private const string BeforeScreenshotPath = "ClientApp/public/screenshot-before.png";
    private const string AfterScreenshotPath = "ClientApp/public/screenshot-after.png";

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

    private class ComputedStyle
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Display { get; set; }
        public bool Visibility { get; set; }
        public float Opacity { get; set; }
    }
    
    public async Task ScreenshotVisibleElements()
    {
        await Page.ScreenshotAsync(new() { Path = BeforeScreenshotPath });
        var html = await Page.ContentAsync();
        
        // TODO: Write html to file for debugging so I can see what elements are there.
        
        var elements = await Page.QuerySelectorAllAsync("a, input, button, textarea");

        Func<IElementHandle, Task<string>> isElementVisible = async (element) =>
        {
            return await element.EvaluateAsync<string>(@"(element) => {
                var style = window.getComputedStyle(element);

    // Check if the element is not visible
    if (style.display === 'none' || 
        style.visibility !== 'visible' || 
        style.opacity === ""0"" || 
        style.width === ""0"" || 
        style.height === ""0"") {
        return false;
    }

    // Check if the element is off-screen
    var rect = element.getBoundingClientRect();
    if (rect.bottom < 0 || 
        rect.right < 0 || 
        rect.left > window.innerWidth || 
        rect.top > window.innerHeight) {
        return false;
    }

    // Check if the element is behind another element
    var point = {
        x: rect.left + (rect.right - rect.left)/2,
        y: rect.top + (rect.bottom - rect.top)/2
    };
    var behindElement = document.elementFromPoint(point.x, point.y);
    if (behindElement !== null && behindElement !== element) {
        return false;
    }

    return true;
            }");
        };

        int i = 0;
        
        foreach (var element in elements)
        {
            i++;
            string isVisible = await isElementVisible(element);

            if (isVisible.ToLower() == "true")
            {
                await element.ScreenshotAsync(new() { Path = $"ClientApp/public/screenshot-{i}.png" });
            }
        }
    }
    
    /// <summary>
    /// Chooses an action based on the current page and takes it.
    /// </summary>
    /// <returns></returns>
    public async Task<ActionResult> TakeActionAsync()
    {
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
            BeforeScreenshotPath = BeforeScreenshotPath,
            BeforeScreenshotBytes = Convert.ToBase64String(beforeScreenshotBytes),
            AfterScreenshotPath = AfterScreenshotPath,
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