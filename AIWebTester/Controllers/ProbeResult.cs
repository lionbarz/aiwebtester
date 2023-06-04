namespace AIWebTester.Controllers;

public class ProbeResult
{
    // Is the new HTML expected based on the previous HTML and action?
    public bool IsExpected { get; set; }
    
    // Explains why it's expected or not.
    public string? Explanation { get; set; }
}