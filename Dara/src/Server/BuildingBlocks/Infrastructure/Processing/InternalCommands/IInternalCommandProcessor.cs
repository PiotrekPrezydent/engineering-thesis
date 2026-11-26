namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public interface IInternalCommandProcessor
{
    // executes the command once; a processed or missing command does nothing
    Task ProcessAsync(Guid commandId, CancellationToken ct);

    // call in a new scope after ProcessAsync threw, so nothing tracked by the failed attempt is saved
    Task RecordFailureAsync(Guid commandId, Exception error, CancellationToken ct);
}
