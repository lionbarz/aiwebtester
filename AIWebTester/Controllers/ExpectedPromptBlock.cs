namespace AIWebTester.Controllers;

internal class ExpectedPromptBlock
{
    public string BeforeSummary { get; private init; }
    
    public string AfterSummary { get; private init; }
    
    public string Action { get; private init; }
    
    public bool? IsExpected { get; private set; }
    
    public string? Reason { get; private set; }

    public ExpectedPromptBlock(string beforeSummary, string action, string afterSummary)
    {
        BeforeSummary = beforeSummary;
        AfterSummary = afterSummary;
        Action = action;
    }
    
    public ExpectedPromptBlock(string beforeSummary, string action, string afterSummary, bool isExpected, string reason)
    {
        BeforeSummary = beforeSummary;
        AfterSummary = afterSummary;
        Action = action;
        IsExpected = isExpected;
        Reason = reason;
    }

    public override string ToString()
    {
        return IsExpected == null
            ? $"Before: {BeforeSummary}\nAction: {Action}\nAfter: {AfterSummary}\nExpected: "
            : $"Before: {BeforeSummary}\nAction: {Action}\nAfter: {AfterSummary}\nExpected: {IsExpected}\nReason: {Reason}";
    }
}