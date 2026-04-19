using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record ChangeNodesMeshCodeCommand() : IApplicationCommand;

public record ChangeNodesMeshCodeCommandResult() : IApplicationCommandResult;

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