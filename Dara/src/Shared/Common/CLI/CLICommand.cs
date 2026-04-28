namespace Dara.Shared.Common.CLI
{
    public class CLICommand : Attribute
    {
        public string[] CommandNames;
    
        public CLICommand(params string[] commandNames)
        {
            CommandNames = commandNames;
        }
    }
}