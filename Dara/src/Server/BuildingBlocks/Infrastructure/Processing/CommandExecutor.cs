using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing;

public class CommandExecutor : ICommandExecutor
{
    private readonly IHandlersResolver _handlersResolver;
    
    public CommandExecutor(IHandlersResolver handlersResolver)
    {
        _handlersResolver = handlersResolver;
    }
    
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = _handlersResolver.GetCommandHandler<TCommand>();
            
        await handler.HandleAsync(command);
    }

    public async Task<TCommandResult> ExecuteAsync<TCommand, TCommandResult>(TCommand command) where TCommand : ICommand<TCommandResult>
    {
        var handler = _handlersResolver.GetCommandHandler<TCommand, TCommandResult>();
        
        return await handler.HandleAsync(command);
    }
}