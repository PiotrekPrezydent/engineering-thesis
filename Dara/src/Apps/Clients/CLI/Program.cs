using Dara.Apps.Clients.CLI.ConsoleCommands;
using Dara.Shared.Common.Console;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Apps.Clients.CLI;

class Program
{
    //public static HubConnection Connection;
    static async Task Main(string[] args)
    {
        // string serverUrl = "http://127.0.0.1:5273/auth"; 
        //     
        // Connection = new HubConnectionBuilder()
        //     .WithUrl(serverUrl)
        //     .WithAutomaticReconnect()
        //     .Build();
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