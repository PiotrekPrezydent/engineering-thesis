using Dara.Nodes.CLI.ConsoleCommands;
using Dara.Nodes.CLI.Services;

namespace Dara.Nodes.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        ConsoleCommandInterpreter.BindObjectCommands(new BasicCommands());
        ConsoleCommandInterpreter.BindObjectCommands(new AuthClient());
        
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