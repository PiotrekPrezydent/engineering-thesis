using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes.ChangeNodesMeshCode;

public class ChangeNodesMeshCodeCommandHandler : IApplicationCommandHandler<ChangeNodesMeshCodeCommand,
    ChangeNodesMeshCodeCommandResult>
{
    public ChangeNodesMeshCodeCommandHandler()
    {
    }

    public async Task<ChangeNodesMeshCodeCommandResult> HandleAsync(ChangeNodesMeshCodeCommand command)
    {
        return new();
    }
}