using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes;

public record DeleteNodeCommand() : IApplicationCommand;

public record DeleteNodeCommandResult() : IApplicationCommandResult;

public class DeleteNodeCommandHandler : IApplicationCommandHandler<DeleteNodeCommand, DeleteNodeCommandResult>
{
    public DeleteNodeCommandHandler()
    {
    }

    public async Task<DeleteNodeCommandResult> HandleAsync(DeleteNodeCommand command)
    {
        return new();
    }
}