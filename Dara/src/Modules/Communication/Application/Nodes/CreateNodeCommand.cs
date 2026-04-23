using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes;

public record CreateNodeCommand() : IApplicationCommand;

public record CreateNodeCommandResult() : IApplicationCommandResult;

public class CreateNodeCommandHandler : IApplicationCommandHandler<CreateNodeCommand, CreateNodeCommandResult>
{
    public CreateNodeCommandHandler()
    {
    }

    public async Task<CreateNodeCommandResult> HandleAsync(CreateNodeCommand command)
    {
        return new();
    }
}