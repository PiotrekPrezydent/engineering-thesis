namespace Dara.Shared.Common.Logging;

using Console = System.Console;

public class ConsoleLogger
{
    string Intend => new ('\t', _globalIntend);
    private static int _globalIntend = 0;
    
    string _prefix;
    string _operation;

    private const string START_OP = "[ START ]";
    private const string ELEMENT_OP = "  [ELEMENT]";
    private const string END_OP = "[ _END_ ]";

    public ConsoleLogger(string prefix = "")
    {
        _prefix = prefix;
        _operation = "";
    }
    
    public ConsoleLogger(object owner)
    {
        _prefix = owner.GetType().Name + " => ";
        _operation = "";
    }
    
    public void Start(string operation)
    {
        if (_globalIntend > 0)
            _globalIntend++;
        
        operation = operation.ToUpper();
        _operation = " { " + _prefix + operation + " } ";
        
        string log = "\n";

        log += $"{Intend}{START_OP}{_operation}";
        
        Console.WriteLine(log);
        _globalIntend++;
    }

    public void Element(string prettyElementName, object elementValue)
    {
        prettyElementName = prettyElementName.ToUpper();
        string log = "";
        
        log+=ELEMENT_OP + _operation;
        log += $"- {prettyElementName}";
        log += $"\t__TYPE__:{elementValue.GetType().Name}";
        log += $"\t__VALUE_TO_STRING__:{elementValue.ToString()}";
        
        Console.WriteLine(Intend + log);
    }

    public void End()
    {
        _globalIntend--;
        
        string log = "";
        
        log += $"{Intend}{END_OP}{_operation}";
        
        Console.WriteLine($"{log}\n");
        
        _operation = "";
        
        if (_globalIntend > 0)
            _globalIntend--;
    }
}