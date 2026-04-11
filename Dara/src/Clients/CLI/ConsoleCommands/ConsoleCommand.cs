namespace Dara.Nodes.CLI.ConsoleCommands;

public class ConsoleCommand : Attribute
{
    public string CommandName;
    
    public ConsoleCommand(string commandName)
    {
        CommandName = commandName;
    }
}