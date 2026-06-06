using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Dara.Shared.Logging;

public class SharedLogFormatter : ConsoleFormatter
{
    public SharedLogFormatter(string name = "SharedLogFormatter" ) : base(name) { }

    protected static Dictionary<(int Start, int End), string> _eventTypes = new()
    {
        [(SharedLogsIdsRanges.DotnetStart, SharedLogsIdsRanges.DotnetEnd)] = "DOTNET",
        [(SharedLogsIdsRanges.SharedStart,  SharedLogsIdsRanges.SharedEnd)] = "SHARED",
    };
    
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null)
            return;
        
        string timeColor = AnsiColors.Cyan;
        string categoryColor = AnsiColors.Yellow;
        string logLevelColor = GetLogLevelColor(logEntry.LogLevel);
        
        string fullClassName = logEntry.Category;
        string logCategory = GetEventTypeById(logEntry.EventId.Id);

        string className = GetSimpleClassName(fullClassName);

        string timeText = $"[{DateTime.Now:HH:mm:ss}]";
        string logLevelText = $"[{GetLogLevelText(logEntry.LogLevel)}]";
        string eventNameText = logEntry.EventId.Name ?? "";
        string categoryText = $"[{logCategory}";
        categoryText += " @ " + className + " - " + eventNameText;
        categoryText = categoryText.PadRight(128) + "]";
        
        textWriter.Write($"{timeColor}{timeText}{AnsiColors.Reset} ");
        textWriter.Write($"{logLevelColor}{logLevelText}{AnsiColors.Reset} ");
        textWriter.WriteLine($"{categoryColor}{categoryText}{AnsiColors.Reset} # {fullClassName}");
        textWriter.WriteLine($":::::::::: {message}");
        
        if (logEntry.Exception is null)
            return;
        
        textWriter.WriteLine($"{AnsiColors.Red}{logEntry.Exception.GetType().Name}: {logEntry.Exception.Message}");
        textWriter.WriteLine(logEntry.Exception.StackTrace + AnsiColors.Reset);
    }

    public static void AddEventTypeRange(int startRange, int endRange, string name)
    {
        _eventTypes.Add((startRange,endRange), name);
    }
    
    string GetEventTypeById(int eventId)
    {
        foreach (var range in _eventTypes)
        {
            if (eventId >= range.Key.Start && eventId <= range.Key.End)
                return range.Value;
        }
        return "NONE";
    }

    string GetLogLevelColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => AnsiColors.Gray,
            LogLevel.Debug => AnsiColors.Gray,
            LogLevel.Information => AnsiColors.Green,
            LogLevel.Warning => AnsiColors.Yellow,
            LogLevel.Error => AnsiColors.Red,
            LogLevel.Critical => AnsiColors.Magenta,
            _ => AnsiColors.White
        };
    }

    string GetLogLevelText(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF", 
            LogLevel.Warning => "WRN", 
            LogLevel.Error => "ERR", 
            LogLevel.Critical => "FTL", 
            _ => "???", 
        };
    } 
    
    string GetSimpleClassName(string fullClassName)
    {
        int lastDotIndex = fullClassName.LastIndexOf('.');
        return lastDotIndex >= 0 ? fullClassName.Substring(lastDotIndex + 1) : fullClassName;
    }
    
}