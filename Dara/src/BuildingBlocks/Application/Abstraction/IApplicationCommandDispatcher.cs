namespace Dara.BuildingBlocks.Application.Abstraction;

public interface IApplicationCommandDispatcher
{
    //Task<CommandResult> DispatchAsync<TCommand>(TCommand command) where TCommand : IApplicationCommand;

    Task<CommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command)
        where TCommand : IApplicationCommand
        where TCommandResult : IApplicationCommandResult;
}