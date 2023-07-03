namespace AIWebTester.Controllers.OpenAIModels;

public class FunctionParameterProperty
{
    public FunctionParameterProperty(string type)
    {
        Type = type;
    }

    public string Type { get; set; }
    public string? Description { get; set; }
    public string[]? Enum { get; set; }
}