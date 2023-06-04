using System.Text;

namespace AIWebTester.Controllers;

public class ChatGptPrompter
{
    private static readonly ExpectedPromptBlock ExpectedShot1 = new ExpectedPromptBlock(
        "<html><p>Find your favorite shoes</p><button>Go</button></html>",
        "Click <button>Go</button>",
        "<html><p>Sorry, something went wrong.<html>",
        false,
        "The site displayed an error page.");
    
    private static readonly ExpectedPromptBlock ExpectedShot2 = new ExpectedPromptBlock(
        "<html><p>Find your favorite shoes</p><button>Go</button></html>",
        "Click <button>Go</button>",
        "<html><p>Nike Air</p><html>",
        true,
        "The site displayed a shoe.");

    private static readonly ActionPromptBlock ActionShot1 = new ActionPromptBlock()
    {
        Html = "<html><p>Find your favorite shoes</p><input id='input-55'/><button id='button-3'>Search</button></html>",
        Action = "Kind",
        Text = "Nike Air",
        TargetId = "input-55",
        Explain = "Entering a search term."
    };
    
    private static readonly ActionPromptBlock ActionShot2 = new ActionPromptBlock()
    {
        Html = "<html><p>Find your favorite shoes</p><input id='input-55' value='Nike Air'/><button id='button-3'>Search</button></html>",
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
    public static async Task<string> PromptForIsExpectedAsync(string htmlBefore, string action, string htmlAfter)
    {
        var adapter = new ChatGptAdapter();
        var sb = new StringBuilder();
        sb.Append(ExpectedShot1);
        sb.Append("\n\n");
        sb.Append(ExpectedShot2);
        sb.Append("\n\n");
        sb.Append(new ExpectedPromptBlock(htmlBefore, action, htmlAfter));
        var result = await adapter.CompleteAsync(sb.ToString());
        return result;
    }

    public static async Task<Action> PromptForActionAsync(string html)
    {
        var adapter = new ChatGptAdapter();
        var sb = new StringBuilder();
        sb.Append(ActionShot1);
        sb.Append("\n\n");
        sb.Append(ActionShot2);
        sb.Append("\n\n");
        sb.Append(new ActionPromptBlock()
        {
            Html = html
        });
        var result = await adapter.CompleteAsync(sb.ToString());

        return Utils.ParseAction(result);
    }
}