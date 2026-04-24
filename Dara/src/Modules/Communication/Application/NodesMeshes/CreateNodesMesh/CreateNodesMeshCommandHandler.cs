using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes.CreateNodesMesh;

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