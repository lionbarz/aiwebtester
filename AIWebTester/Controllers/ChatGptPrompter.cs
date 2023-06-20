using System.Text;

namespace AIWebTester.Controllers;

public class ChatGptPrompter
{
    private static readonly ExpectedPromptBlock ExpectedShot1 = new(
        "<html><p>Find your favorite shoes</p><button>Go</button></html>",
        "Click <button>Go</button>",
        "<html><p>Sorry, something went wrong.<html>",
        false,
        "The site displayed an error page instead of showing search results.");
    
    private static readonly ExpectedPromptBlock ExpectedShot2 = new(
        "<html><p>Find your favorite shoes</p><button>Go</button></html>",
        "Click <button>Go</button>",
        "<html><p>Nike Air</p><html>",
        true,
        "The site displayed search results that match the search term.");

    private static readonly ActionPromptBlock ActionShot1 = new()
    {
        Html = "<html><p>Find your favorite shoes</p><input id='input-55'/><button id='button-3'>Search</button></html>",
        HtmlSummary = "Page to search for shoes",
        Action = "Type",
        Text = "Nike Air",
        TargetId = "input-55",
        Explain = "Entering a search term."
    };
    
    private static readonly ActionPromptBlock ActionShot2 = new()
    {
        Html = "<html><p>Find your favorite shoes</p><input id='input-55' value='Nike Air'/><button id='button-3'>Search</button></html>",
        HtmlSummary = "Page with search results",
        Action = "Click",
        Text = "None",
        TargetId = "button-3",
        Explain = "Searching for the term that was typed into the search box."
    };
    
    /// <summary>
    /// Given an HTML page, an action, and HTML after, gets from ChatGPT
    /// whether it's expected or not, and why.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<string> PromptForIsExpectedAsync(string beforeSummary, string action, string afterSummary)
    {
        var adapter = new GptAdapter();
        var sb = new StringBuilder();
        sb.Append(ExpectedShot1);
        sb.Append("\n\n");
        sb.Append(ExpectedShot2);
        sb.Append("\n\n");
        sb.Append(new ExpectedPromptBlock(beforeSummary, action, afterSummary));
        var result = await adapter.CompleteAsync(sb.ToString());
        return result;
    }

    public static async Task<Action> PromptForActionAsync(string html)
    {
        var actionPromptBlock = new ActionPromptBlock()
        {
            Html = html
        };
        var actionPromptString = actionPromptBlock.ToString();
        var adapter = new GptAdapter();
        var sb = new StringBuilder();
        sb.Append(ActionShot1);
        sb.Append("\n\n");
        sb.Append(ActionShot2);
        sb.Append("\n\n");
        sb.Append(actionPromptString);
        var result = await adapter.CompleteAsync(sb.ToString());

        return Utils.ParseAction(result);
    }
}