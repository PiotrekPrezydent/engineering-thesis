using Dara.Apps.Clients.CLI.ConsoleCommands;
using Dara.Shared.Common.Console;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dara.Apps.Clients.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleCommandInterpreter ci = new();

            BasicCommands bc = new();
            DaraConnection con = new();
            ConnectionCommands cc = new(con.Connection);
            ClientCommands acc = new(con.Connection);
        
            ci.BindObjectCommands(bc);
            ci.BindObjectCommands(con);
            ci.BindObjectCommands(cc);
            ci.BindObjectCommands(acc);
        
            do
            {
                Console.Write("Type command: ");
                string? read = Console.ReadLine();
            
                if (string.IsNullOrWhiteSpace(read))
                    continue;
            
                await ci.HandleAsync(read);
            
            } while(true);
        }
    
    }
}