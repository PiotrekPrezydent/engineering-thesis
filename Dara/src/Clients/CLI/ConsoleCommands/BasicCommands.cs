namespace Dara.Nodes.CLI.ConsoleCommands;

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