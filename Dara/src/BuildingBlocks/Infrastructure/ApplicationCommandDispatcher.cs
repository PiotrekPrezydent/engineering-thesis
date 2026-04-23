using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class ApplicationCommandDispatcher : IApplicationCommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsoleLogger _consoleLogger;
    
    public ApplicationCommandDispatcher(IServiceProvider serviceProvider)
    {
        _consoleLogger = new ConsoleLogger(this);
        _consoleLogger.Start("CREATE");
        
        _serviceProvider = serviceProvider;
        
        _consoleLogger.Element("SERVICE", _serviceProvider);
        _consoleLogger.End();
    }
    
    // public async Task<CommandResult> DispatchAsync<TCommand>(TCommand command) where TCommand : IApplicationCommand
    // {
    //     _consoleLogger.Start("HANDLING COMMAND");
    //     
    //     var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand>>();
    //     
    //     _consoleLogger.Element("HANDLER", handler);
    //     _consoleLogger.Element("COMMAND", command);
    //     try
    //     {
    //         var result = await handler.HandleAsync(command);
    //         _consoleLogger.Element("RESULT", result);
    //         _consoleLogger.End();
    //     
    //         return result;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine("CATCHED IN DISPATCHER: {0}", e);
    //     }
    //
    //
    //     return null;
    // }

    public async Task<CommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) 
        where TCommandResult : IApplicationCommandResult 
        where TCommand : IApplicationCommand
    {
        _consoleLogger.Start("HANDLING COMMAND WITH RESULT");
        
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand, TCommandResult>>();
        
        _consoleLogger.Element("HANDLER", handler);
        _consoleLogger.Element("COMMAND", command);
        CommandResult cr = new();
        try
        {
            var result = await handler.HandleAsync(command);
            
            _consoleLogger.Element("HANDLER-RESULT", result);
            
            cr.SetExpectedResult(result);
        }
        catch (Exception ex)
        {
            cr.SetException(ex);
        }
        _consoleLogger.Element("COMMAND-RESULT", cr);
        

        _consoleLogger.End();
        
        return cr;
    }
}