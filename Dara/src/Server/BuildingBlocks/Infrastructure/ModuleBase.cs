using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Microsoft.Extensions.DependencyInjection;


namespace Dara.Server.BuildingBlocks.Infrastructure;

public abstract class ModuleBase : IModule 
{
    protected IModuleCompositionRoot _compositionRoot;

    protected ModuleBase(IModuleCompositionRoot compositionRoot)
    {
        _compositionRoot = compositionRoot;
    }

    public async Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        using (var scope = _compositionRoot.CreateScope())
        {
            var executor = scope.ServiceProvider.GetRequiredService<ICommandExecutor>();
        
            await executor.ExecuteAsync(command);
        }
    }

    public async Task<TResult> ExecuteCommandAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        using (var scope = _compositionRoot.CreateScope())
        {
            var executor = scope.ServiceProvider.GetRequiredService<ICommandExecutor>();
        
            var result = await executor.ExecuteAsync<TCommand, TResult>(command);
            return result;
        }
    }

    public async Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        using (var scope = _compositionRoot.CreateScope())
        {
            var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        
            var result = await handler.HandleAsync(query);
            return result;
        }
    }
    
}