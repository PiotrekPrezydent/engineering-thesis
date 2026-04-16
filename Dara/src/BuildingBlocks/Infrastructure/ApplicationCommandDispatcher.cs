using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Contracts;
using Dara.BuildingBlocks.Domain.Commands;
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
        Console.WriteLine($"RUNNING COMMAND: {command}");
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand>>();
        await handler.HandleAsync(command);
    }

    public async Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) 
        where TCommandResult : IApplicationCommandResult 
        where TCommand : IApplicationCommand
    {
        Console.WriteLine($"RUNNING COMMAND WITH RESULT: {command}");
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand, TCommandResult>>();
        return await handler.HandleAsync(command);
    }
}