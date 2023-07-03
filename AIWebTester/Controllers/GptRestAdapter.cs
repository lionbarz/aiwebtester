using System.Text.Json;
using AIWebTester.Controllers.OpenAIModels;

namespace AIWebTester.Controllers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

/// <summary>
/// Uses the Open AI REST API so it can support the latest functions.
/// </summary>
public class GptRestAdapter
{
    private static readonly HttpClient client = new();
    private const string openaiApiKey = "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";
    private const string openaiEndpoint = "https://api.openai.com/v1/chat/completions";
    
    public static async Task<ChatCompletionResponse> CompleteChat(IEnumerable<Message> messages, IEnumerable<Function> functions)
    {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openaiApiKey);

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    model = "gpt-3.5-turbo-0613",
                    messages = messages.Select(m => new {
                        role = m.Role,
                        content = m.Content
                        }),
                    functions = functions.Select(f => new
                    {
                        name = f.Name,
                        description = f.Description,
                        parameters = new
                        {
                            type = f.Parameters.Type,
                            properties = new Dictionary<string, object>(f.Parameters.Properties.Select(x =>
                            {
                                var dict = new Dictionary<string, object>()
                                {
                                    { "type", x.Value.Type },
                                    { "description", x.Value.Description }
                                };

                                if (x.Value.Enum != null)
                                {
                                    dict["enum"] = x.Value.Enum;
                                }
                                
                                return new KeyValuePair<string, object>(
                                    x.Key,
                                    dict
                                );
                            })),
                            required = f.Parameters.Required
                        }
                    })
                }),
                Encoding.UTF8,
                "application/json");

            var contentString = await jsonContent.ReadAsStringAsync();
            var response = await client.PostAsync(openaiEndpoint, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();

            if (responseJson == null)
            {
                throw new Exception($"deserialized completion response was null: {responseString}");
            }
            
            return responseJson;
    }
}