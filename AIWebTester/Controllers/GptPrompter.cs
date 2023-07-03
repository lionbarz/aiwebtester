using AIWebTester.Controllers.OpenAIModels;
using Newtonsoft.Json;

namespace AIWebTester.Controllers;

/// <summary>
/// Remembers the message history and prompts GPT for
/// steps in the probe: action, is expected.
/// </summary>
public class GptPrompter
{
    /// <summary>
    /// Gets back an action given a page.
    /// </summary>
    /// <param name="html">What's on the page that we want to take action on.</param>
    /// <returns>Action details</returns>
    public async Task<Action> PromptForActionAsync(string html)
    {
        var chatCompletion = await GptRestAdapter.CompleteChat(new Message[]
        {
            new()
            {
                Role = "user",
                Content = $"Extract an action to be taken on the following HTML page.\n\n{html}"
            } 
        }, new Function[]
        {
            new Function()
            {
                Name = "extract_action",
                Description = "Find actions that can be taken by a user on a given HTML page.",
                Parameters = new FunctionParameters()
                {
                    Type = "object",
                    Properties = new Dictionary<string, FunctionParameterProperty>()
                    {
                        { "htmlSummary", new FunctionParameterProperty("string")
                        {
                            Description = "Brief summary of what the given HTML displays."
                        }},
                        { "action", new FunctionParameterProperty("string")
                        {
                            Description = "Some interaction a user can do on the page.",
                            Enum = new []{ "Click", "Type" }
                        }},
                        { "text", new FunctionParameterProperty("string")
                        {
                            Description = "The text to be typed if the action type is \"Type\"."
                        }},
                        { "targetId", new FunctionParameterProperty("string")
                        {
                            Description = "The ID of the HTML element to be clicked or typed into."
                        }},
                        { "explain", new FunctionParameterProperty("string")
                        {
                            Description = "A brief explanation of the action to be taken."
                        }}
                    },
                    Required = new []{"htmlSummary","action","text","targetId","explain"}
                }
            }
        });

        if (chatCompletion.Choices.Length == 0)
        {
            throw new Exception("The GPT chat completion returned zero choices.");
        }

        var completionOption = chatCompletion.Choices[0];

        if (completionOption.Finish_Reason != "function_call")
        {
            throw new Exception(
                $"Chat completed without function call. Reason: {completionOption.Finish_Reason}. Message: {completionOption.Message}");
        }

        var functionCallArguments =
            JsonConvert.DeserializeObject<ActionPromptBlock>(completionOption.Message.Function_Call.Arguments);
        
        if (functionCallArguments.Action != "Click" && functionCallArguments.Action != "Type")
        {
            throw new ArgumentException($"The action string needs to be either \"Click\" or \"Type\". Got {functionCallArguments.Action}");
        }

        return new Action()
        {
            Kind = functionCallArguments.Action == "Type" ? ActionKind.Type : ActionKind.Click,
            ElementId = functionCallArguments.TargetId,
            Explain = functionCallArguments.Explain,
            PageSummary = functionCallArguments.HtmlSummary,
            TypeText = functionCallArguments.Text
        };
    }
}