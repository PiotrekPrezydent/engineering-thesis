using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace LADR.Prototype.WebServer;

public class SKManager
{
    public string API;
    public SKManager(string api)
    {
        API = api;
    }
    
    public async Task<List<FunctionCallContent>> SendGeminiPrompt(string prompt, List<object> pluginsObjects)
    {
        var builder = Kernel.CreateBuilder();
        builder.AddGoogleAIGeminiChatCompletion(
            modelId: "gemini-2.5-flash",
            apiKey: API
        );
        
        foreach (var plugin in pluginsObjects)
        {
            Console.WriteLine("Adding plugin object" + plugin.GetType().Name);
            builder.Plugins.AddFromObject(plugin);
        }
        var kernel = builder.Build();
        Console.WriteLine("ended adding objects");
        
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);
        
        var executionSettings = new GeminiPromptExecutionSettings
        {
            ToolCallBehavior = GeminiToolCallBehavior.EnableKernelFunctions
        };
        
        Console.WriteLine("Sending prompt " + prompt + " to gemini");
        
        var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
        return response.Items.OfType<FunctionCallContent>().ToList();
    }
}