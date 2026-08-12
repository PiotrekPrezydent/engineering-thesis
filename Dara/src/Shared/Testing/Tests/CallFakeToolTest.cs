using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dara.Shared.Testing.Plugins;
using Google.GenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Dara.Shared.Testing.Tests;
//we can easly make AIFunctionTool from value object with just name of function, return type name, description, and list of parameters (type name, parameter name, description, requirement data, nullable data)


public class CallFakeToolTest : ITest
{
    public async Task Run(string api)
    {
        var client = new Client(apiKey: api);
        var models = await client.Models.ListAsync();
        await foreach (var model in models)
        {
            Console.WriteLine(model.Name + " - " + model.DisplayName);
        }
        var chatClient = client.AsIChatClient("gemini-3.1-flash-lite");
        
        //real function that has been recreated lower in static method
        var getStockPriceTool = AIFunctionFactory.Create(
            [Description("Gets the current stock price for a given ticker symbol.")]
            (string ticker) => 
            {
                return ticker.ToUpper() == "MSFT" ? "$420.00" : "$100.00";
            }, 
            name: "GetStockPrice"
        );
        
        var getStockPriceToolMock = FunctionDataTransformer.Transform(StockPriceToolMock);
        
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine(JsonSerializer.Serialize(getStockPriceTool.AsDeclarationOnly(), jsonOptions));
        Console.WriteLine(JsonSerializer.Serialize(getStockPriceToolMock, jsonOptions));
        var chatOptions = new ChatOptions
        {
            //Tools = [getStockPriceTool],
            Tools = [getStockPriceToolMock],
            ToolMode = ChatToolMode.RequireAny
        };
        
        var messages = new List<ChatMessage> { new ChatMessage(ChatRole.User, "What is the stock price of MSFT?") };
        Console.WriteLine("Sending request...");
        var response = await chatClient.GetResponseAsync(messages, chatOptions);
        foreach (var message in response.Messages)
        {
            foreach (var content in message.Contents)
            {
                if (content is FunctionCallContent functionCall)
                {
                    Console.WriteLine($"\ntool execution:");
                    Console.WriteLine($"Function Name: {functionCall.Name}");
                    
                    foreach (var arg in functionCall.Arguments)
                    {
                        
                        Console.WriteLine($"Argument -> {arg.Key}: {arg.Value}");
                    }

                }
            }
        }

        foreach (var message in messages)
        {
            Console.WriteLine(message.RawRepresentation);
        }
        
        Console.WriteLine("FIN");
    }
    
    private static FunctionData StockPriceToolMock = new FunctionData()
    {
        Name = "GetStockPriceMOCK",
        Description = "Gets the current stock price for a given ticker symbol.",
        Parameters = new List<ParameterData>()
        {
            new ParameterData()
            {
                Name = "ticker",
                Type = "string",
            }
        },
        ReturnType = "string"
    };

}