using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Groups.AddParticipantToGroup;

public class AddParticipantToGroupCommandHandler : ICommandHandler<AddParticipantToGroupCommand>
{
    private readonly IGroupRepository _repository;

    public AddParticipantToGroupCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddParticipantToGroupCommand command)
    {
        var group = await _repository.GetByIdAsync(new(command.GroupId));
        group.AddParticipant(new ParticipantId(command.ParticipantId));
    }
}