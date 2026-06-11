using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.BuildingBlocks.Application;

public interface IModule
{
    Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand;
    
    Task<TResult> ExecuteCommandAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
    
    Task<TResult> ExecuteQueryAsync<TQuery,TResult>(TQuery query) where TQuery : IQuery<TResult>;
}