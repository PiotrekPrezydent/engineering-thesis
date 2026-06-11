using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.BuildingBlocks.Infrastructure.Processing;

public interface ICommandExecutor
{
    Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    
    Task<TCommandResult> ExecuteAsync<TCommand, TCommandResult>(TCommand command) where TCommand : ICommand<TCommandResult>;
}