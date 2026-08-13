using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantGroups;

public class UpdateParticipantNameCommandHandler : ICommandHandler<UpdateParticipantGroupCommand>
{
    private readonly IParticipantRepository _repository;

    public UpdateParticipantNameCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateParticipantGroupCommand command)
    {
        var participant = await _repository.GetByIdAsync(new(command.ParticipantId));
        var groupId = new ParticipantGroupId(command.GroupId);
        
        if(!participant.IsMemberOfGroup(groupId))
            return;

        if (command.Option == UpdateProjectionListOption.Add)
            participant.AddGroup(groupId);
        else if (command.Option == UpdateProjectionListOption.Remove)
            participant.RemoveGroup(groupId);
        else
            throw new ArgumentOutOfRangeException();
        
    }
}