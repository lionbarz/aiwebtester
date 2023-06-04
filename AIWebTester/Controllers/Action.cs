namespace AIWebTester.Controllers;

public class Action
{
    /// <summary>
    /// Whether the action is to type or click.
    /// </summary>
    public ActionKind? Kind { get; set; }
    
    /// <summary>
    /// If action is to type, then the text to type.
    /// </summary>
    public string? TypeText { get; set; }
    
    /// <summary>
    /// The ID of the element to click or type into.
    /// </summary>
    public string? ElementId { get; set; }
    
    /// <summary>
    /// English explanation about what the action is doing.
    /// </summary>
    public string? Explain { get; set; }
}