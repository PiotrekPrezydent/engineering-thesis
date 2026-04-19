using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.Nodes;

public record ChangeNodeNameCommand() : IApplicationCommand;

public record ChangeNodeNameCommandResult() : IApplicationCommandResult;

public class ChangeNodeNameCommandHandler : IApplicationCommandHandler<ChangeNodeNameCommand, ChangeNodeNameCommandResult>
{
    public ChangeNodeNameCommandHandler()
    {
    }

    public async Task<ChangeNodeNameCommandResult> HandleAsync(ChangeNodeNameCommand command)
    {
        return new();
    }
}