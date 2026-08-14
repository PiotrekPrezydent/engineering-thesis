using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Logging;

public class ModuleLogger : ILogger
{
    private readonly string _categoryName;
    private readonly ConcurrentQueue<ModuleLogEntry> _logQueue;
    private readonly Action _onExceptionLog;

    public ModuleLogger(string categoryName, ConcurrentQueue<ModuleLogEntry> logQueue, Action onExceptionLog)
    {
        _categoryName = categoryName;
        _logQueue = logQueue;
        _onExceptionLog = onExceptionLog;
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var date = DateTime.UtcNow;
        var message = formatter(state, exception);
        
        _logQueue.Enqueue(new ModuleLogEntry(date, logLevel, _categoryName, message, exception));
        if(logLevel >= LogLevel.Error || exception is not null)
            _onExceptionLog.Invoke();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}