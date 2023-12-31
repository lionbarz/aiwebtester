using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AIWebTesterTests;

public class PlaywrightTests
{
    [Test]
    public async Task Test1()
    {
        var beforeScreenshotPath = "ClientApp/public/screenshot-before.png";
        var afterScreenshotPath = "ClientApp/public/screenshot-after.png";
        
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new ()
        {
            Args = new[] { "--allow-insecure-localhost"}
        });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("http://www.amazon.com");
        await page.ScreenshotAsync(new() { Path = beforeScreenshotPath });

        await ScreenshotElements(page);
        
        //await ScreenshotVisibleInteractiveElements(page);
        //var element = page.GetByText("Store");
        //await element.ScreenshotAsync(new() { Path = "ClientApp/public/screenshot-element.png" });
        //await element.ClickAsync();
        //await page.ScreenshotAsync(new() { Path = afterScreenshotPath });
        
        await browser.CloseAsync();
        playwright.Dispose();
    }

    private async Task ScreenshotElements(IPage page)
    {
        int x = 0;
        int y = 0;
        const int increment = 100;
        
        // Elements that are within an anchor element are put here, so that multiple elements
        // under the same anchor can be considered to be the same link and we just use the anchor.
        // The map is from the ID of the anchor element to a list of IDs of the child elements.
        var elementsUnderAnchors = new Dictionary<string, ICollection<string>>();
        
        await page.ExposeFunctionAsync("handleMouseEvent", async (string cursor, string id, string anchorId, int perfTimeMs) =>
        {
            Console.WriteLine($"Cursor: {cursor}, ID: {id}, Anchor ID: {anchorId}, Perf Ms: {perfTimeMs}");

            if (cursor == "pointer")
            {
                try
                {
                    var elementHandle = await page.QuerySelectorAsync($"[data-tester-id='{id}']");

                    if (elementHandle != null)
                    {
                        await elementHandle.ScreenshotAsync(
                            new() { Path = $"ClientApp/public/{id}.png" });

                        if (!string.IsNullOrEmpty(anchorId))
                        {
                            var anchorElementHandle =
                                await page.QuerySelectorAsync($"[data-tester-anchor-id='{anchorId}']");

                            if (anchorElementHandle != null)
                            {
                                await anchorElementHandle.ScreenshotAsync(
                                    new() { Path = $"ClientApp/public/{anchorId}.png" });
                            }
                            else
                            {
                                throw new Exception($"There's no anchor with ID {anchorId}");
                            }

                            if (elementsUnderAnchors.ContainsKey(anchorId))
                            {
                                elementsUnderAnchors[anchorId].Add(id);
                            }
                            else
                            {
                                elementsUnderAnchors.Add(anchorId, new List<string>() { id });
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No element for ID {id}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (x < 1000)
            {
                x += increment;
            }
            else
            {
                if (y < 1000)
                {
                    y += increment;
                    x = 0;
                }
            }

            if (x <= 1000 && y <= 1000)
            {
                await page.Mouse.MoveAsync(x, y);
            }

            return true;
        });
        
        await page.ExposeFunctionAsync("onError", (string error) =>
        {
            Console.WriteLine($"Error: {error}");
            return true;
        });
        
        await page.EvaluateAsync(@"() => {
            document.addEventListener('mousemove', (event) => {
try {
                let startTime = performance.now();
                // Retrieve event data and target information
                //const targetElement = JSON.stringify(event.target);
                const targetElementHtml = event.target.outerHTML;
                var cursor = (window.getComputedStyle(event.target)).cursor;

                var anchorParent = event.target.closest('a');
                var anchorId = null;
                if (anchorParent) {
                    anchorId = anchorParent.getAttribute('data-tester-anchor-id');
                    if (anchorId === null || anchorId === '') {
                        // The element doesn't have an ID attribute, so set it
                        anchorId = 'anchor-id-' + event.x + '-' + event.y;
                        anchorParent.setAttribute('data-tester-anchor-id', anchorId);
                    }
                }

                var id = event.target.getAttribute('data-tester-id');
                if (id === null || id === '') {
                    // The element doesn't have an ID attribute, so set it
                    id = 'id-' + event.x + '-' + event.y;
                    event.target.setAttribute('data-tester-id', id);
                }
                
                let endTime = performance.now();

                // Call the exposed C# function with event data
                window.handleMouseEvent(cursor, id, anchorId, endTime - startTime);
} catch (e) { onError(e.message); }
            });
        }");

        await page.Mouse.MoveAsync(x, y);
        
        // Give a chance for events to fire.
        await Task.Delay(20000);
    }
    
    private async Task ScreenshotVisibleInteractiveElements(IPage page)
    {
        //var visibleElements = await page.QuerySelectorAllAsync("a input");
        var links = await page.GetByRole(AriaRole.Link).AllAsync();
        var buttons = await page.GetByRole(AriaRole.Button).AllAsync();
        var visibleElements = links.Concat<ILocator>(buttons);
        
        var i = 0;
        
        foreach (var element in visibleElements)
        {
            i++;

            if (i == 100)
            {
                return;
            }
            
            try
            {
                await element.ScreenshotAsync(new() { Path = $"ClientApp/public/screenshot-element-{i}.png" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}