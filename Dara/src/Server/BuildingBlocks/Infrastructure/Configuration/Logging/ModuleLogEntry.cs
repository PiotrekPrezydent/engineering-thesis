using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Logging;

public record ModuleLogEntry(DateTime EntryTime, LogLevel LogLevel, string Category,string Message, Exception? Exception);