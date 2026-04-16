namespace Dara.Shared.Common.Logging;

using Console = System.Console;

public class ConsoleLogger
{
    private string _operation;
    public ConsoleLogger(string operation)
    {
        _operation = operation;
    }
    
    public void Start()
    {
        Console.WriteLine($"[ {_operation} STARTED ]" );
    }

    public void Element(string prettyElementName, object elementValue)
    {
        string log = "\t";

        log += $"- {prettyElementName}";
        log += $"\t__TYPE__:{elementValue.GetType().Name}";
        log += $"\t__VALUE_TO_STRING__:{elementValue.ToString()}";
        
        Console.WriteLine(log);

    }

    public void End()
    {
        Console.WriteLine($"[ {_operation} ENDED ]" );
    }
}