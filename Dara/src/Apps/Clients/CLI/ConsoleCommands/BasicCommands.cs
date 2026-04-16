using Dara.Shared.Common.Console;

namespace Dara.Apps.Clients.CLI.ConsoleCommands;

public class BasicCommands
{
    
    [ConsoleCommand("help")]
    void HelpCommand()
    {
        System.Console.WriteLine("Help command WIP");
    }

    void ClearCommand()
    {
        System.Console.Clear();
    }
}