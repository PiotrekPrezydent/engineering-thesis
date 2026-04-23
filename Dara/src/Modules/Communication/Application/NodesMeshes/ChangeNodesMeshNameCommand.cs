using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record ChangeNodesMeshNameCommand() : IApplicationCommand;

public record ChangeNodesMeshNameCommandResult() : IApplicationCommandResult;

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