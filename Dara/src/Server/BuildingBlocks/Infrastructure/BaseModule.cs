using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure;

public abstract class BaseModule<TCompositionRoot> : IModule where TCompositionRoot : ICompositionRoot
{
    public async Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        using (var scope = TCompositionRoot.CreteScope())
        {
            var executor = scope.ServiceProvider.GetRequiredService<ICommandExecutor>();
        
            await executor.ExecuteAsync(command);
        }
    }

    public async Task<TResult> ExecuteCommandAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        using (var scope = TCompositionRoot.CreteScope())
        {
            var executor = scope.ServiceProvider.GetRequiredService<ICommandExecutor>();
        
            var result = await executor.ExecuteAsync<TCommand, TResult>(command);
            return result;
        }
    }

    public async Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        using (var scope = TCompositionRoot.CreteScope())
        {
            var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        
            var result = await handler.HandleAsync(query);
            return result;
        }
    }
}