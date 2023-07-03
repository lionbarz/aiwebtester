namespace AIWebTester.Controllers.OpenAIModels;

/// <summary>
/// Examples here: https://openai.com/blog/function-calling-and-other-api-updates?ref=upstract.com
/// </summary>
public class FunctionParameters
{
    public string Type { get; set; }
    public IDictionary<string, FunctionParameterProperty> Properties { get; set; }
    public string[] Required { get; set; }
}