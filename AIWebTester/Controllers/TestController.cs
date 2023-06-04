using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace AIWebTester.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    public class GetResult
    {
        public string? Prompt { get; set; }
        public string? Result { get; set; }
    }
    
    [HttpGet]
    public async Task<GetResult> Get()
    {
        TestablePage page;
        
        try
        {
            page = await GetPage();
            return new GetResult()
            {
                Result = "Test done!"
            };
        }
        catch (Exception e)
        {
            return new GetResult()
            {
                Result = "Couldn't get page: " + e
            };
        }
    }

    public async Task<string> GetTextForInput(TestablePage page)
    {
        if (page.Input == null)
        {
            throw new Exception("There's no input on the page");
        }
        
        string prompt =
            $"What might you type into the input before pressing \"{page.Input.SubmitLabel}\"? Give me a concrete example. Give me just what the example text might be. You answer should be only what I would type into the text box.";

        var chatGptAdapter = new ChatGptAdapter();
        return await chatGptAdapter.CompleteAsync($"{page.ToString()}\n{prompt}");
    }

    public async Task<GetResult> GetTestResult(TestablePage page, string textInput)
    {
        OpenAIAPI api = new OpenAIAPI("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");
        string pageDescription =
            $"It is a website that has a header that says \"{page.Heading}\", a label that says \"{page.Input.InputLabel}\", and a button that says \"{page.Input.SubmitLabel}\".";

        var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 50,
            Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, "You are a tester who is testing a website."),
                new ChatMessage(ChatMessageRole.User, page.ToString()),
                new ChatMessage(ChatMessageRole.User, $"You type {textInput} into the input field and click {page.Input.InputLabel}. Now the website says \"paragraphs[0]\". Did the website behave as someone might expect?"),
                new ChatMessage(ChatMessageRole.User, $"Answer just yes or no.")
            }
        });

        return new GetResult();
    }
    
    /// <summary>
    /// Demo: This assumes one input per form/submit.
    /// </summary>
    public class PageInput
    {
        // What is the prompt to the input? What are you supposed to input?
        public string? InputLabel { get; set; }
        
        // What does the submit button say? Ex: Search, Go, Submit.
        public string? SubmitLabel { get; set; }
    }
    public class TestablePage
    {
        public string? Heading { get; set; }
        public PageInput? Input { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Heading) || Input == null)
            {
                return "It's a website with no heading or no input fields.";
            }
            
            return $"It is a website that has a header that says \"{Heading}\", a label that says \"{Input.InputLabel}\", and a button that says \"{Input.SubmitLabel}\".";
        }
    }
    
    private async Task<TestablePage> GetPage()
    {
        var result = new TestablePage();
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new ()
        {
            Args = new[] { "--allow-insecure-localhost"}
        });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://localhost:44401/");

        var headings = page.GetByRole(AriaRole.Heading);
        var numHeadings = await headings.CountAsync();

        if (numHeadings > 0)
        {
            result.Heading = await headings.First.TextContentAsync();
        }
        
        var buttons = page.GetByRole(AriaRole.Button);
        var numButtons = await buttons.CountAsync();

        if (numButtons > 0)
        {
            result.Input = new PageInput()
            {
                SubmitLabel = await buttons.First.TextContentAsync()
            };
        }

        var labels = page.Locator("label");
        var numLabels = await labels.CountAsync();

        if (numLabels > 0)
        {
            if (result.Input == null)
            {
                result.Input = new PageInput();
            }
            
            result.Input.InputLabel = await labels.First.TextContentAsync();
        }

        await page.ScreenshotAsync(new()
        {
            Path = "screenshot-before.png"
        });

        var textForInput = await GetTextForInput(result);
        textForInput = textForInput.Replace("\"", "");
        
        await page.GetByRole(AriaRole.Textbox).FillAsync(textForInput);
        await buttons.First.ClickAsync();
        
        await page.ScreenshotAsync(new()
        {
            Path = "screenshot-after.png"
        });


        var paragraphs = page.GetByRole(AriaRole.Paragraph);
        
        
        return result;
    }
}