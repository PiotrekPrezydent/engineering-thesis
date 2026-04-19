using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record DeleteNodesMeshCommand() : IApplicationCommand;

public record DeleteNodesMeshCommandResult() : IApplicationCommandResult;

public class DeleteNodesMeshCommandHandler : IApplicationCommandHandler<DeleteNodesMeshCommand, DeleteNodesMeshCommandResult>
{
    public DeleteNodesMeshCommandHandler()
    {
    }

    public async Task<DeleteNodesMeshCommandResult> HandleAsync(DeleteNodesMeshCommand command)
    {
        return new();
    }
}