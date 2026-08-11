using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;

namespace Dara.Server.BuildingBlocks.Infrastructure;

public abstract class ModuleBase : IModule 
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IHandlersResolver _handlersResolver;

    protected ModuleBase(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver)
    {
        _commandExecutor = commandExecutor;
        _handlersResolver = handlersResolver;
    }

    public async Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        await _commandExecutor.ExecuteAsync(command);
    }

    public async Task<TResult> ExecuteCommandAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        return await _commandExecutor.ExecuteAsync<TCommand, TResult>(command);
    }

    public async Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        var handler = _handlersResolver.GetQueryHandler<TQuery, TResult>();
        return await handler.HandleAsync(query);
    }
}