namespace Dara.Server.BuildingBlocks.Application.Commands;

// queues a command of the module in the current unit of work; it is stored with the next SaveChanges
// of the scope and the internal commands worker executes it later, retrying until it succeeds
public interface IInternalCommandScheduler
{
    Task EnqueueAsync<TCommand>(TCommand command) where TCommand : ICommand;
}
