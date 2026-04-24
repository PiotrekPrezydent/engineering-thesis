using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes.ChangeNodesMeshName;

public class ChangeNodesMeshNameCommandHandler : IApplicationCommandHandler<ChangeNodesMeshNameCommand,
    ChangeNodesMeshNameCommandResult>
{
    public ChangeNodesMeshNameCommandHandler()
    {
    }

    public async Task<ChangeNodesMeshNameCommandResult> HandleAsync(ChangeNodesMeshNameCommand command)
    {
        return new();
    }
}