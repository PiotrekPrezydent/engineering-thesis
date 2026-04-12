using Dara.Core.Shared.Auth;
using Dara.Nodes.CLI.ConsoleCommands;
using Dara.Nodes.CLI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Nodes.CLI;

class Program
{
    public static HubConnection Connection;
    static async Task Main(string[] args)
    {
        string serverUrl = "http://127.0.0.1:5273/auth"; 
            
        Connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .Build();
        
        ConsoleCommandInterpreter.BindObjectCommands(new BasicCommands());
        AuthClient handler = new AuthClient(Connection);
        ConsoleCommandInterpreter.BindObjectCommands(handler);

        Connection.Register<IAuthHubClient>(handler);
        await Connection.StartAsync();
        
        do
        {
            Console.Write("Type command: ");
            string? read = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(read))
                continue;
            
            ConsoleCommandInterpreter.Handle(read);
            
        } while(true);
    }
    
}