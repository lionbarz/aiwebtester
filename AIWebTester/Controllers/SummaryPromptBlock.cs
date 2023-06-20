namespace AIWebTester.Controllers;

public class SummaryPromptBlock
{
    public string? Html { get; set; }
    
    public string? Summary { get; set; }

    public override string ToString()
    {
        return $"Html: {Html}\nSummary: {Summary}";
    }
}