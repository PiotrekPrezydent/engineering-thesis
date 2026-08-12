using System.Text.Json;
using Dara.Shared.Testing.Plugins;
using Google.GenAI;
using Microsoft.Extensions.AI;

namespace Dara.Shared.Testing.Tests;

public class CallCorrentUserToolTest : ITest
{
    public static string SystemPromptFormat =>
        """
        You are an orchestration agent controlling multiple clients in group that are connected to RPC server.
        Your goal is to complete request specified by the current group message using tool calls.
        It is the server that controls the real tool call, each tool call result is mocked and can be used as parameter in next tool call.
        The result of each tool call is mocked and contains only an identifier of the previous tool call. You may use that result as an input to a subsequent tool call when appropriate.
        By doing tool calls you are creating plan of remote calls that server will dispatch after full plan was accepted.

        Client and group member naming is used alternately, but they represent same instance of connection to server
        Each tool belongs to a specific client in server, the client name is included in the tool name as prefix in format: [CLIENT-NAME]_[TOOL-NAME].
        Each user chat message have known format specifying client name, and message: [CLIENT-NAME] : [MESSAGE]
        
        Available clients in group:
        {0}
        
        Rules:
        1. Always choose tools according to the client they belong to.
        2. Never assume that two clients have the same state, files, directories, or capabilities.
        3. When the group member asks to perform an action involving multiple members, use the appropriate tool for each member.
        4. A tool call must target the client that owns that capability.
        5. You may use the result of a previous tool call as input for a later tool call.
        6. Do not invent tools or capabilities that are not available.
        7. Prefer the client explicitly mentioned by the group member.
        8. Every tool argument must come from:
            - the user's request,
            - available group/client context,
            - or the result of a previous tool call.
        9. If a required argument cannot be obtained from these sources, do not call the tool and and pass a message indicating the missing data.
        
        When you have enough information to complete the request, stop making tool calls and provide a concise summary of the planned actions.
        """;
    
    // note:
    // ai having information of capabilities for each client successfully chosed correct ones to call and used previous calls result as arguments when needed
    // bob phone requested to take photo from his laptop and save it to his phone
    // the plan was to call F1: CameraCapture on laptop, F2: PhotosPath on phone, and then SaveFile(F2,F1) 
    public async Task Run(string api)
    {
        var client = new Client(apiKey: api);
        var chatClient = client.AsIChatClient("gemini-3.1-flash-lite");
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        
        var bobPhoneFunctions = GetBobPhoneFunctions().Select(FunctionDataTransformer.Transform).ToList();
        var bobLaptopFunctions = GetBobLaptopFunctions().Select(FunctionDataTransformer.Transform).ToList();

        var chatOptions = new ChatOptions
        {
            ToolMode = ChatToolMode.Auto,
            Tools = new List<AITool>()
        };

        foreach (var function in bobPhoneFunctions)
        {
            chatOptions.Tools.Add(function);
        }

        foreach (var function in bobLaptopFunctions)
        {
            chatOptions.Tools.Add(function);
        }

        var avaibleUsers = "Bob-Phone" + "\n" + "Bob-Laptop";
        
        var systemPrompt = string.Format(SystemPromptFormat, avaibleUsers);
        var prompt = "Bob-Phone" + " : " + "Take photo from my laptop camera and save it to my phone.";
        
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, prompt)
        };

        //tool call loop
        int i = 0;
        while (true)
        {
            Console.WriteLine($"\n\n SENDING REQUEST ITERATION: {i++}");
            var response = await chatClient.GetResponseAsync(
                messages,
                chatOptions);
            
            Console.WriteLine("RESPONSE MESSAGES COUNT: " + response.Messages.Count);
            // 
            foreach (var message in response.Messages)
            {
                Console.WriteLine("RESPONSE MESSAGE DATA:");
                Console.WriteLine($"\tAUTHOR : {SaveToString(message.AuthorName)}");
                Console.WriteLine($"\tROLE : {SaveToString(message.Role)}");
                Console.WriteLine($"\tTEXT : {SaveToString(message.Text)}");
                Console.WriteLine($"\tCONTENTS : {SaveToString(message.Contents.Count)} ---- JOIN {string.Join(", ", message.Contents.Select(SaveToString))}");
                Console.WriteLine("");
                messages.Add(message);
            }
            
            
            var functionCalls = response.Messages
                .SelectMany(m => m.Contents)
                .OfType<FunctionCallContent>()
                .ToList();
        
            Console.WriteLine("FUNCTION CALLS COUNT: " + functionCalls.Count);
            if (functionCalls.Count == 0)
            {
                Console.WriteLine("NO FUNCTION CALLS WRITING MESSAGES CONTENT");

                foreach (var message in response.Messages)
                {
                    foreach (var content in message.Contents)
                    {
                        Console.WriteLine(content);
                    }
                }
                
                break;
            }
            
            
            foreach (var functionCall in functionCalls)
            {
                Console.WriteLine("TOOL CALL DATA:");
                Console.WriteLine($"\tFunction: {functionCall.Name}");
                Console.WriteLine($"\tCallId:   {functionCall.CallId}");
                if (functionCall.Arguments == null)
                {
                    Console.WriteLine("\tARGUMENTS NULL");
                }
                else
                {
                    Console.WriteLine("\tARGUMENTS COUNT :  " + functionCall.Arguments.Count);
                    foreach (var argument in functionCall.Arguments)
                    {
                        Console.WriteLine($"\t\tArgument: {argument.Key} = {argument.Value}");
                    }
                }

                string mock = $"RESULT-OF({functionCall.Name}-{functionCall.CallId})";
                var result = new FunctionResultContent(functionCall.CallId, mock);
                messages.Add(new ChatMessage(ChatRole.Tool,[result]));
            }
            
        }
        
    }


    static string SaveToString(object? value)
    {
        if (value == null)
            return "*NULL*";
        
        return value.ToString() ?? "ToStringNULL";
    }

    List<FunctionData> GetBobPhoneFunctions()
    {
        var result = new List<FunctionData>();
        var camera = CommonFunctionDatas.CameraCapture;
        camera.Name = "Bob-Phone_" + camera.Name;

        var save = CommonFunctionDatas.SaveFile;
        save.Name = "Bob-Phone_" + save.Name;
        
        var path = CommonFunctionDatas.PhotosDirectory;
        path.Name = "Bob-Phone_" + path.Name;
        
        result.Add(camera);
        result.Add(save);
        result.Add(path);
        
        return result;
    }

    List<FunctionData> GetBobLaptopFunctions()
    {
        var result = new List<FunctionData>();
        var camera = CommonFunctionDatas.CameraCapture;
        camera.Name = "Bob-Laptop_" + camera.Name;

        var save = CommonFunctionDatas.SaveFile;
        save.Name = "Bob-Laptop_" + save.Name;

        var path = CommonFunctionDatas.PhotosDirectory;
        path.Name = "Bob-Laptop_" + path.Name;
        
        result.Add(camera);
        result.Add(save);
        result.Add(path);
        
        return result;
    }
}