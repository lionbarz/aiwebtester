namespace AIWebTester.Controllers.OpenAIModels;

/// <summary>
/// This is meant to be sent to the OpenAI API
/// completions endpoints.
/// </summary>
public class Message
{
    /// <summary>
    /// "user", "assistant", or "function"
    /// </summary>
    public string Role { get; set; }
    public string Content { get; set; }
    public FunctionCall? Function_Call { get; set; }
}