using Dara.Server.BuildingBlocks.Application;
using Dara.Shared.Common;

namespace Dara.BuildingBlocks.Infrastructure.ModuleCommands;

public interface IModuleCommandRunner
{
    public Task<WrappedResult> ExecuteAsync<TCommand>(TCommand command) 
        where TCommand : IModuleCommand;

    public Task<WrappedResult<TResult>> ExecuteAsync<TCommand, TResult>(TCommand command) 
        where TCommand : IModuleCommand<TResult>
        where TResult : class;
}