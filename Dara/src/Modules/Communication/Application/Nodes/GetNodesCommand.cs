using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes;

public record GetNodesCommand() : IApplicationCommand;

public record GetNodesCommandResult() : IApplicationCommandResult;

public class GetNodesCommandHandler : IApplicationCommandHandler<GetNodesCommand, GetNodesCommandResult>
{
    public GetNodesCommandHandler()
    {
    }

    public async Task<GetNodesCommandResult> HandleAsync(GetNodesCommand command)
    {
        return new();
    }
}