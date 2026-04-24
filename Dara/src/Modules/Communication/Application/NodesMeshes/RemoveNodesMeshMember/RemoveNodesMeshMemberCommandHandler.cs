using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes.RemoveNodesMeshMember;

public class RemoveNodesMeshMemberCommandHandler : IApplicationCommandHandler<RemoveNodesMeshMemberCommand,
    RemoveNodesMeshMemberCommandResult>
{
    public RemoveNodesMeshMemberCommandHandler()
    {
    }

    public async Task<RemoveNodesMeshMemberCommandResult> HandleAsync(RemoveNodesMeshMemberCommand command)
    {
        return new();
    }
}