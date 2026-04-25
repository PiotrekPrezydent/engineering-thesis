namespace Dara.Shared.Common.Console
{
    public class ConsoleCommand : Attribute
    {
        public string[] CommandNames;
    
        public ConsoleCommand(params string[] commandNames)
        {
            CommandNames = commandNames;
        }
    }
}