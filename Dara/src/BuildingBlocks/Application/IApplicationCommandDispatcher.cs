namespace Dara.BuildingBlocks.Application;

public interface IApplicationCommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : IApplicationCommand;

    Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command)
        where TCommand : IApplicationCommand
        where TCommandResult : IApplicationCommandResult;
}