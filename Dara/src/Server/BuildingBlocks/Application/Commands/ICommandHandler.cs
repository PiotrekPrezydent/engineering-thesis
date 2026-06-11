namespace Dara.Server.BuildingBlocks.Application.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public Task HandleAsync(TCommand command);
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    public Task<TResult> HandleAsync(TCommand command);
}