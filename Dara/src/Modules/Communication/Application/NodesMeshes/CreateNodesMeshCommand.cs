using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.NodesMeshes;

public record CreateNodesMeshCommand() : IApplicationCommand;

public record CreateNodesMeshCommandResult() : IApplicationCommandResult;

public class CreateNodesMeshCommandHandler : IApplicationCommandHandler<CreateNodesMeshCommand, CreateNodesMeshCommandResult>
{
    public CreateNodesMeshCommandHandler()
    {
    }

    public async Task<CreateNodesMeshCommandResult> HandleAsync(CreateNodesMeshCommand command)
    {
        return new();
    }
}