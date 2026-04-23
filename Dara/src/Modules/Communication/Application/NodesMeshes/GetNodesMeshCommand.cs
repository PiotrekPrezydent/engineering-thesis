using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record GetNodesMeshCommand() : IApplicationCommand;

public record GetNodesMeshCommandResult() : IApplicationCommandResult;

public class GetNodesMeshCommandHandler : IApplicationCommandHandler<GetNodesMeshCommand, GetNodesMeshCommandResult>
{
    public GetNodesMeshCommandHandler()
    {
    }

    public async Task<GetNodesMeshCommandResult> HandleAsync(GetNodesMeshCommand command)
    {
        return new();
    }
}