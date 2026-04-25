namespace Dara.BuildingBlocks.Application.Abstraction
{
    // public interface IApplicationCommandHandler<in TCommand> 
//     where TCommand : IApplicationCommand
// {
//     Task<CommandResult> HandleAsync(TCommand command);
// }

    public interface IApplicationCommandHandler<in TCommand, TCommandResult> 
        where TCommand : IApplicationCommand 
        where TCommandResult : IApplicationCommandResult
    {
        Task<TCommandResult> HandleAsync(TCommand command);
    }
}