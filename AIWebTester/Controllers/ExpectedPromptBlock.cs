namespace AIWebTester.Controllers;

internal class ExpectedPromptBlock
{
    public string HtmlBefore { get; private init; }
    
    public string HtmlAfter { get; private init; }
    
    public string Action { get; private init; }
    
    public bool? IsExpected { get; private set; }
    
    public string? Reason { get; private set; }

    public ExpectedPromptBlock(string htmlBefore, string action, string htmlAfter)
    {
        HtmlBefore = htmlBefore;
        HtmlAfter = htmlAfter;
        Action = action;
    }
    
    public ExpectedPromptBlock(string htmlBefore, string action, string htmlAfter, bool isExpected, string reason)
    {
        HtmlBefore = htmlBefore;
        HtmlAfter = htmlAfter;
        Action = action;
        IsExpected = isExpected;
        Reason = reason;
    }

    public override string ToString()
    {
        return IsExpected == null
            ? $"Before: {HtmlBefore}\nAction: {Action}\nAfter: {HtmlAfter}\nExpected: "
            : $"Before: {HtmlBefore}\nAction: {Action}\nAfter: {HtmlAfter}\nExpected: {IsExpected}\nReason: {Reason}";
    }
}