using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Contracts;
using Dara.BuildingBlocks.Domain.Commands;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class ApplicationCommandDispatcher : IApplicationCommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public ApplicationCommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TCommand>(TCommand command) 
        where TCommand : IApplicationCommand
    {
        var cl = new ConsoleLogger("HANDLING COMMAND");
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand>>();
        
        cl.Element("HANDLER", handler);
        cl.Element("COMMAND", command);
        
        await handler.HandleAsync(command);
        
        cl.End();
    }

    public async Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) 
        where TCommandResult : IApplicationCommandResult 
        where TCommand : IApplicationCommand
    {
        var cl = new ConsoleLogger("HANDLING COMMAND WITH RESULT");
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand, TCommandResult>>();
        
        cl.Element("HANDLER", handler);
        cl.Element("COMMAND", command);
        
        var result = await handler.HandleAsync(command);
        
        cl.Element("RESULT", result);
        cl.End();
        
        return result;
    }
}