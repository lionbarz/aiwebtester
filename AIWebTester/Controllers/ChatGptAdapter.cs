using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace AIWebTester.Controllers;

public class ChatGptAdapter
{
    private static readonly string API_KEY = "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";
    
    public async Task<string> CompleteAsync(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            throw new ArgumentException("Need some prompts.");
        }
        
        OpenAIAPI api = new OpenAIAPI(API_KEY);

        var result = await api.Completions.CreateCompletionAsync(new CompletionRequest()
        {
            Model = Model.DavinciText,
            MaxTokens = 50,
            Temperature = 0.5,
            Prompt = prompt
        });

        if (result.Completions.Count == 0)
        {
            throw new Exception("No completions");
        }
        
        var completion = result.Completions[0];
        
        return completion.Text;
    }
}