namespace Dara.Shared.Common.Logging;

using Console = System.Console;

public class ConsoleLogger
{
    public static string Intend => new ('\t', _intend);
    private static int _intend = 0;
    
    private string _operation;
    
    
    public ConsoleLogger(string operation, bool autoStart = true)
    {
        _operation = operation;
        
        if(autoStart)
            Start();
    }
    
    public void Start()
    {
        Console.WriteLine($"{Intend}[ {_operation} STARTED ]");
        _intend++;
    }

    public void Element(string prettyElementName, object elementValue)
    {
        string log = "";
        log += $"- {prettyElementName}";
        log += $"\t__TYPE__:{elementValue.GetType().Name}";
        log += $"\t__VALUE_TO_STRING__:{elementValue.ToString()}";
        
        Console.WriteLine(Intend + log);
    }

    public void End()
    {
        Console.WriteLine($"{Intend}[ {_operation} ENDED ]" );
        _intend--;
    }


}