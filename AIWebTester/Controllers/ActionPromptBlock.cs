namespace AIWebTester.Controllers;

public class ActionPromptBlock
{
    public string? Html { get; set; }
    
    /// <summary>
    /// "Click" or "Kind"
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// The text to input, or "None" if action is to click.
    /// </summary>
    public string? Text { get; set; }
    public string? TargetId { get; set; }
    public string? Explain { get; set; }

    public override string ToString()
    {
        return Action == null
            ? $"Html: {Html}\nAction: "
            : $"Html: {Html}\nAction: {Action}\nText: {Text}\nTargetId: {TargetId}\nExplain: {Explain}";
    }
}