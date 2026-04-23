using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes;

public record GetNodeCommand() : IApplicationCommand;

public record GetNodeCommandResult() : IApplicationCommandResult;

public class GetNodeCommandHandler : IApplicationCommandHandler<GetNodeCommand, GetNodeCommandResult>
{
    public GetNodeCommandHandler()
    {
    }

    public async Task<GetNodeCommandResult> HandleAsync(GetNodeCommand command)
    {
        return new();
    }
}