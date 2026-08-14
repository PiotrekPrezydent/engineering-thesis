using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.AddParticipantToGroup;

public class AddParticipantToGroupCommandHandler : ICommandHandler<AddParticipantToGroupCommand>
{
    private readonly IParticipantGroupRepository _repository;

    public AddParticipantToGroupCommandHandler(IParticipantGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddParticipantToGroupCommand command)
    {
        var group = await _repository.GetByIdAsync(new(command.GroupId));
        group.AddParticipant(new ParticipantId(command.ParticipantId));
    }
}