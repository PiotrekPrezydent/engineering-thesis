using System.Reflection;

namespace Dara.Shared.Common.Logging;

public delegate void Log(params object[] args);

public class Logger
{
    public readonly string InstanceName;

    private static int _instances = 0;
    
    public Logger(string instanceName, object ownerClass, LoggingType loggingType)
    {
        InstanceName = instanceName;
        _instances++;

        foreach (var log in ownerClass.GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(e=>e.FieldType == typeof(Log)))
        {
            var config = log.GetCustomAttribute<LogConfig>();
            log.SetValue(null, new Log(args =>
            {
                string formated = string.Format(config.LogContext, args);
                
                switch (loggingType)
                {
                    case LoggingType.Console:
                        ConsoleLog(config.EventName, formated,(ConsoleColor)_instances);
                        break;
                    case LoggingType.Other:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(loggingType), loggingType, null);
                }    
            }));
        }
    }

    void ConsoleLog(string eventName, string msg, ConsoleColor color)
    {
        Console.ResetColor();
        Console.Write("[ ");
        
        Console.ForegroundColor = color;
        Console.Write($"{InstanceName,-30}");
        Console.ResetColor();
        
        Console.Write(" ] #");
        
        Console.ForegroundColor = color;
        Console.Write(eventName);
        Console.ResetColor();
        
        Console.Write(new string(':', 8));
        Console.Write(" ");
        
        Console.Write(msg + "\n");
    }
}
