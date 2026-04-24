using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes.DeleteNodesMesh;

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