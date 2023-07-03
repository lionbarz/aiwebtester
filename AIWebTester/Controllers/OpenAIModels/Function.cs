namespace AIWebTester.Controllers.OpenAIModels;

public class Function
{
    public string Name { get; set; }
    public string Description { get; set; }
    public FunctionParameters Parameters { get; set; }
}