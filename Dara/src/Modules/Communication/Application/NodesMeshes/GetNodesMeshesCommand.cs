using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record GetNodesMeshesCommand() : IApplicationCommand;

public record GetNodesMeshesCommandResult() : IApplicationCommandResult;

public class GetNodesMeshesCommandHandler : IApplicationCommandHandler<GetNodesMeshesCommand, GetNodesMeshesCommandResult>
{
    public GetNodesMeshesCommandHandler()
    {
    }

    public async Task<GetNodesMeshesCommandResult> HandleAsync(GetNodesMeshesCommand command)
    {
        return new();
    }
}