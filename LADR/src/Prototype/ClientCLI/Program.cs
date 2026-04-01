using Microsoft.AspNetCore.SignalR.Client;

namespace LADR.Prototype.ClientCLI;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("start");

        PluginManager pm = new();
        var plugins = Directory.GetFiles(Directory.GetCurrentDirectory() + "/plugins", "*.dll");

        foreach (var file in plugins)
            pm.LoadPlugin(file);
        
        string serverUrl = "http://127.0.0.1:5194/clients"; 
            
        var connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .Build();
        
        connection.On<string>("ReceiveMessage", (message) =>
        {
            Console.WriteLine($"\nserver response: {message}");
        });
        
        connection.On<List<object>>("PluginsAdded", (message) =>
        {
            Console.WriteLine($"\nserver response: ADDED PLUGINS: {message.Count}");
        });
        
        string roomId = "1";
        
        try
        {
            Console.WriteLine("Connecting...");
            await connection.StartAsync();
            Console.WriteLine("Connected");
            
            Console.WriteLine("Joining room...");
            await connection.InvokeAsync("JoinRoom", roomId);
            Console.WriteLine("Joined room");
            
            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) 
                    continue;

                if (input == "addall")
                {
                    await connection.InvokeAsync("AddPlugins", roomId, pm.Plugins);
                    continue;
                }

                if (input.StartsWith("prompt"))
                {
                    input = input.Substring("prompt".Length);
                    await connection.InvokeAsync("SendPrompt", roomId, input);
                }
                
                await connection.InvokeAsync("SendMessageToRoom", roomId, $"Message to room: {input}");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}