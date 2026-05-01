namespace Dara.Shared.Common.Logging;

[AttributeUsage(AttributeTargets.Field)]
public class LogConfig : Attribute
{
    public string EventName { get; }

    public string LogContext { get; }
    
    public LogConfig(string eventName, string logContext)
    {
        EventName = eventName;
        LogContext = logContext;
    }
}