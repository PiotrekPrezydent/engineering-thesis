namespace Dara.Shared.Common.CLI;

public class CLILogger
{
    public string InstanceName { get; }
    public String Colons => new(':', ColonLenght);

    ConsoleColor _nameColor;
    
    const int ColonLenght = 4;
    const int NameLenght = 24;
    
    public CLILogger(string instanceName, ConsoleColor color)
    {
        InstanceName = instanceName.ToUpper();
        if (InstanceName.Length > NameLenght)
            throw new ArgumentException($"Instance name '{InstanceName}' is too long.");
        
        _nameColor = color;
    }

    public void Log(string message)
    {
        WriteColoredName();
        
        Console.Write($" {Colons} ");
        Console.Write(message);
        Console.Write("\n");
    }

    public void LogMany(params string[] messages)
    {
        WriteColoredName();
        foreach (var message in messages)
        {
            WriteColoredDot();
            Console.Write(message);
            
            Console.Write("\n");
            Console.Write("\t\t\t");
        }
        Console.Write("\n");
    }

    //TD Full operation logger that will take method and method parameters values as argument, and will in every step of method check value of method arguments.
    // public void LogOperation(Task operation)
    // {
    // }

    void WriteColoredName()
    {
        Console.ResetColor();
        Console.Write("[ ");
        
        Console.ForegroundColor = _nameColor;
        Console.Write(InstanceName);
        Console.Write(new string(' ', NameLenght - (2 + InstanceName.Length + 2)));
        
        Console.ResetColor();
        Console.Write(" ]");
    }

    void WriteColoredDot()
    {
        Console.ResetColor();
        Console.ForegroundColor = _nameColor;
        Console.Write(" * ");
        Console.ResetColor();
    }
}
