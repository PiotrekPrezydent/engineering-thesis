using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.RemoveParticipantFromGroup;

public class RemoveParticipantFromGroupCommandHandler : ICommandHandler<RemoveParticipantFromGroupCommand>
{
    private readonly IParticipantGroupRepository _repository;

    public RemoveParticipantFromGroupCommandHandler(IParticipantGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RemoveParticipantFromGroupCommand command)
    {
        var group = await _repository.GetByIdAsync(new(command.GroupId));
        group.RemoveParticipant(new ParticipantId(command.ParticipantId));
    }
}