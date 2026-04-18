namespace Dara.Shared.Common.Console;

public class ConsoleCommand : Attribute
{
    public string CommandName;
    
    public ConsoleCommand(string commandName)
    {
        CommandName = commandName;
    }
}