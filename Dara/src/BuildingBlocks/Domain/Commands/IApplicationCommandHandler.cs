namespace Dara.BuildingBlocks.Domain.Commands;

public interface IApplicationCommandHandler<in TCommand> 
    where TCommand : IApplicationCommand
{
    Task HandleAsync(TCommand command);
}

public interface IApplicationCommandHandler<in TCommand, TCommandResult> 
    where TCommand : IApplicationCommand 
    where TCommandResult : IApplicationCommandResult
{
    Task<TCommandResult> HandleAsync(TCommand command);
}