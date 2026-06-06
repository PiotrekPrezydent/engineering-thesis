using Dara.Server.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure.ModuleCommands;
using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.Logging;

public static partial class ModuleCommandRunnerLogMessages
{
    [LoggerMessage(
        LogLevel.Information,
        EventName = "CallingCommandHandler",
        EventId = LogsIdsRanges.CoreStart + 301,
        Message = "Called command handler {commandHandlerName} for command {commandName} used object: {commandObject}.")]
    public static partial void LogCommandHandlerCalled(this ILogger<IModuleCommandRunner> logger, string commandHandlerName, string commandName, IModuleCommand commandObject);
    
    [LoggerMessage(
        LogLevel.Error,
        EventName = "CommandHandlerException",
        EventId = LogsIdsRanges.CoreStart + 302,
        Message = "Command handler {commandHandlerName} for command {commandName} used object: {commandObject} throw exception of type {exceptionType}, exception message: {exceptionMessage}.")]
    public static partial void LogCommandHandlerException(this ILogger<IModuleCommandRunner> logger, string commandHandlerName, string commandName, IModuleCommand commandObject, string exceptionType, string exceptionMessage);
    
    [LoggerMessage(
        LogLevel.Error,
        EventName = "ModuleCommandRunnerException",
        EventId = LogsIdsRanges.CoreStart + 303,
        Message = "Module command runner throw exception of type {exceptionType}, exception message: {exceptionMessage}.")]
    public static partial void LogModuleCommandRunnerException(this ILogger<IModuleCommandRunner> logger, string exceptionType, string exceptionMessage);

}