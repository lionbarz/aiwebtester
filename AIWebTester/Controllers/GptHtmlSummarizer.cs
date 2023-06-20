using System.Text;

namespace AIWebTester.Controllers;

public class GptHtmlSummarizer
{
    private static readonly SummaryPromptBlock SummaryShot1 = new()
    {
        Html = "<html><p>Find your favorite shoes</p><input id=\"term\" value=\"Nike\"/><button>Go</button></html>",
        Summary = "Paragraph about finding your favorite shoes. Input with id=\"term\" and value=\"Nike\". Button that says \"Go\"."
    };
    private static readonly SummaryPromptBlock SummaryShot2 = new()
    {
        Html = "<p>Sorry, something went wrong.</p>",
        Summary = "Paragraph about something going wrong."
    };
    private static readonly SummaryPromptBlock SummaryShot3 = new()
    {
        Html = "<label>User name</label><input/><label>Password</label><input/><button>Sign in</button></html>",
        Summary = "Label saying \"User name\". Input with no ID. Label saying \"Password\". Input. Button that says \"Sign in\"."
    };
    private static readonly SummaryPromptBlock SummaryShot4 = new()
    {
        Html = "<label>Country</label><select></select>",
        Summary = "Label saying \"User name\". Input. Label saying \"Password\". Input. Button that says \"Sign in\"."
    };
    
    /// <summary>
    /// Takes in HTML and returns a summary of the HTML in
    /// English that is shorter than the HTML and describes
    /// only what is visible to the user.
    /// TODO: This might be near impossible. How can it be known what's visible without executing all the JS? I think the only way to solve this is with a model that takes in a screenshot.
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static async Task<string> SummarizeHtml(string html)
    {
        var adapter = new GptAdapter();
        var sb = new StringBuilder();
        sb.Append(SummaryShot1);
        sb.Append("\n\n");
        sb.Append(SummaryShot2);
        sb.Append("\n\n");
        sb.Append(SummaryShot3);
        sb.Append("\n\n");
        sb.Append(new SummaryPromptBlock()
        {
            Html = html
        });
        var result = await adapter.CompleteAsync(sb.ToString());

        return result;
    }
}