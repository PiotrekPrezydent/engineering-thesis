using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Groups.RemoveParticipantFromGroup;

public class RemoveParticipantFromGroupCommandHandler : ICommandHandler<RemoveParticipantFromGroupCommand>
{
    private readonly IGroupRepository _repository;

    public RemoveParticipantFromGroupCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RemoveParticipantFromGroupCommand command)
    {
        var group = await _repository.GetByIdAsync(new(command.GroupId));
        group.RemoveParticipant(new ParticipantId(command.ParticipantId));
    }
}