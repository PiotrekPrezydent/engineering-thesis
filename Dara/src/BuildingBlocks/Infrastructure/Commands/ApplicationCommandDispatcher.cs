using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.Commands;

public class ApplicationCommandDispatcher : IApplicationCommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsoleLogger _consoleLogger;
    
    public ApplicationCommandDispatcher(IServiceProvider serviceProvider)
    {
        _consoleLogger = new ConsoleLogger(this);
        _consoleLogger.Start("CREATE");
        
        _serviceProvider = serviceProvider;
        
        _consoleLogger.Element(_serviceProvider);
        _consoleLogger.End();
    }
    
    public async Task<CommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) 
        where TCommandResult : IApplicationCommandResult 
        where TCommand : IApplicationCommand
    {
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand, TCommandResult>>();
        
        _consoleLogger.Element(handler);
        _consoleLogger.Element(command);
        CommandResult cr = new();
        try
        {
            var result = await handler.HandleAsync(command);
            
            _consoleLogger.Element(result);
            
            cr.SetExpectedResult(result);
        }
        catch (Exception ex)
        {
            cr.SetException(ex);
        }
        _consoleLogger.Element(cr);
        

        _consoleLogger.End();
        
        return cr;
    }
}