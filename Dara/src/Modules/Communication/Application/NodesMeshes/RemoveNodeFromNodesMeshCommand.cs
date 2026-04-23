using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record RemoveNodeFromNodesMeshCommand() : IApplicationCommand;

public record RemoveNodeFromNodesMeshCommandResult() : IApplicationCommandResult;

public class RemoveNodeFromNodesMeshCommandHandler : IApplicationCommandHandler<RemoveNodeFromNodesMeshCommand,
    RemoveNodeFromNodesMeshCommandResult>
{
    public RemoveNodeFromNodesMeshCommandHandler()
    {
    }

    public async Task<RemoveNodeFromNodesMeshCommandResult> HandleAsync(RemoveNodeFromNodesMeshCommand command)
    {
        return new();
    }
}