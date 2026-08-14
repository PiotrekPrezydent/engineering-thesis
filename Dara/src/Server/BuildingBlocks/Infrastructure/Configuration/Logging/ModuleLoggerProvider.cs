using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Logging;

public class ModuleLoggerProvider : ILoggerProvider
{
    private readonly string _moduleName;
    private readonly ConcurrentQueue<ModuleLogEntry> _logQueue;
    private readonly Timer _timer;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private DateTime _lastFlushTime;
    
    private int _isFlushing = 0;
    private bool _isExceptionCaused = false;
    public ModuleLoggerProvider(string moduleName)
    {
        _moduleName = moduleName;
        _logQueue = new ConcurrentQueue<ModuleLogEntry>();
        _lastFlushTime = DateTime.UtcNow;
        
        _timer = new Timer(FlushLogs, null, _interval, _interval);
    }
    
    public ILogger CreateLogger(string categoryName)
    {
        return new ModuleLogger(categoryName,  _logQueue, TriggerExceptionFlush);
    }
    
    public void Dispose()
    {
        _timer.Dispose();
        FlushLogs(null);
    }
    
    private void TriggerExceptionFlush()
    {
        _isExceptionCaused = true;
        _timer.Change(TimeSpan.Zero, _interval);
    }
    
    const string TimeColor = AnsiColors.Cyan;
    const string ModuleColor = AnsiColors.Yellow;
    const string CategoryColor = AnsiColors.BrightMagenta;

    private int maxTextSize => (int)(Console.WindowWidth * 0.6);

    public string InSquareBrackers(string text, string color) => $"{color}[{text}]{AnsiColors.Reset}";
    
    private void FlushLogs(object? state)
    {
        if (Interlocked.CompareExchange(ref _isFlushing, 1, 0) != 0) 
            return;

        try
        {
            var sb = new StringBuilder();
            var currentLogs = new List<ModuleLogEntry>();
            
            while (_logQueue.TryDequeue(out var log))
                currentLogs.Add(log);
            
            if (currentLogs.Count == 0)
                return;
            sb.AppendLine("\n");
            sb.Append(InSquareBrackers(_moduleName,ModuleColor));
            sb.Append(" ######## ");
            sb.Append(InSquareBrackers(_lastFlushTime.ToString("HH:mm:ss.ffffff"), TimeColor));
            sb.Append(" -------- ");
            sb.Append(InSquareBrackers(DateTime.Now.ToString("HH:mm:ss.ffffff"), TimeColor));
            sb.AppendLine();
            
            var groupedLogs = currentLogs.GroupBy(l => l.Category);
            foreach (var group in groupedLogs)
            {
                var contextLevel = LogLevel.None;
                var contextSecond = -1;
                foreach (var log in group)
                {
                    if (log.LogLevel != contextLevel || log.EntryTime.Second > contextSecond)
                    {
                        contextLevel = log.LogLevel;
                        contextSecond = log.EntryTime.Second;
                        sb.AppendLine();
                        sb.Append(InSquareBrackers(log.EntryTime.ToString("HH:mm:ss"), TimeColor));
                        sb.Append(" ");
                        sb.Append(InSquareBrackers(GetLogLevelText(log.LogLevel), GetLogLevelColor(log.LogLevel)));
                        sb.Append(" ");
                        sb.Append(InSquareBrackers(log.Category, CategoryColor));
                        sb.Append(" ");
                        sb.Append(InSquareBrackers(_moduleName,ModuleColor));
                        sb.AppendLine();
                    }
              
                    if (log.Message.Length > maxTextSize)
                    {
                        sb.AppendLine();
                        sb.Append(InSquareBrackers(log.EntryTime.ToString(".ffff"), TimeColor));
                        sb.Append(" ");
                        sb.Append(log.Message.Substring(0, maxTextSize));
                        sb.AppendLine();
                        sb.Append(InSquareBrackers(new string('=',5),TimeColor));
                        sb.Append(" ");
                        sb.AppendLine(log.Message.Substring(maxTextSize));
                        sb.AppendLine();
                        continue;
                    }
                    
                    sb.Append(InSquareBrackers(log.EntryTime.ToString(".ffff"), TimeColor));
                    sb.Append(" ");
                    sb.AppendLine(log.Message);

                    if (log.Exception != null)
                    {
                        sb.AppendLine(log.Exception.ToString());
                    }
                }
            }
            
            Console.Write(sb.ToString());
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _lastFlushTime = DateTime.UtcNow;
            Interlocked.Exchange(ref _isFlushing, 0);
        }
    }
    string GetCategoryHeader(ModuleLogEntry logEntry) => GetTimeTextInColor(logEntry.EntryTime) + " " + GetLogLevelTextInColor(logEntry.LogLevel) + " " + GetModuleTextInColor() + " " + GetCategoryTextInColor(logEntry.Category);
    
    string GetTimeTextInColor(DateTime time) => $"{AnsiColors.Cyan}[{time:HH:mm:ss.fffff}]{AnsiColors.Reset}";

    string GetLogLevelTextInColor(LogLevel logLevel) => $"{GetLogLevelColor(logLevel)}[{GetLogLevelText(logLevel)}]{AnsiColors.Reset}";

    string GetModuleTextInColor() => AnsiColors.Yellow + "["+_moduleName+"]"+AnsiColors.Reset;
    
    string GetCategoryTextInColor(string category) => AnsiColors.BrightMagenta + "["+category+"]"+AnsiColors.Reset;
    
    string GetModuleTextHeader()
    {
        var str = AnsiColors.Yellow+"[ "+"MODULE :::::::: " +_moduleName;
        str = str.PadRight(32) + "]" + AnsiColors.Reset;
        
        str += " LOG DATA FROM :::: " + GetTimeTextInColor(_lastFlushTime) + " TO "  + GetTimeTextInColor(DateTime.UtcNow);
        return str;
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
}