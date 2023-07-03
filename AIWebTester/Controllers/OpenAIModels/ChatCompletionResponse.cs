namespace AIWebTester.Controllers.OpenAIModels;

public class ChatCompletionResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public int Created { get; set; }
    public string Model { get; set; }
    public Choice[] Choices { get; set; }
}