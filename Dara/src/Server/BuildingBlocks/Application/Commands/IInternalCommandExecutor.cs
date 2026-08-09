namespace Dara.Server.BuildingBlocks.Application.Commands;

public interface IInternalCommandExecutor
{
    Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    
    Task<TCommandResult> ExecuteAsync<TCommand, TCommandResult>(TCommand command) where TCommand : ICommand<TCommandResult>;
}