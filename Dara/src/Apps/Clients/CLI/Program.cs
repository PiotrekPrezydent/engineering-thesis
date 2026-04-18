using Dara.Apps.Clients.CLI.ConsoleCommands;
using Dara.Shared.Common.Console;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dara.Apps.Clients.CLI;

class Program
{
    static HubConnection _connection;
    static async Task Main(string[] args)
    {
        string serverUrl = "http://127.0.0.1:5273/app"; 
            
        _connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .Build();

        await _connection.StartAsync();
        
        ConsoleCommandInterpreter cci = new();
        cci.BindObjectCommands(new BasicCommands());
        do
        {
            Console.Write("Type command: ");
            string? read = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(read))
                continue;
            
            cci.Handle(read);
            
        } while(true);
    }
    
}