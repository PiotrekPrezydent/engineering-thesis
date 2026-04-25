namespace Dara.BuildingBlocks.Application.Commands
{
    public interface IModuleCommandHandler<in TCommand, TCommandResult> : IModuleCommandHandler
        where TCommand : IModuleCommand 
        where TCommandResult : IModuleCommandResult
    {
        Task<TCommandResult> HandleAsync(TCommand command);
    }

    public interface IModuleCommandHandler;
}