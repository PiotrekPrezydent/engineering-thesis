using Dara.Shared.Common.Console;

namespace Dara.Apps.Clients.CLI.ConsoleCommands;

public class BasicCommands
{
    
    [ConsoleCommand("help")]
    async Task Help()
    {
        System.Console.WriteLine("Help command WIP");
    }
    
    [ConsoleCommand("clear","cls")]
    async Task Clear()
    {
        System.Console.Clear();
    }
    
}