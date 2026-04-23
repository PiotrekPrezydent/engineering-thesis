using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record AddNodeToNodesMeshCommand() : IApplicationCommand;

public record AddNodeToNodesMeshCommandResult() : IApplicationCommandResult;

public class AddNodeToNodesMeshCommandHandler : IApplicationCommandHandler<AddNodeToNodesMeshCommand,
    AddNodeToNodesMeshCommandResult>
{
    public AddNodeToNodesMeshCommandHandler()
    {
    }

    public async Task<AddNodeToNodesMeshCommandResult> HandleAsync(AddNodeToNodesMeshCommand command)
    {
        return new();
    }
}