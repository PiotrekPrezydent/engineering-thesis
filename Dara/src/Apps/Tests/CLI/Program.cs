using Microsoft.AspNetCore.SignalR.Client;

namespace Dara.Apps.Tests.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        string serverUrl = "http://127.0.0.1:5100/app"; 
            
        var Connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .Build();
        
        await Connection.StartAsync();
        Console.WriteLine("Hello, World!");
        Console.ReadKey();
    }
}