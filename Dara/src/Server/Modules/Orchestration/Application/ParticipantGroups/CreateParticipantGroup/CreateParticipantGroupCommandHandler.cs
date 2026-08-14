using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.CreateParticipantGroup;

public class CreateParticipantGroupCommandHandler : ICommandHandler<CreateParticipantGroupCommand>
{
    private readonly IParticipantGroupRepository _repository;

    public CreateParticipantGroupCommandHandler(IParticipantGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateParticipantGroupCommand command)
    {
        var group = ParticipantGroup.Create(new ParticipantGroupId(command.GroupId), new ParticipantId(command.CreatorId));
        await _repository.AddAsync(group);
    }
}